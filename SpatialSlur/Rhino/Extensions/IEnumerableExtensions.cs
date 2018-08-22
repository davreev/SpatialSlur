
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Generic;

using Rhino.Geometry;

using SpatialSlur;
using SpatialSlur.Meshes;
using SpatialSlur.Rhino;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class IEnumerableExtensions
    {
        #region IEnumerable<Vector3d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static Vector3d Mean(this IEnumerable<Vector3d> vectors)
        {
            var sum = new Vector3d();
            int count = 0;

            foreach (Vector3d v in vectors)
            {
                sum += v;
                count++;
            }

            return sum / count;
        }

        #endregion


        #region IEnumerable<Point3d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point3d Mean(this IEnumerable<Point3d> points)
        {
            var sum = new Point3d();
            int count = 0;

            foreach (Point3d p in points)
            {
                sum += p;
                count++;
            }

            return sum / count;
        }

        #endregion


        #region IEnumerable<Line>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static HeGraph3d ToHeGraph(this IEnumerable<Line> lines, double tolerance = D.ZeroTolerance, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return HeGraph3d.Factory.CreateFromLineSegments(lines, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }

        #endregion


        #region IEnumerable<Polyline>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh3d ToHeMesh(this IEnumerable<Polyline> polylines, double tolerance = D.ZeroTolerance)
        {
            return HeMesh3d.Factory.CreateFromPolylines(polylines, tolerance);
        }

        #endregion
    }
}

#endif