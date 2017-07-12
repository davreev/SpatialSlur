using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurData;

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
        /// <typeparam name="U"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMin<T, U>(this IEnumerable<T> sequence, Func<T, U> selector)
            where U : IComparable<U>
        {
            T tMin = sequence.ElementAt(0);
            U uMin = selector(tMin);

            foreach (T t in sequence.Skip(1))
            {
                var u = selector(t);

                if (u.CompareTo(uMin) < 0)
                {
                    tMin = t;
                    uMin = u;
                }
            }

            return tMin;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMax<T, U>(this IEnumerable<T> sequence, Func<T, U> selector)
            where U : IComparable<U>
        {
            T tMax = sequence.ElementAt(0);
            U uMax = selector(tMax);

            foreach (T t in sequence.Skip(1))
            {
                var u = selector(t);
                if (u.CompareTo(uMax) > 0)
                {
                    tMax = t;
                    uMax = u;
                }
            }

            return tMax;
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
        public static void Normalize<T>(this IEnumerable<T> items, Func<T, double> getValue, Action<T, double> setValue)
        {
            double sum = 0.0;

            foreach (var t in items)
                sum += getValue(t);

            if (sum > 0.0)
            {
                double inv = 1.0 / sum;

                foreach (var t in items)
                    setValue(t, getValue(t) * inv);
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
            GetCovarianceMatrix(vectors, vectors.Mean(), result);
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
            Array.Clear(result, 0, 4);

            // calculate covariance matrix
            foreach (Vec2d v in vectors)
            {
                Vec3d d = v - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[3] += d.Y * d.Y;
            }

            // set symmetric values
            result[2] = result[1];
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
            GetCovarianceMatrix(vectors, vectors.Mean(), result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vec3d> vectors, Vec3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Vec3d v in vectors)
            {
                Vec3d d = v - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
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

            Array.Clear(result, 0, vectors.ElementAt(0).Length);

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

            Array.Clear(result, 0, vectors.ElementAt(0).Length);

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
            var mean = new double[vectors.ElementAt(0).Length];
            vectors.Mean(mean);
            GetCovarianceMatrix(vectors, mean, result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<double[]> vectors, double[] mean, double[] result)
        {
            int n = mean.Length;
            Array.Clear(result, 0, n * n);

            // calculate lower triangular covariance matrix
            foreach (double[] v in vectors)
            {
                for (int j = 0; j < n; j++)
                {
                    double dj = v[j] - mean[j];
                    result[j * n + j] += dj * dj; // diagonal entry

                    for (int k = j + 1; k < n; k++)
                        result[j * n + k] += dj * (v[k] - mean[k]);
                }
            }

            // fill out upper triangular
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * n + i] = result[i * n + j];
            }
        }

        #endregion
    }
}