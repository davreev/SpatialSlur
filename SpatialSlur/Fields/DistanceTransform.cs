
/*
 * Notes
 * 
 * impl refs
 * https://github.com/prideout/heman/blob/master/src/distance.c
 * http://cs.brown.edu/people/pfelzens/papers/dt-final.pdf
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.Collections;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public class DistanceTransform
    {
        #region Nested Types

        /// <summary>
        /// Static creation methods for generic type inference
        /// </summary>
        static class View
        {
            /// <summary>
            /// 
            /// </summary>
            public static View<T> Create<T>(T[] source, int start)
            {
                return new View<T>(source, start, 1);
            }


            /// <summary>
            /// 
            /// </summary>
            public static View<T> Create<T>(T[] source, int start, int stride)
            {
                return new View<T>(source, start, stride);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        readonly struct View<T>
        {
            private readonly T[] _source;
            private readonly int _start;
            private readonly int _stride;

            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="start"></param>
            /// <param name="stride"></param>
            public View(T[] source, int start, int stride)
            {
                _source = source;
                _start = start;
                _stride = stride;
            }


            /// <summary>
            /// 
            /// </summary>
            public T[] Source
            {
                get => _source;
            }


            /// <summary>
            /// 
            /// </summary>
            public int Start
            {
                get => _start;
            }


            /// <summary>
            /// 
            /// </summary>
            public int Stride
            {
                get => _stride;
            }


            /// <summary>
            /// Note that this does not perform an additional bounds check.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ref T this[int index]
            {
                get => ref _source[index * _stride + _start];
            }
        }

        #endregion


        #region Static Members

        private static readonly int _batchCount = Environment.ProcessorCount << 2;


        /// <summary>
        /// 
        /// </summary>
        private static void CalculateL2Sqr(View<double> source, View<(int V, double Z)> parabolas, double scale, int count, View<double> result)
        {
            // impl ref
            // http://cs.brown.edu/people/pfelzens/papers/dt-final.pdf

            var scaleSqr = scale * scale;
            var scale2 = scale * 2.0;

            // compute lower envelope
            {
                int k = 0; // index of right-most parabola

                // set the kth parabola and ready the next
                parabolas[0] = (0, double.NegativeInfinity);
                parabolas[1].Z = double.PositiveInfinity;

                for (int i = 1; i < count; i++)
                {
                    var s = Intersect(i, parabolas[k].V);

                    // backtrack while the parabola at i is lower than the kth (i.e. its intersection precedes that of the kth)
                    while (s <= parabolas[k].Z)
                        s = Intersect(i, parabolas[--k].V);

                    // set the kth parabola and ready the next
                    parabolas[++k] = (i, s);
                    parabolas[k + 1].Z = double.PositiveInfinity;
                }
            }

            // evaluate distance
            {
                int k = 0; // index of the parabola to the left of i

                for (int i = 0; i < count; i++)
                {
                    var ti = i * scale;
                    while (parabolas[k + 1].Z < ti) k++;
                    result[i] = DistanceSqr(i, parabolas[k].V);
                }
            }

            // Returns x coordinate of intersection between parabolas at q and r
            double Intersect(int q, int r)
            {
                return ((source[q] + q * q * scaleSqr) - (source[r] + r * r * scaleSqr)) / ((q - r) * scale2);
            }

            // Returns the squared distance between q and r
            double DistanceSqr(int q, int r)
            {
                var d = (q - r) * scale;
                return d * d + source[r];
            }
        }


#if OBSOLETE
        /// <summary>
        /// Assumes unit scale
        /// </summary>
        private static void CalculateL2Sqr(View<double> source, View<(int V, double Z)> parabolas, int count, View<double> result)
        {
            // impl ref
            // http://cs.brown.edu/people/pfelzens/papers/dt-final.pdf

            // compute lower envelope
            {
                int k = 0; // index of right-most parabola

                // set the kth parabola and ready the next
                parabolas[0] = (0, double.NegativeInfinity);
                parabolas[1].Z = double.PositiveInfinity;

                for (int i = 1; i < count; i++)
                {
                    var s = Intersect(i, parabolas[k].V);

                    // backtrack while the parabola at i is lower than the kth (i.e. its intersection precedes that of the kth)
                    while (s <= parabolas[k].Z)
                        s = Intersect(i, parabolas[--k].V);

                    // set kth parabola and ready the next
                    parabolas[++k] = (i, s);
                    parabolas[k + 1].Z = double.PositiveInfinity;
                }
            }

            // evaluate distance
            {
                int k = 0; // index of the parabola to the left of i

                for (int i = 0; i < count; i++)
                {
                    while (parabolas[k + 1].Z < i) k++;
                    result[i] = DistanceSqr(i, parabolas[k].V);
                }
            }

            // Returns x coordinate of intersection between parabolas at q and r
            double Intersect(int q, int r)
            {
                return ((source[q] + q * q) - (source[r] + r * r)) / ((q - r) << 1);
            }

            // Returns the squared distance between q and r
            double DistanceSqr(int q, int r)
            {
                var d = (q - r);
                return d * d + source[r];
            }
        }
#endif


        /// <summary>
        /// Re-allocates the source array if its length is less than the given minimum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="min"></param>
        private static void EnsureCapacity<T>(ref T[] source, int min)
        {
            if (source.Length < min)
                source = new T[min];
        }

#endregion


        private double[] _partial = Array.Empty<double>(); // buffer for storing partial results
        private (int, double)[] _parabolas = Array.Empty<(int, double)>(); // buffer for storing parabolas


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void CalculateL2(GridField2d<double> field, double[] result, bool parallel = false)
        {
            CalculateL2Sqr(field, result, parallel);

            if (parallel)
                Vector.Parallel.Sqrt(result, result);
            else
                Vector.Sqrt(result, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void CalculateL2Sqr(GridField2d<double> field, double[] result, bool parallel = false)
        {
            if (parallel)
            {
                CalculateL2SqrParallel(field, result);
                return;
            }

            var source = field.Values;
            (var nx, var ny) = field.Count;
            (var tx, var ty) = field.Scale;

            // resize buffers if necessary
            EnsureCapacity(ref _parabolas, Math.Max(nx, ny) + 1);
            EnsureCapacity(ref _partial, field.CountXY);

            var parabolas = View.Create(_parabolas, 0);
     
            // compute y direction
            {
                for (int x = 0; x < nx; x++)
                {
                    var src = View.Create(source, x, nx);
                    var dst = View.Create(_partial, x, nx);
                    CalculateL2Sqr(src, parabolas, ty, ny, dst);
                }
            }

            // compute x direction
            {
                for (int y = 0; y < ny; y++)
                {
                    var start = y * nx;
                    var src = View.Create(_partial, start);
                    var dst = View.Create(result, start);
                    CalculateL2Sqr(src, parabolas, tx, nx, dst);
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="result"></param>
        private void CalculateL2SqrParallel(GridField2d<double> field, double[] result)
        {
            var source = field.Values;
            (var nx, var ny) = field.Count;
            (var tx, var ty) = field.Scale;

            // resize buffers if necessary
            var batchSize = Math.Max(nx, ny) + 1;
            EnsureCapacity(ref _parabolas, batchSize * _batchCount);
            EnsureCapacity(ref _partial, field.CountXY);

            var batchesX = new UniformPartitioner(0, nx, _batchCount);
            var batchesY = new UniformPartitioner(0, ny, _batchCount);
            
            // compute y direction
            Parallel.For(0, _batchCount, i =>
            {
                (var x0, var x1) = batchesX[i];
                var parabolas = View.Create(_parabolas, i * batchSize); // buffer for current thread

                for (int x = x0; x < x1; x++)
                {
                    var src = View.Create(source, x, nx);
                    var dst = View.Create(_partial, x, nx);
                    CalculateL2Sqr(src, parabolas, ty, ny, dst);
                }
            });

            // compute x direction
            Parallel.For(0, _batchCount, i =>
            {
                (var y0, var y1) = batchesY[i];
                var parabolas = View.Create(_parabolas, i * batchSize); // buffer for current thread

                for (int y = y0; y < y1; y++)
                {
                    var start = y * nx;
                    var src = View.Create(_partial, start);
                    var dst = View.Create(result, start);
                    CalculateL2Sqr(src, parabolas, tx, nx, dst);
                }
            });
        }
    }
}
