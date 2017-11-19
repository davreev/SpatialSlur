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
    public class OnCurve : OnTarget<Curve>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Vec3d ClosestPoint(Curve curve, Vec3d point)
        {
            curve.ClosestPoint(point, out double t);
            return curve.PointAt(t);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="parallel"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnCurve(Curve curve, bool parallel, double weight = 1.0, int capacity = DefaultCapacity)
            : base(curve, ClosestPoint, parallel, weight, capacity)
        {
        }
    }
}

#endif