
/*
 * Notes
 * 
 * References
 * http://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class DistanceFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double Sphere(Vector3d point, double radius)
        {
            return point.Length - radius;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double Box(Vector3d point, Vector3d size)
        {
            var d = Vector3d.Abs(point) - size;
            return Math.Min(d.ComponentMax, 0.0) + Vector3d.Min(d, 0.0).Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="axis"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static double Capsule(Vector3d point, Vector3d start, Vector3d axis, double radius)
        {
            var d = point - start;
            var t = SlurMath.Saturate(Vector3d.Dot(d, axis) / axis.SquareLength);
            return (d - axis * t).Length - radius;
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
