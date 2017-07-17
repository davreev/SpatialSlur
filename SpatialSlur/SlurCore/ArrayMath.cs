using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
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
    public static class ArrayMath
    {
        #region double[]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, double tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[] v0, double[] v1, double tolerance, int count)
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
        public static double Sum(double[] vector)
        {
            return Sum(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(double[] vector, int count)
        {
            double sum = 0.0;

            for (int i = 0; i < count; i++)
                sum += vector[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(double[] vector)
        {
            return Sum(vector) / vector.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(double[] vector, int count)
        {
            return Sum(vector, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(double[] vector, double[] weights)
        {
            return WeightedSum(vector, weights, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(double[] vector, double[] weights, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += vector[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(double[] vector, double[] weights)
        {
            return WeightedMean(vector, weights, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(double[] vector, double[] weights, int count)
        {
            double sum = 0.0;
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vector[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(double[] vector)
        {
            return Max(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(double[] vector, int count)
        {
            double result = vector[0];

            for (int i = 1; i < count; i++)
            {
                double t = vector[i];
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
        public static void MaxParallel(double[] v0, double[] v1, double[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Max(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(double[] vector)
        {
            return Min(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(double[] vector, int count)
        {
            double result = vector[0];

            for (int i = 1; i < count; i++)
            {
                double t = vector[i];
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
        public static void MinParallel(double[] v0, double[] v1, double[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Min(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(double[] vector, double[] result)
        {
            Abs(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(double[] vector, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Abs(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(double[] vector, double[] result)
        {
            AbsParallel(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(double[] vector, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Abs(vector[i]);
            });
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
            double result = 0.0;

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
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
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
            double d = NormL2(v0, count) * NormL2(v1, count);

            if (d > 0.0)
                return Math.Acos(SlurMath.Clamp(Dot(v0, v1, count) / d, -1.0, 1.0)); // clamp dot product to remove noise

            return double.NaN;
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
        public static void ProjectParallel(double[] v0, double[] v1, double[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v1, Dot(v0, v1, count) / Dot(v1, v1, count), count, result);
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
        public static void RejectParallel(double[] v0, double[] v1, double[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(double[] v0, double[] v1, int count, double[] result)
        {
            ProjectParallel(v0, v1, count, result);
            SubtractParallel(v0, result, count, result);
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
        public static void ReflectParallel(double[] v0, double[] v1, double[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v1, Dot(v0, v1, count) / Dot(v1, v1, count) * 2.0, count, result);
            AddScaledParallel(result, v0, -1.0, count, result);
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
        /// 
        /// </summary>
        public static void MatchProjectionParallel(double[] v0, double[] v1, double[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v0, Dot(v1, v1, count) / Dot(v0, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(double[] v0, double[] v1, double[] v2, double[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(double[] v0, double[] v1, double[] v2, int count, double[] result)
        {
            ScaleParallel(v0, Dot(v1, v2, count) / Dot(v0, v2, count), count, result);
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
            double d = Dot(vector, vector, count);

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
        public static bool UnitizeParallel(double[] vector, double[] result)
        {
            return UnitizeParallel(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool UnitizeParallel(double[] vector, int count, double[] result)
        {
            double d = Dot(vector, vector, count);

            if (d > 0.0)
            {
                ScaleParallel(vector, 1.0 / Math.Sqrt(d), count, result);
                return true;
            }

            return false;
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
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


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
        public static double SquareDistanceL2(double[] v0, double[] v1)
        {
            return SquareDistanceL2(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareDistanceL2(double[] v0, double[] v1, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
            {
                double d = v1[i] - v0[i];
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
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
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
        public static void AddParallel(double[] v0, double v1, double[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(double[] v0, double v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(double[] v0, double[] v1, double[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
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
        public static void SubtractParallel(double[] v0, double[] v1, double[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(double[] vector, double t, double[] result)
        {
            Scale(vector, t, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(double[] vector, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vector[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(double[] vector, double t, double[] result)
        {
            ScaleParallel(vector, t, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(double[] vector, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vector[i] * t;
            });
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
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] v1, double t, double[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] v1, double[] t, double[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(double[] v0, double t0, double[] v1, double t1, double[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(double[] v0, double t0, double[] v1, double t1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] t0, double[] v1, double[] t1, double[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] t0, double[] v1, double[] t1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
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
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double v2, double t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double v2, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double[] v2, double t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double[] v2, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double[] v2, double[] t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(double[] v0, double[] v1, double[] v2, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2[i]) * t[i];
            });
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
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(double[] v0, double[] v1, double[] result)
        {
            MultiplyParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
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
        /// Component-wise division
        /// </summary>
        public static void DivideParallel(double[] v0, double[] v1, double[] result)
        {
            DivideParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void DivideParallel(double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] / v1[i];
            });
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


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double v1, double t, double[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double t, double[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double[] t, double[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
            });
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
            double st = 1.0 / Math.Sin(angle);
            AddScaled(v0, Math.Sin((1.0 - t) * angle) * st, v1, Math.Sin(t * angle) * st, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(double[] v0, double[] v1, double t, double[] result)
        {
            SlerpParallel(v0, v1, t, Angle(v0, v1), v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(double[] v0, double[] v1, double t, double angle, double[] result)
        {
            SlerpParallel(v0, v1, t, angle, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(double[] v0, double[] v1, double t, double angle, int count, double[] result)
        {
            double st = 1.0 / Math.Sin(angle);
            AddScaledParallel(v0, Math.Sin((1.0 - t) * angle) * st, v1, Math.Sin(t * angle) * st, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(double[] vector, Domain domain, double[] result)
        {
            Normalize(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(double[] vector, Domain domain, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(double[] vector, Domain domain, double[] result)
        {
            NormalizeParallel(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(double[] vector, Domain domain, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vector[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(double[] vector, Domain domain, double[] result)
        {
            Evaluate(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(double[] vector, Domain domain, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(double[] vector, Domain domain, double[] result)
        {
            EvaluateParallel(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(double[] vector, Domain domain, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vector[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(double[] vector, Domain from, Domain to, double[] result)
        {
            Remap(vector, from, to, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(double[] vector, Domain from, Domain to, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain.Remap(vector[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(double[] vector, Domain from, Domain to, double[] result)
        {
            RemapParallel(vector, from, to, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(double[] vector, Domain from, Domain to, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, vector.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain.Remap(vector[i], from, to);
            });
        }

        #endregion


        #region Vec2d[]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, double tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, double tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, Vec2d tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, Vec2d tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, Vec2d[] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec2d[] v0, Vec2d[] v1, Vec2d[] tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance[i])) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(Vec2d[] vectors)
        {
            return Sum(vectors, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(Vec2d[] vectors, int count)
        {
            Vec2d sum = new Vec2d();

            for (int i = 0; i < count; i++)
                sum += vectors[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(Vec2d[] vectors)
        {
            return Sum(vectors) / vectors.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(Vec2d[] vectors, int count)
        {
            return Sum(vectors, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(Vec2d[] vectors, double[] weights)
        {
            return WeightedSum(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(Vec2d[] vectors, double[] weights, int count)
        {
            var result = new Vec2d();

            for (int i = 0; i < count; i++)
                result += vectors[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(Vec2d[] vectors, double[] weights)
        {
            return WeightedMean(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(Vec2d[] vectors, double[] weights, int count)
        {
            var sum = new Vec2d();
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vectors[i] * w;
                wsum += w;
            }

            return sum / wsum;
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
        public static void MaxParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Max(v0[i], v1[i]);
            });
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
        public static void MinParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Min(v0[i], v1[i]);
            });
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
        public static void AbsParallel(Vec2d[] vectors, Vec2d[] result)
        {
            AbsParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(Vec2d[] vectors, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Abs(vectors[i]);
            });
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
        public static void Dot(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
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
        public static void AbsDotParallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AbsDotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.AbsDot(v0[i], v1[i]);
            });
        }


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
                result[i] = v0[i] ^ v1[i];
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            CrossParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] ^ v1[i];
            });
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
        public static void AngleParallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AngleParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Angle(v0[i], v1[i]);
            });
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
        public static void ProjectParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Project(v0[i], v1[i]);
            });
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
        public static void RejectParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Reject(v0[i], v1[i]);
            });
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
        public static void ReflectParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Reflect(v0[i], v1[i]);
            });
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
        public static void MatchProjectionParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.MatchProjection(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, Vec2d[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.MatchProjection(v0[i], v1[i], v2[i]);
            });
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
            {
                var v = vectors[i];
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(Vec2d[] vectors, Vec2d[] result)
        {
            UnitizeParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(Vec2d[] vectors, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vectors[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
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
        public static void NormL2Parallel(Vec2d[] vectors, double[] result)
        {
            NormL2Parallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2Parallel(Vec2d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].Length;
            });
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
        public static void NormL1Parallel(Vec2d[] vectors, double[] result)
        {
            NormL1Parallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1Parallel(Vec2d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].ManhattanLength;
            });
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
        public static void DistanceL2Parallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DistanceL2Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2Parallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].DistanceTo(v1[i]);
            });
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
        public static void SquareDistanceL2Parallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            SquareDistanceL2Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2Parallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].SquareDistanceTo(v1[i]);
            });
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
        public static void DistanceL1Parallel(Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DistanceL1Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1Parallel(Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].ManhattanDistanceTo(v1[i]);
            });
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
        public static void AddParallel(Vec2d[] v0, Vec2d v1, Vec2d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec2d[] v0, Vec2d v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
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
        public static void SubtractParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] vectors, double t, Vec2d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] vectors, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] vectors, double[] t, Vec2d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec2d[] vectors, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec2d[] vectors, double t, Vec2d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec2d[] vectors, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec2d[] vectors, double[] t, Vec2d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec2d[] vectors, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t[i];
            });
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
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, double t0, Vec2d[] v1, double t1, Vec2d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, double t0, Vec2d[] v1, double t1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, Vec2d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
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
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
            });
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
        public static void LerpParallel(Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1[i], t[i]);
            });
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


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            Normalize(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            NormalizeParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            Evaluate(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            EvaluateParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec2d[] vectors, Domain2d from, Domain2d to, Vec2d[] result)
        {
            Remap(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec2d[] vectors, Domain2d from, Domain2d to, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain2d.Remap(vectors[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(Vec2d[] vectors, Domain2d from, Domain2d to, Vec2d[] result)
        {
            RemapParallel(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(Vec2d[] vectors, Domain2d from, Domain2d to, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain2d.Remap(vectors[i], from, to);
            });
        }

        #endregion


        #region Vec3d[]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, double tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, double tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, Vec3d tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, Vec3d tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, Vec3d[] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(Vec3d[] v0, Vec3d[] v1, Vec3d[] tolerance, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], tolerance[i])) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(Vec3d[] vectors)
        {
            return Sum(vectors, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(Vec3d[] vectors, int count)
        {
            Vec3d sum = new Vec3d();

            for (int i = 0; i < count; i++)
                sum += vectors[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(Vec3d[] vectors)
        {
            return Sum(vectors) / vectors.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(Vec3d[] vectors, int count)
        {
            return Sum(vectors, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(Vec3d[] vectors, double[] weights)
        {
            return WeightedSum(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(Vec3d[] vectors, double[] weights, int count)
        {
            var result = new Vec3d();

            for (int i = 0; i < count; i++)
                result += vectors[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(Vec3d[] vectors, double[] weights)
        {
            return WeightedMean(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(Vec3d[] vectors, double[] weights, int count)
        {
            var sum = new Vec3d();
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vectors[i] * w;
                wsum += w;
            }

            return sum / wsum;
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
        public static void MaxParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Max(v0[i], v1[i]);
            });
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
        public static void MinParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Min(v0[i], v1[i]);
            });
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
        public static void AbsParallel(Vec3d[] vectors, Vec3d[] result)
        {
            AbsParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(Vec3d[] vectors, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Abs(vectors[i]);
            });
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
        public static void Dot(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
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
        public static void AbsDotParallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AbsDotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.AbsDot(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Cross(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] ^ v1[i];
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            CrossParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] ^ v1[i];
            });
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
        public static void AngleParallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AngleParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Angle(v0[i], v1[i]);
            });
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
        public static void ProjectParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Project(v0[i], v1[i]);
            });
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
        public static void RejectParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Reject(v0[i], v1[i]);
            });
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
        public static void ReflectParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Reflect(v0[i], v1[i]);
            });
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
        public static void MatchProjectionParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.MatchProjection(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, Vec3d[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.MatchProjection(v0[i], v1[i], v2[i]);
            });
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
            {
                var v = vectors[i];
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(Vec3d[] vectors, Vec3d[] result)
        {
            UnitizeParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(Vec3d[] vectors, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vectors[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
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
        public static void NormL2Parallel(Vec3d[] vectors, double[] result)
        {
            NormL2Parallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL2Parallel(Vec3d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].Length;
            });
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
        public static void NormL1Parallel(Vec3d[] vectors, double[] result)
        {
            NormL1Parallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormL1Parallel(Vec3d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].ManhattanLength;
            });
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
        public static void DistanceL2Parallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DistanceL2Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL2Parallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].DistanceTo(v1[i]);
            });
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
        public static void SquareDistanceL2Parallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            SquareDistanceL2Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SquareDistanceL2Parallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].SquareDistanceTo(v1[i]);
            });
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
        public static void DistanceL1Parallel(Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DistanceL1Parallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DistanceL1Parallel(Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].ManhattanDistanceTo(v1[i]);
            });
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
        public static void AddParallel(Vec3d[] v0, Vec3d v1, Vec3d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec3d[] v0, Vec3d v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
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
        public static void SubtractParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] vectors, double t, Vec3d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] vectors, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] vectors, double[] t, Vec3d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(Vec3d[] vectors, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec3d[] vectors, double t, Vec3d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec3d[] vectors, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec3d[] vectors, double[] t, Vec3d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(Vec3d[] vectors, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t[i];
            });
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
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, double t0, Vec3d[] v1, double t1, Vec3d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, double t0, Vec3d[] v1, double t1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, Vec3d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
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
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
            });
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
        public static void LerpParallel(Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            LerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1[i], t[i]);
            });
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


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            SlerpParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpParallel(Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            Normalize(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            NormalizeParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            Evaluate(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            EvaluateParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec3d[] vectors, Domain3d from, Domain3d to, Vec3d[] result)
        {
            Remap(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(Vec3d[] vectors, Domain3d from, Domain3d to, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain3d.Remap(vectors[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(Vec3d[] vectors, Domain3d from, Domain3d to, Vec3d[] result)
        {
            RemapParallel(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(Vec3d[] vectors, Domain3d from, Domain3d to, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain3d.Remap(vectors[i], from, to);
            });
        }

        #endregion


        #region double[][]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double tolerance, int count)
        {
            return ApproxEquals(v0, v1, tolerance, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double tolerance, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], tolerance, size)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[] tolerance, int count)
        {
            return ApproxEquals(v0, v1, tolerance, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[] tolerance, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], tolerance, size)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[][] tolerance)
        {
            return ApproxEquals(v0, v1, tolerance, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[][] tolerance, int count)
        {
            return ApproxEquals(v0, v1, tolerance, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(double[][] v0, double[][] v1, double[][] tolerance, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], tolerance[i], size)) return false;

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
