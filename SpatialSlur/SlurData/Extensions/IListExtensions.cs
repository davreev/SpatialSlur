using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

using SpatialSlur.SlurCore;

/*
 * Notes
 * All IList extension methods are redirected to equivalent array extension methods where possible for better performance.
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public static class IListExtensions
    {
        #region IList<Vec2d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        /// <param name="tolerance"></param>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public static bool Consolidate(this IList<Vec2d> points, double radius, double tolerance = SlurMath.ZeroTolerance, int maxSteps = 4)
        {
            return DataUtil.ConsolidatePoints(points, radius, tolerance, maxSteps);
        }

        #endregion


        #region IList<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        /// <param name="tolerance"></param>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public static bool Consolidate(this IList<Vec3d> points, double radius, double tolerance = SlurMath.ZeroTolerance, int maxSteps = 4)
        {
            return DataUtil.ConsolidatePoints(points, radius, tolerance, maxSteps);
        }

        #endregion
    }
}
