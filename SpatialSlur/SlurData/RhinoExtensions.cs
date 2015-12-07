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
        /// <param name="v"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Vector2d v)
        {
            VecKd result = new VecKd(2);
            result[0] = v.X;
            result[1] = v.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Vector3d v)
        {
            VecKd result = new VecKd(3);
            result[0] = v.X;
            result[1] = v.Y;
            result[2] = v.Z;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Point2d p)
        {
            VecKd result = new VecKd(2);
            result[0] = p.X;
            result[1] = p.Y;
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static VecKd ToVecKd(this Point3d p)
        {
            VecKd result = new VecKd(3);
            result[0] = p.X;
            result[1] = p.Y;
            result[2] = p.Z;
            return result;
        }
    }
}
