using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using SpatialSlur.SlurRhino;
using SpatialSlur.SlurRhino.Remesher;

using static SpatialSlur.SlurCore.SlurMath;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.LoopGrowth
{
    using V = HeMeshLG.V;
    using E = HeMeshLG.E;
    using F = HeMeshLG.F;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LoopGrower
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="features"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static LoopGrower Create<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, MeshFeature target, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
            where TV : HeVertex<TV, TE, TF>, IVertex3d
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var copy = HeMeshLG.Factory.CreateCopy(mesh, (v0, v1) => v0.Position = v1.Position, delegate { }, delegate { });
            return new LoopGrower(copy, target, features, tolerance);
        }

        #endregion


        private const double TargetBinScale = 3.5; // as a factor of radius
        private const double TargetLoadFactor = 3.0;

        //
        // simulation mesh
        //

        private HeMesh<V, E, F> _mesh;
        private HeElementList<V> _verts;
        private HeElementList<E> _hedges;
        private HashGrid3d<V> _grid;

        //
        // constraint objects
        //

        private MeshFeature _target;
        private List<IFeature> _features;
        private IField3d<double> _lengthField;

        //
        // simulation settings
        //

        private LoopGrowerSettings _settings;
        private int _stepCount = 0;
        private int _vertTag = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="features"></param>
        /// <param name="tolerance"></param>
        public LoopGrower(HeMesh<V, E, F> mesh, MeshFeature target, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
        {
            _mesh = mesh;
            _verts = _mesh.Vertices;
            _hedges = _mesh.Halfedges;
            _settings = new LoopGrowerSettings();
            _stepCount = 0;

            Target = target;
            InitFeatures(features, tolerance);
            ProjectToFeatures();
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitFeatures(IEnumerable<IFeature> features, double tolerance)
        {
            _features = new List<IFeature>();
            var tolSqr = tolerance * tolerance;

            // create features
            foreach (var f in features)
            {
                int index = _features.Count;
                _features.Add(f);

                // if vertex is close enough, assign feature
                foreach (var v in _verts)
                {
                    if (v.FeatureIndex > -1) continue; // skip if already assigned

                    var p = v.Position;
                    if (p.SquareDistanceTo(f.ClosestPoint(p)) < tolSqr)
                        v.FeatureIndex = index;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh<V, E, F> Mesh
        {
            get { return _mesh; }
        }


        /// <summary>
        /// 
        /// </summary>
        public MeshFeature Target
        {
            get { return _target; }
            set { _target = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IFeature> Features
        {
            get { return _features; }
            set { _features = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public IField3d<double> EdgeLengthField
        {
            get { return _lengthField; }
            set { _lengthField = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public LoopGrowerSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int StepCount
        {
            get { return _stepCount; }
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public void Step()
        {
            for (int i = 0; i < _settings.SubSteps; i++)
            {
                if (++_stepCount % _settings.RefineFrequency == 0)
                    Refine();
  
                CalculateForces();
                UpdateVertices();
                ProjectToFeatures();
            }
        }


        #region Dynamics

        /// <summary>
        /// Calculates all forces in the system
        /// </summary>
        /// <returns></returns>
        private void CalculateForces()
        {
            //PullToFeatures(_settings.FeatureWeight);
            //LaplacianFair(_settings.SmoothWeight);

            if (_stepCount % _settings.CollideFrequency == 0)
                SphereCollide(_settings.CollideRadius, 1.0);

            LaplacianFairBoundary(_settings.SmoothWeight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        private void LaplacianFair(double weight)
        {
            for(int i = 0; i < _verts.Count; i++)
            {
                var v0 = _verts[i];
                if (v0.IsRemoved) continue;

                // calculate graph laplacian
                var sum = new Vec3d();
                var count = 0;

                foreach(var v1 in v0.ConnectedVertices)
                {
                    sum += v1.Position;
                    count++;
                }

                double t = 1.0 / count;
                var move = sum * t - v0.Position;

                // apply to central vertex
                v0.MoveSum += move * weight;
                v0.WeightSum += weight;
  
                // distribute negated to neighbours
                move *= -t;
                foreach (var v1 in v0.ConnectedVertices)
                {
                    v1.MoveSum += move * weight;
                    v1.WeightSum += weight;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        private void LaplacianFairBoundary(double weight)
        {
            for (int i = 0; i < _verts.Count; i++)
            {
                var v0 = _verts[i];
                if (v0.IsRemoved) continue;

                if (v0.IsBoundary)
                {
                    var he = v0.FirstOut;

                    var v1 = he.PrevInFace.Start;
                    var v2 = he.NextInFace.Start;
                    var move = (v1.Position + v2.Position) * 0.5 - v0.Position;

                    // apply to central vertex
                    v0.MoveSum += move * weight;
                    v0.WeightSum += weight;

                    // distribute negated to neighbours
                    move *= -0.5;
                    v1.MoveSum += move * weight;
                    v1.WeightSum += weight;
                    v2.MoveSum += move * weight;
                    v2.WeightSum += weight;
                }
                else
                {
                    var sum = new Vec3d();
                    var count = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += v1.Position;
                        count++;
                    }

                    double t = 1.0 / count;
                    var move = sum * t - v0.Position;

                    // apply to central vertex
                    v0.MoveSum += move * weight;
                    v0.WeightSum += weight;

                    // distribute negated to neighbours
                    move *= -t;
                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        v1.MoveSum += move * weight;
                        v1.WeightSum += weight;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="weight"></param>
        private void SphereCollide(double radius, double weight)
        {
            UpdateGrid(radius);

            var rad2 = radius * 2.0;
            var rad2Sqr = rad2 * rad2;

            // insert vertices
            foreach (var v in _verts)
                _grid.Insert(v.Position, v);

            // search from each vertex and handle collisions
            Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = _verts[i];
                    var p0 = v0.Position;

                    var moveSum = new Vec3d();
                    int count = 0;

                    _grid.Search(new Domain3d(p0, rad2), v1 =>
                    {
                        var move = v1.Position - p0;
                        var d = move.SquareLength;

                        if (d < rad2Sqr && d > 0.0)
                        {
                            moveSum += move * ((1.0 - rad2 / Math.Sqrt(d)) * 0.5);
                            count++;
                        }

                        return true;
                    });
                    
                    if (count == 0) continue;
              
                    v0.MoveSum += moveSum * weight;
                    v0.WeightSum += weight;
                }
            });

            _grid.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateGrid(double radius)
        {
            if (_grid == null)
            {
                _grid = new HashGrid3d<V>((int)(_verts.Count * TargetLoadFactor), radius * TargetBinScale);
                return;
            }

            int minCount = (int)(_verts.Count * TargetLoadFactor * 0.5);
            if (_grid.BinCount < minCount)
                _grid.Resize((int)(_verts.Count * TargetLoadFactor));

            _grid.BinScale = radius * TargetBinScale;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private void UpdateVertices()
        {
            double timeStep = _settings.TimeStep;
            double damping = _settings.Damping;

            Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = _verts[i];
                    v.Velocity *= damping;

                    double w = v.WeightSum;
                    if (w > 0.0) v.Velocity += v.MoveSum * (timeStep / w);
                    v.Position += v.Velocity * timeStep;

                    v.MoveSum = new Vec3d();
                    v.WeightSum = 0.0;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        private void ProjectToFeatures()
        {
            Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = _verts[i];

                    var p = v.Position;
                    int fi = v.FeatureIndex;

                    // snap to feature if one exists
                    if (fi != -1)
                        v.Position = _features[v.FeatureIndex].ClosestPoint(v.Position);

                    // snap to target if one exists
                    if (_target != null)
                        v.Position = _target.ClosestPoint(v.Position);
                }
            });
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        private void PullToFeatures(double weight)
        {
            Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = _verts[i];
                    if (v.IsRemoved || v.FeatureIndex == -1) continue;

                    var p = v.Position;
                    int fi = v.FeatureIndex;

                    if (fi == -1) continue;
                    v.MoveSum += (_features[fi].ClosestPoint(p) - p) * weight;
                    v.WeightSum += weight;
                }
            });
        }

        #endregion


        #region Topology

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private void Refine()
        {
            UpdateMaxLengths(true);
            SplitEdges(_hedges.Count);
        }


        /*
        /// <summary>
        /// Splits long edges
        /// </summary>
        private void SplitEdges(int count)
        {
            _vertTag++;

            for (int i = 0; i < count; i += 2)
            {
                var he = _hedges[i];
                if (he.IsRemoved) continue;

                var v0 = he.Start;
                var v1 = he.End;

                // don't split edge that spans between 2 different features
                var fi0 = v0.FeatureIndex;
                var fi1 = v1.FeatureIndex;
                // if (fi0 > -1 && fi1 > -1 && fi0 != fi1) continue;
                
                var p0 = v0.Position;
                var p1 = v1.Position;

                // split edge if length exceeds max
                if (p0.SquareDistanceTo(p1) > he.MaxLength * he.MaxLength)
                {
                    var v2 = _mesh.SplitEdge(he).Start;

                    // set attributes of new vertex
                    v2.Position = (v0.Position + v1.Position) * 0.5;
                    // v2.FeatureIndex = Math.Min(fi0, fi1);

                    // if same feature
                    v2.FeatureIndex = (fi0 == fi1) ? -1 : Math.Min(fi0, fi1);
                }
            }
        }
        */


        /// <summary>
        /// Splits long edges
        /// </summary>
        private void SplitEdges(int count)
        {
            _vertTag++;

            for (int i = 0; i < count; i += 2)
            {
                var he = _hedges[i];
                if (he.IsRemoved) continue;

                var v0 = he.Start;
                var v1 = he.End;
                
                var fi = GetSplitFeature(v0.FeatureIndex, v1.FeatureIndex);
                if (fi < -1) continue; // don't split between different features
                
                var p0 = v0.Position;
                var p1 = v1.Position;

                // split edge if length exceeds max
                if (p0.SquareDistanceTo(p1) > he.MaxLength * he.MaxLength)
                {
                    var v2 = _mesh.SplitEdge(he).Start;

                    // set attributes of new vertex
                    v2.Position = (v0.Position + v1.Position) * 0.5;
                    v2.FeatureIndex = fi;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fi0"></param>
        /// <param name="fi1"></param>
        /// <returns></returns>
        private static int GetSplitFeature(int fi0, int fi1)
        {
            if (fi0 == -1 || fi1 == -1) return -1; // only one on feature
            if (fi0 == fi1) return fi0; // both on same feature
            return -2; // on different features
        }

        #endregion


        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        void UpdateMaxLengths(bool parallel = false)
        {
            var lengthRange = _settings.LengthRange;

            // set length targets to default if no field
            if (_lengthField == null)
            {
                var min = lengthRange.Min;

                for (int i = 0; i < _hedges.Count; i += 2)
                    _hedges[i].MaxLength = min;

                return;
            }

            // evaluate field
            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var he = _hedges[i << 1];
                    if (he.IsRemoved) continue;

                    var p = (he.Start.Position + he.End.Position) * 0.5;
                    he.MaxLength = lengthRange.Evaluate(_lengthField.ValueAt(p));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _hedges.Count >> 1), body);
            else
                body(Tuple.Create(0, _hedges.Count >> 1));
        }

        #endregion
    }
}
