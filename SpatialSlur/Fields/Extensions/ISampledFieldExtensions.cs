
/*
 * Notes
 * 
 * TODO update using ArrayView
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class ISampledFieldExtensions
    {
        #region ISampledField<T>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static ISampledField<T> Duplicate<T>(this ISampledField<T> field)
            where T : struct
        {
            return field.Duplicate(true);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Clear<T>(this ISampledField<T> field)
            where T : struct
        {
            field.Values.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void Set<T>(this ISampledField<T> field, T value)
        {
            field.Values.Set(value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        public static void Set<T>(this ISampledField<T> field, ISampledField<T> other)
        {
            field.Values.Set(other.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="values"></param>
        public static void Set<T>(this ISampledField<T> field, IEnumerable<T> values)
        {
            field.Values.Set(values);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="action"></param>
        /// <param name="parallel"></param>
        public static void Action<T>(this ISampledField<T> field, Action<T> action, bool parallel = false)
        {
            field.Values.Action(action, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="field"></param>
        /// <param name="converter"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convert<T, U>(this ISampledField<T> field, Func<T, U> converter, ISampledField<U> result, bool parallel = false)
        {
            field.Values.Convert(converter, result.Values, parallel);
        }

        #endregion


        #region  ISampledField<double>

        /// <summary>
        /// 
        /// </summary>
        public static double Min(this ISampledField<double> field)
        {
            return Vector.Min(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(this ISampledField<double> field)
        {
            return Vector.Max(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this ISampledField<double> field)
        {
            return Vector.Sum(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this ISampledField<double> field)
        {
            return Vector.Mean(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this ISampledField<double> f0, ISampledField<double> f1, double epsilon = D.ZeroTolerance)
        {
            return Vector.ApproxEquals(f0.Values, f1.Values, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Max(f0.Values, f1.Values, result.Values);
            else
                Vector.Max(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Min(f0.Values, f1.Values, result.Values);
            else
                Vector.Min(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this ISampledField<double> f0, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Abs(f0.Values, result.Values);
            else
                Vector.Abs(f0.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Add(f0.Values, f1.Values, result.Values);
            else
                Vector.Add(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Subtract(f0.Values, f1.Values, result.Values);
            else
                Vector.Subtract(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Multiply(this ISampledField<double> f0, double t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Multiply(f0.Values, t, result.Values);
            else
                Vector.Multiply(f0.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<double> f0, ISampledField<double> f1, double t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaled(f0.Values, f1.Values, t, result.Values);
            else
                Vector.AddScaled(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
            else
                Vector.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<double> f0, double t0, ISampledField<double> f1, double t1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
            else
                Vector.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<double> f0, ISampledField<double> t0, ISampledField<double> f1, ISampledField<double> t1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
            else
                Vector.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> f2, double t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
            else
                Vector.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> f2, ISampledField<double> t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
            else
                Vector.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.MultiplyPointwise(f0.Values, f1.Values, result.Values);
            else
                Vector.MultiplyPointwise(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.DividePointwise(f0.Values, f1.Values, result.Values);
            else
                Vector.DividePointwise(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<double> f0, ISampledField<double> f1, double t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Lerp(f0.Values, f1.Values, t,  result.Values);
            else
                Vector.Lerp(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<double> f0, ISampledField<double> f1, ISampledField<double> t, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Lerp(f0.Values, f1.Values, t.Values, result.Values);
            else
                Vector.Lerp(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this ISampledField<double> field, Intervald interval, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Normalize(field.Values, interval, result.Values);
            else
                Vector.Normalize(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this ISampledField<double> field, Intervald interval, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Evaluate(field.Values, interval, result.Values);
            else
                Vector.Evaluate(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this ISampledField<double> field, Intervald from, Intervald to, ISampledField<double> result, bool parallel = false)
        {
            if (parallel)
                Vector.Parallel.Remap(field.Values, from, to, result.Values);
            else
                Vector.Remap(field.Values, from, to, result.Values);
        }

        #endregion


        #region  ISampledField<Vector2d>

        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Min(this ISampledField<Vector2d> field)
        {
            return Matrix.ColumnMin(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Max(this ISampledField<Vector2d> field)
        {
            return Matrix.ColumnMax(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Sum(this ISampledField<Vector2d> field)
        {
            return Matrix.ColumnSum(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector2d Mean(this ISampledField<Vector2d> field)
        {
            return Matrix.ColumnMean(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, double epsilon = D.ZeroTolerance)
        {
            return Matrix.ApproxEquals(f0.Values, f1.Values, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Max(f0.Values, f1.Values, result.Values);
            else
                Matrix.Max(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Min(f0.Values, f1.Values, result.Values);
            else
                Matrix.Min(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this ISampledField<Vector2d> f0, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Abs(f0.Values, result.Values);
            else
                Matrix.Abs(f0.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Add(f0.Values, f1.Values, result.Values);
            else
                Matrix.Add(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Subtract(f0.Values, f1.Values, result.Values);
            else
                Matrix.Subtract(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Multiply(this ISampledField<Vector2d> f0, double t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Multiply(f0.Values, t, result.Values);
            else
                Matrix.Multiply(f0.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, double t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, f1.Values, t, result.Values);
            else
                Matrix.AddScaled(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<double> t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
            else
                Matrix.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<Vector2d> f0, double t0, ISampledField<Vector2d> f1, double t1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
            else
                Matrix.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<Vector2d> f0, ISampledField<double> t0, ISampledField<Vector2d> f1, ISampledField<double> t1, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
            else
                Matrix.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> f2, double t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
            else
                Matrix.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<Vector2d> f2, ISampledField<double> t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
            else
                Matrix.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, double t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Lerp(f0.Values, f1.Values, t, result.Values);
            else
                Matrix.Lerp(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<Vector2d> f0, ISampledField<Vector2d> f1, ISampledField<double> t, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.LerpColumns(f0.Values, f1.Values, t.Values, result.Values);
            else
                Matrix.LerpColumns(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this ISampledField<Vector2d> field, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.UnitizeColumns(field.Values, result.Values);
            else
                Matrix.UnitizeColumns(field.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this ISampledField<Vector2d> field, Interval2d interval, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.NormalizeColumns(field.Values, interval, result.Values);
            else
                Matrix.NormalizeColumns(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this ISampledField<Vector2d> field, Interval2d interval, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.EvaluateColumns(field.Values, interval, result.Values);
            else
                Matrix.EvaluateColumns(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this ISampledField<Vector2d> field, Interval2d from, Interval2d to, ISampledField<Vector2d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.RemapColumns(field.Values, from, to, result.Values);
            else
                Matrix.RemapColumns(field.Values, from, to, result.Values);
        }

        #endregion


        #region  ISampledField<Vector3d>

        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Min(this ISampledField<Vector3d> field)
        {
            return Matrix.ColumnMin(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Max(this ISampledField<Vector3d> field)
        {
            return Matrix.ColumnMax(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Sum(this ISampledField<Vector3d> field)
        {
            return Matrix.ColumnSum(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vector3d Mean(this ISampledField<Vector3d> field)
        {
            return Matrix.ColumnMean(field.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, double epsilon = D.ZeroTolerance)
        {
            return Matrix.ApproxEquals(f0.Values, f1.Values, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Max(f0.Values, f1.Values, result.Values);
            else
                Matrix.Max(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Min(f0.Values, f1.Values, result.Values);
            else
                Matrix.Min(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this ISampledField<Vector3d> f0, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Abs(f0.Values, result.Values);
            else
                Matrix.Abs(f0.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Add(f0.Values, f1.Values, result.Values);
            else
                Matrix.Add(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Subtract(f0.Values, f1.Values, result.Values);
            else
                Matrix.Subtract(f0.Values, f1.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Multiply(this ISampledField<Vector3d> f0, double t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Multiply(f0.Values, t, result.Values);
            else
                Matrix.Multiply(f0.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, double t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, f1.Values, t, result.Values);
            else
                Matrix.AddScaled(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<double> t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
            else
                Matrix.AddScaled(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<Vector3d> f0, double t0, ISampledField<Vector3d> f1, double t1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
            else
                Matrix.AddScaled(f0.Values, t0, f1.Values, t1, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this ISampledField<Vector3d> f0, ISampledField<double> t0, ISampledField<Vector3d> f1, ISampledField<double> t1, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
            else
                Matrix.AddScaled(f0.Values, t0.Values, f1.Values, t1.Values, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> f2, double t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
            else
                Matrix.AddScaledDelta(f0.Values, f1.Values, f2.Values, t, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<Vector3d> f2, ISampledField<double> t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
            else
                Matrix.AddScaledDelta(f0.Values, f1.Values, f2.Values, t.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, double t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.Lerp(f0.Values, f1.Values, t, result.Values);
            else
                Matrix.Lerp(f0.Values, f1.Values, t, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this ISampledField<Vector3d> f0, ISampledField<Vector3d> f1, ISampledField<double> t, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.LerpColumns(f0.Values, f1.Values, t.Values, result.Values);
            else
                Matrix.LerpColumns(f0.Values, f1.Values, t.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this ISampledField<Vector3d> field, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.UnitizeColumns(field.Values, result.Values);
            else
                Matrix.UnitizeColumns(field.Values, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this ISampledField<Vector3d> field, Interval3d interval, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.NormalizeColumns(field.Values, interval, result.Values);
            else
                Matrix.NormalizeColumns(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this ISampledField<Vector3d> field, Interval3d interval, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.EvaluateColumns(field.Values, interval, result.Values);
            else
                Matrix.EvaluateColumns(field.Values, interval, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this ISampledField<Vector3d> field, Interval3d from, Interval3d to, ISampledField<Vector3d> result, bool parallel = false)
        {
            if (parallel)
                Matrix.Parallel.RemapColumns(field.Values, from, to, result.Values);
            else
                Matrix.RemapColumns(field.Values, from, to, result.Values);
        }

        #endregion
    }
}
