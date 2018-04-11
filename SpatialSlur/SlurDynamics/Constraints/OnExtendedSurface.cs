#if USING_RHINO

using System;

using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnExtendedSurface : OnTarget<Surface>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Vec3d ClosestPoint(Surface surface, Vec3d point)
        {
            surface.ClosestPoint(point, out double u, out double v);

            Vec3d cp = surface.PointAt(u, v);
            Vec3d cn = surface.NormalAt(u, v);
            return point + Vec3d.Project(cp - point, cn);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnExtendedSurface(int index, Surface surface, double weight = 1.0)
            : base(index, surface, ClosestPoint, weight)
        {
        }
    }
}

#endif