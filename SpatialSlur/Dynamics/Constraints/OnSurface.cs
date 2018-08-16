
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnSurface : OnTarget<Surface>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="surface"></param>
        /// <param name="weight"></param>
        public OnSurface(int index, Surface surface, double weight = 1.0)
            : base(index, surface, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override Vector3d GetClosestPoint(Vector3d point)
        {
            var srf = Target;

            srf.ClosestPoint(point, out double u, out double v);
            return srf.PointAt(u, v);
        }
    }
}

#endif