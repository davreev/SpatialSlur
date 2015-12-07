using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// a catch-all static class for stray general purpose geometry methods
    /// </summary>
    public static class GeometryUtil
    {
        [Obsolete("Use GetShortestVector instead")]
        /// <summary>
        /// returns the volume gradient of a tetrahedron defined by the 4 given points
        /// this is the same as finding the shortest vector between the two diagonals of the skew quadrilateral defined by the same 4 vertices
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vec3d GetVolumeGradient(Vec3d p0, Vec3d p1, Vec3d p2, Vec3d p3)
        {
            Vec3d u = p2 - p0;
            Vec3d v = p3 - p1;
            Vec3d w = p0 - p1;

            double duu = u * u;
            double duv = u * v;
            double dvv = v * v;
            double duw = u * w;
            double dvw = v * w;

            double t = 1.0 / (duu * dvv - duv * duv);
            double tu = (duv * dvw - dvv * duw) * t;
            double tv = (duu * dvw - duv * duw) * t;

            return w + tu * u - tv * v;
        }


        /// <summary>
        /// Returns the closest pair of points between two skew lines a and b
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static void GetClosestPoints(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1, out double ta, out double tb)
        {
            Vec3d u = a1 - a0;
            Vec3d v = b1 - b0;
            Vec3d w = a0 - b0;

            double duu = u * u;
            double duv = u * v;
            double dvv = v * v;
            double duw = u * w;
            double dvw = v * w;

            double t = 1.0 / (duu * dvv - duv * duv);
            ta = (duv * dvw - dvv * duw) * t;
            tb = (duu * dvw - duv * duw) * t;
        }


        /// <summary>
        /// Returns the shortest span vector between two skew lines a and b.
        /// This can also be understood as the volume gradient of the tetrahedron defined by skew lines a and b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vec3d GetShortestVector(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1)
        {
            Vec3d u = a1 - a0;
            Vec3d v = b1 - b0;
            Vec3d w = a0 - b0;

            double duu = u * u;
            double duv = u * v;
            double dvv = v * v;
            double duw = u * w;
            double dvw = v * w;

            double t = 1.0 / (duu * dvv - duv * duv);
            double tu = (duv * dvw - dvv * duw) * t;
            double tv = (duu * dvw - duv * duw) * t;

            return w + tu * u - tv * v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="unitNormal"></param>
        /// <returns></returns>
        public static Vec3d ReflectInPlane(Vec3d point, Vec3d origin, Vec3d unitNormal)
        {
            Vec3d d = point - origin;
            d -= 2.0 * (d * unitNormal) * unitNormal;
            return d + origin;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="unitNormal"></param>
        /// <param name="unitNormal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d direction, Vec3d origin, Vec3d unitNormal)
        {
            double t = ((origin - point) * unitNormal) / (direction * unitNormal);
            return point + direction * t;
        }


        /// <summary>
        /// Returns the numerical approximation of the gradient of the given function with respect to v0.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vec"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d GetGradient(Func<Vec3d, double> func, Vec3d vec, double delta)
        {
            Vec3d d = new Vec3d(delta, 0.0, 0.0);
            double gx0 = func(vec - d);
            double gx1 = func(vec + d);

            d = new Vec3d(0.0, delta, 0.0);
            double gy0 = func(vec - d);
            double gy1 = func(vec + d);

            d = new Vec3d(0.0, 0.0, delta);
            double gz0 = func(vec - d);
            double gz1 = func(vec + d);

            return new Vec3d(gx1 - gx0, gy1 - gy0, gz1 - gz0) / (delta * 2.0);
        }


        /// <summary>
        /// returns the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec3d> vectors)
        {
            Vec3d mean;
            return GetCovariance(vectors, out mean);
        }


        /// <summary>
        /// returns the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec3d> vectors, out Vec3d mean)
        {
            // calculate mean
            mean = new Vec3d();
            foreach (Vec3d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[9];

            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[2] += d.x * d.z;
                result[4] += d.y * d.y;
                result[5] += d.y * d.z;
                result[8] += d.z * d.z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
            return result;
        }


        /// <summary>
        /// returns the the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec2d> vectors)
        {
            Vec2d mean;
            return GetCovariance(vectors, out mean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovariance(IList<Vec2d> vectors, out Vec2d mean)
        {
            // calculate mean
            mean = new Vec2d();
            foreach (Vec2d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[4];
            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[3] += d.y * d.y;
            }

            // set symmetric values
            result[2] = result[1];
            return result;
        }
    }
}
