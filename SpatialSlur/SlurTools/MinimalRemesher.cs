#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurTools.Features;
using SpatialSlur.SlurRhino;
using Rhino.Geometry;

/*
 * Notes
 * 
 * Dynamic remeshing based on implemetation described in
 * https://nccastaff.bournemouth.ac.uk/jmacey/MastersProjects/MSc15/08Tanja/report.pdf
 * 
 * Other references
 * http://graphics.stanford.edu/courses/cs468-12-spring/LectureSlides/13_Remeshing1.pdf
 */

namespace SpatialSlur.SlurTools
{
    using V = MinimalRemesher.HeMesh.Vertex;
    using E = MinimalRemesher.HeMesh.Halfedge;
    using F = MinimalRemesher.HeMesh.Face;

    /// <summary>
    /// 
    /// </summary>
    public static class MinimalRemesher
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
            public static Solver Create<TV, TE, TF>(HeMeshBase<TV, TE, TF> mesh, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
                where TV : HeMeshBase<TV, TE, TF>.Vertex, IVertex3d
                where TE : HeMeshBase<TV, TE, TF>.Halfedge
                where TF : HeMeshBase<TV, TE, TF>.Face
            {
                var copy = HeMesh.Factory.CreateCopy(mesh, (v0, v1) => v0.Position = v1.Position, delegate { }, delegate { });
                return new Solver(copy, features, tolerance);
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

            private List<IFeature> _features;

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
            public Solver(HeMesh mesh, IEnumerable<IFeature> features, double tolerance = 1.0e-4)
            {
                _mesh = mesh;
                _mesh.Compact();

                // triangulate all faces starting with the shortest diagonal
                _mesh.TriangulateFaces(FaceTriangulators.Strip.CreateFromMin(mesh, he =>
                {
                    var p0 = he.Start.Position;
                    var p1 = he.NextInFace.End.Position;
                    return p0.SquareDistanceTo(p1);
                }));

                // initialize features
                InitFeatures(features, tolerance);

                _settings = new Settings();
                _stepCount = 0;
            }


            #region Initialization

            /// <summary>
            /// 
            /// </summary>
            private void InitFeatures(IEnumerable<IFeature> features, double tolerance)
            {
                _features = new List<IFeature>();
                var tolSqr = tolerance * tolerance;

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

                InitBoundaryFeatures();
                FixVertices();
            }


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
                    foreach (var he1 in he0.CirculateFace)
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
                var poly = new Polyline(hedge.CirculateFace.Select(he => (Point3d)he.Start.Position));
                poly.Add(poly[0]);

                return new MeshFeature(RhinoFactory.Mesh.CreateExtrusion(poly, new Vector3d()));
            }


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
            public List<IFeature> Features
            {
                get { return _features; }
                set { _features = value; }
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
                UniformSmooth(1.0);
                PullToFeatures(_settings.FeatureWeight);
                UpdateVertices(_settings.TimeStep, _settings.Damping);

                if (++_stepCount % _settings.RefineFrequency == 0) Refine();
            }


            #region Dynamics

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

                        if (v.IsFeature)
                        {
                            var p = v.Position;
                            v.DeltaSum += (_features[v.FeatureIndex].ClosestPoint(p) - p) * weight;
                            v.WeightSum += weight;
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
                        v.Velocity *= damping;

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
                var hedges = _mesh.Halfedges;
                int count = hedges.Count;

                double tolerance = _settings.LengthTarget * _maxLengthFactor * (1.0 + _settings.LengthTolerance);
                tolerance *= tolerance;
                _vertStamp++;

                for (int i = 0; i < count; i += 2)
                {
                    var he0 = hedges[i];
                    if (he0.IsUnused) continue;

                    var v0 = he0.Start;
                    var v1 = he0.End;

                    var p0 = v0.Position;
                    var p1 = v1.Position;

                    // split edge if length exceeds max
                    if (p0.SquareDistanceTo(p1) > tolerance)
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
                var hedges = _mesh.Halfedges;
                bool compact = false;

                double tolerance = _settings.LengthTarget * _minLengthFactor * (1.0 - _settings.LengthTolerance);
                tolerance *= tolerance;

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

                if (p0.SquareDistanceTo(p1) < tolerance)
                {
                    var he2 = he0.NextInFace; // removed if collapse is successful
                    var he3 = he1.NextInFace;

                    // update attributes on successful collapse
                    if (_mesh.CollapseEdge(he0))
                    {
                        // update position if symmetric
                        if (symmetric) v1.Position = (p0 + p1) * 0.5;

                        // inherit edge features
                        if (he2.IsFeature) he2.NextInFace.FeatureIndex = he2.FeatureIndex;
                        if (he3.IsFeature) he3.NextInFace.FeatureIndex = he3.FeatureIndex;

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
                    var v1 = he0.PreviousInFace.Start;
                    var v3 = he1.PreviousInFace.Start;

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
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Settings
        {
            private double _lengthTarget = 1.0;
            private double _lengthTolerance = 0.1;
            private double _featureWeight = 100.0;
            private double _damping = 0.1;
            private double _timeStep = 1.0;
            private int _refineFreq = 3;


            /// <summary>
            /// 
            /// </summary>
            public double LengthTarget
            {
                get { return _lengthTarget; }
                set { _lengthTarget = Math.Max(value, 0.0); }
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
            public double TimeStep
            {
                get { return _timeStep; }
                set { _timeStep = Math.Max(value, 0.0); }
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
            public int RefineFrequency
            {
                get { return _refineFreq; }
                set { _refineFreq = Math.Max(value, 1); }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class HeMesh : HeMeshBase<V, E, F>
        {
            #region Nested types




            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Vertex : HeMeshBase<V, E, F>.Vertex
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
            public new class Halfedge : HeMeshBase<V, E, F>.Halfedge
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
            public new class Face : HeMeshBase<V, E, F>.Face
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
        public class HeMeshFactory : HeMeshBaseFactory<HeMesh, V, E, F>
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

#endif
