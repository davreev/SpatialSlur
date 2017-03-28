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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> sequence, Comparison<T> compare)
        {
            T tMin = sequence.ElementAt(0);

            foreach (T t in sequence.Skip(1))
                if (compare(t, tMin) < 0) tMin = t;

            return tMin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMin<T>(this IEnumerable<T> sequence, Func<T, double> selector)
        {
            T tMin = sequence.ElementAt(0);
            double dMin = selector(tMin);

            foreach (T t in sequence.Skip(1))
            {
                double d = selector(t);
                if (d < dMin)
                {
                    tMin = t;
                    dMin = d;
                }
            }

            return tMin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> sequence, Comparison<T> compare)
        {
            T tMax = sequence.ElementAt(0);

            foreach (T t in sequence.Skip(1))
                if (compare(t, tMax) > 0) tMax = t;

            return tMax;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T SelectMax<T>(this IEnumerable<T> sequence, Func<T, double> selector)
        {
            T tMax = sequence.ElementAt(0);
            double dMax = selector(tMax);

            foreach (T t in sequence.Skip(1))
            {
                double d = selector(t);
                if (d > dMax)
                {
                    tMax = t;
                    dMax = d;
                }
            }

            return tMax;
        }

        #endregion


        #region IEnumerable<double>

        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this IEnumerable<double> vector)
        {
            if (vector is double[])
                return ArrayExtensions.Sum((double[])vector);

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
                return ArrayExtensions.Mean((double[])vector);

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
                return ArrayExtensions.Sum((Vec2d[])vectors);

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
                return ArrayExtensions.Mean((Vec2d[])vectors);

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
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[3] += d.y * d.y;
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
                return ArrayExtensions.Sum((Vec3d[])vectors);

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
                return ArrayExtensions.Mean((Vec3d[])vectors);

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
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[2] += d.x * d.z;
                result[4] += d.y * d.y;
                result[5] += d.y * d.z;
                result[8] += d.z * d.z;
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
                ArrayExtensions.Sum((double[][])vectors, result);
                return;
            }

            Array.Clear(result, 0, vectors.ElementAt(0).Length);

            foreach (double[] v in vectors)
                ArrayExtensions.Add(result, v, result);
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
                ArrayExtensions.Mean((double[][])vectors, result);
                return;
            }

            Array.Clear(result, 0, vectors.ElementAt(0).Length);

            int count = 0;
            foreach (double[] v in vectors)
            {
                ArrayExtensions.Add(result, v, result);
                count++;
            }

            ArrayExtensions.Scale(result, 1.0 / count, result);
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