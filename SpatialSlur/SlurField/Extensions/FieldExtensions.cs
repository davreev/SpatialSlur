using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */
  
namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class FieldExtensions
    {
        #region IField<T>

        /// <summary>
        /// 
        /// </summary>
        public static void Clear<T>(this IField<T> field)
        {
            Array.Clear(field.Values, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public static void Set<T>(this IField<T> field, T value)
        {
            field.Values.SetRange(value, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="other"></param>
        public static void Set<T>(this IField<T> field, IField<T> other)
        {
            field.Values.SetRange(other.Values, 0, 0, field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="field"></param>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void Convert<T, U>(this IField<T> field, Func<T, U> func, IField<U> result, bool parallel = false)
        {
            if (parallel)
                field.Values.ConvertRangeParallel(0, field.Count, func, result.Values);
            else
                field.Values.ConvertRange(0, field.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field and its indices.
        /// </summary>
        public static void Function<T, U>(this IField<T> field, Func<T, int, U> func, IField<U> result, bool parallel = false)
        {
            if (parallel)
                field.Values.ConvertRangeParallel(0, field.Count, func, result.Values);
            else
                field.Values.ConvertRange(0, field.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field.
        /// </summary>
        public static void Function<T, U>(this IField<T> field, Func<T, U> func, IField<U> result, bool parallel = false)
        {
            if (parallel)
                field.Values.ConvertRangeParallel(0, field.Count, func, result.Values);
            else
                field.Values.ConvertRange(0, field.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field and another field
        /// </summary>
        public static void Function<T0, T1, U>(this IField<T0> f0, IField<T1> f1, Func<T0,T1,U> func, IField<U> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.FunctionParallel(f1.Values, f0.Count, func, result.Values);
            else
                f0.Values.Function(f1.Values, f0.Count, func, result.Values);
        }


        /// <summary>
        /// Sets the resulting field to some function of this field and two others
        /// </summary>
        public static void Function<T0, T1, T2, U>(this IField<T0> f0, IField<T1> f1, IField<T2> f2, Func<T0, T1, T2, U> func, IField<U> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.FunctionParallel(f1.Values, f2.Values, f0.Count, func, result.Values);
            else
                f0.Values.Function(f1.Values, f2.Values, f0.Count, func, result.Values);
        }

        #endregion


        #region  IField<double>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IField<double> f0, IField<double> f1, double epsilon)
        {
            return f0.Values.ApproxEquals(f1.Values, epsilon, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this IField<double> field)
        {
            return field.Values.Sum(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this IField<double> field)
        {
            return field.Values.Mean(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(this IField<double> field)
        {
            return field.Values.Max(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MaxParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Max(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(this IField<double> field)
        {
            return field.Values.Min(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MinParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Min(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IField<double> f0, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AbsParallel(f0.Count, result.Values);
            else
                f0.Values.Abs(f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Add(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.SubtractParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Subtract(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IField<double> f0, double t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.ScaleParallel(t, f0.Count, result.Values);
            else
                f0.Values.Scale(t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<double> f0, IField<double> f1, double t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<double> f0, IField<double> f1, IField<double> t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<double> f0, double t0, IField<double> f1, double t1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0, f1.Values, t1, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<double> f0, IField<double> t0, IField<double> f1, IField<double> t1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<double> f0, IField<double> f1, IField<double> f2, double t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<double> f0, IField<double> f1, IField<double> f2, IField<double> t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MultiplyParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Multiply(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(this IField<double> f0, IField<double> f1, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.DivideParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Divide(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<double> f0, IField<double> f1, double t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values,t, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<double> f0, IField<double> f1, IField<double> t, IField<double> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IField<double> field, Domain domain, IField<double> result, bool parallel = false)
        {
            if (parallel)
                field.Values.NormalizeParallel(domain, result.Values);
            else
                field.Values.Normalize(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IField<double> field, Domain domain, IField<double> result, bool parallel = false)
        {
            if (parallel)
                field.Values.EvaluateParallel(domain, result.Values);
            else
                field.Values.Evaluate(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IField<double> field, Domain from, Domain to, IField<double> result, bool parallel = false)
        {
            if (parallel)
                field.Values.RemapParallel(from, to, result.Values);
            else
                field.Values.Remap(from, to, result.Values);
        }

        #endregion


        #region  IField<Vec2d>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IField<Vec2d> f0, IField<Vec2d> f1, Vec2d epsilon)
        {
            return f0.Values.ApproxEquals(f1.Values, epsilon, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(this IField<Vec2d> field)
        {
            return field.Values.Sum(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(this IField<Vec2d> field)
        {
            return field.Values.Mean(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MaxParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Max(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MinParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Min(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IField<Vec2d> f0, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AbsParallel(f0.Count, result.Values);
            else
                f0.Values.Abs(f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Add(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.SubtractParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Subtract(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IField<Vec2d> f0, double t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.ScaleParallel(t, f0.Count, result.Values);
            else
                f0.Values.Scale(t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<Vec2d> f0, IField<Vec2d> f1, double t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<Vec2d> f0, IField<Vec2d> f1, IField<double> t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<Vec2d> f0, double t0, IField<Vec2d> f1, double t1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0, f1.Values, t1, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<Vec2d> f0, IField<double> t0, IField<Vec2d> f1, IField<double> t1, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> f2, double t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<Vec2d> f0, IField<Vec2d> f1, IField<Vec2d> f2, IField<double> t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<Vec2d> f0, IField<Vec2d> f1, double t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values, t, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<Vec2d> f0, IField<Vec2d> f1, IField<double> t, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IField<Vec2d> field, Domain2d domain, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.NormalizeParallel(domain, result.Values);
            else
                field.Values.Normalize(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IField<Vec2d> field, Domain2d domain, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.EvaluateParallel(domain, result.Values);
            else
                field.Values.Evaluate(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IField<Vec2d> field, Domain2d from, Domain2d to, IField<Vec2d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.RemapParallel(from, to, result.Values);
            else
                field.Values.Remap(from, to, result.Values);
        }

        #endregion


        #region  IField<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this IField<Vec3d> f0, IField<Vec3d> f1, Vec3d epsilon)
        {
            return f0.Values.ApproxEquals(f1.Values, epsilon, f0.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(this IField<Vec3d> field)
        {
            return field.Values.Sum(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(this IField<Vec3d> field)
        {
            return field.Values.Mean(field.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MaxParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Max(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.MinParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Min(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this IField<Vec3d> f0, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AbsParallel(f0.Count, result.Values);
            else
                f0.Values.Abs(f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Add(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.SubtractParallel(f1.Values, f0.Count, result.Values);
            else
                f0.Values.Subtract(f1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this IField<Vec3d> f0, double t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.ScaleParallel(t, f0.Count, result.Values);
            else
                f0.Values.Scale(t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<Vec3d> f0, IField<Vec3d> f1, double t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + f1 * t
        /// </summary>
        public static void AddScaled(this IField<Vec3d> f0, IField<Vec3d> f1, IField<double> t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<Vec3d> f0, double t0, IField<Vec3d> f1, double t1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0, f1.Values, t1, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0, f1.Values, t1, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 * t0 + f1 * t1
        /// </summary>
        public static void AddScaled(this IField<Vec3d> f0, IField<double> t0, IField<Vec3d> f1, IField<double> t1, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledParallel(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaled(t0.Values, f1.Values, t1.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> f2, double t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// result = f0 + (f1 - f2) * t
        /// </summary>
        public static void AddScaledDelta(this IField<Vec3d> f0, IField<Vec3d> f1, IField<Vec3d> f2, IField<double> t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.AddScaledDeltaParallel(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.AddScaledDelta(f1.Values, f2.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<Vec3d> f0, IField<Vec3d> f1, double t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values, t, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this IField<Vec3d> f0, IField<Vec3d> f1, IField<double> t, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                f0.Values.LerpToParallel(f1.Values, t.Values, f0.Count, result.Values);
            else
                f0.Values.LerpTo(f1.Values, t.Values, f0.Count, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this IField<Vec3d> field, Domain3d domain, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.NormalizeParallel(domain, result.Values);
            else
                field.Values.Normalize(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this IField<Vec3d> field, Domain3d domain, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.EvaluateParallel(domain, result.Values);
            else
                field.Values.Evaluate(domain, result.Values);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this IField<Vec3d> field, Domain3d from, Domain3d to, IField<Vec3d> result, bool parallel = false)
        {
            if (parallel)
                field.Values.RemapParallel(from, to, result.Values);
            else
                field.Values.Remap(from, to, result.Values);
        }

        #endregion






        #region double[]

        /// <summary>
        /// 
        /// </summary>
        internal static double ValueAt(this double[] values, int[] indices, double[] weights)
        {
            double sum = 0.0;

            for(int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this double[] values, int[] indices, double[] weights, double amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }

        #endregion


        #region Vec2d[]

        /// <summary>
        /// 
        /// </summary>
        internal static Vec2d ValueAt(this Vec2d[] values, int[] indices, double[] weights)
        {
            Vec2d sum = new Vec2d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this Vec2d[] values, int[] indices, double[] weights, Vec2d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }

        #endregion


        #region Vec3d[]

        /// <summary>
        /// 
        /// </summary>
        internal static Vec3d ValueAt(this Vec3d[] values, int[] indices, double[] weights)
        {
            Vec3d sum = new Vec3d();

            for (int i = 0; i < indices.Length; i++)
                sum += values[indices[i]] * weights[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        internal static void IncrementAt(this Vec3d[] values, int[] indices, double[] weights, Vec3d amount)
        {
            for (int i = 0; i < indices.Length; i++)
                values[indices[i]] += amount * weights[i];
        }

        #endregion
    }
}
