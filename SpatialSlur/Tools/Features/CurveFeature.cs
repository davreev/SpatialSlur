
/*
 * Notes
 */

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Tools
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CurveFeature : IFeature
    {
        private Curve _curve;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        public CurveFeature(Curve curve)
        {
            _curve = curve;
        }


        /// <inheritdoc />
        public int Rank
        {
            get { return 1; }
        }


        /// <inheritdoc />
        public Vector3d ClosestPoint(Vector3d point)
        {
            _curve.ClosestPoint(point, out double t);
            return _curve.PointAt(t);
        }
    }
}

#endif