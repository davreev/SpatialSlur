using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurTools.Features
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PointFeature : IFeature
    {
        private Vec3d _point;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public PointFeature(Vec3d point)
        {
            _point = point;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Rank
        {
            get { return 0; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            return _point;
        }
    }
}
