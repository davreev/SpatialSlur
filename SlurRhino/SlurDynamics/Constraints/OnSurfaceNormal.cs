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
    public class OnSurfaceNormal : OnTarget<Surface>
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
            var cp = surface.PointAt(u, v).ToVec3d();
            var cn = surface.NormalAt(u, v).ToVec3d();
            return point + Vec3d.Project(cp - point, cn);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnSurfaceNormal(Surface surface, int capacity, double weight = 1.0)
            : base(surface, ClosestPoint, capacity, weight)
        {
        }
    }
}
