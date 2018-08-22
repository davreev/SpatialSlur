
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using D = SpatialSlur.SlurMath.Constantsd;
using F = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static class Geometry
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
        public static bool LineLineClosestPoints(Vector3d startA, Vector3d endA, Vector3d startB, Vector3d endB, out double ta, out double tb)
        {
            return LineLineClosestPoints(endA - startA, endB - startB, startA - startB, out ta, out tb);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="directionA"></param>
        /// <param name="startB"></param>
        /// <param name="directionB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static bool LineLineClosestPoints2(Vector3d startA, Vector3d directionA, Vector3d startB, Vector3d directionB, out double ta, out double tb)
        {
            return LineLineClosestPoints(directionA, directionB, startA - startB, out ta, out tb);
        }


        /// <summary>
        /// Returns the shortest vector from line a to line b.
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="endA"></param>
        /// <param name="startB"></param>
        /// <param name="endB"></param>
        /// <returns></returns>
        public static Vector3d LineLineShortestVector(Vector3d startA, Vector3d endA, Vector3d startB, Vector3d endB)
        {
            Vector3d u = endA - startA;
            Vector3d v = endB - startB;
            Vector3d w = startA - startB;

            LineLineClosestPoints(u, v, w, out double tu, out double tv);
            return v * tv - u * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="directionA"></param>
        /// <param name="startB"></param>
        /// <param name="directionB"></param>
        /// <returns></returns>
        public static Vector3d LineLineShortestVector2(Vector3d startA, Vector3d directionA, Vector3d startB, Vector3d directionB)
        {
            Vector3d w = startA - startB;

            LineLineClosestPoints(directionA, directionB, w, out double tu, out double tv);
            return directionB * tv - directionA * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        private static bool LineLineClosestPoints(Vector3d u, Vector3d v, Vector3d w, out double tu, out double tv)
        {
            // impl ref
            // http://geomalgorithms.com/a07-_distance.html

            return (SolveSymmetric(u.SquareLength, -Vector3d.Dot(u, v), v.SquareLength, -Vector3d.Dot(w, u), Vector3d.Dot(w, v), out tu, out tv));
        }


        /// <summary>
        /// Solves the given symmetric 2x2 system via Cramer's rule
        /// </summary>
        private static bool SolveSymmetric(double a00, double a01, double a11, double b0, double b1, out double x0, out double x1)
        {
            var detA = a00 * a11 - a01 * a01;

            if (Math.Abs(detA) > 0.0)
            {
                detA = 1.0 / detA;
                x0 = (a11 * b0 - a01 * b1) * detA;
                x1 = (a00 * b1 - a01 * b0) * detA;
                return true;
            }

            // no unique solution
            x0 = x1 = 0.0;
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
        public static bool LineLineIntersection(Vector2d startA, Vector2d endA, Vector2d startB, Vector2d endB, out double ta, out double tb)
        {
            return LineLineIntersection(endA - startA, endB - startB, startB - startA, out ta, out tb);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startA"></param>
        /// <param name="directionA"></param>
        /// <param name="startB"></param>
        /// <param name="directionB"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static bool LineLineIntersection2(Vector2d startA, Vector2d directionA, Vector2d startB, Vector2d directionB, out double ta, out double tb)
        {
            return LineLineIntersection(directionA, directionB, startB - startA, out ta, out tb);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="tu"></param>
        /// <param name="tv"></param>
        private static bool LineLineIntersection(Vector2d u, Vector2d v, Vector2d w, out double tu, out double tv)
        {
            // impl ref
            // https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
            
            var c = Vector2d.Cross(u, v);

            if (Math.Abs(c) > 0.0)
            {
                c = 1.0 / c;
                tu = Vector2d.Cross(w, v) * c;
                tv = Vector2d.Cross(w, u) * c;
                return true;
            }

            // lines are parallel, no solution
            tu = tv = 0.0;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <returns></returns>
        public static bool LineSphereIntersect(Vector3d start, Vector3d direction, Vector3d center, float radius, out Vector3d near, out Vector3d far)
        {
            var a = center - start;
            var b = Vector3d.Project(a, direction);
            var c = b - a;

            var cc = c.SquareLength;
            var rr = radius * radius;

            // intersections exist
            if (cc < rr)
            {
                var p = start + b;
                var dp = b * (Math.Sqrt(rr - cc) / b.Length);
                near = p - dp;
                far = p + dp;
                return true;
            }

            // no intersections, return closest point on sphere
            near = far = center + c * (radius / Math.Sqrt(cc));
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="originA"></param>
        /// <param name="normalA"></param>
        /// <param name="originB"></param>
        /// <param name="normalB"></param>
        /// <param name="point"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool PlanePlaneIntersect(Vector3d originA, Vector3d normalA, Vector3d originB, Vector3d normalB, out Vector3d point, out Vector3d axis)
        {
            axis = Vector3d.Cross(normalA, normalB);

            if(axis.SquareLength > 0.0)
            {
                var d = Vector3d.Reject(normalB, normalA);
                point = ProjectToPlaneAlong(originA, originB, normalB, d);
                return true;
            }

            // normals are parallel, no unique solution
            point = Vector3d.Zero;
            return false;
        }


        /// <summary>
        /// Returns 0 if the given points are coincident, 1 if they're colinear, 2 if they're coplanar, and 3 otherwise.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static int PrincipalComponentAnalysis(IEnumerable<Vector3d> points, out Vector3d origin, out Vector3d xAxis, out Vector3d yAxis, double tolerance = D.ZeroTolerance)
        {
            origin = points.Mean();
            var covm = Matrix3d.CreateCovariance(points, origin);

            Matrix3d.Decompose.EigenSymmetric(ref covm, out Matrix3d vecs, out Vector3d vals, tolerance);
            xAxis = vecs.Column0;
            yAxis = vecs.Column1;

            return
                Math.Abs(vals.X) < tolerance ? 0 :
                Math.Abs(vals.Y) < tolerance ? 1 :
                Math.Abs(vals.Z) < tolerance ? 2 : 3;
        }

        
        /// <summary>
        /// Returns true if a unique plane was found i.e. the given points are not coincident or colinear.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FitPlaneToPoints(IEnumerable<Vector3d> points, out Vector3d origin, out Vector3d normal, double tolerance = D.ZeroTolerance)
        {
            // impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
            // http://www.ilikebigbits.com/blog/2017/9/24/fitting-a-plane-to-noisy-points-in-3d

            origin = points.Mean();
            return FitPlaneToPoints(points, origin, out normal, tolerance);
        }


        /// <summary>
        /// Returns true if a unique plane was found i.e. the given points are not coincident or colinear.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FitPlaneToPoints(IEnumerable<Vector3d> points, Vector3d origin, out Vector3d normal, double tolerance = D.ZeroTolerance)
        {
            // impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
            // http://www.ilikebigbits.com/blog/2017/9/24/fitting-a-plane-to-noisy-points-in-3d

            var covm = Matrix3d.CreateCovariance(points, origin);

            Matrix3d.Decompose.EigenSymmetric(ref covm, out Matrix3d vecs, out Vector3d vals, tolerance);
            normal = vecs.Column2;

            // Check for degeneracy -> if 2nd eigenvalue is 0, the points are colinear at best
            return Math.Abs(vals.Y) >= tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FitLineToPoints(IEnumerable<Vector3d> points, out Vector3d start, out Vector3d direction, double tolerance = D.ZeroTolerance)
        {
            // impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf

            start = points.Mean();
            return FitLineToPoints(points, start, out direction, tolerance);
        }


        /// <summary>
        /// Returns true if a unique plane was found i.e. the given points are not coincident or colinear.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool FitLineToPoints(IEnumerable<Vector3d> points, Vector3d start, out Vector3d direction, double tolerance = D.ZeroTolerance)
        {
            // impl refs
            // https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf

            var covm = Matrix3d.CreateCovariance(points, start);

            Matrix3d.Decompose.EigenSymmetric(ref covm, out Matrix3d vecs, out Vector3d vals, tolerance);
            direction = vecs.Column0;

            // Check for degeneracy -> if 1st eigenvalue is 0, then points are coincident
            return Math.Abs(vals.X) >= tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool FitSphereToPoints(IEnumerable<Vector3d> points, out Vector3d origin, out double radius)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool FitCircleToPoints(IEnumerable<Vector2d> points, out Vector2d origin, out double radius)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool FitCircleToPoints(IEnumerable<Vector3d> points, out Vector3d origin, out Vector3d normal, out double radius)
        {
            // TODO
            throw new NotImplementedException();

            // fit plane followed by 2d fit circle
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3d ReflectInPlane(Vector3d point, Vector3d origin, Vector3d normal)
        {
            return point + Vector3d.Project(origin - point, normal) * 2.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3d ProjectToPlane(Vector3d point, Vector3d origin, Vector3d normal)
        {
            return point + Vector3d.Project(origin - point, normal);
        }


        /// <summary>
        /// Projects a vector to the given plane along the given direction.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3d ProjectToPlaneAlong(Vector3d vector, Vector3d normal, Vector3d direction)
        {
            return vector - Vector3d.MatchProjection(direction, vector, normal);
        }


        /// <summary>
        /// Projects a point to the given plane along the given direction.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3d ProjectToPlaneAlong(Vector3d point, Vector3d origin, Vector3d normal, Vector3d direction)
        {
            return point + Vector3d.MatchProjection(direction, origin - point, normal);
        }
    

        /// <summary>
        /// Returns false if the 2 given vectors are parallel.
        /// The direction of the first vector is maintained.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static bool Orthonormalize(ref Vector3d x, ref Vector3d y, out Vector3d z)
        {
            z = Vector3d.Cross(x, y);
            double m = z.SquareLength;

            if (m > 0.0)
            {
                x /= x.Length;
                z /= Math.Sqrt(m);
                y = Vector3d.Cross(z, x);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static double GetPolygonArea(IEnumerable<Vector2d> points)
        {
            var itr = points.GetEnumerator();
            itr.MoveNext();

            var first = itr.Current;

            var p0 = first;
            var sum = 0.0;
            
            while(itr.MoveNext())
            {
                var p1 = itr.Current;
                sum += Vector2d.Cross(p0, p1);
                p0 = p1;
            }

            sum += Vector2d.Cross(p0, first);
            return sum * 0.5;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="unitNormal"></param>
        /// <returns></returns>
        public static double GetPolygonArea(IEnumerable<Vector3d> points, Vector3d unitNormal)
        {
            var itr = points.GetEnumerator();
            itr.MoveNext();

            var first = itr.Current;

            var p0 = first;
            var sum = Vector3d.Zero;

            while (itr.MoveNext())
            {
                var p1 = itr.Current;
                sum += Vector3d.Cross(p0, p1);
                p0 = p1;
            }

            sum += Vector3d.Cross(p0, first);
            return Vector3d.Dot(sum, unitNormal) * 0.5;
        }


        /// <summary>
        /// Returns the area of the planar quad defined by the given points.
        /// If the given points are not co-planar, this method returns the area of the quad when projected onto the plane defined by the cross product of its two diagonals.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetPlanarQuadArea(Vector3d p0, Vector3d p1, Vector3d p2, Vector3d p3)
        {
            return Vector3d.Cross(p2 - p0, p3 - p1).Length * 0.5;
        }


        /// <summary>
        /// Returns the center of the circle that passes through the 3 given points.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3d GetCurvatureCenter(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            return p1 + GetCurvatureVector(p0 - p1, p2 - p1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3d GetCurvatureVector(Vector3d v0, Vector3d v1)
        {
            // impl ref
            // http://www.block.arch.ethz.ch/brg/files/2013-ijss-vanmele-shaping-tension-structures-with-actively-bent-linear-elements_1386929572.pdf

            Vector3d v2 = Vector3d.Cross(v0, v1);
            return Vector3d.Cross((v0.SquareLength * v1 - v1.SquareLength * v0), v2) / (2.0 * v2.SquareLength);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3d GetCircumcenter(Vector3d p0, Vector3d p1, Vector3d p2)
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
        public static Vector3d GetIncenter(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);
            double perimInv = 1.0 / (d01 + d12 + d20); // inverse perimeter
            return p0 * (d12 * perimInv) + p1 * (d20 * perimInv) + p2 * (d01 * perimInv);
        }


        /// <summary>
        /// Returns the angle of the vector in the given basis.
        /// Assumes the given axes are orthonormal.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <returns></returns>
        public static double GetPolarAngle(Vector3d vector, Vector3d xAxis, Vector3d yAxis)
        {
            return Math.Atan2(Vector3d.Dot(vector, yAxis), Vector3d.Dot(vector, xAxis));
        }

        
        /// <summary>
        /// Returns the signed minimum difference between the two angles.
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <returns></returns>
        public static double GetMinAngleDifference(double a0, double a1)
        {
            var d0 = SlurMath.Repeat(a0 - a1, D.TwoPi);
            return d0 > D.Pi ? d0 - D.TwoPi : d0;
        }


        /// <summary>
        /// Returns the barycentric coordinates for the given point with respect to triangle a, b, c
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetBarycentric(Vector3d point, Vector3d a, Vector3d b, Vector3d c, out Vector3d result)
        {
            // impl ref
            // http://realtimecollisiondetection.net/

            var u = b - a;
            var v = c - a;
            var w = point - a;

            if(SolveSymmetric(u.SquareLength, Vector3d.Dot(u,v), v.SquareLength, Vector3d.Dot(w,u), Vector3d.Dot(w,v), out result.Y, out result.Z))
            {
                result.X = 1.0 - result.Y - result.Z;
                return true;
            }

            // no unique solution
            result = Vector3d.Zero;
            return false;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsInTriangle(Vector3d point, Vector3d a, Vector3d b, Vector3d c)
        {
            GetBarycentric(point, a, b, c, out Vector3d w);
            return w.ComponentMin < 0.0;
        }


        /// <summary>
        /// Returns the gradient of the vertex values of the given triangle
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static Vector3d GetGradient(Vector3d p0, Vector3d p1, Vector3d p2, double t0, double t1, double t2)
        {
            // impl ref
            // https://www.cs.cmu.edu/~kmcrane/Projects/HeatMethod/paper.pdf

            var d0 = p2 - p1;
            var d1 = p0 - p2;
            var d2 = p1 - p0;

            var n = Vector3d.Cross(d0, d1);
            var a = 1.0 / n.Length;
            n *= a;

            return (t0 * Vector3d.Cross(n, d0) + t1 * Vector3d.Cross(n, d1) + t2 * Vector3d.Cross(n, d2)) * a * 0.5;
        }


        /// <summary>
        /// Returns the area gradient of the given trianglue with respect to p0.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3d GetAreaGradient(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            // impl ref
            // http://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleMeshDerivativesCheatSheet.pdf

            var d = p2 - p1;
            var n = Vector3d.Cross(d, p0 - p1);

            n.Unitize();
            return Vector3d.Cross(n, d) * 0.5;
        }


        /// <summary>
        /// Returns the area gradient of the given triangle with respect to each vertex
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="g0"></param>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        public static void GetAreaGradients(Vector3d p0, Vector3d p1, Vector3d p2, out Vector3d g0, out Vector3d g1, out Vector3d g2)
        {
            // impl ref
            // http://www.cs.cmu.edu/~kmcrane/Projects/Other/TriangleMeshDerivativesCheatSheet.pdf

            var d0 = p1 - p0;
            var d1 = p2 - p1;
            var d2 = p0 - p2;

            var n = Vector3d.Cross(d0, d1);
            n.Unitize();

            g0 = Vector3d.Cross(n, d1) * 0.5;
            g1 = Vector3d.Cross(n, d2) * 0.5;
            g2 = -(g0 + g1);
        }


        /// <summary>
        /// Returns the aspect ratio of the triangle defined by 3 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetAspect(Vector3d p0, Vector3d p1, Vector3d p2)
        {
            double maxEdge = 0.0; // longest edge
            double minAlt = Double.PositiveInfinity; // shortest altitude

            Vector3d v0 = p1 - p0;
            Vector3d v1 = p2 - p1;
            Vector3d v2 = p0 - p2;

            Sub(v0, v1);
            Sub(v1, v2);
            Sub(v2, v0);

            return Math.Sqrt(maxEdge) / Math.Sqrt(minAlt);

            void Sub(Vector3d a, Vector3d b)
            {
                maxEdge = Math.Max(maxEdge, a.SquareLength);
                minAlt = Math.Min(minAlt, Vector3d.Reject(b, a).SquareLength);
            }
        }


        /// <summary>
        /// Returns the aspect ratio of the tetrahedra defined by 4 given points.
        /// This is defined as the longest edge / shortest altitude.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetAspect(Vector3d p0, Vector3d p1, Vector3d p2, Vector3d p3)
        {
            double minEdge = 0.0;
            double maxAlt = double.PositiveInfinity;

            Vector3d v0 = p1 - p0;
            Vector3d v1 = p2 - p1;
            Vector3d v2 = p3 - p2;
            Vector3d v3 = p0 - p3;

            Sub(v0, v1, v2);
            Sub(v1, v2, v3);
            Sub(v2, v3, v0);
            Sub(v3, v0, v1);

            return Math.Sqrt(minEdge) / Math.Sqrt(maxAlt);

            void Sub(Vector3d a, Vector3d b, Vector3d c)
            {
                minEdge = Math.Max(minEdge, a.SquareLength);
                maxAlt = Math.Min(maxAlt, Vector3d.Project(c, Vector3d.Cross(a, b)).SquareLength);
            }
        }


        /// <summary>
        /// Returns the signed angle between the left and right normals.
        /// </summary>
        /// <param name="unitAxis"></param>
        /// <param name="leftNormal"></param>
        /// <param name="rightNormal"></param>
        /// <returns></returns>
        public static double GetDihedralAngle(Vector3d unitAxis, Vector3d leftNormal, Vector3d rightNormal)
        {
            // impl ref
            // http://brickisland.net/DDGFall2017/2017/10/12/assignment-1-coding-investigating-curvature/

            return
                Math.Atan2(
                Vector3d.Dot(unitAxis, Vector3d.Cross(leftNormal, rightNormal)),
                Vector3d.Dot(leftNormal, rightNormal));
        }


        /// <summary>
        /// Calculates the planarity of the given quad as the shortest distance between the 2 diagonals over the mean diagonal length.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static double GetPlanarity(Vector3d p0, Vector3d p1, Vector3d p2, Vector3d p3)
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
        public static Vector2d GetGradient(Func<Vector2d, double> function, Vector2d vector, double epsilon = D.ZeroTolerance)
        {
            (var x, var y) = vector;

            double gx = function(new Vector2d(x + epsilon, y)) - function(new Vector2d(x - epsilon, y));
            double gy = function(new Vector2d(x, y + epsilon)) - function(new Vector2d(x, y - epsilon));

            return new Vector2d(gx, gy) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static Vector3d GetGradient(Func<Vector3d, double> function, Vector3d vector, double epsilon = D.ZeroTolerance)
        {
            (var x, var y, var z) = vector;

            double gx = function(new Vector3d(x + epsilon, y, z)) - function(new Vector3d(x - epsilon, y, z));
            double gy = function(new Vector3d(x, y + epsilon, z)) - function(new Vector3d(x, y - epsilon, z));
            double gz = function(new Vector3d(x, y, z + epsilon)) - function(new Vector3d(x, y, z - epsilon));

            return new Vector3d(gx, gy, gz) / (2.0 * epsilon);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="vector"></param>
        /// <param name="result"></param>
        /// <param name="epsilon"></param>
        public static void GetGradient(Func<double[], double> function, double[] vector, double[] result, double epsilon = D.ZeroTolerance)
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
