using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Utility class for stray constants and static methods.
    /// </summary>
    public static class GeometryUtil
    {
        /// <summary>
        /// Returns parameters for the closest pair of points between lines a and b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static void LineLineClosestPoints(Vec3d startA, Vec3d endA, Vec3d startB, Vec3d endB, out double ta, out double tb)
        {
            LineLineClosestPoints(endA - startA, endB - startB, startA - startB, out ta, out tb);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="deltaA"></param>
        /// <param name="startB"></param>
        /// <param name="deltaB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static void LineLineClosestPoints2(Vec3d startA, Vec3d deltaA, Vec3d startB, Vec3d deltaB, out double ta, out double tb)
        {
            LineLineClosestPoints(deltaA, deltaB, startA - startB, out ta, out tb);
        }


        /// <summary>
        /// Returns the shortest vector from line a to line b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <returns></returns>
        public static Vec3d LineLineShortestVector(Vec3d startA, Vec3d endA, Vec3d startB, Vec3d endB)
        {
            Vec3d u = endA - startA;
            Vec3d v = endB - startB;
            Vec3d w = startA - startB;

            LineLineClosestPoints(u, v, w, out double tu, out double tv);
            return v * tv - u * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="deltaA"></param>
        /// <param name="startB"></param>
        /// <param name="deltaB"></param>
        /// <returns></returns>
        public static Vec3d LineLineShortestVector2(Vec3d startA, Vec3d deltaA, Vec3d startB, Vec3d deltaB)
        {
            Vec3d w = startA - startB;

            LineLineClosestPoints(deltaA, deltaB, w, out double tu, out double tv);
            return deltaB * tv - deltaA * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LineLineClosestPoints(Vec3d u, Vec3d v, Vec3d w, out double tu, out double tv)
        {
            double uu = u.SquareLength;
            double uv = u * v;
            double vv = v.SquareLength;
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
            return point + Vec3d.MatchProjection(direction, origin - point, normal);
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
        /// <param name="vector"></param>
        /// <param name="unitX"></param>
        /// <param name="unitY"></param>
        /// <returns></returns>
        public static double GetAngleInPlane(Vec3d vector, Vec3d unitX, Vec3d unitY)
        {
            double t = Math.Atan2(vector * unitX, vector * unitY);
            return (t < 0.0) ? t + SlurMath.TwoPI : t; // shift discontinuity to 0
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
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetTriAspect(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return GetTriAspect(new Vec3d[] { p0, p1, p2 });
        }


        /// <summary>
        /// Returns the aspect ratio of the triangle defined by 3 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTriAspect(Vec3d[] points)
        {
            double maxEdge = 0.0; // longest edge
            double minAlt = Double.PositiveInfinity; // shortest altitude

            for(int i = 0; i < 3; i++)
            {
                Vec3d p0 = points[i];
                Vec3d p1 = points[(i + 1) % 3];
                Vec3d p2 = points[(i + 2) % 3];

                Vec3d v0 = p1 - p0;
                Vec3d v1 = p2 - p1;

                maxEdge = Math.Max(maxEdge, v0.SquareLength);
                minAlt = Math.Min(minAlt, Vec3d.Reject(v1, v0).SquareLength);
            }

            return Math.Sqrt(maxEdge) / Math.Sqrt(minAlt);
        }


        /// <summary>
        /// Returns the area gradient of the given trianglue with respect to p0.
        /// https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf p 64
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetTriAreaGrad(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            var d = p2 - p1;
            var n = d ^ (p0 - p1);

            n.Unitize();
            return (n ^ d) * 0.5;
        }


        /// <summary>
        /// Returns the area gradient of the given trianglue with respect to each vertex
        /// https://www.cs.cmu.edu/~kmcrane/Projects/DDG/paper.pdf p 64
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="g0"></param>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static void GetTriAreaGrads(Vec3d p0, Vec3d p1, Vec3d p2, out Vec3d g0, out Vec3d g1, out Vec3d g2)
        {
            var d0 = p1 - p0;
            var d1 = p2 - p1;
            var d2 = p0 - p2;

            var n = d0 ^ d1;
            n.Unitize();

            g0 = n ^ d1 * 0.5;
            g1 = n ^ d2 * 0.5;
            g2 = g0 + g1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetTetraAspect(Vec3d p0, Vec3d p1, Vec3d p2, Vec3d p3)
        {
            return GetTetraAspect(new Vec3d[] { p0, p1, p2, p3 });
        }


        /// <summary>
        /// Returns the aspect ratio of the tetrahedra defined by 4 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTetraAspect(Vec3d[] points)
        {
            double minEdge = 0.0;
            double maxAlt = double.PositiveInfinity;

            for (int i = 0; i < 4; i++)
            {
                Vec3d p0 = points[i];
                Vec3d p1 = points[(i + 1) & 3];
                Vec3d p2 = points[(i + 2) & 3];
                Vec3d p3 = points[(i + 3) & 3];

                Vec3d v0 = p1 - p0;
                Vec3d v1 = p2 - p1;
                Vec3d v2 = p3 - p2;

                minEdge = Math.Max(minEdge, v0.SquareLength);
                maxAlt = Math.Min(maxAlt, Vec3d.Project(v2, Vec3d.Cross(v0, v1)).SquareLength);
            }

            return Math.Sqrt(minEdge) / Math.Sqrt(maxAlt);
        }


        /// <summary>
        /// Returns entries of a rotation matrix in column major order.
        /// Assumes the given axis is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="result"></param>
        public static void GetRotationMatrix(Vec3d axis, double angle, double[] result)
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1.0 - c;

            result[0] = c + axis.X * axis.X * t; // m00
            result[4] = c + axis.Y * axis.Y * t; // m11
            result[8] = c + axis.Z * axis.Z * t; // m22

            double p0 = axis.X * axis.Y * t;
            double p1 = axis.Z * s;

            result[1] = p0 + p1; // m01
            result[3] = p0 - p1; // m10

            p0 = axis.X * axis.Z * t;
            p1 = axis.Y * s;

            result[2] = p0 - p1; // m02
            result[6] = p0 + p1; // m20

            p0 = axis.Y * axis.Z * t; 
            p1 = axis.X * s;

            result[5] = p0 + p1; // m21
            result[7] = p0 - p1; // m12
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
            double x = vector.X;
            double y = vector.Y;
       
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
            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;

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
    }
}
