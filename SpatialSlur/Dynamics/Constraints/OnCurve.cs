
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
    public class OnCurve : OnTarget<Curve>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="curve"></param>
        /// <param name="weight"></param>
        public OnCurve(int index, Curve curve, double weight = 1.0)
            : base(index, curve, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override Vector3d GetClosestPoint(Vector3d point)
        {
            var crv = Target;
            crv.ClosestPoint(point, out double t);
            return crv.PointAt(t);
        }
    }
}

#endif