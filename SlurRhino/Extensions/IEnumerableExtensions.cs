using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;


/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableExtensions
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


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vector3d> vectors, double[] result)
        {
            GetCovarianceMatrix(vectors, Mean(vectors), result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Vector3d> vectors, Vector3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Vector3d v in vectors)
            {
                Vector3d d = v - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
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


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Point3d> points, double[] result)
        {
            GetCovarianceMatrix(points, Mean(points), result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(this IEnumerable<Point3d> points, Point3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Point3d p in points)
            {
                Vector3d d = p - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
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
        public static HeGraph3d ToHeGraph(this IEnumerable<Line> lines, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
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
        public static HeMesh3d ToHeMesh(this IEnumerable<Polyline> polylines, double tolerance = 1.0e-8)
        {
            return HeMesh3d.Factory.CreateFromPolylines(polylines, tolerance);
        }

        #endregion
    }
}
