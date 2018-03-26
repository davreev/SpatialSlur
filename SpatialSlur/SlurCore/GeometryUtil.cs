
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Utility class for stray constants and static methods.
    /// </summary>
    public static class GeometryUtil
    {
        /// <summary>
        /// Returns parameters for the closest pair of points between lines a and b.
        /// Returns false if the solution is not unique i.e. the given lines are parallel.
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static bool LineLineClosestPoints(Vec3d startA, Vec3d endA, Vec3d startB, Vec3d endB, out double ta, out double tb)
        {
            return LineLineClosestPoints(endA - startA, endB - startB, startA - startB, out ta, out tb);
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
        public static bool LineLineClosestPoints2(Vec3d startA, Vec3d deltaA, Vec3d startB, Vec3d deltaB, out double ta, out double tb)
        {
            return LineLineClosestPoints(deltaA, deltaB, startA - startB, out ta, out tb);
        }


        /// <summary>
        /// Returns the shortest vector from line a to line b.
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
        private static bool LineLineClosestPoints(Vec3d u, Vec3d v, Vec3d w, out double tu, out double tv)
        {
            // impl ref
            /// http://geomalgorithms.com/a07-_distance.html

            double uu = u.SquareLength;
            double vv = v.SquareLength;

            double uv = Vec3d.Dot(u, v);
            double uw = Vec3d.Dot(u, w);
            double vw = Vec3d.Dot(v, w);

            var d = uu * vv - uv * uv;

            if (Math.Abs(d) > 0.0)
            {
                d = 1.0 / d;
                tu = (uv * vw - vv * uw) * d;
                tv = (uu * vw - uv * uw) * d;
                return true;
            }

            // lines are parallel, infinite solutions
            tu = tv = 0.0;
            return false;
        }


        /// <summary>
        /// Returns paramters for the point of intersection between lines a and b.
        /// Returns false if there is no solution i.e. lines are parallel.
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static bool LineLineIntersection(Vec2d startA, Vec2d endA, Vec2d startB, Vec2d endB, out double ta, out double tb)
        {
            return LineLineIntersection(endA - startA, endB - startB, startB - startA, out ta, out tb);
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
        public static bool LineLineIntersection2(Vec2d startA, Vec2d deltaA, Vec2d startB, Vec2d deltaB, out double ta, out double tb)
        {
            return LineLineIntersection(deltaA, deltaB, startB - startA, out ta, out tb);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="tu"></param>
        /// <param name="tv"></param>
        private static bool LineLineIntersection(Vec2d u, Vec2d v, Vec2d w, out double tu, out double tv)
        {
            // impl ref
            // https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
            
            var c = Vec2d.Cross(u, v);

            if (Math.Abs(c) > 0.0)
            {
                c = 1.0 / c;
                tu = Vec2d.Cross(w, v) * c;
                tv = Vec2d.Cross(w, u) * c;
                return true;
            }

            // lines are parallel, no solution
            tu = tv = 0.0;
            return false;
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
        [Obsolete("Use ProjectToPlaneAlong instead")]
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d origin, Vec3d normal, Vec3d direction)
        {
            return point + Vec3d.MatchProjection(direction, origin - point, normal);
        }


        /// <summary>
        /// Projects a vector to the given plane along the given direction.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlaneAlong(Vec3d vector, Vec3d normal, Vec3d direction)
        {
            return vector - Vec3d.MatchProjection(direction, vector, normal);
        }


        /// <summary>
        /// Projects a point to the given plane along the given direction.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlaneAlong(Vec3d point, Vec3d origin, Vec3d normal, Vec3d direction)
        {
            return point + Vec3d.MatchProjection(direction, origin - point, normal);
        }
    

        /// <summary>
        /// Returns false if the 2 given vectors are parallel.
        /// The direction of the first vector is maintained.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static bool Orthonormalize(ref Vec3d x, ref Vec3d y, out Vec3d z)
        {
            z = Vec3d.Cross(x, y);
            double m = z.SquareLength;

            if (m > 0.0)
            {
                x /= x.Length;
                z /= Math.Sqrt(m);
                y = Vec3d.Cross(z, x);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetPolygonArea(IEnumerable<Vec2d> points)
        {
            var itr = points.GetEnumerator();
            itr.MoveNext();

            var first = itr.Current;

            var p0 = first;
            var sum = 0.0;
            
            while(itr.MoveNext())
            {
                var p1 = itr.Current;
                sum += Vec2d.Cross(p0, p1);
                p0 = p1;
            }

            sum += Vec2d.Cross(p0, first);
            return sum * 0.5;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetPolygonArea(IEnumerable<Vec3d> points, Vec3d unitNormal)
        {
            var itr = points.GetEnumerator();
            itr.MoveNext();

            var first = itr.Current;

            var p0 = first;
            var sum = Vec3d.Zero;

            while (itr.MoveNext())
            {
                var p1 = itr.Current;
                sum += Vec3d.Cross(p0, p1);
                p0 = p1;
            }

            sum += Vec3d.Cross(p0, first);
            return Vec3d.Dot(sum, unitNormal) * 0.5;
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
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetCircumcenter(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return p0 + GetCurvatureVector(p1 - p0, p2 - p0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetIncenter(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);
            double perimInv = 1.0 / (d01 + d12 + d20); // inverse perimeter
            return p0 * (d12 * perimInv) + p1 * (d20 * perimInv) + p2 * (d01 * perimInv);
        }

        
        /// <summary>
        /// Assumes the given axes are orthonormal.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public static double GetAngleInPlane(Vec3d vector, Vec3d xAxis, Vec3d yAxis)
        {
            double t = Math.Atan2(Vec3d.Dot(vector, xAxis), Vec3d.Dot(vector, yAxis));
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
        /// Returns the aspect ratio of the triangle defined by 3 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTriAspect(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            double maxEdge = 0.0; // longest edge
            double minAlt = Double.PositiveInfinity; // shortest altitude

            Vec3d v0 = p1 - p0;
            Vec3d v1 = p2 - p1;
            Vec3d v2 = p0 - p2;

            Sub(v0, v1);
            Sub(v1, v2);
            Sub(v2, v0);

            return Math.Sqrt(maxEdge) / Math.Sqrt(minAlt);

            void Sub(Vec3d a, Vec3d b)
            {
                maxEdge = Math.Max(maxEdge, a.SquareLength);
                minAlt = Math.Min(minAlt, Vec3d.Reject(b, a).SquareLength);
            }
        }


        /// <summary>
        /// Returns the area gradient of the given trianglue with respect to p0.
        /// http://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleMeshDerivativesCheatSheet.pdf
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetTriAreaGradient(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            var d = p2 - p1;
            var n = Vec3d.Cross(d, p0 - p1);

            n.Unitize();
            return Vec3d.Cross(n, d) * 0.5;
        }


        /// <summary>
        /// Returns the area gradient of the given triangle with respect to each vertex
        /// http://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleMeshDerivativesCheatSheet.pdf
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="g0"></param>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static void GetTriAreaGradients(Vec3d p0, Vec3d p1, Vec3d p2, out Vec3d g0, out Vec3d g1, out Vec3d g2)
        {
            var d0 = p1 - p0;
            var d1 = p2 - p1;
            var d2 = p0 - p2;

            var n = Vec3d.Cross(d0, d1);
            n.Unitize();

            g0 = Vec3d.Cross(n, d1) * 0.5;
            g1 = Vec3d.Cross(n, d2) * 0.5;
            g2 = g0 + g1;
        }


        /// <summary>
        /// Returns the aspect ratio of the tetrahedra defined by 4 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetTetraAspect(Vec3d p0, Vec3d p1, Vec3d p2, Vec3d p3)
        {
            double minEdge = 0.0;
            double maxAlt = double.PositiveInfinity;

            Vec3d v0 = p1 - p0;
            Vec3d v1 = p2 - p1;
            Vec3d v2 = p3 - p2;
            Vec3d v3 = p0 - p3;

            Sub(v0, v1, v2);
            Sub(v1, v2, v3);
            Sub(v2, v3, v0);
            Sub(v3, v0, v1);

            return Math.Sqrt(minEdge) / Math.Sqrt(maxAlt);

            void Sub(Vec3d a, Vec3d b, Vec3d c)
            {
                minEdge = Math.Max(minEdge, a.SquareLength);
                maxAlt = Math.Min(maxAlt, Vec3d.Project(c, Vec3d.Cross(a, b)).SquareLength);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        public static double GetDihedralAngle(Vec3d start, Vec3d end, Vec3d left, Vec3d right)
        {
            var d = end - start;
            return GetDihedralAngle(d.Unit, Vec3d.Cross(d, left - end), Vec3d.Cross(d, start - right));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rightNormal"></param>
        /// <param name="leftNormal"></param>
        /// <returns></returns>
        public static double GetDihedralAngle(Vec3d unitAxis, Vec3d leftNormal, Vec3d rightNormal)
        {
            // impl ref
            // http://brickisland.net/DDGFall2017/2017/10/12/assignment-1-coding-investigating-curvature/

            return 
                Math.Atan2(
                Vec3d.Dot(unitAxis, Vec3d.Cross(leftNormal, rightNormal)), 
                Vec3d.Dot(leftNormal, rightNormal)
                ) + Math.PI;
        }


        /// <summary>
        /// Calculates planarity as the shortest distance between the 2 diagonals over the mean diagonal length.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetQuadPlanarity(Vec3d p0, Vec3d p1, Vec3d p2, Vec3d p3)
        {
            var d0 = p2 - p0;
            var d1 = p3 - p1;
            var d2 = LineLineShortestVector2(p0, d0, p1, d1);
            return d2.Length / ((d1.Length + d0.Length) * 0.5);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vec2d GetGradient(Func<Vec2d, double> function, Vec2d vector, double epsilon = SlurMath.ZeroTolerance)
        {
            (var x, var y) = vector;

            double gx = function(new Vec2d(x + epsilon, y)) - function(new Vec2d(x - epsilon, y));
            double gy = function(new Vec2d(x, y + epsilon)) - function(new Vec2d(x, y - epsilon));

            return new Vec2d(gx, gy) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vec3d GetGradient(Func<Vec3d, double> function, Vec3d vector, double epsilon = SlurMath.ZeroTolerance)
        {
            (var x, var y, var z) = vector;

            double gx = function(new Vec3d(x + epsilon, y, z)) - function(new Vec3d(x - epsilon, y, z));
            double gy = function(new Vec3d(x, y + epsilon, z)) - function(new Vec3d(x, y - epsilon, z));
            double gz = function(new Vec3d(x, y, z + epsilon)) - function(new Vec3d(x, y, z - epsilon));

            return new Vec3d(gx, gy, gz) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <param name="epsilon"></param>
        public static void GetGradient(Func<double[], double> function, double[] vector, double[] result, double epsilon = SlurMath.ZeroTolerance)
        {
            double d2 = 1.0 / (epsilon * 2.0);

            for (int i = 0; i < vector.Length; i++)
            {
                double t = vector[i];

                vector[i] = t + epsilon;
                double g0 = function(vector);

                vector[i] = t - epsilon;
                double g1 = function(vector);

                result[i] = (g0 - g1) * d2;
                vector[i] = t;
            }
        }
    }
}
