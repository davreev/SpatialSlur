using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Utility class for stray methods.
    /// </summary>
    public static class MeshUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getNeighbours"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static T NearestMin<T, K>(T start, Func<T, IEnumerable<T>> getNeighbours, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                var t1 = getNeighbours(t0).SelectMin(getKey);
                var k1 = getKey(t1);

                if (k1.CompareTo(k0) >= 0)
                    break;

                t0 = t1;
                k0 = k1;
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getNeighbours"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static T NearestMax<T, K>(T start, Func<T, IEnumerable<T>> getNeighbours, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                var t1 = getNeighbours(t0).SelectMin(getKey);
                var k1 = getKey(t1);

                if (k1.CompareTo(k0) <= 0)
                    break;

                t0 = t1;
                k0 = k1;
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getNeighbours"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static IEnumerable<T> WalkToMin<T, K>(T start, Func<T, IEnumerable<T>> getNeighbours, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                yield return t0;

                var t1 = getNeighbours(t0).SelectMin(getKey);
                var k1 = getKey(t1);

                if (k1.CompareTo(k0) >= 0)
                    yield break;

                t0 = t1;
                k0 = k1;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getNeighbours"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static IEnumerable<T> WalkToMax<T, K>(T start, Func<T, IEnumerable<T>> getNeighbours, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                yield return t0;

                var v1 = getNeighbours(t0).SelectMin(getKey);
                var k1 = getKey(v1);

                if (k1.CompareTo(k0) <= 0)
                    yield break;

                t0 = v1;
                k0 = k1;
            }
        }


        /// <summary>
        /// Throws an exception if the topology of the given mesh is not valid.
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        internal static void CheckTopology<TV, TE>(HeGraph<TV, TE> graph)
            where TV : HeGraph<TV, TE>.Vertex
            where TE : HeGraph<TV, TE>.Halfedge
        {
            var verts = graph.Vertices;
            var hedges = graph.Halfedges;

            // ensure halfedges are reciprocally linked
            foreach (var he in hedges)
            {
                if (he.IsUnused) continue;
                if (he.Previous.Next != he && he.Next.Previous != he) Throw();
                if (he.Start.IsUnused) Throw();
            }

            // ensure consistent start vertex during circulation
            foreach (var v in verts)
            {
                foreach (var he in v.OutgoingHalfedges)
                    if (he.Start != v) Throw();
            }

            void Throw()
            {
                throw new Exception("The topology of the given mesh is invalid");
            }
        }


        /// <summary>
        /// Throws an exception if the topology of the given mesh is not valid.
        /// </summary>
        /// <typeparam name="TV"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <param name="mesh"></param>
        internal static void CheckTopology<TV,TE,TF>(HeMesh<TV,TE,TF> mesh)
            where TV : HeMesh<TV, TE, TF>.Vertex
            where TE : HeMesh<TV, TE, TF>.Halfedge
            where TF : HeMesh<TV, TE, TF>.Face
        {
            var verts = mesh.Vertices;
            var hedges = mesh.Halfedges;
            var faces = mesh.Faces;

            // ensure halfedges are reciprocally linked
            foreach (var he in hedges)
            {
                if (he.IsUnused) continue;
                if (he.Previous.Next != he && he.Next.Previous != he) Throw();
                if (he.Start.IsUnused) Throw();
                if (he.Face.IsUnused) Throw();
            }

            // ensure consistent start vertex during circulation
            foreach (var v in verts)
            {
                foreach (var he in v.OutgoingHalfedges)
                    if (he.Start != v) Throw();
            }

            // ensure consistent face during circulation
            foreach (var f in faces)
            {
                foreach (var he in f.Halfedges)
                    if (he.Face != f) Throw();
            }

            void Throw()
            {
                throw new Exception("The topology of the given mesh is invalid");
            }
        }
    }
}
