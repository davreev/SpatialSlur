
/*
 * Notes
 */

#if USING_RHINO

using Rhino.Geometry;

namespace SpatialSlur.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class PointCloudFeature : IFeature
    {
        private PointCloud _pointCloud;


        /// <summary>
        /// 
        /// </summary>
        public PointCloudFeature(PointCloud pointCloud)
        {
            _pointCloud = pointCloud;
        }


        /// <inheritdoc />
        public int Rank
        {
            get { return 0; }
        }


        /// <inheritdoc />
        public Vector3d ClosestPoint(Vector3d point)
        {
            var index = _pointCloud.ClosestPoint(point);
            return _pointCloud[index].Location;
        }
    }
}

#endif