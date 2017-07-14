using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurDynamics;

using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Constraints
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
            surface.ClosestPoint(point.ToPoint3d(), out double u, out double v);
            return surface.PointAt(u, v).ToVec3d();
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnSurface(Surface surface, int capacity, double weight = 1.0)
            : base(surface, ClosestPoint, capacity, weight)
        {
        }
    }
}
