using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;

/*
 * Notes
 * 
 * Dynamic remeshing based on implemetation described in
 * https://nccastaff.bournemouth.ac.uk/jmacey/MastersProjects/MSc15/08Tanja/report.pdf
 * 
 * Other references
 * http://graphics.stanford.edu/courses/cs468-12-spring/LectureSlides/13_Remeshing1.pdf
 */

namespace SpatialSlur.SlurRhino.Remesher
{
    using V = HeMeshSim.Vertex;
    using E = HeMeshSim.Halfedge;
    using F = HeMeshSim.Face;


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DynamicRemesher
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="features"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static DynamicRemesher Create<TV, TE, TF>(HeMeshBase<TV, TE, TF> mesh, MeshFeature target, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
            where TV : HeVertex<TV, TE, TF>, IVertex3d
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var copy = HeMeshSim.Factory.CreateCopy(mesh, (v0, v1) => v0.Position = v1.Position, delegate { }, delegate { });
            return new DynamicRemesher(copy, target, features, tolerance);
        }

        #endregion


        private const double _maxLengthFactor = 4.0 / 3.0;
        private const double _minLengthFactor = 4.0 / 5.0;

        //
        // simulation mesh
        //
        
        private HeMeshSim _mesh;
        private HeElementList<V> _verts;
        private HeElementList<E> _hedges;
        private HeElementList<F> _faces;

        //
        // constraint objects
        //

        private MeshFeature _target;
        private List<IFeature> _features;
        private IField3d<double> _lengthField;

        //
        // simulation settings
        //

        private DynamicRemesherSettings _settings;
        private int _stepCount = 0;
        private int _refineCount = 0;
        private int _vertTag = int.MinValue;
        private int _faceTag = int.MinValue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="target"></param>
        /// <param name="features"></param>
        /// <param name="tolerance"></param>
        public DynamicRemesher(HeMeshSim mesh, MeshFeature target, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
        {
            _mesh = mesh;
            _verts = _mesh.Vertices;
            _hedges = _mesh.Halfedges;
            _faces = _mesh.Faces;

            // triangulate all faces starting with the shortest diagonal
            Func<F, E> getStart = f => f.Halfedges.SelectMin(he => he.Start.Position.SquareDistanceTo(he.NextInFace.End.Position));
            _mesh.TriangulateFaces(FaceTriangulators.Strip.Create(mesh, getStart));

            // initialize features
            _target = target;
            InitFeatures(features, tolerance);

            _settings = new DynamicRemesherSettings();
            _stepCount = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitFeatures(IEnumerable<IFeature> features, double tolerance)
        {
            _features = new List<IFeature>();
            var tt = tolerance * tolerance;

            // create features
            foreach (var f in features)
            {
                int index = _features.Count;
                _features.Add(f);

                // if vertex is close enough, assign feature
                foreach (var v in _verts)
                {
                    if (v.FeatureIndex != -1) continue; // skip if already assigned
          
                    var p = v.Position;
                    if (p.SquareDistanceTo(f.ClosestPoint(p)) < tt)
                        v.FeatureIndex = index;
                }
            }
            
            // create boundary features
            foreach (var he0 in _mesh.GetHoles())
            {
                var poly = new Polyline(he0.CirculateFace.Select(he => he.Start.Position.ToPoint3d()));
                poly.Add(poly[0]);

                int index = _features.Count;
                _features.Add(new MeshFeature(RhinoFactory.Mesh.CreateExtrusion(poly, new Vector3d())));

                // set verts that haven't been assigned
                foreach (var v in he0.CirculateFace.Select(he1 => he1.Start))
                    if (v.FeatureIndex == -1) v.FeatureIndex = index;
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        private void InitFeatures(IEnumerable<IFeature> features, double tolerance)
        {
            _features = new List<IFeature>();
            var tt = tolerance * tolerance;

            // create default boundary features
            CreateBoundaryFeatures();

            // override with additional features
            foreach (var f in features)
            {
                int index = _features.Count;
                _features.Add(f);

                // if vertex is close enough, assign feature
                foreach (var v in _verts)
                {
                    var p = v.Position;
                    if (p.SquareDistanceTo(f.ClosestPoint(p)) < tt)
                        v.FeatureIndex = index;
                }
            }
        }


        /// <summary>
        /// Returns the number of boundaries in the mesh;
        /// </summary>
        private void CreateBoundaryFeatures()
        {
            foreach (var he0 in _mesh.GetHoles())
            {
                var poly = new Polyline(he0.CirculateFace.Select(he => he.Start.Position.ToPoint3d()));
                poly.Add(poly[0]);

                int index = _features.Count;
                _features.Add(new MeshFeature(MeshUtil.Extrude(poly, new Vector3d())));

                // set vertex feature ids
                foreach (var he1 in he0.CirculateFace)
                    he1.Start.FeatureIndex = index;
            }
        }
        */


        /// <summary>
        /// 
        /// </summary>
        public HeMeshBase<V, E, F> Mesh
        {
            get { return _mesh; }
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
        public List<IFeature> Features
        {
            get { return _features; }
            set { _features = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public DynamicRemesherSettings Settings
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
            }
        }


        #region Dynamics

        /// <summary>
        /// Calculates all forces in the system
        /// </summary>
        /// <returns></returns>
        private void CalculateForces()
        {
            _mesh.GetVertexNormals(v => v.Position, (v, n) => v.Normal = n, true);


            // TODO Add NaN checks in force calcs?
            PullToFeatures(_settings.FeatureWeight);
            TangentialSmooth(_settings.SmoothWeight);
            // TangentIncircles(_settings.TangentWeight);
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
                    if (v.IsRemoved) continue;

                    var p = v.Position;
                    var fi = v.FeatureIndex;

                    if(fi == -1)
                    {
                        v.MoveSum += (_target.ClosestPoint(p) - p);
                        v.WeightSum += 1.0;
                    }
                    else
                    {
                        v.MoveSum += (_features[fi].ClosestPoint(p) - p) * weight;
                        v.WeightSum += weight;
                    }
                }
            });
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        private void TangentialSmooth(double weight)
        {
            Func<V, Vec3d> getPosition = v => v.Position;

            Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = _verts[i];
                    if (v.IsRemoved) continue;

                    Vec3d mean = v.ConnectedVertices.Mean(getPosition);
                    Vec3d move = Vec3d.Reject(mean - v.Position, v.Normal);

                    v.MoveSum += move * weight;
                    v.WeightSum += weight;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        private void TangentIncircles(double weight)
        {
            for (int i = 0; i < _hedges.Count; i += 2)
            {
                var he = _hedges[i];
                if (he.IsRemoved || he.IsBoundary) continue;

                var v0 = he.PrevInFace.Start;
                var v1 = he.Start;
                he = he.Twin;
                var v2 = he.PrevInFace.Start;
                var v3 = he.Start;

                // equalize sum of opposite edges
                Vec3d d0 = v1.Position - v0.Position;
                Vec3d d1 = v2.Position - v1.Position;
                Vec3d d2 = v3.Position - v2.Position;
                Vec3d d3 = v0.Position - v3.Position;

                double m0 = d0.Length;
                double m1 = d1.Length;
                double m2 = d2.Length;
                double m3 = d3.Length;

                double sum0 = m0 + m2;
                double sum1 = m1 + m3;

                // compute projection magnitude as deviation from mean
                double mean = (sum0 + sum1) * 0.5;
                sum0 = (sum0 - mean) * 0.125; // 0.25 / 2 (2 deltas applied per index)
                sum1 = (sum1 - mean) * 0.125;

                // scale deltas
                d0 *= sum0 / m0;
                d1 *= sum1 / m1;
                d2 *= sum0 / m2;
                d3 *= sum1 / m3;

                // cache deltas
                v0.MoveSum += (d0 - d3) * weight;
                v0.WeightSum += weight;

                v1.MoveSum += (d1 - d0) * weight;
                v1.WeightSum += weight;

                v2.MoveSum += (d2 - d1) * weight;
                v2.WeightSum += weight;

                v3.MoveSum += (d3 - d2) * weight;
                v3.WeightSum += weight;
            }
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

        #endregion


        #region Topology

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private void Refine()
        {
            // update attributes
            UpdateTargetLengths(true);
      
            // alternate between split and collapse
            if ((_refineCount++ & 1) == 0)
                SplitEdges(_hedges.Count);
            else
                CollapseEdges(_hedges.Count);

            SpinEdges();
        }


        /// <summary>
        /// Splits long edges
        /// </summary>
        private void SplitEdges(int count)
        {
            double tol = _settings.LengthTolerance;
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
                double maxLength = he.TargetLength * _maxLengthFactor * (1.0 + tol);
                maxLength *= maxLength;
                
                if (p0.SquareDistanceTo(p1) > maxLength)
                {
                    var v2 = _mesh.SplitEdgeFace(he).Start;

                    // set attributes of new vertex
                    v2.Position = (p0 + p1) * 0.5;
                    v2.FeatureIndex = fi;
                    v2.RefineTag = _vertTag;
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


        /// <summary>
        /// Collapses short edges
        /// </summary>
        private void CollapseEdges(int count)
        {
            double tol = _settings.LengthTolerance;
            bool compact = false;

            for (int i = 0; i < count; i += 2)
            {
                var he = _hedges[i];
                if (he.IsRemoved || he.IsBridge) continue;

                var v0 = he.Start;
                var v1 = he.End;

                // don't collapse between different features
                if (v0.FeatureIndex != v1.FeatureIndex) continue;

                // don't collapse to/from tagged vertices
                if (v0.RefineTag == _vertTag || v1.RefineTag == _vertTag) continue;

                var p0 = v0.Position;
                var p1 = v1.Position;

                // collapse edge if length is less than min
                double minLength = he.TargetLength * _minLengthFactor * (1.0 - tol);
                minLength *= minLength;

                if (p0.SquareDistanceTo(p1) < minLength)
                {
                    // update attributes if collapse is successful
                    if(_mesh.CollapseEdge(he))
                    {
                        v1.Position = (p0 + p1) * 0.5;
                        v1.RefineTag = _vertTag;
                        compact = true;
                    }
                }
            }

            // compact the mesh if any edges were removed
            if (compact) _mesh.Compact();
        }


        /// <summary>
        /// Attempts to equalize the valence of vertices by spinning interior edges
        /// </summary>
        /// <returns></returns>
        private void SpinEdges()
        {
            _mesh.GetVertexDegrees((v, d) => v.DegreeCached = d, true);
            _faceTag++;

            for (int i = 0; i < _hedges.Count; i += 2)
            {
                var he0 = _hedges[i];
                if (he0.IsRemoved || he0.IsBoundary) continue;
                var he1 = _hedges[i + 1];

                var f0 = he0.Face;
                var f1 = he1.Face;

                // only allow 1 edge spin per face
                if (f0.RefineTag == _faceTag || f1.RefineTag == _faceTag) continue;

                var v0 = he0.Start;
                var v2 = he1.Start;
                int fi0 = v0.FeatureIndex;

                // don't allow spin if start and end vertex belong to the same feature
                if (fi0 > -1 && fi0 == v2.FeatureIndex) continue;

                var v1 = he0.PrevInFace.Start;
                var v3 = he1.PrevInFace.Start;
                int vi1 = v1.Index;
                int vi3 = v3.Index;

                // current valence error
                int d0 = v0.DegreeCached - (v0.IsBoundary ? 4 : 6);
                int d1 = v1.DegreeCached - (v1.IsBoundary ? 4 : 6);
                int d2 = v2.DegreeCached - (v2.IsBoundary ? 4 : 6);
                int d3 = v3.DegreeCached - (v3.IsBoundary ? 4 : 6);
                int err0 = d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3;

                // flipped valence error
                d0--; d1++; d2--; d3++;
                int err1 = d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3;

                // flip edge if it results in less error
                if (err1 < err0 && _mesh.SpinEdge(he0))
                {
                    v0.DegreeCached--; v2.DegreeCached--;
                    v1.DegreeCached++; v3.DegreeCached++;
                    f0.RefineTag = f1.RefineTag = _faceTag; // tag faces
                }
            }
        }

        #endregion


        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        void UpdateTargetLengths(bool parallel = false)
        {
            var d = _settings.LengthRange;

            // set length targets to default if no field
            if (_lengthField == null)
            {
                for (int i = 0; i < _hedges.Count; i += 2)
                    _hedges[i].TargetLength =  d.A;

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
                    he.TargetLength = d.Evaluate(_lengthField.ValueAt(p));
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
