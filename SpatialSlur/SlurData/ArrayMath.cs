
/*
 * Notes
 * 
 * TODO test SIMD optimization https://docs.microsoft.com/en-us/dotnet/api/system.numerics.vector-1?view=netcore-2.0
 */

using System;
using System.Collections.Concurrent;
using SpatialSlur.SlurCore;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ArrayMath
    {
        #region Nested types

        /// <summary>
        /// Parallel implementations
        /// </summary>
        public static partial class Parallel
        {
            #region T[]

            /// <summary>
            /// Sets the result to some function of the given vector.
            /// </summary>
            public static void Function<T, U>(T[] values, Func<T, U> func, U[] result)
            {
                Function(values, values.Length, func, result);
            }


            /// <summary>
            /// Sets the result to some function of the given vector.
            /// </summary>
            public static void Function<T, U>(T[] values, int count, Func<T, U> func, U[] result)
            {
                ForEach(Partitioner.Create(0, count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = func(values[i]);
                });
            }


            /// <summary>
            /// Sets the result to some function of the 2 given vectors.
            /// </summary>
            public static void Function<T0, T1, U>(T0[] v0, T1[] v1, Func<T0, T1, U> func, U[] result)
            {
                Function(v0, v1, v0.Length, func, result);
            }


            /// <summary>
            /// Sets the result to some function of the 2 given vectors.
            /// </summary>
            public static void Function<T0, T1, U>(T0[] v0, T1[] v1, int count, Func<T0, T1, U> func, U[] result)
            {
                ForEach(Partitioner.Create(0, count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = func(v0[i], v1[i]);
                });
            }


            /// <summary>
            /// Sets the result to some function of 3 given vectors.
            /// </summary>
            public static void Function<T0, T1, T2, U>(T0[] v0, T1[] v1, T2[] v2, Func<T0, T1, T2, U> func, U[] result)
            {
                Function(v0, v1, v2, v0.Length, func, result);
            }


            /// <summary>
            /// Sets the result to some function of 3 given vectors.
            /// </summary>
            public static void Function<T0, T1, T2, U>(T0[] v0, T1[] v1, T2[] v2, int count, Func<T0, T1, T2, U> func, U[] result)
            {
                ForEach(Partitioner.Create(0, count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = func(v0[i], v1[i], v2[i]);
                });
            }

            #endregion


            #region double[]

            /// <summary>
            /// 
            /// </summary>
            public static bool Unitize(double[] vector, double[] result)
            {
                return Unitize(vector, vector.Length, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static bool Unitize(double[] vector, int count, double[] result)
            {
                double d = ArrayMath.Dot(vector, vector, count);

                if (d > 0.0)
                {
                    Scale(vector, 1.0 / Math.Sqrt(d), count, result);
                    return true;
                }

                return false;
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Slerp(double[] v0, double[] v1, double t, double[] result)
            {
                Slerp(v0, v1, t, ArrayMath.Angle(v0, v1), v0.Length, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Slerp(double[] v0, double[] v1, double t, double angle, double[] result)
            {
                Slerp(v0, v1, t, angle, v0.Length, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Slerp(double[] v0, double[] v1, double t, double angle, int count, double[] result)
            {
                double sa = Math.Sin(angle);
                
                if (sa > 0.0)
                {
                    var saInv = 1.0 / sa;
                    var at = angle * t;
                    AddScaled(v0, Math.Sin(angle - at) * saInv, v1, Math.Sin(at) * saInv, count, result);
                }
            }

            #endregion


            #region Vec2d[] 

            /// <summary>
            /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
            /// </summary>
            public static void Cross(Vec2d[] v0, Vec2d[] v1, double[] result)
            {
                Cross(v0, v1, v0.Length, result);
            }


            /// <summary>
            /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
            /// </summary>
            public static void Cross(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
            {
                ForEach(Partitioner.Create(0, count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Vec2d.Cross(v0[i], v1[i]);
                });
            }

            #endregion


            #region Vec3d[]

            /// <summary>
            /// 
            /// </summary>
            public static void Cross(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
            {
                Cross(v0, v1, v0.Length, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Cross(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
            {
                ForEach(Partitioner.Create(0, count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Vec3d.Cross(v0[i], v1[i]);
                });
            }

            #endregion
        }
        
        #endregion


        #region T[]

        /// <summary>
        /// Sets the result to some function of the given vector.
        /// </summary>
        public static void Function<T, U>(T[] values, Func<T, U> func, U[] result)
        {
            Function(values, values.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the given vector.
        /// </summary>
        public static void Function<T, U>(T[] values, int count, Func<T, U> func, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = func(values[i]);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void Function<T0, T1, U>(T0[] v0, T1[] v1, Func<T0, T1, U> func, U[] result)
        {
            Function(v0, v1, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void Function<T0, T1, U>(T0[] v0, T1[] v1, int count, Func<T0, T1, U> func, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = func(v0[i], v1[i]);
        }


        /// <summary>
        /// Sets the result to some function of the 3 given vectors.
        /// </summary>
        public static void Function<T0, T1, T2, U>(T0[] v0, T1[] v1, T2[] v2, Func<T0, T1, T2, U> func, U[] result)
        {
            Function(v0, v1, v2, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 3 given vectors.
        /// </summary>
        public static void Function<T0, T1, T2, U>(T0[] v0, T1[] v1, T2[] v2, int count, Func<T0, T1, T2, U> func, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = func(v0[i], v1[i], v2[i]);
        }

        #endregion


        #region double[]
        
        /// <summary>
        /// Returns Euclidean length
        /// </summary>
        public static double NormL2(double[] vector)
        {
            return NormL2(vector, vector.Length);
        }


        /// <summary>
        /// Returns Euclidean length
        /// </summary>
        public static double NormL2(double[] vector, int count)
        {
            return Math.Sqrt(Dot(vector, vector, count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL2(double[] v0, double[] v1)
        {
            return DistanceL2(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL2(double[] v0, double[] v1, int count)
        {
            return Math.Sqrt(SquareDistanceL2(v0, v1, count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(double[] vector, double[] result)
        {
            return Unitize(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(double[] vector, int count, double[] result)
        {
            var d = Dot(vector, vector, count);

            if (d > 0.0)
            {
                Scale(vector, 1.0 / Math.Sqrt(d), count, result);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Angle(double[] v0, double[] v1)
        {
            return Angle(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Angle(double[] v0, double[] v1, int count)
        {
            var d = Dot(v0, v0, count) * Dot(v1, v1, count);
            return d > 0.0 ? SlurMath.AcosSafe(Dot(v0, v1) / Math.Sqrt(d)) : 0.0;
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(double[] v0, double[] v1, double t, double[] result)
        {
            Slerp(v0, v1, t, Angle(v0, v1), v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(double[] v0, double[] v1, double t, double angle, double[] result)
        {
            Slerp(v0, v1, t, angle, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(double[] v0, double[] v1, double t, double angle, int count, double[] result)
        {
            double sa = Math.Sin(angle);

            if (sa > 0.0)
            {
                var saInv = 1.0 / sa;
                var at = angle * t;
                AddScaled(v0, Math.Sin(angle - at) * saInv, v1, Math.Sin(at) * saInv, count, result);
            }
        }
        
        #endregion


        #region Vec2d[]

        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Cross(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Cross(v0[i], v1[i]);
        }

        #endregion


        #region Vec3d[]

        /// <summary>
        /// 
        /// </summary>
        public static void Cross(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Cross(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Cross(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Cross(v0[i], v1[i]);
        }

        #endregion


        #region double[][]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double tolerance = SlurMath.ZeroTolerance)
        {
            return ApproxEquals(v0, v1, v0.Length, v0[0].Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, int count, double tolerance = SlurMath.ZeroTolerance)
        {
            return ApproxEquals(v0, v1, count, v0[0].Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, int count, int size, double tolerance = SlurMath.ZeroTolerance)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], size, tolerance)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(double[][] vectors, double[] result)
        {
            Sum(vectors, vectors.Length, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(double[][] vectors, int count, double[] result)
        {
            Sum(vectors, count, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(double[][] vectors, int count, int size, double[] result)
        {
            Array.Clear(result, 0, size);

            for (int i = 0; i < count; i++)
                Add(result, vectors[i], size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(double[][] vectors, double[] result)
        {
            Sum(vectors, result);
            Scale(result, 1.0 / vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(double[][] vectors, int count, double[] result)
        {
            Sum(vectors, count, result);
            Scale(result, 1.0 / count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(double[][] vectors, int count, int size, double[] result)
        {
            Sum(vectors, count, size, result);
            Scale(result, 1.0 / count, size, result);
        }

        #endregion
    }
}
