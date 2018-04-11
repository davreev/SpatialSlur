
/*
 * Notes
 */

using System;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurTools
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


        /// <inheritdoc />
        public int Rank
        {
            get { return 0; }
        }


        /// <inheritdoc />
        public Vec3d ClosestPoint(Vec3d point)
        {
            return _point;
        }
    }
}
