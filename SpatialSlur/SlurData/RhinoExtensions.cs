using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public static class RhinoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Vector2d vector)
        {
            VecKd result = new VecKd(2);
            result[0] = vector.X;
            result[1] = vector.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Vector3d vector)
        {
            VecKd result = new VecKd(3);
            result[0] = vector.X;
            result[1] = vector.Y;
            result[2] = vector.Z;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Point2d point)
        {
            VecKd result = new VecKd(2);
            result[0] = point.X;
            result[1] = point.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Point3d point)
        {
            VecKd result = new VecKd(3);
            result[0] = point.X;
            result[1] = point.Y;
            result[2] = point.Z;
            return result;
        }
    }
}
