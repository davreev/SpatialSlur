using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurData;

using static SpatialSlur.SlurData.DataUtil;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region IEnumerable<T>

        /// <summary>
        /// Assumes the number of elements in this sequence doesn't exceed the length of the given array.
        /// Returns the number of items in this sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seqeunce"></param>
        /// <param name="array"></param>
        public static int ToArray<T>(this IEnumerable<T> seqeunce, T[] array)
        {
            int index = 0;

            foreach (var item in seqeunce)
                array[index++] = item;

            return index;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seqeunce"></param>
        /// <param name="action"></param>
        public static void Action<T>(this IEnumerable<T> seqeunce, Action<T> action)
        {
            foreach (var t in seqeunce)
                action(t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static T SelectMin<T, K>(this IEnumerable<T> sequence, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = sequence.First();
            var k0 = getKey(t0);

            foreach (T t in sequence.Skip(1))
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
        /// <param name="sequence"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var t0 = sequence.First();
            var k0 = getValue(t0);

            foreach (T t in sequence.Skip(1))
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
        /// <param name="sequence"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> sequence, Func<T, int> getValue)
        {
            var t0 = sequence.First();
            var k0 = getValue(t0);

            foreach (T t in sequence.Skip(1))
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
        /// <param name="sequence"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static T SelectMax<T, K>(this IEnumerable<T> sequence, Func<T, K> getKey)
            where K : IComparable<K>
        {
            var t0 = sequence.First();
            var k0 = getKey(t0);

            foreach (T t in sequence.Skip(1))
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
        /// <param name="sequence"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var t0 = sequence.First();
            var k0 = getValue(t0);

            foreach (T t in sequence.Skip(1))
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
        /// <param name="sequence"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> sequence, Func<T, int> getValue)
        {
            var t0 = sequence.First();
            var k0 = getValue(t0);

            foreach (T t in sequence.Skip(1))
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
        public static Vec2d Sum<T>(this IEnumerable<T> items, Func<T, Vec2d> getValue)
        {
            var result = new Vec2d();

            foreach (var t in items)
                result += getValue(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec3d Sum<T>(this IEnumerable<T> items, Func<T, Vec3d> getValue)
        {
            var result = new Vec3d();

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
        public static Vec2d WeightedSum<T>(this IEnumerable<T> items, Func<T, Vec2d> getValue, Func<T, double> getWeight)
        {
            var result = new Vec2d();

            foreach (var t in items)
                result += getValue(t) * getWeight(t);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec3d WeightedSum<T>(this IEnumerable<T> items, Func<T, Vec3d> getValue, Func<T, double> getWeight)
        {
            var result = new Vec3d();

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
        public static Vec2d Mean<T>(this IEnumerable<T> items, Func<T, Vec2d> getValue)
        {
            var result = new Vec2d();
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
        public static Vec3d Mean<T>(this IEnumerable<T> items, Func<T, Vec3d> getValue)
        {
            var result = new Vec3d();
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
        public static Vec2d WeightedMean<T>(this IEnumerable<T> items, Func<T, Vec2d> getValue, Func<T, double> getWeight)
        {
            var result = new Vec2d();
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
        public static Vec3d WeightedMean<T>(this IEnumerable<T> items, Func<T, Vec3d> getValue, Func<T, double> getWeight)
        {
            var result = new Vec3d();
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
            if (vector is double[])
                return ArrayMath.Sum((double[])vector);

            double sum = 0.0;
            foreach (double t in vector) sum += t;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this IEnumerable<double> vector)
        {
            if (vector is double[])
                return ArrayMath.Mean((double[])vector);

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


        #region IEnumerable<Vec2d>

        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(this IEnumerable<Vec2d> vectors)
        {
            if (vectors is Vec2d[])
                return ArrayMath.Sum((Vec2d[])vectors);

            Vec2d sum = new Vec2d();
            foreach (Vec2d v in vectors) sum += v;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(this IEnumerable<Vec2d> vectors)
        {
            if (vectors is Vec2d[])
                return ArrayMath.Mean((Vec2d[])vectors);

            Vec2d sum = new Vec2d();
            int count = 0;

            foreach (Vec2d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vec2d> vectors, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(this IEnumerable<Vec2d> vectors, Vec2d mean, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, mean, result);
        }

        #endregion


        #region IEnumerable<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(this IEnumerable<Vec3d> vectors)
        {
            if (vectors is Vec3d[])
                return ArrayMath.Sum((Vec3d[])vectors);

            var sum = new Vec3d();
            foreach (Vec3d v in vectors) sum += v;
            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(this IEnumerable<Vec3d> vectors)
        {
            if (vectors is Vec3d[])
                return ArrayMath.Mean((Vec3d[])vectors);

            Vec3d sum = new Vec3d();
            int count = 0;

            foreach (Vec3d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vec3d> vectors, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vec3d> vectors, Vec3d mean, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, mean, result);
        }

        #endregion


        #region IEnumerable<double[]>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void Sum(this IEnumerable<double[]> vectors, double[] result)
        {
            if (vectors is double[][])
            {
                ArrayMath.Sum((double[][])vectors, result);
                return;
            }

            Array.Clear(result, 0, vectors.First().Length);

            foreach (double[] v in vectors)
                ArrayMath.Add(result, v, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void Mean(this IEnumerable<double[]> vectors, double[] result)
        {
            if (vectors is double[][])
            {
                ArrayMath.Mean((double[][])vectors, result);
                return;
            }

            Array.Clear(result, 0, vectors.First().Length);

            int count = 0;
            foreach (double[] v in vectors)
            {
                ArrayMath.Add(result, v, result);
                count++;
            }

            ArrayMath.Scale(result, 1.0 / count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<double[]> vectors, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<double[]> vectors, double[] mean, double[] result)
        {
            DataUtil.GetCovarianceMatrix(vectors, result);
        }

        #endregion
    }
}