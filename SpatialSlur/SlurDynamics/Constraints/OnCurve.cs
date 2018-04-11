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
        public OnCurve(int index, Curve curve, double weight = 1.0)
            : base(index, curve, ClosestPoint, weight)
        {
        }
    }
}

#endif