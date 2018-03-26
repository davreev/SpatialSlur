
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
        public static double Box(Vec3d point, Vec3d size)
        {
            // TODO
            throw new NotImplementedException();
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
