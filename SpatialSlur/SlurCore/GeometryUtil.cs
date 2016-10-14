using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurGraph;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// General purpose static geometry methods.
    /// </summary>
    public static class GeometryUtil
    {
        /// <summary>
        /// Returns parameters for the closest pair of points between lines a and b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static void LineLineClosestPoints(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1, out double ta, out double tb)
        {
            LineLineClosestPoints(a1 - a0, b1 - b0, a0 - b0, out ta, out tb);
        }


        /// <summary>
        /// Returns the shortest vector from line a to line b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static Vec3d LineLineShortestVector(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1)
        {
            Vec3d u = a1 - a0;
            Vec3d v = b1 - b0;
            Vec3d w = a0 - b0;

            double tu, tv;
            LineLineClosestPoints(u, v, w, out tu, out tv);

            return v * tv - u * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LineLineClosestPoints(Vec3d u, Vec3d v, Vec3d w, out double tu, out double tv)
        {
            double uu = u * u;
            double uv = u * v;
            double vv = v * v;
            double uw = u * w;
            double vw = v * w;

            double denom = 1.0 / (uu * vv - uv * uv);
            tu = (uv * vw - vv * uw) * denom;
            tv = (uu * vw - uv * uw) * denom;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ReflectInPlane(Vec3d point, Vec3d origin, Vec3d normal)
        {
            return point + Vec3d.Project(origin - point, normal) * 2.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d origin, Vec3d normal)
        {
            return point + Vec3d.Project(origin - point, normal);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d origin, Vec3d normal, Vec3d direction)
        {
            return point + direction * (((origin - point) * normal) / (direction * normal));
        }


        /// <summary>
        /// Returns the center of the circle that passes through the 3 given points.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetCurvatureCenter(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return p1 + GetCurvatureVector(p0 - p1, p2 - p1);
        }


        /// <summary>
        /// http://www.block.arch.ethz.ch/brg/files/2013-ijss-vanmele-shaping-tension-structures-with-actively-bent-linear-elements_1386929572.pdf
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d GetCurvatureVector(Vec3d v0, Vec3d v1)
        {
            Vec3d v2 = Vec3d.Cross(v0, v1);
            return Vec3d.Cross((v0.SquareLength * v1 - v1.SquareLength * v0), v2) / (2.0 * v2.SquareLength);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vec3d GetBarycentric(Vec3d point, Vec3d a, Vec3d b, Vec3d c)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsInTriangle(Vec3d point, Vec3d a, Vec3d b, Vec3d c)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the aspect ratio of the triangle defined by 3 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTriAspect(Vec3d[] points)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the aspect ratio of the tetrahedra defined by 4 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTetraAspect(Vec3d[] points)
        {
            // get length of longest edge
            double d0 = 0.0;
            for (int i = 0; i < 4; i++)
            {
                Vec3d p0 = points[i];

                for (int j = i + 1; j < 4; j++)
                    d0 = Math.Max(d0, p0.SquareDistanceTo(points[j]));
            }

            // get shortest altitude
            double d1 = Double.PositiveInfinity;
            for (int i = 0; i < 4; i++)
            {
                Vec3d p0 = points[i];
                Vec3d p1 = points[(i + 1) & 3];
                Vec3d p2 = points[(i + 2) & 3];
                Vec3d p3 = points[(i + 3) & 3];

                Vec3d d = Vec3d.Project(p1 - p0, Vec3d.Cross(p2 - p0, p3 - p0));
                d1 = Math.Min(d1, d.SquareLength);
            }

            return Math.Sqrt(d0) / Math.Sqrt(d1);
        }


        /// <summary>
        /// Returns entries of a rotation matrix in column major order.
        /// Assumes the given axis is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double[] GetRotationMatrix(Vec3d axis, double angle)
        {
            double[] result = new double[9];
            GetRotationMatrix(axis, angle, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="result"></param>
        public static void GetRotationMatrix(Vec3d axis, double angle, double[] result)
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1.0 - c;

            result[0] = c + axis.x * axis.x * t; // m00
            result[4] = c + axis.y * axis.y * t; // m11
            result[8] = c + axis.z * axis.z * t; // m22

            double tmp1 = axis.x * axis.y * t;
            double tmp2 = axis.z * s;
            result[1] = tmp1 + tmp2; // m01
            result[3] = tmp1 - tmp2; // m10

            tmp1 = axis.x * axis.z * t;
            tmp2 = axis.y * s;
            result[2] = tmp1 - tmp2; // m02
            result[6] = tmp1 + tmp2; tmp1 = axis.y * axis.z * t; // m20

            tmp2 = axis.x * s;
            result[5] = tmp1 + tmp2; // m21
            result[7] = tmp1 - tmp2; // m12
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec2d GetGradient(Func<Vec2d, double> func, Vec2d vector, double delta)
        {
            double x = vector.x;
            double y = vector.y;
       
            double gx = func(new Vec2d(x + delta, y)) - func(new Vec2d(x - delta, y));
            double gy = func(new Vec2d(x, y + delta)) - func(new Vec2d(x, y - delta));
            return new Vec2d(gx, gy) / (delta * 2.0);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d GetGradient(Func<Vec3d, double> func, Vec3d vector, double delta)
        {
            double x = vector.x;
            double y = vector.y;
            double z = vector.z;

            double gx = func(new Vec3d(x + delta, y, z)) - func(new Vec3d(x - delta, y, z));
            double gy = func(new Vec3d(x, y + delta, z)) - func(new Vec3d(x, y - delta, z));
            double gz = func(new Vec3d(x, y, z + delta)) - func(new Vec3d(x, y, z - delta));
            return new Vec3d(gx, gy, gz) / (delta * 2.0);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        public static Vecd GetGradient(Func<Vecd, double> func, Vecd vector, double delta)
        {
            Vecd result = new Vecd(vector.Count);
            GetGradient(func, vector, delta, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <param name="result"></param>
        public static void GetGradient(Func<Vecd, double> func, Vecd vector, double delta, Vecd result)
        {
            double d2 = 1.0 / (delta * 2.0);

            for (int i = 0; i < vector.Count; i++)
            {
                double t = vector[i];

                vector[i] = t + delta;
                double g0 = func(vector);

                vector[i] = t - delta;
                double g1 = func(vector);

                result[i] = (g0 - g1) * d2;
                vector[i] = t;
            }
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        public static double[] GetGradient(Func<double[], double> func, double[] vector, double delta)
        {
            double[] result = new double[vector.Length];
            GetGradient(func, vector, delta, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <param name="result"></param>
        public static void GetGradient(Func<double[], double> func, double[] vector, double delta, double[] result)
        {
            double d2 = 1.0 / (delta * 2.0);

            for (int i = 0; i < vector.Length; i++)
            {
                double t = vector[i];

                vector[i] = t + delta;
                double g0 = func(vector);

                vector[i] = t - delta;
                double g1 = func(vector);

                result[i] = (g0 - g1) * d2;
                vector[i] = t;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec2d> vectors, out Vec2d mean)
        {
            double[] result = new double[4];
            GetCovarianceMatrix(vectors, result, out mean);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(IList<Vec2d> vectors, double[] result, out Vec2d mean)
        {
            // calculate mean
            mean = new Vec2d();
            foreach (Vec2d v in vectors) mean += v;
            mean /= vectors.Count;

            // set result to 0
            Array.Clear(result, 0, 4);

            // calculate covariance matrix
            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[3] += d.y * d.y;
            }

            // set symmetric values
            result[2] = result[1];
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec3d> vectors, out Vec3d mean)
        {
            double[] result = new double[9];
            GetCovarianceMatrix(vectors, result, out mean);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        public static void GetCovarianceMatrix(IList<Vec3d> vectors, double[] result, out Vec3d mean)
        {
            // calculate mean
            mean = new Vec3d();
            foreach (Vec3d v in vectors) mean += v;
            mean /= vectors.Count;

            // set result to 0
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
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
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vecd> vectors, out Vecd mean)
        {
            int n = vectors[0].Count;
            double[] result = new double[n * n];
            mean = new Vecd(n);
            
            GetCovarianceMatrixImpl(vectors, result, mean);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="meanOut"></param>
        public static void GetCovarianceMatrix(IList<Vecd> vectors, double[] result, Vecd meanOut)
        {
            Array.Clear(result, 0, result.Length);
            Array.Clear(meanOut.Values, 0, meanOut.Count);
            GetCovarianceMatrixImpl(vectors, result, meanOut);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetCovarianceMatrixImpl(IList<Vecd> vectors, double[] result, Vecd meanOut)
        {
            int n = vectors[0].Count;

            // calculate mean
            for (int i = 0; i < vectors.Count; i++) meanOut.Add(vectors[i]);
            meanOut.Scale(1.0 / vectors.Count);

            // calculate lower triangular covariance matrix
            for (int i = 0; i < vectors.Count; i++)
            {
                double[] vec = vectors[i].Values;

                for (int j = 0; j < n; j++)
                {
                    double dj = vec[j] - meanOut[j];
                    result[j * n + j] += dj * dj; // diagonal entry

                    for (int k = j + 1; k < n; k++)
                        result[j * n + k] += dj * (vec[k] - meanOut[k]);
                }
            }

            // fill out upper triangular
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * n + i] = result[i * n + j];
            }
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<double[]> vectors, out double[] mean)
        {
            int n = vectors[0].Length;
            double[] result = new double[n * n];
            mean = new double[n];

            GetCovarianceMatrixImpl(vectors, result, mean);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="meanOut"></param>
        public static void GetCovarianceMatrix(IList<double[]> vectors, double[] result, double[] meanOut)
        {
            Array.Clear(result, 0, result.Length);
            Array.Clear(meanOut, 0, meanOut.Length);
            GetCovarianceMatrixImpl(vectors, result, meanOut);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetCovarianceMatrixImpl(IList<double[]> vectors, double[] result, double[] meanOut)
        {
            int n = vectors[0].Length;

            // calculate mean
            for (int i = 0; i < vectors.Count; i++) VecMath.Add(meanOut, vectors[i], n, meanOut);
            VecMath.Scale(meanOut, 1.0 / vectors.Count, n, meanOut);

            // calculate lower triangular covariance matrix
            for (int i = 0; i < vectors.Count; i++)
            {
                double[] vec = vectors[i];
       
                for (int j = 0; j < n; j++)
                {
                    double dj = vec[j] - meanOut[j];
                    result[j * n + j] += dj * dj; // diagonal entry

                    for (int k = j + 1; k < n; k++)
                        result[j * n + k] += dj * (vec[k] - meanOut[k]);
                }
            }

            // fill out upper triangular
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * n + i] = result[i * n + j];
            }
        }
    }
}
