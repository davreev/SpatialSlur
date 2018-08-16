
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeMeshUnroller
    {
        #region static

        private static Func<HeMesh3d.Vertex, Vector3d> _getPosition = v => v.Position;

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        public static void DetachFaceCycles(HeMesh3d mesh, HeMesh3d.Face start)
        {
            DetachFaceCycles(mesh, start, Set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="setEdge"></param>
        public static void DetachFaceCycles<V, E, F>(HeMesh<V, E, F> mesh, F start, Action<E, E> setEdge)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            Impl<V, E, F>.DetachFaceCycles(mesh, start, setEdge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="setUnrolledPosition"></param>
        /// <param name="getUnrollFactor"></param>
        public static void Unroll(HeMesh3d mesh, HeMesh3d.Face start, Action<HeMesh3d.Vertex, Vector3d> setUnrolledPosition, Func<HeMesh3d.Halfedge, double> getUnrollFactor = null)
        {
            Unroll(mesh, start, _getPosition, setUnrolledPosition, getUnrollFactor);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="setUnrolledPosition"></param>
        /// <param name="getUnrollFactor"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F start, Action<V, Vector3d> setUnrolledPosition, Func<E, double> getUnrollFactor = null)
            where V : HeMesh<V, E, F>.Vertex, IPosition3d
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            Impl<V, E, F>.Unroll(mesh, start, Position3d<V>.Get, setUnrolledPosition, getUnrollFactor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="getPosition"></param>
        /// <param name="setUnrolledPosition"></param>
        /// <param name="getUnrollFactor"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F start, Func<V, Vector3d> getPosition, Action<V, Vector3d> setUnrolledPosition, Func<E, double> getUnrollFactor = null)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            Impl<V, E, F>.Unroll(mesh, start, getPosition, setUnrolledPosition, getUnrollFactor);
        }
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        private static void Set(HeMesh3d.Halfedge he0, HeMesh3d.Halfedge he1)
        {
            Set(he0.Start, he1.Start);
            Set(he0.End, he1.End);

            void Set(HeMesh3d.Vertex v0, HeMesh3d.Vertex v1)
            {
                v0.Position = v1.Position;
                v0.Normal = v1.Normal;
            }
        }
   

        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        private static class Impl<V, E, F>
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="start"></param>
            /// <param name="setEdge"></param>
            public static void DetachFaceCycles(HeMesh<V, E, F> mesh, F start, Action<E, E> setEdge)
            {
                var currTag = mesh.Halfedges.NextTag;

                // tag traversed edges during BFS
                foreach (var he in mesh.GetFacesBreadthFirst2(start.Yield()))
                    he.Edge.Tag = currTag;

                var edges = mesh.Edges;
                var ne = edges.Count;

                // detach all untagged edges
                for (int i = 0; i < ne; i++)
                {
                    var he0 = edges[i];

                    if (he0.IsUnused || he0.IsBoundary || he0.Tag == currTag)
                        continue;

                    var he1 = mesh.DetachEdgeImpl(he0);
                    setEdge(he1, he0);
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="start"></param>
            /// <param name="getPosition"></param>
            /// <param name="setUnrolledPosition"></param>
            /// <param name="getUnrollFactor"></param>
            public static void Unroll(HeMesh<V, E, F> mesh, F start, Func<V, Vector3d> getPosition, Action<V, Vector3d> setUnrolledPosition, Func<E, double> getUnrollFactor = null)
            {
                const string errorMessage = "Face cycle detected. The given mesh cannot be unrolled.";
                start.UnusedCheck();
                mesh.Faces.OwnsCheck(start);

                // stack of edges with corresponding transforms
                var queue = new Queue<(E, Orient3d)>();
                var tag = mesh.Faces.NextTag;

                // current unroll transform
                var unroll = Orient3d.Identity;

                // enqueue twin of each halfedge in the first face
                foreach (var he in start.Halfedges)
                {
                    var v = he.Start;
                    setUnrolledPosition(v, getPosition(v));
                    TryEnqueue(he.Twin);
                }

                // breadth-first walk and transform
                while (queue.Count > 0)
                {
                    (var he0, var m0) = queue.Dequeue();

                    var m1 = getUnrollFactor == null ?
                        GetHalfedgeTransform(he0, getPosition) :
                        GetHalfedgeTransform(he0, getPosition, getUnrollFactor(he0));

                    unroll = m0.Apply(ref m1);
                    var he1 = he0.Next;

                    // enqueue first hedge
                    TryEnqueue(he1.Twin);
                    he1 = he1.Next;

                    // transform and enqueue remaining hedges
                    do
                    {
                        var v = he1.Start;
                        setUnrolledPosition(v, unroll.Apply(getPosition(v)));

                        TryEnqueue(he1.Twin);
                        he1 = he1.Next;
                    } while (he1 != he0);
                }

                void TryEnqueue(E hedge)
                {
                    var f = hedge.Face;
                    if (f == null) return;

                    if (f.Tag == tag)
                        throw new ArgumentException(errorMessage);

                    queue.Enqueue((hedge, unroll));
                    f.Tag = tag;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static Orient3d GetHalfedgeTransform(E hedge, Func<V, Vector3d> getPosition, double unrollFactor)
            {
                if (unrollFactor < 0.0) return Orient3d.Identity;

                var he0 = hedge;
                var he1 = he0.Twin;

                Vector3d p0 = getPosition(he0.Start);
                Vector3d p1 = getPosition(he1.Start);

                Vector3d p2 = (getPosition(he0.Previous.Start) + getPosition(he0.Next.End)) * 0.5;
                Vector3d p3 = (getPosition(he1.Previous.Start) + getPosition(he1.Next.End)) * 0.5;
                
                Vector3d x = p1 - p0;
                Vector3d y0 = p2 - p0;
                Vector3d y1 = (unrollFactor < 1.0) ? y0.SlerpTo(p1 - p3, unrollFactor) : p1 - p3; // TODO handle anti-parallel case

                var t0 = new Orient3d(OrthoBasis3d.CreateFromXY(x, y0), p0);
                var t1 = new Orient3d(OrthoBasis3d.CreateFromXY(x, y1), p0);
                return Orient3d.CreateFromTo(ref t0, ref t1);
            }


            /// <summary>
            /// 
            /// </summary>
            private static Orient3d GetHalfedgeTransform(E hedge, Func<V, Vector3d> getPosition)
            {
                var he0 = hedge;
                var he1 = he0.Twin;

                Vector3d p0 = getPosition(he0.Start);
                Vector3d p1 = getPosition(he1.Start);

                Vector3d p2 = (getPosition(he0.Previous.Start) + getPosition(he0.Next.End)) * 0.5;
                Vector3d p3 = (getPosition(he1.Previous.Start) + getPosition(he1.Next.End)) * 0.5;

                Vector3d x = p1 - p0;
                var t0 = new Orient3d(OrthoBasis3d.CreateFromXY(x, p2 - p0), p0);
                var t1 = new Orient3d(OrthoBasis3d.CreateFromXY(x, p1 - p3), p0);
                return Orient3d.CreateFromTo(ref t0, ref t1);
            }
        }

        #endregion
    }
}
