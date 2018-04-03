using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeMeshUnroller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="setEdge"></param>
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
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <param name="setEdge"></param>
        public static void Unroll(HeMesh3d mesh, HeMesh3d.Face start, Action<HeMesh3d.Vertex, Vec3d> setUnrolledPosition, Func<HeMesh3d.Halfedge, double> getUnrollFactor = null)
        {
            Unroll(mesh, start, HeMesh3d.Vertex.Accessors.Position, setUnrolledPosition, getUnrollFactor);
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
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F start, Func<V, Vec3d> getPosition, Action<V, Vec3d> setUnrolledPosition, Func<E, double> getUnrollFactor = null)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            Impl<V, E, F>.Unroll(mesh, start, getPosition, setUnrolledPosition, getUnrollFactor);
        }
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
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
            public static void Unroll(HeMesh<V, E, F> mesh, F start, Func<V, Vec3d> getPosition, Action<V, Vec3d> setUnrolledPosition, Func<E, double> getUnrollFactor = null)
            {
                const string errorMessage = "Face cycle detected. The given mesh cannot be unrolled.";
                start.UnusedCheck();
                mesh.Faces.OwnsCheck(start);

                if (getUnrollFactor == null)
                    getUnrollFactor = he => 1.0;

                // stack of edges with corresponding transforms
                var queue = new Queue<(E, Orient3d)>();
                var curr = Orient3d.Identity;
                var tag = mesh.Faces.NextTag;

                // push twin of each halfedge in the start face
                foreach (var he in start.Halfedges)
                {
                    var v = he.Start;
                    setUnrolledPosition(v, getPosition(v));
                    TryEnqueue(he.Twin);
                }

                // breadth-first walk and transform
                while (queue.Count > 0)
                {
                    (var he0, var prev) = queue.Dequeue();
                    curr = prev.Apply(GetHalfedgeTransform(he0, getPosition, getUnrollFactor));
                    var he1 = he0.Next;

                    // push first without applying transform
                    {
                        TryEnqueue(he1.Twin);
                        he1 = he1.Next;
                    }

                    // transform and push remaining
                    do
                    {
                        var v = he1.Start;
                        var p = getPosition(v);
                        setUnrolledPosition(v, curr.Apply(p));

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

                    queue.Enqueue((hedge, curr));
                    f.Tag = tag;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            private static Orient3d GetHalfedgeTransform(E hedge, Func<V, Vec3d> getPosition, Func<E, double> getUnrollFactor = null)
            {
                if (getUnrollFactor == null)
                    return GetHalfedgeTransform(hedge, getPosition);

                var t = getUnrollFactor(hedge);
                if (t < 0.0) return Orient3d.Identity;

                var he0 = hedge;
                var he1 = he0.Twin;

                Vec3d p0 = getPosition(he0.Start);
                Vec3d p1 = getPosition(he1.Start);

                Vec3d p2 = (getPosition(he0.Previous.Start) + getPosition(he0.Next.End)) * 0.5;
                Vec3d p3 = (getPosition(he1.Previous.Start) + getPosition(he1.Next.End)) * 0.5;

                Vec3d x = p1 - p0;
                Vec3d y0 = p2 - p0;
                Vec3d y1 = (t < 1.0) ? y0.SlerpTo(p1 - p3, t) : p1 - p3; // TODO handle anti-parallel case

                return Orient3d.CreateFromTo(
                    new Orient3d(p0, x, y0),
                    new Orient3d(p0, x, y1)
                    );
            }

            
            /// <summary>
            /// 
            /// </summary>
            private static Orient3d GetHalfedgeTransform(E hedge, Func<V, Vec3d> getPosition)
            {
                var he0 = hedge;
                var he1 = he0.Twin;

                Vec3d p0 = getPosition(he0.Start);
                Vec3d p1 = getPosition(he1.Start);

                Vec3d p2 = (getPosition(he0.Previous.Start) + getPosition(he0.Next.End)) * 0.5;
                Vec3d p3 = (getPosition(he1.Previous.Start) + getPosition(he1.Next.End)) * 0.5;

                Vec3d x = p1 - p0;

                return Orient3d.CreateFromTo(
                    new Orient3d(p0, x, p2 - p0),
                    new Orient3d(p0, x, p1 - p3)
                    );
            }
        }

        #endregion
    }
}
