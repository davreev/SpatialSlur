
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.Collections;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class IEnumerableExtensions
    {
        #region IEnumerable<T>
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            int index = 0;

            foreach(var item in source)
            {
                if (predicate(item))
                    yield return index;

                index++;
            }
        }


        /// <summary>
        /// Moves source elements into the given array.
        /// This array is expanded as necessary to fit all source items.
        /// Returns the number of items in the source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="array"></param>
        public static int ToArray<T>(this IEnumerable<T> source, ref T[] array)
        {
            const int minCapacity = 4;
            int n = 0;

            foreach (var item in source)
            {
                if (array.Length == n)
                    Array.Resize(ref array, Math.Max(n << 1, minCapacity));

                array[n++] = item;
            }

            return n;
        }


        /// <summary>
        /// Moves source elements into the given list.
        /// This list is expanded as necessary to fit all source items.
        /// Returns the number of items in the source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int ToList<T>(this IEnumerable<T> source, List<T> list)
        {
            var itr = source.GetEnumerator();
            int n = 0;

            // insert while there's space in the list
            while (itr.MoveNext())
            {
                if (list.Count == n)
                    break;

                list[n++] = itr.Current;
            }

            // add any remaining elements
            while (itr.MoveNext())
            {
                list.Add(itr.Current);
                n++;
            }
            
            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static RefList<T> ToRefList<T>(this IEnumerable<T> source)
        {
            return new RefList<T>(source);
        }


        /// <summary>
        /// Moves source elements into the given list.
        /// This list is expanded as necessary to fit all source items.
        /// Returns the number of items in the source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int ToRefList<T>(this IEnumerable<T> source, RefList<T> list)
        {
            var itr = source.GetEnumerator();
            int n = 0;

            // insert while theres space in the list
            while (itr.MoveNext())
            {
                if (list.Count == n)
                    break;

                list[n++] = itr.Current;
            }

            // add any remaining elements
            while (itr.MoveNext())
            {
                list.Add(itr.Current);
                n++;
            }

            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static T SelectMin<T, K>(this IEnumerable<T> source, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = source.First();
            var k0 = getKey(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getKey(t);

                if (k.CompareTo(k0) < 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> source, Func<T, double> getValue)
        {
            var t0 = source.First();
            var k0 = getValue(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getValue(t);

                if (k.CompareTo(k0) < 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> source, Func<T, int> getValue)
        {
            var t0 = source.First();
            var k0 = getValue(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getValue(t);

                if (k.CompareTo(k0) < 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static T SelectMax<T, K>(this IEnumerable<T> source, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = source.First();
            var k0 = getKey(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getKey(t);
                if (k.CompareTo(k0) > 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> source, Func<T, double> getValue)
        {
            var t0 = source.First();
            var k0 = getValue(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getValue(t);
                if (k.CompareTo(k0) > 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> source, Func<T, int> getValue)
        {
            var t0 = source.First();
            var k0 = getValue(t0);

            foreach (T t in source.Skip(1))
            {
                var k = getValue(t);
                if (k.CompareTo(k0) > 0)
                {
                    t0 = t;
                    k0 = k;
                }
            }

            return t0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2d Sum<T>(this IEnumerable<T> items, Func<T, Vector2d> getValue)
        {
            var result = new Vector2d();

            foreach (var t in items)
                result += getValue(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3d Sum<T>(this IEnumerable<T> items, Func<T, Vector3d> getValue)
        {
            var result = new Vector3d();

            foreach (var t in items)
                result += getValue(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double WeightedSum<T>(this IEnumerable<T> items, Func<T, double> getValue, Func<T, double> getWeight)
        {
            var result = 0.0;

            foreach (var t in items)
                result += getValue(t) * getWeight(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2d WeightedSum<T>(this IEnumerable<T> items, Func<T, Vector2d> getValue, Func<T, double> getWeight)
        {
            var result = new Vector2d();

            foreach (var t in items)
                result += getValue(t) * getWeight(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3d WeightedSum<T>(this IEnumerable<T> items, Func<T, Vector3d> getValue, Func<T, double> getWeight)
        {
            var result = new Vector3d();

            foreach (var t in items)
                result += getValue(t) * getWeight(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double Mean<T>(this IEnumerable<T> items, Func<T, double> getValue)
        {
            var result = 0.0;
            int n = 0;

            foreach (var t in items)
            {
                result += getValue(t);
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2d Mean<T>(this IEnumerable<T> items, Func<T, Vector2d> getValue)
        {
            var result = new Vector2d();
            int n = 0;

            foreach (var t in items)
            {
                result += getValue(t);
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3d Mean<T>(this IEnumerable<T> items, Func<T, Vector3d> getValue)
        {
            var result = new Vector3d();
            int n = 0;

            foreach (var t in items)
            {
                result += getValue(t);
                n++;
            }

            return result / n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double WeightedMean<T>(this IEnumerable<T> items, Func<T, double> getValue, Func<T, double> getWeight)
        {
            var result = 0.0;
            var wsum = 0.0;

            foreach (var t in items)
            {
                var w = getWeight(t);
                result += getValue(t) * w;
                wsum += w;
            }

            return result / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2d WeightedMean<T>(this IEnumerable<T> items, Func<T, Vector2d> getValue, Func<T, double> getWeight)
        {
            var result = new Vector2d();
            var wsum = 0.0;

            foreach (var t in items)
            {
                var w = getWeight(t);
                result += getValue(t) * w;
                wsum += w;
            }

            return result / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3d WeightedMean<T>(this IEnumerable<T> items, Func<T, Vector3d> getValue, Func<T, double> getWeight)
        {
            var result = new Vector3d();
            var wsum = 0.0;

            foreach (var t in items)
            {
                var w = getWeight(t);
                result += getValue(t) * w;
                wsum += w;
            }

            return result / wsum;
        }
        

        /// <summary>
        /// 
        /// </summary>
        public static void Normalize<T>(this IEnumerable<T> items, Property<T, double> value)
        {
            double sum = 0.0;

            foreach (var t in items)
                sum += value.Get(t);

            if (sum > 0.0)
            {
                double inv = 1.0 / sum;

                foreach (var t in items)
                    value.Set(t, value.Get(t) * inv);
            }
        }

        #endregion


        #region IEnumerable<double>

        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this IEnumerable<double> vector)
        {
            if (vector is double[] arr)
                return Vector.Sum(arr);

            double sum = 0.0;
            foreach (double t in vector) sum += t;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this IEnumerable<double> vector)
        {
            if (vector is double[] arr)
                return Vector.Mean(arr);

            double sum = 0.0;
            int count = 0;

            foreach (double t in vector)
            {
                sum += t;
                count++;
            }

            return sum / count;
        }

        #endregion


        #region IEnumerable<Vector2d>

        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Sum(this IEnumerable<Vector2d> vectors)
        {
            if (vectors is Vector2d[] arr)
                return Matrix.ColumnSum(arr);

            Vector2d sum = new Vector2d();
            foreach (Vector2d v in vectors) sum += v;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Mean(this IEnumerable<Vector2d> vectors)
        {
            if (vectors is Vector2d[] arr)
                return Matrix.ColumnMean(arr);

            Vector2d sum = new Vector2d();
            int count = 0;

            foreach (Vector2d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2d> RemoveCoincident(this IEnumerable<Vector2d> points, double tolerance = D.ZeroTolerance)
        {
            return Proximity.RemoveCoincident(points, p => p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Vector2d> RemoveCoincident(this IEnumerable<Vector2d> points, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            return Proximity.RemoveCoincident(points, p => p, out indexMap, tolerance);
        }

        #endregion


        #region IEnumerable<Vector3d>

        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Sum(this IEnumerable<Vector3d> vectors)
        {
            if (vectors is Vector3d[] arr)
                return Matrix.ColumnSum(arr);

            var sum = new Vector3d();
            foreach (Vector3d v in vectors) sum += v;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Mean(this IEnumerable<Vector3d> vectors)
        {
            if (vectors is Vector3d[] arr)
                return Matrix.ColumnMean(arr);

            Vector3d sum = new Vector3d();
            int count = 0;

            foreach (Vector3d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3d> RemoveCoincident(this IEnumerable<Vector3d> points, double tolerance = D.ZeroTolerance)
        {
            return Proximity.RemoveCoincident(points, p => p, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Vector3d> RemoveCoincident(this IEnumerable<Vector3d> points, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            return Proximity.RemoveCoincident(points, p => p, out indexMap, tolerance);
        }

        #endregion


        #region IEnumerable<double[]>

        /// <summary>
        /// 
        /// </summary>
        public static void Sum(this IEnumerable<double[]> vectors, double[] result)
        {
            result.Set(vectors.First());

            foreach(var v in vectors.Skip(1))
                Vector.Add(result, v, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(this IEnumerable<double[]> vectors, double[] result)
        {
            result.Set(vectors.First());
            int n = 1;

            foreach (var v in vectors.Skip(1))
            {
                Vector.Add(result, v, result);
                n++;
            }

            Vector.Multiply(result, 1.0 / n, result);
        }

        #endregion
    }
}