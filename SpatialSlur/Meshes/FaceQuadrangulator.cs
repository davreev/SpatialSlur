
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class FaceQuadrangulator
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        private class Fan : IFaceQuadrangulator
        {
            /// <inheritdoc />
            public IEnumerable<(V, V, V, V)> GetQuads<V, E, F>(HeMesh<V, E, F>.Halfedge start)
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

                    he = he.Next;
                    var v3 = he.Start;

                    if (v3 == v0)
                    {
                        yield return (v0, v1, v2, null);
                        break;
                    }

                    yield return (v0, v1, v2, v3);
                    v1 = v3;
                } while (true);
            }


            /// <inheritdoc />
            public void Quadrangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();
                mesh.Halfedges.OwnsCheck(start);

                if (start.IsHole)
                    return;

                var he0 = start;
                var he1 = he0.Next.Next.Next;

                while (he1 != he0 && he1.Next != he0)
                {
                    he0 = mesh.SplitFaceImpl(he0, he1);
                    he1 = he1.Next.Next;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class Strip : IFaceQuadrangulator
        {
            /// <inheritdoc />
            public IEnumerable<(V, V, V, V)> GetQuads<V, E, F>(HeMesh<V, E, F>.Halfedge start)
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

                    he0 = he0.Previous;
                    var v3 = he0.Start;

                    if (v2 == v3)
                    {
                        yield return (v0, v1, v2, null);
                        break;
                    }

                    yield return (v0, v1, v2, v3);
                    v0 = v3;
                    v1 = v2;
                } while (true);
            }


            /// <inheritdoc />
            public void Quadrangulate<V, E, F>(HeMesh<V, E, F> mesh, E start)
                where V : HeMesh<V, E, F>.Vertex
                where E : HeMesh<V, E, F>.Halfedge
                where F : HeMesh<V, E, F>.Face
            {
                start.UnusedCheck();
                mesh.Halfedges.OwnsCheck(start);

                if (start.IsHole)
                    return;

                var he0 = start;
                var he1 = he0.Next.Next.Next;

                while (he1 != he0 && he1.Next != he0)
                {
                    he0 = mesh.SplitFaceImpl(he0, he1).Previous;
                    he1 = he1.Next;
                }
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFaceQuadrangulator CreateFan()
        {
            return new Fan();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFaceQuadrangulator CreateStrip()
        {
            return new Strip();
        }
    }
}
