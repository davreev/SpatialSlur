
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
    internal static class GraphSearch
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getConnected"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static T NearestMin<T, K>(T start, Func<T, IEnumerable<T>> getConnected, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                var t1 = getConnected(t0).SelectMin(getKey);
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
        /// <param name="getConnected"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static T NearestMax<T, K>(T start, Func<T, IEnumerable<T>> getConnected, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                var t1 = getConnected(t0).SelectMin(getKey);
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
        /// <param name="getConnected"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static IEnumerable<T> WalkToMin<T, K>(T start, Func<T, IEnumerable<T>> getConnected, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                yield return t0;

                var t1 = getConnected(t0).SelectMin(getKey);
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
        /// <param name="getConnected"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        internal static IEnumerable<T> WalkToMax<T, K>(T start, Func<T, IEnumerable<T>> getConnected, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = start;
            var k0 = getKey(t0);

            while (true)
            {
                yield return t0;

                var v1 = getConnected(t0).SelectMin(getKey);
                var k1 = getKey(v1);

                if (k1.CompareTo(k0) <= 0)
                    yield break;

                t0 = v1;
                k0 = k1;
            }
        }
    }
}
