
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurTools
{
    using V = SteinerFinder.HeGraph.Vertex;
    using E = SteinerFinder.HeGraph.Halfedge;

    /// <summary>
    /// 
    /// </summary>
    public static class SteinerFinder
    {
        /// <summary>
        /// 
        /// </summary>
        public class Solver
        {
            #region Static

            /// <summary>
            /// 
            /// </summary>
            /// <param name="points"></param>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static Solver CreateFromPoints(IEnumerable<Vec3d> points, Settings settings = null)
            {
                var graph = new HeGraph();

                // add vertices
                foreach (var p in points)
                    graph.AddVertex().Position = p;

                // add central vertex
                int nv = graph.Vertices.Count;
                graph.AddVertex().Position = points.Mean();

                // add edges
                for (int i = 0; i < nv; i++)
                    graph.AddEdge(i, nv);

                return new Solver(graph, settings);
            }

            #endregion


            private HeGraph _graph;
            private Settings _settings;

            private double _maxDelta = double.MaxValue;
            private int _stepCount = 0;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="graph"></param>
            /// <param name="settings"></param>
            internal Solver(HeGraph graph, Settings settings = null)
            {
                _graph = graph;
                _graph.Compact();
                _settings = settings ?? new Settings();

                SetTerminalStatus();
                UpdateAttributes();
            }


            /// <summary>
            /// 
            /// </summary>
            private void SetTerminalStatus()
            {
                foreach (var v in _graph.Vertices)
                    v.IsTerminal = v.IsDegree1;
            }


            /// <summary>
            /// 
            /// </summary>
            public HeGraph Graph
            {
                get { return _graph; }
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


            /// <summary>
            /// 
            /// </summary>
            public bool IsConverged
            {
                get { return _maxDelta < _settings.ToleranceSquared; }
            }


            /// <summary>
            /// 
            /// </summary>
            public void Step()
            {
                if (++_stepCount % _settings.RefineFrequency == 0)
                    Refine();

                ApplyConstantTension();
                UpdateVertices();
                UpdateAttributes();
            }


            /// <summary>
            /// 
            /// </summary>
            private void ApplyConstantTension()
            {
                var verts = _graph.Vertices;

                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                 {
                     for (int i = range.Item1; i < range.Item2; i++)
                     {
                         var v = verts[i];
                         if (v.IsTerminal) continue; // skip terminals

                     Vec3d fsum = new Vec3d();
                         double dmin = double.MaxValue;
                         int n = 0;

                         foreach (var he in v.OutgoingHalfedges)
                         {
                             var d = he.Length;

                             if (d > 0.0)
                             {
                                 fsum += he.Tangent;
                                 dmin = Math.Min(d, dmin);
                                 n++;
                             }
                         }

                     // dmin = Math.Min(0.1, dmin); // TEST set ceiling for dmin to slow convergence
                     v.ForceSum += fsum * (dmin / n); // scale force by min edge length for stability
                 }
                 });
            }


            /// <summary>
            /// 
            /// </summary>
            private void UpdateVertices()
            {
                var verts = _graph.Vertices;
                var damping = _settings.Damping;
                var timeStep = _settings.TimeStep;

                _maxDelta = 0.0;

                Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
                 {
                     double dmax = 0.0;

                     for (int i = range.Item1; i < range.Item2; i++)
                     {
                         var v = verts[i];
                         v.Velocity *= (1.0 - damping);

                         v.Velocity += v.ForceSum * timeStep;
                         v.Position += v.Velocity * timeStep;
                         v.ForceSum = Vec3d.Zero;

                         dmax = Math.Max(dmax, v.Velocity.SquareLength);
                     }

                     if (dmax > _maxDelta)
                         Interlocked.Exchange(ref _maxDelta, dmax);
                 });
            }


            /// <summary>
            /// 
            /// </summary>
            private void Refine()
            {
                CollapseEdges();
                ZipEdges();
            }


            /// <summary>
            /// 
            /// </summary>
            void CollapseEdges()
            {
                var hedges = _graph.Halfedges;
                int count = 0;

                double dmin = _settings.MinLength;
                dmin *= dmin;

                for (int i = 0; i < hedges.Count; i += 2)
                {
                    var he0 = hedges[i];
                    var he1 = hedges[i + 1];

                    var v0 = he0.Start;
                    var v1 = he1.Start;

                    if (v0 == v1)
                    {
                        he0.MakeUnused(); // remove if self loop
                    }
                    else if (he0.IsAtDegree2 && !v0.IsTerminal)
                    {
                        CollapseEdge(he0); // collapse degree 2
                    }
                    else if (he1.IsAtDegree2 && !v1.IsTerminal)
                    {
                        CollapseEdge(he1); // collapse degree 2
                    }
                    else
                    {
                        var p0 = v0.Position;
                        var p1 = v1.Position;

                        // collapse short edges
                        if (p0.SquareDistanceTo(p1) < dmin)
                        {
                            if (v0.IsTerminal)
                            {
                                if (!v1.IsTerminal)
                                    CollapseEdge(he1);
                            }
                            else if (v1.IsTerminal)
                            {
                                if (!v0.IsTerminal)
                                    CollapseEdge(he0);
                            }
                            else
                            {
                                CollapseEdge(he0);
                            }
                        }
                    }
                }

                void CollapseEdge(HeGraph.Halfedge hedge)
                {
                    _graph.CollapseEdge(hedge);
                    count++;
                }

                /*
                // cleanup any multi-edges created during collapse
                hedges.RemoveMultiEdges();
                */

                // compact and update attributes if any edges were collapsed
                if (count > 0)
                {
                    _graph.Compact();
                    UpdateAttributes();
                }
            }


            /// <summary>
            /// 
            /// </summary>
            void ZipEdges()
            {
                var hedges = _graph.Halfedges;
                var verts = _graph.Vertices;
                int nv = verts.Count;

                for (int i = 0; i < nv; i++)
                {
                    var v = verts[i];

                    // check for valence error
                    int deg = v.Degree;
                    if (deg < (v.IsTerminal ? 2 : 4)) continue;

                    // zip if found pair
                    if (FindZipPair(v, out HeGraph.Halfedge he0, out HeGraph.Halfedge he1))
                    {
                        var he2 = _graph.ZipEdges(he0, he1);
                        he2.Start.Position = v.Position;
                    }
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="vertex"></param>
            /// <param name="he0"></param>
            /// <param name="he1"></param>
            /// <returns></returns>
            private bool FindZipPair(V vertex, out E he0, out E he1)
            {
                // TODO use HashGrid for high valence vertices

                var first = vertex.First;
                var p0 = vertex.Position;

                he0 = he1 = null;
                double xmax = double.MinValue;

                // find closest pair unit vectors around start
                foreach (var he2 in first.CirculateStart)
                {
                    Vec3d t2 = he2.Tangent;
                    var he3 = he2.NextAtStart;

                    while (he3 != first)
                    {
                        double x = Vec3d.Dot(t2, he3.Tangent);
                        if (x > xmax)
                        {
                            xmax = x;
                            he0 = he2;
                            he1 = he3;
                        }

                        he3 = he3.NextAtStart;
                    }
                }

                // return true if angle > 120 degrees
                return xmax > -0.5;
            }


            /// <summary>
            /// 
            /// </summary>
            private void UpdateAttributes()
            {
                var edges = _graph.Edges;

                // set edge tangents and lengths
                edges.Action(he =>
                {
                    var v = he.End.Position - he.Start.Position;
                    var d = v.SquareLength;

                    if (d > 0.0)
                    {
                        d = Math.Sqrt(d);
                        he.Tangent = v / d;
                        he.Length = d;
                    }
                    else
                    {
                        he.Tangent = Vec3d.Zero;
                        he.Length = 0.0;
                    }
                }, true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public class Settings
        {
            private double _minLength = 1.0e-4;
            private double _damping = 0.5;
            private double _timeStep = 1.0;
            private double _tolerance = 1.0e-4;
            private int _refineFreq = 10;


            /// <summary>
            /// 
            /// </summary>
            public double MinLength
            {
                get { return _minLength; }
                set { _minLength = Math.Max(value, 0.0); }
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
            public double Tolerance
            {
                get { return _tolerance; }
                set { _tolerance = Math.Max(value, 0.0); }
            }


            /// <summary>
            /// 
            /// </summary>
            internal double ToleranceSquared
            {
                get { return _tolerance * _tolerance; }
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
        public class HeGraph : HeGraph<V, E>
        {
            #region Nested types

            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Vertex : HeGraph<V, E>.Vertex, IPosition3d, INormal3d
            {
                /// <summary></summary>
                public Vec3d Position;
                /// <summary></summary>
                public Vec3d Velocity;
                /// <summary></summary>
                public Vec3d ForceSum;
                /// <summary></summary>
                public bool IsTerminal;


                #region Explicit interface implementations

                /// <inheritdoc />
                Vec3d IPosition3d.Position
                {
                    get { return Position; }
                    set { Position = value; }
                }


                /// <inheritdoc />
                Vec3d INormal3d.Normal
                {
                    get { return new Vec3d(); }
                    set { throw new NotImplementedException(); }
                }

                #endregion
            }


            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public new class Halfedge : HeGraph<V, E>.Halfedge
            {
                private Vec3d _tangent;
                private double _length;


                /// <summary>
                /// 
                /// </summary>
                public double Length
                {
                    get { return _length; }
                    set
                    {
                        _length = Twin._length = value;
                    }
                }


                /// <summary>
                /// 
                /// </summary>
                public Vec3d Tangent
                {
                    get { return _tangent; }
                    set
                    {
                        _tangent = value;
                        Twin._tangent = -value;
                    }
                }
            }

            #endregion


            #region Static

            /// <summary></summary>
            public static readonly HeGraphFactory Factory = new HeGraphFactory();
        
            #endregion


            /// <summary>
            /// 
            /// </summary>
            public HeGraph()
                : base()
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="vertexCapacity"></param>
            /// <param name="hedgeCapacity"></param>
            public HeGraph(int vertexCapacity, int hedgeCapacity)
                : base(vertexCapacity, hedgeCapacity)
            {
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            public HeGraph(HeGraph other)
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
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class HeGraphFactory : HeGraphFactory<HeGraph, V, E>
        {
            /// <inheritdoc />
            public sealed override HeGraph Create(int vertexCapacity, int halfedgeCapacity)
            {
                return new HeGraph(vertexCapacity, halfedgeCapacity);
            }
        }
    }
}
