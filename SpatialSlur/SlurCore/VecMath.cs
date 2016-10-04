using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Static methods for generic/common operations on arrays of data.
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    internal static class VecMath
    {
        //

        /// <summary>
        /// Sets the result to some function of a given vector.
        /// </summary>
        public static void Function<T, U>(Func<T, U> func, T[] vector, int length, U[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = func(vector[i]);
        }


        /// <summary>
        /// Sets the result to some function of a given vector.
        /// </summary>
        public static void FunctionParallel<T, U>(Func<T, U> func, T[] vector, int length, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(vector[i]);
            });
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void Function<T0, T1, U>(Func<T0, T1, U> func, T0[] v0, T1[] v1, int length, U[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = func(v0[i], v1[i]);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, U>(Func<T0, T1, U> func, T0[] v0, T1[] v1, int length, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// Sets the result to some function of the 3 given vectors.
        /// </summary>
        public static void Function<T0, T1, T2, U>(Func<T0, T1, T2, U> func, T0[] v0, T1[] v1, T2[] v2, int length, U[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = func(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// Sets the result to some function of 3 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, T2, U>(Func<T0, T1, T2, U> func, T0[] v0, T1[] v1, T2[] v2, int length, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(v0[i], v1[i], v2[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(double[] v0, double[] v1, int length)
        {
            double result = 0.0;

            for (int i = 0; i < length; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Equals(double[] v0, double[] v1, double epsilon, int length)
        {
            for (int i = 0; i < length; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Equals(double[] v0, double[] v1, double[] epsilon, int length)
        {
            for (int i = 0; i < length; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Length(double[] vector, int length)
        {
            return Math.Sqrt(Dot(vector, vector, length));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareLength(double[] vector, int length)
        {
            return Dot(vector, vector, length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double ManhattanLength(double[] vector, int length)
        {
            double result = 0.0;

            for (int i = 0; i < length; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Distance(double[] v0, double[] v1, int length)
        {
            return Math.Sqrt(SquareDistance(v0, v1, length));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double SquareDistance(double[] v0, double[] v1, int length)
        {
            double result = 0.0;

            for (int i = 0; i < length; i++)
            {
                double d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double ManhattanDistance(double[] v0, double[] v1, int length)
        {
            double result = 0.0;

            for (int i = 0; i < length; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(double[] v0, double[] v1, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(double[] v0, double[] v1, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(double[] v0, double[] v1, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(double[] v0, double[] v1, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(double[] vector, double factor, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = vector[i] * factor;
        }


        /// <summary>
        /// result = v0 + v1 * factor
        /// </summary>
        public static void AddScaled(double[] v0, double[] v1, double factor, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = v0[i] + v1[i] * factor;
        }


        /// <summary>
        /// result = v0 + v1 * factor
        /// </summary>
        public static void AddScaledParallel(double[] v0, double[] v1, double factor, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * factor;
            });
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(double[] v0, double[] v1, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(double[] vector, double factor, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vector[i] * factor;
            });
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(double[] v0, double[] v1, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(double[] v0, double[] v1, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void DivideParallel(double[] v0, double[] v1, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] / v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(double[] vector, int length, double[] result)
        {
            double d = SquareLength(vector, length);

            if (d > 0.0)
            {
                Scale(vector, 1.0 / Math.Sqrt(d), length, result);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool UnitizeParallel(double[] vector, int length, double[] result)
        {
            double d = SquareLength(vector, length);

            if (d > 0.0)
            {
                MultiplyParallel(vector, 1.0 / Math.Sqrt(d), length, result);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double factor, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], factor);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[] v0, double[] v1, double[] factor, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], factor[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[][] vectors, double factor, int length, double[] result)
        {
            int last = vectors.Length - 1;

            int i;
            factor = SlurMath.Fract(factor * last, out i);

            if (i < 0)
                vectors[0].CopyTo(result, 0);
            else if (i >= last)
                vectors[last].CopyTo(result, 0);
            else
                Lerp(vectors[i], vectors[i + 1], factor, length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(double[][] vectors, double[] factors, int length, double[] result)
        {
            int last = vectors.Length - 1;

            for (int j = 0; j < length; j++)
            {
                int i;
                double t = SlurMath.Fract(factors[j] * last, out i);

                if (i < 0)
                    result[j] = vectors[0][j];
                else if (i >= last)
                    result[j] = vectors[last][j];
                else
                    result[j] = SlurMath.Lerp(vectors[i][j], vectors[i + 1][j], t);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double factor, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], factor);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[] v0, double[] v1, double[] factor, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], factor[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[][] vectors, double factor, int length, double[] result)
        {
            int last = vectors.Length - 1;

            int i;
            factor = SlurMath.Fract(factor * last, out i);

            if (i < 0)
                vectors[0].CopyTo(result, 0);
            else if (i >= last)
                vectors[last].CopyTo(result, 0);
            else
                LerpParallel(vectors[i], vectors[i + 1], factor, length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(double[][] vectors, double[] factors, int length, double[] result)
        {
            int last = vectors.Length - 1;

            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int j = range.Item1; j < range.Item2; j++)
                {
                    int i;
                    double t = SlurMath.Fract(factors[j] * last, out i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = SlurMath.Lerp(vectors[i][j], vectors[i + 1][j], t);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(double[] vector, Domain domain, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = domain.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(double[] vector, Domain domain, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
           {
               for (int i = range.Item1; i < range.Item2; i++)
                   result[i] = domain.Normalize(vector[i]);
           });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(double[] vector, Domain from, Domain to, int length, double[] result)
        {
            for (int i = 0; i < length; i++)
                result[i] = Domain.Remap(vector[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(double[] vector, Domain from, Domain to, int length, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain.Remap(vector[i], from, to);
            });
        }
    }
}
