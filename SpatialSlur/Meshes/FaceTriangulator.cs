
/*
 * Notes
 */

using System;
using System.Collections.Generic;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class FaceTriangulator
    {
        #region Nested types
        
        /// <summary>
        /// 
        /// </summary>
        private class Fan : IFaceTriangulator
        {
            /// <inheritdoc />
            public IEnumerable<(V, V, V)> GetTriangles<V, E, F>(HeMesh<V, E, F>.Halfedge start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();

                var he = start;
                var v0 = he.Start;

                he = he.Next;
                var v1 = he.Start;

                do
                {
                    he = he.Next;
                    var v2 = he.Start;

                    if (v2 == v0) break;
                    yield return (v0, v1, v2);

                    v1 = v2;
                } while (true);
            }


            /// <inheritdoc />
            public void Triangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();
                mesh.Halfedges.OwnsCheck(start);

                if (start.IsHole)
                    return;

                var he0 = start;
                var he1 = he0.Next.Next;

                while (he1.Next != he0)
                {
                    he0 = mesh.SplitFaceImpl(he0, he1);
                    he1 = he1.Next;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class Strip : IFaceTriangulator
        {
            /// <inheritdoc />
            public IEnumerable<(V, V, V)> GetTriangles<V, E, F>(HeMesh<V, E, F>.Halfedge start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();

                var he0 = start;
                var v0 = he0.Start;

                var he1 = he0.Next;
                var v1 = he1.Start;

                do
                {
                    he1 = he1.Next;
                    var v2 = he1.Start;

                    if (v2 == v0) break;
                    yield return (v0, v1, v2);

                    he0 = he0.Previous;
                    var v3 = he0.Start;

                    if (v2 == v3) break;
                    yield return (v0, v2, v3);

                    v0 = v3;
                    v1 = v2;
                } while (true);
            }


            /// <inheritdoc />
            public void Triangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();
                mesh.Halfedges.OwnsCheck(start);

                if (start.IsHole)
                    return;

                var he0 = start;
                var he1 = he0.Next.Next;

                while (he1.Next != he0)
                {
                    he0 = mesh.SplitFaceImpl(he0, he1).Previous;
                    if (he1.Next == he0) break;

                    he0 = mesh.SplitFaceImpl(he0, he1);
                    he1 = he1.Next;
                }
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFaceTriangulator CreateFan()
        {
            return new Fan();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFaceTriangulator CreateStrip()
        {
            return new Strip();
        }
    }
}
