
/*
 * Notes 
 */

#if USING_RHINO

using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// Extension methods for classes in the SpatialSlur.SlurCore namespace
    /// </summary>
    public static class SlurCoreExtensions
    {
        #region Interval2d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval2d interval)
        {
            var x = interval.X;
            var y = interval.Y;
            return new BoundingBox(x.A, y.A, 0.0, x.B, y.B, 0.0);
        }

        #endregion


        #region Interval3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static BoundingBox ToBoundingBox(this Interval3d interval)
        {
            var x = interval.X;
            var y = interval.Y;
            var z = interval.Z;
            return new BoundingBox(x.A, y.A, z.A, x.B, y.B, z.B);
        }

        #endregion


        #region Orient3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orient"></param>
        /// <returns></returns>
        public static Plane ToPlane(this Orient3d orient)
        {
            return new Plane(
                orient.Translation,
                orient.Rotation.X,
                (Vector3d)orient.Rotation.Y
                );
        }

        #endregion
    }
}

#endif