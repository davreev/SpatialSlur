using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using SpatialSlur.SlurMesh;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurTools.Features;

/*
 * Notes
 */

namespace SpatialSlur.SlurTools
{
    using V = DynamicRemesher.HeMesh.Vertex;
    using E = DynamicRemesher.HeMesh.Halfedge;
    using F = DynamicRemesher.HeMesh.Face;

    /// <summary>
    /// 
    /// </summary>
    public static class DynamicRemesher
    {
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Solver
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
            public static Solver Create<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, ISurfaceFeature target, IEnumerable<IFeature> features = null, Settings settings = null)
                where TV : HeMesh<TV,TE,TF>.Vertex, IPosition3d
                where TE : HeMesh<TV, TE, TF>.Halfedge
                where TF : HeMesh<TV, TE, TF>.Face
            {
                var copy = HeMesh.Factory.CreateCopy(mesh, (v0, v1) => v0.Position = v1.Position, delegate { }, delegate { });
                return new Solver(copy, target, features, settings);
            }

            #endregion


            private const double _maxLengthFactor = 4.0 / 3.0;
            private const double _minLengthFactor = 4.0 / 5.0;

            //
            // simulation mesh
            //

            private HeMesh _mesh;

            //
            // constraint objects
            //

            private ISurfaceFeature _target;
            private List<IFeature> _features;
            private IField3d<double> _lengthField;

            //
            // simulation settings
            //

            private Settings _settings;
            private int _stepCount = 0;
            private int _refineCount = 0;

            //
            // misc
            //

            private int _vertStamp = int.MinValue;
            private int _faceStamp = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="target"></param>
            /// <param name="features"></param>
            /// <param name="tolerance"></param>
            public Solver(HeMesh mesh, ISurfaceFeature target, IEnumerable<IFeature> features = null, Settings settings = null)
            {
                if (features == null)
                    features = Enumerable.Empty<IFeature>();

                _mesh = mesh;
                _mesh.Compact();

                _settings = settings ?? new Settings();
                _stepCount = 0;

                // triangulate all faces starting with the shortest diagonal
                _mesh.TriangulateFaces(FaceTriangulators.Strip.CreateFromMin(mesh, he =>
                {
                    var p0 = he.Start.Position;
                    var p1 = he.Next.End.Position;
                    return p0.SquareDistanceTo(p1);
                }));

                // initialize features
                _target = target;
                InitFeatures(features);
            }


            #region Initialization

            /// <summary>
            /// 
            /// </summary>
            private void InitFeatures(IEnumerable<IFeature> features)
            {
                _features = new List<IFeature>();
                var tolSqr = Square(_settings.FeatureTolerance);

                var verts = Mesh.Vertices;
                var edges = Mesh.Edges;

                // assign features to coincident vertices and edges
                foreach (var f in features)
                {
                    int fi = _features.Count;
                    _features.Add(f);
                    _vertStamp++;

                    // if vertex is close enough, assign feature
                    foreach (var v in verts)
                    {
                        var p = v.Position;

                        if (p.SquareDistanceTo(f.ClosestPoint(p)) < tolSqr)
                        {
                            v.Stamp = _vertStamp; // mark as within range of current feature
                            v.FeatureCount++;

                            // assign feature if none assigned or if it outranks the one already assigned
                            if (!v.IsFeature || f.Rank < _features[v.FeatureIndex].Rank)
                                v.FeatureIndex = fi;
                        }
                    }

                    // if mid point of edge is also close enough, assign feature
                    foreach (var he in edges)
                    {
                        var v0 = he.Start;
                        var v1 = he.End;

                        if (v0.Stamp == _vertStamp && v1.Stamp == _vertStamp)
                        {
                            var p = (v0.Position + v1.Position) * 0.5;

                            if (p.SquareDistanceTo(f.ClosestPoint(p)) < tolSqr)
                                he.FeatureIndex = fi;
                        }
                    }
                }

                // InitBoundaryFeatures();
                FixVertices();

                double Square(double x)
                {
                    return x * x;
                }
            }


            /*
            /// <summary>
            /// Create and assign boundary features
            /// </summary>
            private void InitBoundaryFeatures()
            {
                foreach (var he0 in _mesh.GetHoles())
                {
                    var f = CreateBoundaryFeature(he0);
                    int fi = _features.Count;
                    _features.Add(f);

                    // assign feature to any unassigned edges or verts in loop
                    foreach (var he1 in he0.Circulate)
                    {
                        var v = he1.Start;
                        v.FeatureCount++;

                        if (!he1.IsFeature)
                            he1.FeatureIndex = fi;

                        if (!v.IsFeature)
                            v.FeatureIndex = fi;
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private MeshFeature CreateBoundaryFeature(E hedge)
            {
                var poly = new Polyline(hedge.Circulate.Select(he => (Point3d)he.Start.Position));
                poly.Add(poly[0]);

                return new MeshFeature(RhinoFactory.Mesh.CreateExtrusion(poly, new Vector3d()));
            }
            */


            /// <summary>
            /// 
            /// </summary>
            private void FixVertices()
            {
                var verts = _mesh.Vertices;

                // fix vertex if on multiple features or if a degree 1 feature
                foreach (var v in verts)
                {
                    switch (v.FeatureCount)
                    {
                        case 0:
                            break;
                        case 1:
                            if (CheckFixed(v)) v.Fix();
                            break;
                        default:
                            v.Fix();
                            break;
                    }
                }

                // returns true if v has one incident feature edge
                bool CheckFixed(V v)
                {
                    int n = 0;

                    foreach (var he in v.OutgoingHalfedges)
                        if (he.IsFeature) n++;

                    return n == 1;
                }
            }

            #endregion


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
            }


            /// <summary>
            /// 
            /// </summary>
            public IField3d<double> LengthField
            {
                get { return _lengthField; }
                set { _lengthField = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public Settings Settings
            {
                get { return _settings; }
                set { _settings = value ?? throw new ArgumentNullException(); }
            }


            /// <summary>
            /// 
            /// </summary>
            public int StepCount
            {
                get { return _stepCount; }
            }


            #region Dynamics

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            public void Step()
            {
                TangentialSmooth(1.0);
                PullToFeatures(_settings.FeatureWeight);
                UpdateVertices(_settings.TimeStep, _settings.Damping);

                if (_stepCount++ % _settings.RefineFrequency == 0) Refine();
            }


            /// <summary>
            ///
            /// </summary>
            /// <param name="weight"></param>
            /// <returns></returns>
            private void UniformSmooth(double weight)
            {
                Parallel.ForEach(Partitioner.Create(0, _mesh.Vertices.Count), range =>
                {
                    var verts = _mesh.Vertices;

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = verts[i];
                        if (v.IsFixed) continue;

                        var sum = new Vec3d();
                        var count = 0;

                        foreach (var cv in v.ConnectedVertices)
                        {
                            sum += cv.Position;
                            count++;
                        }

                        v.DeltaSum += (sum / count - v.Position) * weight;
                        v.WeightSum += weight;
                    }
                });
            }


            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            private void TangentialSmooth(double weight)
            {
                Parallel.ForEach(Partitioner.Create(0, _mesh.Vertices.Count), range =>
                {
                    var verts = _mesh.Vertices;

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = verts[i];
                        if (v.IsFixed) continue;

                        // calculate uniform laplacian and vertex normal
                        var norm = new Vec3d();
                        var sum = new Vec3d();
                        int count = 0;

                        foreach (var he in v.OutgoingHalfedges)
                        {
                            norm += he.GetNormal(V.GetPosition);
                            sum += he.End.Position;
                            count++;
                        }

                        var d = sum / count - v.Position;
                        var m = norm.SquareLength;

                        // reject onto normal if valid
                        if (m > 0.0)
                            d -= Vec3d.Dot(d, norm) / m * norm;

                        v.DeltaSum += d * weight;
                        v.WeightSum += weight;
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
                Parallel.ForEach(Partitioner.Create(0, _mesh.Vertices.Count), range =>
                {
                    var verts = _mesh.Vertices;

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = verts[i];
                        var p = v.Position;

                        if (v.IsFeature)
                        {
                            v.DeltaSum += (_features[v.FeatureIndex].ClosestPoint(p) - p) * weight;
                            v.WeightSum += weight;
                        }
                        else if (_target != null)
                        {
                            v.DeltaSum += _target.ClosestPoint(p) - p;
                            v.WeightSum += 1.0;
                        }
                    }
                });
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private void UpdateVertices(double timeStep, double damping)
            {
                Parallel.ForEach(Partitioner.Create(0, _mesh.Vertices.Count), range =>
                {
                    var verts = _mesh.Vertices;

                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var v = verts[i];
                        v.Velocity *= (1.0 - damping);

                        double w = v.WeightSum;
                        if (w > 0.0) v.Velocity += v.DeltaSum * (timeStep / w);
                        v.Position += v.Velocity * timeStep;

                        v.DeltaSum = new Vec3d();
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
                switch (_refineCount++ % 3)
                {
                    case 0:
                        SplitEdges();
                        break;
                    case 1:
                        CollapseEdges();
                        break;
                    case 2:
                        SpinEdges();
                        break;
                }
            }


            /// <summary>
            /// Splits long edges
            /// </summary>
            private void SplitEdges()
            {
                UpdateTargetLengths(true);
                _vertStamp++;

                var hedges = _mesh.Halfedges;
                double tolerance = _maxLengthFactor * (1.0 + _settings.LengthTolerance);
                int count = hedges.Count;

                for (int i = 0; i < count; i += 2)
                {
                    var he0 = hedges[i];
                    if (he0.IsUnused) continue;

                    var v0 = he0.Start;
                    var v1 = he0.End;

                    var p0 = v0.Position;
                    var p1 = v1.Position;
                    double maxLength = he0.TargetLength * tolerance;

                    // split edge if length exceeds max
                    if (p0.SquareDistanceTo(p1) > maxLength * maxLength)
                    {
                        var he1 = _mesh.SplitEdgeFace(he0);
                        var v2 = he1.Start;

                        // inherit edge feature
                        v2.FeatureIndex = he1.FeatureIndex = he0.FeatureIndex;

                        // set attributes of new elements
                        v2.Position = (p0 + p1) * 0.5;
                        v2.Stamp = _vertStamp;
                    }
                }
            }


            /// <summary>
            /// Collapses short edges
            /// </summary>
            private void CollapseEdges()
            {
                UpdateTargetLengths(true);

                var hedges = _mesh.Halfedges;
                double tolerance = _minLengthFactor * (1.0 - _settings.LengthTolerance);
                bool compact = false;

                for (int i = 0; i < hedges.Count; i += 2)
                {
                    var he0 = hedges[i];
                    if (he0.IsUnused || he0.IsBridge) continue;

                    var he1 = he0.Twin;

                    // don't collapse to/from stamped vertices
                    if (he0.Start.Stamp == _vertStamp || he1.Start.Stamp == _vertStamp) continue;

                    if (he0.CanCollapse)
                    {
                        if (TryCollapse(he0, he1, tolerance, he1.CanCollapse))
                            compact = true;
                    }
                    else if (he1.CanCollapse)
                    {
                        if (TryCollapse(he1, he0, tolerance, false))
                            compact = true;
                    }
                }

                // compact the mesh if any edges were removed
                if (compact)
                    _mesh.Compact();
            }


            /// <summary>
            /// 
            /// </summary>
            private bool TryCollapse(E he0, E he1, double tolerance, bool symmetric)
            {
                var v0 = he0.Start;
                var v1 = he1.Start;

                var p0 = v0.Position;
                var p1 = v1.Position;
                double minLength = he0.TargetLength * tolerance;

                if (p0.SquareDistanceTo(p1) < minLength * minLength)
                {
                    var he2 = he0.Next; // removed if collapse is successful
                    var he3 = he1.Next; // ''

                    // update attributes on successful collapse
                    if (_mesh.CollapseEdge(he0))
                    {
                        // update position if symmetric
                        if (symmetric) v1.Position = (p0 + p1) * 0.5;

                        // inherit edge features
                        if (he2.IsFeature) he2.Next.FeatureIndex = he2.FeatureIndex;
                        if (he3.IsFeature) he3.Next.FeatureIndex = he3.FeatureIndex;

                        v1.Stamp = _vertStamp;
                        return true;
                    }
                }

                return false;
            }


            /// <summary>
            /// Attempts to equalize the valence of vertices by spinning interior edges
            /// </summary>
            /// <returns></returns>
            private void SpinEdges()
            {
                _mesh.Vertices.Action(v => v.DegreeCached = v.Degree, true);
                _faceStamp++;

                var hedges = _mesh.Halfedges;

                for (int i = 0; i < hedges.Count; i += 2)
                {
                    var he0 = hedges[i];
                    if (he0.IsUnused || he0.IsBoundary || he0.IsFeature) continue;

                    var he1 = he0.Twin;
                    var f0 = he0.Face;
                    var f1 = he1.Face;

                    // only allow 1 edge spin per face
                    if (f0.Stamp == _faceStamp || f1.Stamp == _faceStamp) continue;

                    var v0 = he0.Start;
                    var v2 = he1.Start;
                    var v1 = he0.Previous.Start;
                    var v3 = he1.Previous.Start;

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
                        f0.Stamp = f1.Stamp = _faceStamp;
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
                if (_lengthField != null)
                {
                    UpdateTargetLengths(_lengthField, parallel);
                    return;
                }

                // set edge lengths to default if no fields
                var hedges = _mesh.Halfedges;
                var t = _settings.LengthRange.A;

                for (int i = 0; i < hedges.Count; i += 2)
                    hedges[i].TargetLength = t;
            }


            /// <summary>
            /// 
            /// </summary>
            void UpdateTargetLengths(IField3d<double> length, bool parallel = false)
            {
                var edges = _mesh.Edges;
                var d = _settings.LengthRange;
                
                if (parallel)
                    Parallel.ForEach(Partitioner.Create(0, edges.Count), range => Body(range.Item1, range.Item2));
                else
                    Body(0, edges.Count);

                void Body(int from, int to)
                {
                    for (int i = from; i < to; i++)
                    {
                        var he = edges[i];
                        if (he.IsUnused) continue;

                        var p = (he.Start.Position + he.End.Position) * 0.5;
                        he.TargetLength = d.Evaluate(length.ValueAt(p));
                    }
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
            private Intervald _lengthRange = new Intervald(1.0, 1.0);
            private double _lengthTolerance = 0.01;
            private double _featureWeight = 100.0;
            private double _featureTolerance = 1.0e-4;
            private double _damping = 0.5;
            private double _timeStep = 1.0;
            private int _refineFreq = 3;


            /// <summary>
            /// 
            /// </summary>
            public Intervald LengthRange
            {
                get { return _lengthRange; }
                set { _lengthRange = value; }
            }


            /// <summary>
            /// 
            /// </summary>
            public double LengthTolerance
            {
                get { return _lengthTolerance; }
                set { _lengthTolerance = SlurMath.Saturate(value); }
            }


            /// <summary>
            /// 
            /// </summary>
            public double FeatureWeight
            {
                get { return _featureWeight; }
                set { _featureWeight = Math.Max(value, 0.0); }
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
            #region Nested types

            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Vertex : HeMesh<V, E, F>.Vertex
            {
                #region Static

                /// <summary></summary>
                public static readonly Func<V, Vec3d> GetPosition = v => v.Position;
                /// <summary> </summary>
                public static readonly Action<V, Vec3d> SetPosition = (v, p) => v.Position = p;

                #endregion


                /// <summary></summary>
                public Vec3d Position;
                /// <summary></summary>
                public Vec3d Velocity;

                /// <summary></summary>
                internal Vec3d DeltaSum;
                /// <summary></summary>
                internal double WeightSum;

                /// <summary></summary>
                internal int FeatureCount = 0;
                /// <summary></summary>
                internal int Stamp = int.MinValue;
                /// <summary></summary>
                internal int DegreeCached;

                /// <summary></summary>
                private int _featureIndex = -1;


                /// <summary>
                /// 
                /// </summary>
                public int FeatureIndex
                {
                    get { return _featureIndex; }
                    internal set { _featureIndex = value; }
                }


                /// <summary>
                /// 
                /// </summary>
                public bool IsFeature
                {
                    get { return FeatureIndex != -1; }
                }


                /// <summary>
                /// 
                /// </summary>
                public bool IsFixed
                {
                    get { return FeatureCount == -1; }
                }


                /// <summary>
                /// 
                /// </summary>
                internal void Fix()
                {
                    FeatureCount = -1;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Halfedge : HeMesh<V, E, F>.Halfedge
            {
                private double _targetLength;
                private int _featureIndex = -1;


                /// <summary>
                /// 
                /// </summary>
                public double TargetLength
                {
                    get { return _targetLength; }
                    internal set { _targetLength = Twin._targetLength = value; }
                }


                /// <summary>
                /// 
                /// </summary>
                public int FeatureIndex
                {
                    get { return _featureIndex; }
                    internal set { _featureIndex = Twin._featureIndex = value; }
                }


                /// <summary>
                /// 
                /// </summary>
                public bool IsFeature
                {
                    get { return _featureIndex != -1; }
                }


                /// <summary>
                /// 
                /// </summary>
                internal bool CanCollapse
                {
                    get { return !Start.IsFixed && FeatureIndex == Start.FeatureIndex; }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Face : HeMesh<V, E, F>.Face
            {
                /// <summary></summary>
                internal int Stamp = int.MinValue;
            }

            #endregion


            #region Static

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


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected sealed override V NewVertex()
            {
                return new V();
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            protected sealed override E NewHalfedge()
            {
                return new E();
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
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
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public sealed override HeMesh Create(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
            {
                return new HeMesh(vertexCapacity, halfedgeCapacity, faceCapacity);
            }
        }
    }
}