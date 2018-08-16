
/*
 * Notes
 */

using System;
using SpatialSlur;

namespace SpatialSlur.Tools
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PointFeature : IFeature
    {
        private Vector3d _point;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public PointFeature(Vector3d point)
        {
            _point = point;
        }


        /// <inheritdoc />
        public int Rank
        {
            get { return 0; }
        }


        /// <inheritdoc />
        public Vector3d ClosestPoint(Vector3d point)
        {
            return _point;
        }
    }
}
