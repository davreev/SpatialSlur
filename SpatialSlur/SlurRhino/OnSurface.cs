#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnSurface : OnTarget<Surface>
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
            return surface.PointAt(u, v);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnSurface(Surface surface, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(surface, ClosestPoint, parallel, weight, capacity)
        {
        }
    }
}

#endif