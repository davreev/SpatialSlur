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
    public class PerlinNoiseField : 
        IField2d<double>, IField2d<Vec2d>, IDifferentiableField2d<Vec2d>,
        IField3d<double>, IField3d<Vec3d>, IDifferentiableField3d<Vec3d>
    {
        /// <summary></summary>
        public double ScaleX;
        /// <summary></summary>
        public double ScaleY;
        /// <summary></summary>
        public double ScaleZ;


        /// <summary>
        /// 
        /// </summary>
        public PerlinNoiseField(double scaleX = 1.0, double scaleY = 1.0, double scaleZ = 1.0)
        {
            ScaleX = scaleX;
            ScaleY = scaleY;
            ScaleZ = scaleZ;
        }


        #region 2d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double ValueAt(Vec2d point)
        {
            return PerlinNoise.ValueAt(point.X * ScaleX, point.Y * ScaleY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d VectorAt(Vec2d point)
        {
            return PerlinNoise.VectorAt(point.X * ScaleX, point.Y * ScaleY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d GradientAt(Vec2d point)
        {
            return PerlinNoise.GradientAt(point.X * ScaleX, point.Y * ScaleY);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d CurlAt(Vec2d point)
        {
            return PerlinNoise.CurlAt(point.X * ScaleX, point.Y * ScaleY);
        }

        #endregion


        #region 3d operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double ValueAt(Vec3d point)
        {
            return PerlinNoise.ValueAt(point.X * ScaleX, point.Y * ScaleY, point.Z * ScaleZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d VectorAt(Vec3d point)
        {
            return PerlinNoise.VectorAt(point.X * ScaleX, point.Y * ScaleY, point.Z * ScaleZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d GradientAt(Vec3d point)
        {
            return PerlinNoise.GradientAt(point.X * ScaleX, point.Y * ScaleY, point.Z * ScaleZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d CurlAt(Vec3d point)
        {
            return PerlinNoise.CurlAt(point.X * ScaleX, point.Y * ScaleY, point.Z * ScaleZ);
        }

        #endregion


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vec2d IField2d<Vec2d>.ValueAt(Vec2d point)
        {
            return VectorAt(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vec3d IField3d<Vec3d>.ValueAt(Vec3d point)
        {
            return VectorAt(point);
        }

        #endregion
    }
}
