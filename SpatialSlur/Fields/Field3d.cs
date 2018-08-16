
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Field3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueAt"></param>
        /// <returns></returns>
        public static IField3d<T> Create<T>(Func<Vector3d, T> valueAt)
        {
            return new FuncField3d<T>(valueAt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IField3d<T> CreateTransformed<T>(IField3d<T> other, Transform3d transform)
        {
            transform.Invert();
            return Create(p => other.ValueAt(transform.Apply(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static IField3d<double> CreateUnion(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => DistanceFunctions.Union(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static IField3d<double> CreateDifference(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => DistanceFunctions.Difference(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public static IField3d<double> CreateIntersection(IField3d<double> f0, IField3d<double> f1)
        {
            return Create(p => DistanceFunctions.Intersection(f0.ValueAt(p), f1.ValueAt(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static IField3d<double> CreatePlane(Vector3d point, Vector3d normal)
        {
            normal.Unitize();
            return Create(p => Vector3d.Dot(p - point, normal));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static IField3d<double> CreateSphere(Vector3d center, double radius)
        {
            return Create(p => DistanceFunctions.Sphere(p - center, radius));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IField3d<double> CreateBox(Vector3d size)
        {
            return Create(p => DistanceFunctions.Box(p, size));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="axis"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static IField3d<double> CreateCapsule(Vector3d start, Vector3d axis, double radius)
        {
            return Create(p => DistanceFunctions.Capsule(p, start, axis, radius));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IField3d<double> CreateGyroid()
        {
            return Create(p => ImplicitSurfaces.Gyroid(p));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="function"></param>
        /// <param name="gradient"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static IField3d<double> CreateDistanceToApprox(IField3d<double> function, IField3d<Vector3d> gradient, double threshold)
        {
            // impl ref
            // http://www.iquilezles.org/www/articles/distance/distance.htm
            return Create(p => (function.ValueAt(p) - threshold) / (gradient.ValueAt(p).Length + SlurMath.ZeroToleranced));
        }
    }
}