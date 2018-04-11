
/*
 * Notes
 * 
 * References
 * http://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
 */

using System;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class SDFUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double Sphere(Vec3d point, double radius)
        {
            return point.Length - radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static double Box(Vec3d point, Vec3d scale)
        {
            var d = Vec3d.Abs(point) - scale;
            return Math.Min(d.ComponentMax, 0.0) + Vec3d.Min(d, 0.0).Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double Capsule(Vec3d point, Vec3d start, Vec3d end, double radius)
        {
            var d0 = point - start;
            var d1 = end - start;
            var t = SlurMath.Saturate(Vec3d.Dot(d0, d1) / d1.SquareLength);
            return (d0 - d1 * t).Length - radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static double Union(double d0, double d1)
        {
            return Math.Min(d0, d1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static double Difference(double d0, double d1)
        {
            return Math.Max(-d0, d1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="d0"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static double Intersection(double d0, double d1)
        {
            return Math.Max(d0, d1);
        }
    }
}
