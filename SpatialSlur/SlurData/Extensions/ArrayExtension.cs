
/*
 * Notes
 */

using System;
using SpatialSlur.SlurData;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ArrayExtension
    {
        #region T[]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ArrayView<T> GetView<T>(this T[] array, int count)
        {
            return new ArrayView<T>(array, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ArrayView<T> GetView<T>(this T[] array, int index, int count)
        {
            return new ArrayView<T>(array, index, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyArrayView<T> GetReadOnlyView<T>(this T[] array, int count)
        {
            return new ReadOnlyArrayView<T>(array, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReadOnlyArrayView<T> GetReadOnlyView<T>(this T[] array, int index, int count)
        {
            return new ReadOnlyArrayView<T>(array, index, count);
        }



        /// <summary>
        /// Allows enumeration over indexable segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlyArrayView<T>> Batch<T>(this T[] source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return source.GetReadOnlyView(marker, n);
                marker += n;
            }
        }


        /// <summary>
        /// Allows enumeration over indexable segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlyArrayView<T>> Batch<T>(this ReadOnlyArrayView<T> source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return source.GetSubView(marker, n);
                marker += n;
            }
        }

        #endregion

        
        #region double[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double t, double[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double t, int size, double[] result, bool parallel = false)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);

            if (parallel)
                ArrayMath.Parallel.Lerp(vectors[i], vectors[i + 1], t, size, result);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double[] t, double[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double[] t, int size, double[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, size), range => Body(range.Item1, range.Item2));
            else
                Body(0, size);

            void Body(int from, int to)
            {
                int last = vectors.Length - 1;

                for (int j = from; j < to; j++)
                {
                    double tj = SlurMath.Fract(t[j] * last, out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];

                    result[j] = SlurMath.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            }
        }

        #endregion


        #region Vec2d[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double t, Vec2d[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double t, int size, Vec2d[] result, bool parallel = false)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);

            if (parallel)
                ArrayMath.Parallel.Lerp(vectors[i], vectors[i + 1], t, size, result);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double[] t, Vec2d[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double[] t, int size, Vec2d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, size), range => Body(range.Item1, range.Item2));
            else
                Body(0, size);

            void Body(int from, int to)
            {
                int last = vectors.Length - 1;

                for (int j = from; j < to; j++)
                {
                    double tj = SlurMath.Fract(t[j] * last, out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = Vec2d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            }
        }

        #endregion


        #region Vec3d[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double t, Vec3d[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double t, int size, Vec3d[] result, bool parallel = false)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);

            if (parallel)
                ArrayMath.Parallel.Lerp(vectors[i], vectors[i + 1], t, size, result);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double[] t, Vec3d[] result, bool parallel = false)
        {
            Lerp(vectors, t, vectors[0].Length, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double[] t, int size, Vec3d[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, size), range => Body(range.Item1, range.Item2));
            else
                Body(0, size);

            void Body(int from, int to)
            {
                int last = vectors.Length - 1;

                for (int j = from; j < to; j++)
                {
                    double tj = SlurMath.Fract(t[j] * last, out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = Vec3d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            }
        }

        #endregion
    }
}
