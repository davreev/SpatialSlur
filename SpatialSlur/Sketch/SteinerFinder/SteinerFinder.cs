using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino.SteinerFinder
{
    /// <summary>
    /// 
    /// </summary>
    public class SteinerFinder
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static SteinerFinder CreateFromPoints(IEnumerable<Vec3d> points)
        {
            var graph = new HeGraphSim();

            // add vertices
            foreach(var p in points)
                graph.AddVertex().Position = p;

            // add central vertex
            int nv = graph.Vertices.Count;
            graph.AddVertex().Position = points.Mean();

            // add edges
            for(int i = 0; i < nv; i++)
                graph.AddEdge(i, nv);

            return new SteinerFinder(graph);
        }

        #endregion


        private HeGraphSim _graph;
        private SteinerFinderSettings _settings;

        private double _maxDelta = double.MaxValue;
        private int _stepCount = 0;
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public SteinerFinder(HeGraphSim graph)
        {
            _graph = graph;
            _graph.Compact();

            SetTerminalStatus();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetTerminalStatus()
        {
            foreach(var v in _graph.Vertices)
                v.IsTerminal = v.IsDegree1;
        }


        /// <summary>
        /// 
        /// </summary>
        public HeGraphSim Graph
        {
            get { return _graph; }
        }


        /// <summary>
        /// 
        /// </summary>
        public SteinerFinderSettings Settings
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
            for (int i = 0; i < _settings.SubSteps; i++)
            {
                ApplyConstantTension();
                UpdateVertices();

                if (++_stepCount % _settings.RefineFrequency == 0)
                    Refine();
            }
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
                     var v0 = verts[i];
                     if (v0.IsTerminal) continue; // skip terminals

                     var p0 = v0.Position;
                     Vec3d fsum = new Vec3d();
                     double dmin = double.MaxValue;
                     int n = 0;

                     foreach (var v1 in v0.ConnectedVertices)
                     {
                         Vec3d f = v1.Position - p0;
                         double d = f.SquareLength;

                         if (d > 0.0)
                         {
                             d = Math.Sqrt(d);
                             dmin = Math.Min(d, dmin);
                             fsum += f / d;
                             n++;
                         }
                     }

                     // dmin = Math.Min(0.1, dmin); // TEST set ceiling for dmin to slow convergence
                     v0.ForceSum += fsum * (dmin / n); // scale force by min edge length for better convergence
                 }
             });
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateVertices()
        {
            var verts = _graph.Vertices;
            var damp = _settings.Damping;
            var timeStep = _settings.TimeStep;

            _maxDelta = 0.0;

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
             {
                 double dmax = 0.0;

                 for (int i = range.Item1; i < range.Item2; i++)
                 {
                     var v = verts[i];
                     v.Velocity *= damp;

                     v.Velocity += v.ForceSum * timeStep;
                     v.Position += v.Velocity * timeStep;
                     v.ForceSum = Vec3d.Zero;

                     dmax = Math.Max(dmax, v.Velocity.SquareLength);
                 }

                 if(dmax > _maxDelta)
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
                    he0.Remove(); // remove if self loop
                }
                else if (he0.IsAtDegree2 && !v0.IsTerminal)
                {
                    _graph.CollapseEdge(he0); // collapse degree 2
                }
                else if (he1.IsAtDegree2 && !v1.IsTerminal)
                {
                    _graph.CollapseEdge(he1); // collapse degree 2
                }
                else
                {
                    var p0 = v0.Position;
                    var p1 = v1.Position;

                    // collapse short edges
                    if (p0.SquareDistanceTo(p1) < dmin)
                    {
                        if(v0.IsTerminal)
                        {
                            if (!v1.IsTerminal)
                                _graph.CollapseEdge(he1);
                        }
                        else if(v1.IsTerminal)
                        {
                            if (!v0.IsTerminal)
                                _graph.CollapseEdge(he0);
                        }
                        else
                        {
                            _graph.CollapseEdge(he0);
                        }
                    }
                }
            }

            /*
            // cleanup any multi-edges created during collapse
            hedges.RemoveMultiEdges();
            */
            
            _graph.Compact();
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
                if(FindZipPair(v, out HeGraphSim.Halfedge he0, out HeGraphSim.Halfedge he1))
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
        private bool FindZipPair(HeGraphSim.Vertex vertex, out HeGraphSim.Halfedge he0, out HeGraphSim.Halfedge he1)
        {
            var first = vertex.FirstOut;
            var p0 = vertex.Position;

            he0 = he1 = null;
            double xmax = double.MinValue;

            // find closest pair unit vectors around start
            foreach (var he2 in first.CirculateStart)
            {
                Vec3d d2 = he2.End.Position - p0;
                d2.Unitize();

                var he3 = he2.NextAtStart;
                while (he3 != first)
                {
                    Vec3d d3 = he3.End.Position - p0;
                    d3.Unitize();

                    double x = Vec3d.Dot(d2, d3);
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
    }
}
