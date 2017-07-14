using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurRhino;

using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino.Remesher
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ClosestPoint(Vec3d point)
        {
            _curve.ClosestPoint(point.ToPoint3d(), out double t);
            return _curve.PointAt(t).ToVec3d();
        }
    }
}
