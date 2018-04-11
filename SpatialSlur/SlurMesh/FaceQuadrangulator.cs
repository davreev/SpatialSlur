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
    public static class FaceQuadrangulator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Fan<V, E, F> CreateFan<V, E, F>(HeMesh<V, E, F> mesh)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return new Fan<V, E, F>(mesh, f => f.First);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getStart"></param>
        /// <returns></returns>
        public static Fan<V, E, F> CreateFan<V, E, F>(HeMesh<V, E, F> mesh, Func<F, E> getStart)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return new Fan<V, E, F>(mesh, getStart);
        }


        /// <summary>
        /// Starts the quadrangulation from the halfedge with the minimum key.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static Fan<V, E, F> CreateFan<V, E, F, K>(HeMesh<V, E, F> mesh, Func<E, K> getKey)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
            where K : IComparable<K>
        {
            return new Fan<V, E, F>(mesh, f => f.Halfedges.SelectMin(getKey));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Strip<V, E, F> CreateStrip<V, E, F>(HeMesh<V, E, F> mesh)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return new Strip<V, E, F>(mesh, f => f.First);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getStart"></param>
        /// <returns></returns>
        public static Strip<V, E, F> CreateStrip<V, E, F>(HeMesh<V, E, F> mesh, Func<F, E> getStart)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            return new Strip<V, E, F>(mesh, getStart);
        }


        /// <summary>
        /// Starts the quadrangulation from the minimum halfedge in each face.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static Strip<V, E, F> CreateStrip<V, E, F, K>(HeMesh<V, E, F> mesh, Func<E, K> getKey)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
            where K : IComparable<K>
        {
            return new Strip<V, E, F>(mesh, f => f.Halfedges.SelectMin(getKey));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public class Fan<V, E, F> : IFaceQuadrangulator<V, E, F>
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            private HeMesh<V, E, F> _mesh;
            private Func<F, E> _getStart;


            /// <summary>
            /// 
            /// </summary>
            internal Fan(HeMesh<V, E, F> mesh, Func<F, E> getStart)
            {
                _mesh = mesh ?? throw new ArgumentNullException();
                _getStart = getStart ?? throw new ArgumentNullException();
            }


            /// <inheritdoc />
            public IEnumerable<(V, V, V, V)> GetQuads(F face)
            {
                face.UnusedCheck();

                var he = _getStart(face);
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
            public void Quadrangulate(F face)
            {
                face.UnusedCheck();
                _mesh.Faces.OwnsCheck(face);

                var he0 = _getStart(face);
                var he1 = he0.Next.Next.Next;

                while (he1 != he0 && he1.Next != he0)
                {
                    he0 = _mesh.SplitFaceImpl(he0, he1);
                    he1 = he1.Next.Next;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public class Strip<V, E, F> : IFaceQuadrangulator<V, E, F>
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            private HeMesh<V, E, F> _mesh;
            private Func<F, E> _getStart;


            /// <summary>
            /// 
            /// </summary>
            internal Strip(HeMesh<V, E, F> mesh, Func<F, E> getStart)
            {
                _mesh = mesh ?? throw new ArgumentNullException();
                _getStart = getStart ?? throw new ArgumentNullException();
            }


            /// <inheritdoc />
            public IEnumerable<(V, V, V, V)> GetQuads(F face)
            {
                var he0 = _getStart(face);
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
            public void Quadrangulate(F face)
            {
                face.UnusedCheck();
                _mesh.Faces.OwnsCheck(face);

                var he0 = _getStart(face);
                var he1 = he0.Next.Next.Next;

                while (he1 != he0 && he1.Next != he0)
                {
                    he0 = _mesh.SplitFaceImpl(he0, he1).Previous;
                    he1 = he1.Next;
                }
            }
        }
    }
}
