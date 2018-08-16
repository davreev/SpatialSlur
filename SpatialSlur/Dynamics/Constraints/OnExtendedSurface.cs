
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
    public class OnExtendedSurface : OnTarget<Surface>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="surface"></param>
        /// <param name="weight"></param>
        public OnExtendedSurface(int index, Surface surface, double weight = 1.0)
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

            Vector3d cp = srf.PointAt(u, v);
            Vector3d cn = srf.NormalAt(u, v);
            return point + Vector3d.Project(cp - point, cn);
        }
    }
}

#endif