
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using SpatialSlur.Collections;
using SpatialSlur.Fields;
using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.SlurMath;

namespace SpatialSlur.Tools
{
    using V = LoopGrower.HeMesh.Vertex;
    using E = LoopGrower.HeMesh.Halfedge;
    using F = LoopGrower.HeMesh.Face;

    /// <summary>
    /// 
    /// </summary>
    public static class LoopGrower
    {
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Solver
        {
            #region Static Members

            private const double _radiusToGridScale = 5.0;

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="TV"></typeparam>
            /// <typeparam name="TE"></typeparam>
            /// <typeparam name="TF"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="target"></param>
            /// <param name="features"></param>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static Solver Create<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, ISurfaceFeature target, IEnumerable<IFeature> features = null, Settings settings = null)
                where TV : HeMesh<TV, TE, TF>.Vertex, IPosition3d
                where TE : HeMesh<TV, TE, TF>.Halfedge
                where TF : HeMesh<TV, TE, TF>.Face
            {
                var copy = HeMesh.Factory.CreateCopy(mesh, (v0, v1) => v0.Position = v1.Position, null, null);
                return new Solver(copy, target, features, settings);
            }

            #endregion


            //
            // simulation mesh
            //

            private HeMesh _mesh;
            private NodeList<V> _verts;
            private NodeList<E> _hedges;
            private HashGrid3d<V> _grid;

            //
            // constraint objects
            //

            private ISurfaceFeature _target;
            private List<IFeature> _features;
            private IField3d<double> _growthField;
            private IField3d<Vector3d> _directionField;

            //
            // simulation settings
            //

            private Settings _settings;
            private int _stepCount = 0;
            private int _vertTag = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="target"></param>
            /// <param name="features"></param>
            /// <param name="settings"></param>
            public Solver(HeMesh mesh, ISurfaceFeature target, IEnumerable<IFeature> features = null, Settings settings = null)
            {
                if (features == null)
                    features = Enumerable.Empty<IFeature>();

                _mesh = mesh;
                _mesh.Compact();

                _verts = _mesh.Vertices;
                _hedges = _mesh.Halfedges;
                _settings = settings ?? new Settings();
                _stepCount = 0;

                _target = target;
                InitFeatures(features);

                // start on features
                ProjectToFeatures();
            }


            /// <summary>
            /// 
            /// </summary>
            private void InitFeatures(IEnumerable<IFeature> features)
            {
                // TODO update as per DynamicRemesher.Solver

                _features = new List<IFeature>();
                var tolSqr = Square(_settings.FeatureTolerance);

                // create features
                foreach (var f in features)
                {
                    // TODO assign point features separately

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

                double Square(double x)
                {
                    return x * x;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            public HeMesh Mesh
            {
                get { return _mesh; }
            }


            /// <summary>
            /// 
            /// </summary>
            public ISurfaceFeature Target
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
            public IField3d<double> GrowthField
            {
                get { return _growthField; }
                set { _growthField = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public IField3d<Vector3d> DirectionField
            {
                get { return _directionField; }
                set { _directionField = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public Settings Settings
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
                if (++_stepCount % _settings.RefineFrequency == 0) Refine();
                CalculateProjections();
                UpdateVertices();
                ProjectToFeatures(); // hard constraint
            }


            #region Dynamics

            /// <summary>
            /// Calculates all projections applies to mesh vertices.
            /// </summary>
            /// <returns></returns>
            protected virtual void CalculateProjections()
            {
                if (_stepCount % _settings.CollideFrequency == 0)
                    SphereCollideParallel(_settings.CollideRadius, 1.0);

                LaplacianFair(_settings.SmoothWeight);
                //LaplacianFair(_settings.SmoothWeight, _settings.SmoothWeight * 2.0);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="weight"></param>
            protected void LaplacianFair(double weight)
            {
                for (int i = 0; i < _verts.Count; i++)
                {
                    var v0 = _verts[i];
                    if (v0.IsUnused) continue;

                    // calculate graph laplacian
                    var sum = new Vector3d();
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


            /*
            /// <summary>
            /// 
            /// </summary>
            /// <param name="weightInterior"></param>
            /// <param name="weightBoundary"></param>
            private void LaplacianFair(double weightInterior, double weightBoundary)
            {
                for (int i = 0; i < _verts.Count; i++)
                {
                    var v0 = _verts[i];
                    if (v0.IsUnused) continue;

                    if (v0.IsBoundary)
                    {
                        var he = v0.FirstOut;

                        var v1 = he.PrevInFace.Start;
                        var v2 = he.Next.Start;
                        var move = (v1.Position + v2.Position) * 0.5 - v0.Position;

                        // apply to central vertex
                        v0.MoveSum += move * weightBoundary;
                        v0.WeightSum += weightBoundary;

                        // distribute negated to neighbours
                        move *= -0.5;
                        v1.MoveSum += move * weightBoundary;
                        v1.WeightSum += weightBoundary;
                        v2.MoveSum += move * weightBoundary;
                        v2.WeightSum += weightBoundary;
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
                        v0.MoveSum += move * weightInterior;
                        v0.WeightSum += weightInterior;

                        // distribute negated to neighbours
                        move *= -t;
                        foreach (var v1 in v0.ConnectedVertices)
                        {
                            v1.MoveSum += move * weightInterior;
                            v1.WeightSum += weightInterior;
                        }
                    }
                }
            }
            */


            /// <summary>
            /// 
            /// </summary>
            /// <param name="radius"></param>
            /// <param name="weight"></param>
            protected void SphereCollideParallel(double radius, double weight)
            {
                UpdateGrid(radius);

                // insert vertices
                foreach (var v in _verts)
                    _grid.Insert(v.Position, v);

                // search from each vertex and handle collisions
                Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
                {
                    var rad2 = radius * 2.0;
                    var rad2Sqr = rad2 * rad2;

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v0 = _verts[i];
                        var p0 = v0.Position;

                        var moveSum = new Vector3d();
                        int count = 0;

                        foreach(var v1 in _grid.Search(new Interval3d(p0, rad2)))
                        {
                            var move = v1.Position - p0;
                            var d = move.SquareLength;

                            if (d < rad2Sqr && d > 0.0)
                            {
                                moveSum += move * (1.0 - rad2 / Math.Sqrt(d));
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            v0.MoveSum += moveSum * weight * 0.5;
                            v0.WeightSum += weight;
                        }
                    }
                });

                _grid.Clear();
            }


            /// <summary>
            ///
            /// </summary>
            /// <param name="weight"></param>
            /// <returns></returns>
            protected void PullToFeatures(double weight)
            {
                Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = _verts[i];
                        if (v.IsUnused || v.FeatureIndex == -1) continue;

                        var p = v.Position;
                        int fi = v.FeatureIndex;

                        if (fi != 1)
                            ApplyMove(v, _features[fi]);

                        if (_target != null)
                            ApplyMove(v, _target);
                    }
                });

                void ApplyMove(V vertex, IFeature feature)
                {
                    var p = vertex.Position;
                    vertex.MoveSum += (feature.ClosestPoint(p) - p) * weight;
                    vertex.WeightSum += weight;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private void UpdateGrid(double radius)
            {
                if (_grid == null)
                    _grid = new HashGrid3d<V>(_verts.Count);

                _grid.Scale = radius * _radiusToGridScale;
            }


            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            private void UpdateVertices()
            {
                UpdateGrowthRate();

                double timeStep = _settings.TimeStep;
                double damping = _settings.Damping;

                Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = _verts[i];
                        v.Velocity *= (1.0 - damping);

                        double w = v.WeightSum;
                        if (w > 0.0) v.Velocity += v.MoveSum * (v.GrowthRate * timeStep / w);
                        v.Position += v.Velocity * timeStep;

                        v.MoveSum = new Vector3d();
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

                        // project to feature if one exists
                        if (fi != -1)
                            v.Position = _features[v.FeatureIndex].ClosestPoint(v.Position);

                        // project to target if one exists
                        if (_target != null)
                            v.Position = _target.ClosestPoint(v.Position);
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
                SplitEdges(_hedges.Count);
            }


            /// <summary>
            /// Splits long edges
            /// </summary>
            private void SplitEdges(int count)
            {
                _vertTag++;

                var maxLengthSqr = _settings.CollideRadius * 2.0 * 0.75;
                maxLengthSqr *= maxLengthSqr;

                for (int i = 0; i < count; i += 2)
                {
                    var he = _hedges[i];

                    var v0 = he.Start;
                    var v1 = he.End;

                    var fi = GetSplitFeature(v0.FeatureIndex, v1.FeatureIndex);
                    if (fi < -1) continue; // don't split between different features

                    var p0 = v0.Position;
                    var p1 = v1.Position;

                    // split edge if length exceeds max
                    if (p0.SquareDistanceTo(p1) > maxLengthSqr)
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
            void UpdateGrowthRate()
            {
                // sample growth rate field or set to default
                if (_growthField == null)
                {
                    foreach (var v in _verts)
                        v.GrowthRate = 1.0;
                }
                else
                {
                    Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
                     {
                         for (int i = range.Item1; i < range.Item2; i++)
                         {
                             var v = _verts[i];
                             if (v.IsUnused) continue;
                             v.GrowthRate = _growthField.ValueAt(v.Position);
                         }
                     });
                }

                // sample direction field
                if (_directionField != null)
                {
                    var align = 1.0 - _settings.Alignment;

                    Parallel.ForEach(Partitioner.Create(0, _verts.Count), range =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            var v = _verts[i];
                            if (v.IsUnused || !v.IsDegree2) continue;

                            var n = _directionField.ValueAt(v.Position);
                            var d = v.MoveSum;
                            var dx = Vector3d.Project(d, n);
                            v.MoveSum = dx + (d - dx) * align; // scale down perpendicular component
                    }
                    });
                }
            }

            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Settings
        {
            private double _smoothWeight = 1.0;
            private double _collideRad = 1.0;
            private double _alignment = 0.5;
            private double _featureTolerance = 1.0e-4;
            private double _damping = 0.5;
            private double _timeStep = 1.0;

            private int _collideFreq = 3;
            private int _refineFreq = 5;


            /// <summary>
            /// 
            /// </summary>
            public double SmoothWeight
            {
                get { return _smoothWeight; }
                set { _smoothWeight = Math.Max(value, 0.0); }
            }


            /// <summary>
            /// 
            /// </summary>
            public double CollideRadius
            {
                get { return _collideRad; }
                set { _collideRad = Math.Max(value, 0.0); }
            }


            /// <summary>
            /// 
            /// </summary>
            public double Alignment
            {
                get { return _alignment; }
                set { _alignment = SlurMath.Saturate(value); }
            }
            

            /// <summary>
            /// 
            /// </summary>
            public double FeatureTolerance
            {
                get { return _featureTolerance; }
                set { _featureTolerance = Math.Max(value, 0.0); }
            }


            /// <summary>
            /// 
            /// </summary>
            public double Damping
            {
                get { return _damping; }
                set { _damping = SlurMath.Saturate(value); }
            }


            /// <summary>
            /// 
            /// </summary>
            public double TimeStep
            {
                get { return _timeStep; }
                set { _timeStep = Math.Max(value, 0.0); }
            }


            /// <summary>
            /// 
            /// </summary>
            public int CollideFrequency
            {
                get { return _collideFreq; }
                set { _collideFreq = Math.Max(value, 1); }
            }


            /// <summary>
            /// 
            /// </summary>
            public int RefineFrequency
            {
                get { return _refineFreq; }
                set { _refineFreq = Math.Max(value, 1); }
            }
        }


        /// <summary>
        /// Contains HeMesh element classes used in dynamic remeshing
        /// </summary>
        [Serializable]
        public class HeMesh : HeMesh<V, E, F>
        {
            #region Nested Types
            
            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Vertex : HeMesh<V, E, F>.Vertex, IPosition3d
            {
                /// <summary></summary>
                public Vector3d Position;
                /// <summary></summary>
                public Vector3d Velocity;
                
                internal Vector3d MoveSum;
                internal double WeightSum;
                private double _growthRate;
                private int _featureIndex = -1;


                /// <summary></summary>
                public int FeatureIndex
                {
                    get => _featureIndex;
                    internal set => _featureIndex = value;
                }


                /// <summary>
                /// 
                /// </summary>
                public double GrowthRate
                {
                    get => _growthRate;
                    set => _growthRate = Saturate(value);
                }


                #region Explicit Interface Implementations

                Vector3d IPosition3d.Position
                {
                    get { return Position; }
                    set { Position = value; }
                }

                #endregion
            }


            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Halfedge : HeMesh<V, E, F>.Halfedge
            {
            }


            /// <summary>
            ///
            /// </summary>
            [Serializable]
            public new class Face : HeMesh<V, E, F>.Face
            {
            }

            #endregion


            #region Static Members

            /// <summary></summary>
            public static readonly HeMeshFactory Factory = new HeMeshFactory();

            #endregion


            /// <summary>
            /// 
            /// </summary>
            public HeMesh()
                : base()
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="vertexCapacity"></param>
            /// <param name="hedgeCapacity"></param>
            /// <param name="faceCapacity"></param>
            public HeMesh(int vertexCapacity, int hedgeCapacity, int faceCapacity)
                : base(vertexCapacity, hedgeCapacity, faceCapacity)
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            public HeMesh(HeMesh other)
            {
                Append(other);
            }


            
            /// <inheritdoc />
            protected sealed override V NewVertex()
            {
                return new V();
            }


            /// <inheritdoc />
            protected sealed override E NewHalfedge()
            {
                return new E();
            }


            /// <inheritdoc />
            protected sealed override F NewFace()
            {
                return new F();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class HeMeshFactory : HeMeshFactory<HeMesh, V, E, F>
        {
            /// <inheritdoc />
            public sealed override HeMesh Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
            {
                return new HeMesh(vertexCapacity, halfedgeCapacity, faceCapacity);
            }
        }
    }
}