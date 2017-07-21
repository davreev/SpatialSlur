using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Remesher
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PlaneFeature : IFeature
    {
        private Vec3d _origin;
        private Vec3d _normal;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        public PlaneFeature(Vec3d origin, Vec3d normal)
        {
            _origin = origin;
            _normal = normal.Unitized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            return point + ((_origin - point) * _normal) * _normal;
        }
    }
}