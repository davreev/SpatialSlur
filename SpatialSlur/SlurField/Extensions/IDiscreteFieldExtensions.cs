using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDiscreteFieldExtensions
    {
        #region IDiscreteField<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static ReadOnlySubArray<T> AsReadOnly<T>(this IDiscreteField<T> field)
        {
            return new ReadOnlySubArray<T>(field.Values, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Clear<T>(this IDiscreteField<T> field)
            where T : struct
        {
            Array.Clear(field.Values, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void Set<T>(this IDiscreteField<T> field, T value)
        {
            field.Values.SetRange(value, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        public static void Set<T>(this IDiscreteField<T> field, IDiscreteField<T> other)
        {
            field.Values.SetRange(other.Values, 0, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="sequence"></param>
        public static void Set<T>(this IDiscreteField<T> field, IEnumerable<T> sequence)
        {
            field.Values.Set(sequence);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field.
        /// </summary>
        public static void Function<T, U>(this IDiscreteField<T> field, Func<T, U> func, IDiscreteField<U> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.FunctionParallel(field.Values, field.Count, func, result.Values);
            else
                ArrayMath.Function(field.Values, field.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field and another field
        /// </summary>
        public static void Function<T0, T1, U>(this IDiscreteField<T0> f0, IDiscreteField<T1> f1, Func<T0, T1, U> func, IDiscreteField<U> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.FunctionParallel(f0.Values, f1.Values, f0.Count, func, result.Values);
            else
                ArrayMath.Function(f0.Values, f1.Values, f0.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field and two others
        /// </summary>
        public static void Function<T0, T1, T2, U>(this IDiscreteField<T0> f0, IDiscreteField<T1> f1, IDiscreteField<T2> f2, Func<T0, T1, T2, U> func, IDiscreteField<U> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.FunctionParallel(f0.Values, f1.Values, f2.Values, f0.Count, func, result.Values);
            else
                ArrayMath.Function(f0.Values, f1.Values, f2.Values, f0.Count, func, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public static void Sample<T>(this IDiscreteField2d<T> field, IField2d<T> other, bool parallel = false)
        {
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
            {
                for(int i = range.Item1; i < range.Item2; i++)
                    vals[i] = other.ValueAt(field.CoordinateAt(i));
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        public static void Sample<T, U>(this IDiscreteField2d<T> field, IField2d<U> other, Func<U,T> converter, bool parallel = false)
        {
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    vals[i] = converter(other.ValueAt(field.CoordinateAt(i)));
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public static void Sample<T>(this IDiscreteField3d<T> field, IField3d<T> other, bool parallel = false)
        {
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    vals[i] = other.ValueAt(field.CoordinateAt(i));
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        public static void Sample<T, U>(this IDiscreteField3d<T> field, IField3d<U> other, Func<U, T> converter, bool parallel = false)
        {
            var vals = field.Values;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    vals[i] = converter(other.ValueAt(field.CoordinateAt(i)));
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), body);
            else
                body(Tuple.Create(0, field.Count));
        }

        #endregion


        #region  IDiscreteField<double>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IDiscreteField<double> f0, IDiscreteField<double> f1, double tolerance)
        {
            return ArrayMath.ApproxEquals(f0.Values, f1.Values, tolerance, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this IDiscreteField<double> field)
        {
            return ArrayMath.Sum(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this IDiscreteField<double> field)
        {
            return ArrayMath.Mean(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(this IDiscreteField<double> field)
        {
            return ArrayMath.Max(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MaxParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Max(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(this IDiscreteField<double> field)
        {
            return ArrayMath.Min(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MinParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Min(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IDiscreteField<double> f0, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AbsParallel(f0.Values, f0.Count, result.Values);
            else
                ArrayMath.Abs(f0.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Add(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.SubtractParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Subtract(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IDiscreteField<double> f0, double t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.ScaleParallel(f0.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Scale(f0.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<double> f0, IDiscreteField<double> f1, double t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<double> f0, double t0, IDiscreteField<double> f1, double t1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<double> f0, IDiscreteField<double> t0, IDiscreteField<double> f1, IDiscreteField<double> t1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> f2, double t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> f2, IDiscreteField<double> t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MultiplyParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Multiply(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.DivideParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Divide(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<double> f0, IDiscreteField<double> f1, double t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<double> f0, IDiscreteField<double> f1, IDiscreteField<double> t, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IDiscreteField<double> field, Interval1d interval, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.NormalizeParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Normalize(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IDiscreteField<double> field, Interval1d interval, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.EvaluateParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Evaluate(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IDiscreteField<double> field, Interval1d from, Interval1d to, IDiscreteField<double> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.RemapParallel(field.Values, from, to, result.Values);
            else
                ArrayMath.Remap(field.Values, from, to, result.Values);
        }

        #endregion


        #region  IDiscreteField<Vec2d>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, Vec2d tolerance)
        {
            return ArrayMath.ApproxEquals(f0.Values, f1.Values, tolerance, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(this IDiscreteField<Vec2d> field)
        {
            return ArrayMath.Sum(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(this IDiscreteField<Vec2d> field)
        {
            return ArrayMath.Mean(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MaxParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Max(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MinParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Min(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AbsParallel(f0.Values, f0.Count, result.Values);
            else
                ArrayMath.Abs(f0.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Add(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.SubtractParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Subtract(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IDiscreteField<Vec2d> f0, double t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.ScaleParallel(f0.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Scale(f0.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, double t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<double> t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec2d> f0, double t0, IDiscreteField<Vec2d> f1, double t1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec2d> f0, IDiscreteField<double> t0, IDiscreteField<Vec2d> f1, IDiscreteField<double> t1, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> f2, double t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<Vec2d> f2, IDiscreteField<double> t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, double t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<Vec2d> f0, IDiscreteField<Vec2d> f1, IDiscreteField<double> t, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IDiscreteField<Vec2d> field, Interval2d interval, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.NormalizeParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Normalize(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IDiscreteField<Vec2d> field, Interval2d interval, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.EvaluateParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Evaluate(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IDiscreteField<Vec2d> field, Interval2d from, Interval2d to, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.RemapParallel(field.Values, from, to, result.Values);
            else
                ArrayMath.Remap(field.Values, from, to, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this IDiscreteField<Vec2d> field, IDiscreteField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.UnitizeParallel(field.Values, result.Values);
            else
                ArrayMath.Unitize(field.Values, result.Values);
        }

        #endregion


        #region  IDiscreteField<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, Vec3d tolerance)
        {
            return ArrayMath.ApproxEquals(f0.Values, f1.Values, tolerance, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(this IDiscreteField<Vec3d> field)
        {
            return ArrayMath.Sum(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(this IDiscreteField<Vec3d> field)
        {
            return ArrayMath.Mean(field.Values, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MaxParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Max(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.MinParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Min(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AbsParallel(f0.Values, f0.Count, result.Values);
            else
                ArrayMath.Abs(f0.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Add(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.SubtractParallel(f0.Values, f1.Values, f0.Count, result.Values);
            else
                ArrayMath.Subtract(f0.Values, f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IDiscreteField<Vec3d> f0, double t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.ScaleParallel(f0.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Scale(f0.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, double t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<double> t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec3d> f0, double t0, IDiscreteField<Vec3d> f1, double t1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IDiscreteField<Vec3d> f0, IDiscreteField<double> t0, IDiscreteField<Vec3d> f1, IDiscreteField<double> t1, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledParallel(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> f2, double t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<Vec3d> f2, IDiscreteField<double> t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.AddScaledDeltaParallel(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, double t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IDiscreteField<Vec3d> f0, IDiscreteField<Vec3d> f1, IDiscreteField<double> t, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.LerpParallel(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
            else
                ArrayMath.Lerp(f0.Values, f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IDiscreteField<Vec3d> field, Interval3d interval, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.NormalizeParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Normalize(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IDiscreteField<Vec3d> field, Interval3d interval, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.EvaluateParallel(field.Values, interval, result.Values);
            else
                ArrayMath.Evaluate(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IDiscreteField<Vec3d> field, Interval3d from, Interval3d to, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.RemapParallel(field.Values, from, to, result.Values);
            else
                ArrayMath.Remap(field.Values, from, to, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this IDiscreteField<Vec3d> field, IDiscreteField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                ArrayMath.UnitizeParallel(field.Values, result.Values);
            else
                ArrayMath.Unitize(field.Values, result.Values);
        }

        #endregion
    }
}
