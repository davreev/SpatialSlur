
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Drawing;
using SpatialSlur.SlurCore;
using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.SlurData
{
	/// <summary>
	/// 
	/// </summary>
	public static partial class ArrayMath
	{
		#region Common

		#region double

		/// <summary>
		/// 
		/// </summary>
		public static double Sum(double[] values)
		{
			return Sum(values, values.Length);
		} 


		/// <summary>
		/// 
		/// </summary>
		public static double Sum(double[] values, int count)
		{
			var sum = default(double);

			for (int i = 0; i < count; i++)
				sum += values[i];

			return sum;
		}

		
		/// <summary>
        /// 
        /// </summary>
        public static double Mean(double[] values)
        {
            return Sum(values) / values.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(double[] values, int count)
        {
            return Sum(values, count) / count;
        }

		
        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(double[] values, double[] weights)
        {
            return WeightedSum(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(double[] values, double[] weights, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
                result += values[i] * weights[i];

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(double[] values, double[] weights)
        {
            return WeightedMean(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(double[] values, double[] weights, int count)
        {
            var sum = default(double);
            var wsum = default(double);

            for (int i = 0; i < count; i++)
            {
                var w = weights[i];
                sum += values[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(double[] v0, double v1, double[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(double[] v0, double v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(double[] v0, double[] v1, double[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(double[] v0, double[] v1, double[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Scale(double[] values, double t, double[] result)
        {
            Scale(values, t, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(double[] values, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = values[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(double[] v0, double[] v1, double t, double[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(double[] v0, double[] v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(double[] v0, double[] v1, double[] t, double[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(double[] v0, double t0, double[] v1, double t1, double[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(double[] v0, double t0, double[] v1, double t1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(double[] v0, double[] t0, double[] v1, double[] t1, double[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(double[] v0, double[] t0, double[] v1, double[] t1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double v2, double t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double v2, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double[] v2, double t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double[] v2, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double[] v2, double[] t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(double[] v0, double[] v1, double[] v2, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(double[] v0, double[] v1, double[] result)
        {
            Multiply(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(double[] v0, double[] v1, double[] result)
        {
            Divide(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(double[] vector, Intervald interval, double[] result)
        {
            Normalize(vector, interval, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(double[] vector, Intervald interval, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(double[] values, Intervald interval, double[] result)
        {
            Evaluate(values, interval, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(double[] values, Intervald interval, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Evaluate(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(double[] values, Intervald from, Intervald to, double[] result)
        {
            Remap(values, from, to, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(double[] values, Intervald from, Intervald to, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Intervald.Remap(values[i], from, to);
        }

		#endregion

		#region float

		/// <summary>
		/// 
		/// </summary>
		public static float Sum(float[] values)
		{
			return Sum(values, values.Length);
		} 


		/// <summary>
		/// 
		/// </summary>
		public static float Sum(float[] values, int count)
		{
			var sum = default(float);

			for (int i = 0; i < count; i++)
				sum += values[i];

			return sum;
		}

		
		/// <summary>
        /// 
        /// </summary>
        public static float Mean(float[] values)
        {
            return Sum(values) / values.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Mean(float[] values, int count)
        {
            return Sum(values, count) / count;
        }

		
        /// <summary>
        /// 
        /// </summary>
        public static float WeightedSum(float[] values, float[] weights)
        {
            return WeightedSum(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float WeightedSum(float[] values, float[] weights, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
                result += values[i] * weights[i];

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static float WeightedMean(float[] values, float[] weights)
        {
            return WeightedMean(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float WeightedMean(float[] values, float[] weights, int count)
        {
            var sum = default(float);
            var wsum = default(float);

            for (int i = 0; i < count; i++)
            {
                var w = weights[i];
                sum += values[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(float[] v0, float v1, float[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(float[] v0, float v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(float[] v0, float[] v1, float[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(float[] v0, float[] v1, float[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Scale(float[] values, float t, float[] result)
        {
            Scale(values, t, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(float[] values, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = values[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(float[] v0, float[] v1, float t, float[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(float[] v0, float[] v1, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(float[] v0, float[] v1, float[] t, float[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(float[] v0, float[] v1, float[] t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(float[] v0, float t0, float[] v1, float t1, float[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(float[] v0, float t0, float[] v1, float t1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(float[] v0, float[] t0, float[] v1, float[] t1, float[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(float[] v0, float[] t0, float[] v1, float[] t1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float v2, float t, float[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float v2, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float[] v2, float t, float[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float[] v2, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float[] v2, float[] t, float[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(float[] v0, float[] v1, float[] v2, float[] t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(float[] v0, float[] v1, float[] result)
        {
            Multiply(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(float[] v0, float[] v1, float[] result)
        {
            Divide(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(float[] vector, Intervalf interval, float[] result)
        {
            Normalize(vector, interval, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(float[] vector, Intervalf interval, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(float[] values, Intervalf interval, float[] result)
        {
            Evaluate(values, interval, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(float[] values, Intervalf interval, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Evaluate(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(float[] values, Intervalf from, Intervalf to, float[] result)
        {
            Remap(values, from, to, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(float[] values, Intervalf from, Intervalf to, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Intervalf.Remap(values[i], from, to);
        }

		#endregion

		#region Vec2d

		/// <summary>
		/// 
		/// </summary>
		public static Vec2d Sum(Vec2d[] values)
		{
			return Sum(values, values.Length);
		} 


		/// <summary>
		/// 
		/// </summary>
		public static Vec2d Sum(Vec2d[] values, int count)
		{
			var sum = default(Vec2d);

			for (int i = 0; i < count; i++)
				sum += values[i];

			return sum;
		}

		
		/// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(Vec2d[] values)
        {
            return Sum(values) / values.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(Vec2d[] values, int count)
        {
            return Sum(values, count) / count;
        }

		
        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(Vec2d[] values, double[] weights)
        {
            return WeightedSum(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(Vec2d[] values, double[] weights, int count)
        {
            var result = default(Vec2d);

            for (int i = 0; i < count; i++)
                result += values[i] * weights[i];

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(Vec2d[] values, double[] weights)
        {
            return WeightedMean(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(Vec2d[] values, double[] weights, int count)
        {
            var sum = default(Vec2d);
            var wsum = default(double);

            for (int i = 0; i < count; i++)
            {
                var w = weights[i];
                sum += values[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(Vec2d[] v0, Vec2d v1, Vec2d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec2d[] v0, Vec2d v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] values, double t, Vec2d[] result)
        {
            Scale(values, t, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] values, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = values[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec2d[] v0, double t0, Vec2d[] v1, double t1, Vec2d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec2d[] v0, double t0, Vec2d[] v1, double t1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, Vec2d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Multiply(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Divide(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec2d[] vector, Interval2d interval, Vec2d[] result)
        {
            Normalize(vector, interval, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec2d[] vector, Interval2d interval, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec2d[] values, Interval2d interval, Vec2d[] result)
        {
            Evaluate(values, interval, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec2d[] values, Interval2d interval, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Evaluate(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec2d[] values, Interval2d from, Interval2d to, Vec2d[] result)
        {
            Remap(values, from, to, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec2d[] values, Interval2d from, Interval2d to, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Interval2d.Remap(values[i], from, to);
        }

		#endregion

		#region Vec3d

		/// <summary>
		/// 
		/// </summary>
		public static Vec3d Sum(Vec3d[] values)
		{
			return Sum(values, values.Length);
		} 


		/// <summary>
		/// 
		/// </summary>
		public static Vec3d Sum(Vec3d[] values, int count)
		{
			var sum = default(Vec3d);

			for (int i = 0; i < count; i++)
				sum += values[i];

			return sum;
		}

		
		/// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(Vec3d[] values)
        {
            return Sum(values) / values.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(Vec3d[] values, int count)
        {
            return Sum(values, count) / count;
        }

		
        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(Vec3d[] values, double[] weights)
        {
            return WeightedSum(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(Vec3d[] values, double[] weights, int count)
        {
            var result = default(Vec3d);

            for (int i = 0; i < count; i++)
                result += values[i] * weights[i];

            return result;
        }


		/// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(Vec3d[] values, double[] weights)
        {
            return WeightedMean(values, weights, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(Vec3d[] values, double[] weights, int count)
        {
            var sum = default(Vec3d);
            var wsum = default(double);

            for (int i = 0; i < count; i++)
            {
                var w = weights[i];
                sum += values[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Add(Vec3d[] v0, Vec3d v1, Vec3d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec3d[] v0, Vec3d v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] values, double t, Vec3d[] result)
        {
            Scale(values, t, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] values, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = values[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec3d[] v0, double t0, Vec3d[] v1, double t1, Vec3d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec3d[] v0, double t0, Vec3d[] v1, double t1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, Vec3d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Multiply(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Divide(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec3d[] vector, Interval3d interval, Vec3d[] result)
        {
            Normalize(vector, interval, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec3d[] vector, Interval3d interval, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec3d[] values, Interval3d interval, Vec3d[] result)
        {
            Evaluate(values, interval, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec3d[] values, Interval3d interval, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = interval.Evaluate(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec3d[] values, Interval3d from, Interval3d to, Vec3d[] result)
        {
            Remap(values, from, to, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec3d[] values, Interval3d from, Interval3d to, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Interval3d.Remap(values[i], from, to);
        }

		#endregion

		#endregion

		#region Scalar

		#region double

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, double tolerance = SlurMath.ZeroTolerance)
        {
            return ApproxEquals(v0, v1, v0.Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, int count, double tolerance = SlurMath.ZeroTolerance)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= tolerance) return false;

            return true;
        }

       
        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, double[] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, double[] tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= tolerance[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(double[] values)
        {
            return Max(values, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(double[] values, int count)
        {
            var result = values[0];

            for (int i = 1; i < count; i++)
            {
                var t = values[i];
                if (t > result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(double[] v0, double[] v1, double[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(double[] values)
        {
            return Min(values, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(double[] values, int count)
        {
            var result = values[0];

            for (int i = 1; i < count; i++)
            {
                var t = values[i];
                if (t < result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(double[] v0, double[] v1, double[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(double[] values, double[] result)
        {
            Abs(values, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(double[] values, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Abs(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(double[] v0, double[] v1)
        {
            return Dot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(double[] v0, double[] v1, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AbsDot(double[] v0, double[] v1)
        {
            return AbsDot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AbsDot(double[] v0, double[] v1, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(double[] v0, double[] v1, double[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(double[] v0, double[] v1, double[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(double[] v0, double[] v1, int count, double[] result)
        {
            Project(v0, v1, count, result);
            Subtract(v0, result, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(double[] v0, double[] v1, double[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count) * 2.0, count, result);
            AddScaled(result, v0, -1.0, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(double[] v0, double[] v1, double[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v0, Dot(v1, v1, count) / Dot(v0, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(double[] v0, double[] v1, double[] v2, double[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(double[] v0, double[] v1, double[] v2, int count, double[] result)
        {
            Scale(v0, Dot(v1, v2, count) / Dot(v0, v2, count), count, result);
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static double NormL1(double[] vector)
        {
            return NormL1(vector, vector.Length);
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static double NormL1(double[] vector, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareDistanceL2(double[] v0, double[] v1)
        {
            return SquareDistanceL2(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareDistanceL2(double[] v0, double[] v1, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
            {
                var d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL1(double[] v0, double[] v1)
        {
            return DistanceL1(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double DistanceL1(double[] v0, double[] v1, int count)
        {
            var result = default(double);

            for (int i = 0; i < count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double v1, double t, double[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double t, double[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double[] t, double[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
        }

		#endregion

		#region float

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(float[] v0, float[] v1, float tolerance = SlurMath.ZeroTolerancef)
        {
            return ApproxEquals(v0, v1, v0.Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(float[] v0, float[] v1, int count, float tolerance = SlurMath.ZeroTolerancef)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= tolerance) return false;

            return true;
        }

       
        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(float[] v0, float[] v1, float[] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(float[] v0, float[] v1, float[] tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= tolerance[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Max(float[] values)
        {
            return Max(values, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Max(float[] values, int count)
        {
            var result = values[0];

            for (int i = 1; i < count; i++)
            {
                var t = values[i];
                if (t > result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(float[] v0, float[] v1, float[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Min(float[] values)
        {
            return Min(values, values.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Min(float[] values, int count)
        {
            var result = values[0];

            for (int i = 1; i < count; i++)
            {
                var t = values[i];
                if (t < result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(float[] v0, float[] v1, float[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(float[] v0, float[] v1, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(float[] values, float[] result)
        {
            Abs(values, values.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(float[] values, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Abs(values[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Dot(float[] v0, float[] v1)
        {
            return Dot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float Dot(float[] v0, float[] v1, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float AbsDot(float[] v0, float[] v1)
        {
            return AbsDot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float AbsDot(float[] v0, float[] v1, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(float[] v0, float[] v1, float[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(float[] v0, float[] v1, int count, float[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(float[] v0, float[] v1, float[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(float[] v0, float[] v1, int count, float[] result)
        {
            Project(v0, v1, count, result);
            Subtract(v0, result, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(float[] v0, float[] v1, float[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(float[] v0, float[] v1, int count, float[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count) * 2.0f, count, result);
            AddScaled(result, v0, -1.0f, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(float[] v0, float[] v1, float[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(float[] v0, float[] v1, int count, float[] result)
        {
            Scale(v0, Dot(v1, v1, count) / Dot(v0, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(float[] v0, float[] v1, float[] v2, float[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(float[] v0, float[] v1, float[] v2, int count, float[] result)
        {
            Scale(v0, Dot(v1, v2, count) / Dot(v0, v2, count), count, result);
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static float NormL1(float[] vector)
        {
            return NormL1(vector, vector.Length);
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static float NormL1(float[] vector, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float SquareDistanceL2(float[] v0, float[] v1)
        {
            return SquareDistanceL2(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float SquareDistanceL2(float[] v0, float[] v1, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
            {
                var d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static float DistanceL1(float[] v0, float[] v1)
        {
            return DistanceL1(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static float DistanceL1(float[] v0, float[] v1, int count)
        {
            var result = default(float);

            for (int i = 0; i < count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float v1, float t, float[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float v1, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float[] v1, float t, float[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float[] v1, float t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float[] v1, float[] t, float[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(float[] v0, float[] v1, float[] t, int count, float[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
        }

		#endregion

		#endregion

		#region Vector

		#region Vec2d

		/// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, double tolerance = SlurMath.ZeroTolerance)
        {
            return ApproxEquals(v0, v1, v0.Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, int count, double tolerance = SlurMath.ZeroTolerance)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(Vec2d[] vectors, Vec2d[] result)
        {
            Abs(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(Vec2d[] vectors, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Abs(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Dot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(Vec2d[] v0, Vec2d[] v1, int count,  double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Dot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AbsDot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.AbsDot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Angle(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Angle(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Project(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Reject(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Reflect(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.MatchProjection(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, Vec2d[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.MatchProjection(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(Vec2d[] vectors, Vec2d[] result)
        {
            Unitize(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(Vec2d[] vectors, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Unit;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2(Vec2d[] vectors, double[] result)
        {
            NormL2(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2(Vec2d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1(Vec2d[] vectors, double[] result)
        {
            NormL1(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1(Vec2d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].ManhattanLength;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DistanceL2(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].DistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            SquareDistanceL2(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].SquareDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DistanceL1(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].ManhattanDistanceTo(v1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < v0.Length; i++)
                result[i] = Vec2d.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1[i], t[i]);
        }

		#endregion

		#region Vec3d

		/// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, double tolerance = SlurMath.ZeroTolerance)
        {
            return ApproxEquals(v0, v1, v0.Length, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, int count, double tolerance = SlurMath.ZeroTolerance)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Max(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(Vec3d[] vectors, Vec3d[] result)
        {
            Abs(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(Vec3d[] vectors, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Abs(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            Dot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(Vec3d[] v0, Vec3d[] v1, int count,  double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Dot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AbsDot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.AbsDot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            Angle(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Angle(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Project(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Reject(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Reflect(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.MatchProjection(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, Vec3d[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.MatchProjection(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(Vec3d[] vectors, Vec3d[] result)
        {
            Unitize(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(Vec3d[] vectors, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Unit;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2(Vec3d[] vectors, double[] result)
        {
            NormL2(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2(Vec3d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1(Vec3d[] vectors, double[] result)
        {
            NormL1(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1(Vec3d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].ManhattanLength;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DistanceL2(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].DistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            SquareDistanceL2(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].SquareDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DistanceL1(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].ManhattanDistanceTo(v1[i]);
        }


		/// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < v0.Length; i++)
                result[i] = Vec3d.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            Slerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Slerp(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1[i], t[i]);
        }

		#endregion

		#endregion
	}
}