
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Drawing;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

using D = SpatialSlur.SlurMath.Constantsd;
using F = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
	/// <summary>
	/// Contains common operations for n-dimensional vectors
	/// </summary>
	public static partial class Vector
	{
        /// <summary>
	    /// Contains parallel implementations
	    /// </summary>
        public static partial class Parallel
        {

            #region double

            /// <summary>
            /// 
            /// </summary>
            public static void Max(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Max(v0[i], v1[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Min(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Min(v0[i], v1[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Abs(ReadOnlyArrayView<double> vector, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Abs(vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Sqrt(ReadOnlyArrayView<double> vector, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Sqrt(vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Add(ReadOnlyArrayView<double> vector, double scalar, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = vector[i] + scalar;
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Add(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Subtract(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] - v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Multiply(ReadOnlyArrayView<double> vector, double scalar, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = vector[i] * scalar;
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MultiplyPointwise(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void DividePointwise(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] / v1[i];
                });
            }


            /// <summary>
            /// result = v0 + v1 * t
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i] * t;
                });
            }


            /// <summary>
            /// result = v0 + v1 * t
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i] * t[i];
                });
            }


            /// <summary>
            /// result = v0 * t0 + v1 * t1
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<double> v0, double t0, ReadOnlyArrayView<double> v1, double t1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * t0 + v1[i] * t1;
                });
            }


            /// <summary>
            /// result = v0 * t0 + v1 * t1
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t1, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * t0[i] + v1[i] * t1[i];
                });
            }


            /// <summary>
            /// result = v0 + (v1 - v2) * t
            /// </summary>
            public static void AddScaledDelta(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, double t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + (v1[i] - v2[i]) * t;
                });
            }


            /// <summary>
            /// result = v0 + (v1 - v2) * t
            /// </summary>
            public static void AddScaledDelta(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, ReadOnlyArrayView<double> t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + (v1[i] - v2[i]) * t[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<double> vector, double value, double t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(vector[i], value, t);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(v0[i], v1[i], t);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Normalize(ReadOnlyArrayView<double> vector, Intervald interval, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Normalize(vector[i], interval.A, interval.B);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Evaluate(ReadOnlyArrayView<double> vector, Intervald interval, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(interval.A, interval.B, vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Remap(ReadOnlyArrayView<double> vector, Intervald from, Intervald to, ArrayView<double> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Remap(vector[i], from.A, from.B, to.A, to.B);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Project(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                Multiply(v1, Dot(v0, v1) / Dot(v1, v1), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Reject(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                Project(v0, v1, result);
                Subtract(v0, result, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Reflect(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                Multiply(v1, Dot(v0, v1) / Dot(v1, v1) * 2.0d, result);
                AddScaled(result, v0, -1.0d, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MatchProjection(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
            {
                Multiply(v0, Dot(v1, v1) / Dot(v0, v1), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MatchProjection(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, ArrayView<double> result)
            {
                Multiply(v0, Dot(v1, v2) / Dot(v0, v2), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static bool Unitize(ReadOnlyArrayView<double> vector, ArrayView<double> result)
            {
                var d = Dot(vector, vector);

                if (d > 0.0d)
                {
                    Multiply(vector, 1.0d / SlurMath.Sqrt(d), result);
                    return true;
                }

                return false;
            }

            #endregion


            #region float

            /// <summary>
            /// 
            /// </summary>
            public static void Max(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Max(v0[i], v1[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Min(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Min(v0[i], v1[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Abs(ReadOnlyArrayView<float> vector, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = Math.Abs(vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Sqrt(ReadOnlyArrayView<float> vector, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Sqrt(vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Add(ReadOnlyArrayView<float> vector, float scalar, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = vector[i] + scalar;
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Add(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Subtract(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] - v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Multiply(ReadOnlyArrayView<float> vector, float scalar, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = vector[i] * scalar;
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MultiplyPointwise(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * v1[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void DividePointwise(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] / v1[i];
                });
            }


            /// <summary>
            /// result = v0 + v1 * t
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i] * t;
                });
            }


            /// <summary>
            /// result = v0 + v1 * t
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + v1[i] * t[i];
                });
            }


            /// <summary>
            /// result = v0 * t0 + v1 * t1
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<float> v0, float t0, ReadOnlyArrayView<float> v1, float t1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * t0 + v1[i] * t1;
                });
            }


            /// <summary>
            /// result = v0 * t0 + v1 * t1
            /// </summary>
            public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> t0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t1, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] * t0[i] + v1[i] * t1[i];
                });
            }


            /// <summary>
            /// result = v0 + (v1 - v2) * t
            /// </summary>
            public static void AddScaledDelta(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, float t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + (v1[i] - v2[i]) * t;
                });
            }


            /// <summary>
            /// result = v0 + (v1 - v2) * t
            /// </summary>
            public static void AddScaledDelta(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, ReadOnlyArrayView<float> t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = v0[i] + (v1[i] - v2[i]) * t[i];
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<float> vector, float value, float t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(vector[i], value, t);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(v0[i], v1[i], t);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Lerp(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, v0.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Normalize(ReadOnlyArrayView<float> vector, Intervalf interval, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Normalize(vector[i], interval.A, interval.B);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Evaluate(ReadOnlyArrayView<float> vector, Intervalf interval, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Lerp(interval.A, interval.B, vector[i]);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Remap(ReadOnlyArrayView<float> vector, Intervalf from, Intervalf to, ArrayView<float> result)
            {
                ForEach(Partitioner.Create(0, vector.Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        result[i] = SlurMath.Remap(vector[i], from.A, from.B, to.A, to.B);
                });
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Project(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                Multiply(v1, Dot(v0, v1) / Dot(v1, v1), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Reject(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                Project(v0, v1, result);
                Subtract(v0, result, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void Reflect(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                Multiply(v1, Dot(v0, v1) / Dot(v1, v1) * 2.0f, result);
                AddScaled(result, v0, -1.0f, result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MatchProjection(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
            {
                Multiply(v0, Dot(v1, v1) / Dot(v0, v1), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static void MatchProjection(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, ArrayView<float> result)
            {
                Multiply(v0, Dot(v1, v2) / Dot(v0, v2), result);
            }


            /// <summary>
            /// 
            /// </summary>
            public static bool Unitize(ReadOnlyArrayView<float> vector, ArrayView<float> result)
            {
                var d = Dot(vector, vector);

                if (d > 0.0f)
                {
                    Multiply(vector, 1.0f / SlurMath.Sqrt(d), result);
                    return true;
                }

                return false;
            }

            #endregion

        }


		#region double

        /// <summary>
        /// 
        /// </summary>
        public static double Max(ReadOnlyArrayView<double>  vector)
        {
            var result = vector[0];

            for (int i = 1; i < vector.Count; i++)
				result = Math.Max(vector[i], result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(ReadOnlyArrayView<double>  vector)
        {
            var result = vector[0];

            for (int i = 1; i < vector.Count; i++)
				result = Math.Min(vector[i], result);

            return result;
        }


		/// <summary>
		/// 
		/// </summary>
		public static double Sum(ReadOnlyArrayView<double> vector)
		{
			var sum = default(double);

			for (int i = 0; i < vector.Count; i++)
				sum += vector[i];

			return sum;
		}


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(ReadOnlyArrayView<double> vector, ReadOnlyArrayView<double> weights)
        {
            var result = 0.0d;

            for (int i = 0; i < vector.Count; i++)
                result += vector[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(ReadOnlyArrayView<double> vector)
        {
            return Sum(vector) / vector.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(ReadOnlyArrayView<double> vector, ReadOnlyArrayView<double> weights)
        {
            var sum = 0.0d;
            var wsum = 0.0d;

            for (int i = 0; i < vector.Count; i++)
            {
                var w = weights[i];
                sum += vector[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double epsilon = D.ZeroTolerance)
        {
            for (int i = 0; i < v0.Count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double>  epsilon)
        {
            for (int i = 0; i < v0.Count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<double>  v0, ReadOnlyArrayView<double>  v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = Math.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<double>  v0, ReadOnlyArrayView<double>  v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = Math.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<double> vector, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = Math.Abs(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sqrt(ReadOnlyArrayView<double> vector, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Sqrt(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<double> vector, double value, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = vector[i] + value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<double> vector, double t, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = vector[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<double> v0, double t0, ReadOnlyArrayView<double> v1, double t1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> t0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t1, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - x) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double x, double t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - x) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, double t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, ReadOnlyArrayView<double> t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<double> vector, double value, double t, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Lerp(vector[i], value, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, double t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> t, ArrayView<double> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(ReadOnlyArrayView<double> vector, Intervald interval, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Normalize(vector[i], interval.A, interval.B);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(ReadOnlyArrayView<double> vector, Intervald interval, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Lerp(interval.A, interval.B, vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(ReadOnlyArrayView<double> vector, Intervald from, Intervald to, ArrayView<double> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Remap(vector[i], from.A, from.B, to.A, to.B);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(ReadOnlyArrayView<double>  v0, ReadOnlyArrayView<double>  v1)
        {
            var result = 0.0d;

            for (int i = 0; i < v0.Count; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AbsDot(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double>  v1)
        {
            var result = 0.0d;

            for (int i = 0; i < v0.Count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            Multiply(v1, Dot(v0, v1) / Dot(v1, v1), result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            Project(v0, v1, result);
            Subtract(v0, result, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            Multiply(v1, Dot(v0, v1) / Dot(v1, v1) * 2.0d, result);
            AddScaled(result, v0, -1.0d, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ArrayView<double> result)
        {
            Multiply(v0, Dot(v1, v1) / Dot(v0, v1), result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1, ReadOnlyArrayView<double> v2, ArrayView<double> result)
        {
            Multiply(v0, Dot(v1, v2) / Dot(v0, v2), result);
        }


        /// <summary>
        /// Returns the L1 or Manhattan length of the given vector.
        /// </summary>
        public static double NormL1(ReadOnlyArrayView<double> vector)
        {
            var result = 0.0d;

            for (int i = 0; i < vector.Count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// Returns the L2 or Euclidean length of the given vector.
        /// </summary>
        public static double NormL2(ReadOnlyArrayView<double> vector)
        {
            return SlurMath.Sqrt(Dot(vector, vector));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL1(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1)
        {
            var result = 0.0d;

            for (int i = 0; i < v0.Count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL2(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1)
        {
            return SlurMath.Sqrt(SquareDistanceL2(v0, v1));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareDistanceL2(ReadOnlyArrayView<double> v0, ReadOnlyArrayView<double> v1)
        {
            var result = 0.0d;

            for (int i = 0; i < v0.Count; i++)
            {
                var d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(ReadOnlyArrayView<double> vector, ArrayView<double> result)
        {
            var d = Dot(vector, vector);

            if (d > 0.0d)
            {
                Multiply(vector, 1.0d / SlurMath.Sqrt(d), result);
                return true;
            }

            return false;
        }

		#endregion


		#region float

        /// <summary>
        /// 
        /// </summary>
        public static float Max(ReadOnlyArrayView<float>  vector)
        {
            var result = vector[0];

            for (int i = 1; i < vector.Count; i++)
				result = Math.Max(vector[i], result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Min(ReadOnlyArrayView<float>  vector)
        {
            var result = vector[0];

            for (int i = 1; i < vector.Count; i++)
				result = Math.Min(vector[i], result);

            return result;
        }


		/// <summary>
		/// 
		/// </summary>
		public static float Sum(ReadOnlyArrayView<float> vector)
		{
			var sum = default(float);

			for (int i = 0; i < vector.Count; i++)
				sum += vector[i];

			return sum;
		}


        /// <summary>
        /// 
        /// </summary>
        public static float Sum(ReadOnlyArrayView<float> vector, ReadOnlyArrayView<float> weights)
        {
            var result = 0.0f;

            for (int i = 0; i < vector.Count; i++)
                result += vector[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Mean(ReadOnlyArrayView<float> vector)
        {
            return Sum(vector) / vector.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Mean(ReadOnlyArrayView<float> vector, ReadOnlyArrayView<float> weights)
        {
            var sum = 0.0f;
            var wsum = 0.0f;

            for (int i = 0; i < vector.Count; i++)
            {
                var w = weights[i];
                sum += vector[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float epsilon = F.ZeroTolerance)
        {
            for (int i = 0; i < v0.Count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float>  epsilon)
        {
            for (int i = 0; i < v0.Count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(ReadOnlyArrayView<float>  v0, ReadOnlyArrayView<float>  v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = Math.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(ReadOnlyArrayView<float>  v0, ReadOnlyArrayView<float>  v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = Math.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(ReadOnlyArrayView<float> vector, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = Math.Abs(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sqrt(ReadOnlyArrayView<float> vector, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Sqrt(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<float> vector, float value, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = vector[i] + value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Multiply(ReadOnlyArrayView<float> vector, float t, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = vector[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MultiplyPointwise(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DividePointwise(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<float> v0, float t0, ReadOnlyArrayView<float> v1, float t1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> t0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t1, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - x) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float x, float t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - x) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, float t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, ReadOnlyArrayView<float> t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<float> vector, float value, float t, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Lerp(vector[i], value, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, float t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> t, ArrayView<float> result)
        {
            for (int i = 0; i < v0.Count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(ReadOnlyArrayView<float> vector, Intervalf interval, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Normalize(vector[i], interval.A, interval.B);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(ReadOnlyArrayView<float> vector, Intervalf interval, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Lerp(interval.A, interval.B, vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(ReadOnlyArrayView<float> vector, Intervalf from, Intervalf to, ArrayView<float> result)
        {
            for (int i = 0; i < vector.Count; i++)
                result[i] = SlurMath.Remap(vector[i], from.A, from.B, to.A, to.B);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Dot(ReadOnlyArrayView<float>  v0, ReadOnlyArrayView<float>  v1)
        {
            var result = 0.0f;

            for (int i = 0; i < v0.Count; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float AbsDot(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float>  v1)
        {
            var result = 0.0f;

            for (int i = 0; i < v0.Count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            Multiply(v1, Dot(v0, v1) / Dot(v1, v1), result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            Project(v0, v1, result);
            Subtract(v0, result, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            Multiply(v1, Dot(v0, v1) / Dot(v1, v1) * 2.0f, result);
            AddScaled(result, v0, -1.0f, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ArrayView<float> result)
        {
            Multiply(v0, Dot(v1, v1) / Dot(v0, v1), result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1, ReadOnlyArrayView<float> v2, ArrayView<float> result)
        {
            Multiply(v0, Dot(v1, v2) / Dot(v0, v2), result);
        }


        /// <summary>
        /// Returns the L1 or Manhattan length of the given vector.
        /// </summary>
        public static float NormL1(ReadOnlyArrayView<float> vector)
        {
            var result = 0.0f;

            for (int i = 0; i < vector.Count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// Returns the L2 or Euclidean length of the given vector.
        /// </summary>
        public static float NormL2(ReadOnlyArrayView<float> vector)
        {
            return SlurMath.Sqrt(Dot(vector, vector));
        }


        /// <summary>
        /// 
        /// </summary>
        public static float DistanceL1(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1)
        {
            var result = 0.0f;

            for (int i = 0; i < v0.Count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float DistanceL2(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1)
        {
            return SlurMath.Sqrt(SquareDistanceL2(v0, v1));
        }


        /// <summary>
        /// 
        /// </summary>
        public static float SquareDistanceL2(ReadOnlyArrayView<float> v0, ReadOnlyArrayView<float> v1)
        {
            var result = 0.0f;

            for (int i = 0; i < v0.Count; i++)
            {
                var d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(ReadOnlyArrayView<float> vector, ArrayView<float> result)
        {
            var d = Dot(vector, vector);

            if (d > 0.0f)
            {
                Multiply(vector, 1.0f / SlurMath.Sqrt(d), result);
                return true;
            }

            return false;
        }

		#endregion

	}
}