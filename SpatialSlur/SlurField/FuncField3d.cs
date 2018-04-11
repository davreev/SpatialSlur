
/*
 * Notes
 */

using System;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FuncField3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueAt"></param>
        /// <returns></returns>
        public static FuncField3d<T> Create<T>(Func<Vec3d, T> valueAt)
        {
            return new FuncField3d<T>(valueAt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateUnion(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => SDFUtil.Union(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateDifference(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => SDFUtil.Difference(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateIntersection(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => SDFUtil.Intersection(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateSphere(Vec3d center, double radius)
        {
            return Create(p => SDFUtil.Sphere(p - center, - radius));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateGyroid(Vec3d offset, Vec3d scale)
        {
            var invScale = 1.0 / scale;
            return Create(p => ImplicitSurfaces.Gyroid((p + offset) * invScale));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="grad"></param>
        /// <returns></returns>
        public static FuncField3d<double> CreateDistanceToApprox(IField3d<double> function, IField3d<Vec3d> gradient, double threshold)
        {
            // impl ref
            // http://www.iquilezles.org/www/articles/distance/distance.htm
            return Create(p => (function.ValueAt(p) - threshold) / (gradient.ValueAt(p).Length + SlurMath.ZeroTolerance));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncField3d<T>: IField2d<T>, IField3d<T>
    {
        /// <summary></summary>
        public readonly Func<Vec3d, T> ValueAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueAt"></param>
        internal FuncField3d(Func<Vec3d, T> valueAt)
        {
            ValueAt = valueAt ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(point);
        }


        /// <inheritdoc />
        T IField3d<T>.ValueAt(Vec3d point)
        {
            return ValueAt(point);
        }

        #endregion
    }
}