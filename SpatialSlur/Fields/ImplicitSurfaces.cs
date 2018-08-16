
/*
 * Notes
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImplicitSurfaces
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Gyroid(Vector3d point)
        {
            return Gyroid(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double Gyroid(double x, double y, double z)
        {
            return Math.Sin(x) * Math.Cos(y) + Math.Sin(y) * Math.Cos(z) + Math.Sin(z) * Math.Cos(x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Diamond(Vector3d point)
        {
            return Diamond(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double Diamond(double x, double y, double z)
        {
            double sx = Math.Sin(x);
            double sy = Math.Sin(y);
            double sz = Math.Sin(z);

            double cx = Math.Cos(x);
            double cy = Math.Cos(y);
            double cz = Math.Cos(z);

            return sx * sy * sz + sx * cy * cz + cx * sy * cz + cx * cy * sz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double Neovius(Vector3d point)
        {
            return Neovius(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double Neovius(double x, double y, double z)
        {
            double cx = Math.Cos(x);
            double cy = Math.Cos(y);
            double cz = Math.Cos(z);

            return 3 * (cx + cy + cz) + 4 * cx * cy * cz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double IWP(Vector3d point)
        {
            return IWP(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double IWP(double x, double y, double z)
        {
            double cx = Math.Cos(x);
            double cy = Math.Cos(y);
            double cz = Math.Cos(z);

            return cx * cy + cy * cz + cz * cx - cx * cy * cz + 0.25;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double HybridPW(Vector3d point)
        {
            return HybridPW(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double HybridPW(double x, double y, double z)
        {
            double cx = Math.Cos(x);
            double cy = Math.Cos(y);
            double cz = Math.Cos(z);

            return 4 * (cx * cy + cy * cz + cz * cx) - 3 * cx * cy * cz + 1.4;
        }
    }
}
