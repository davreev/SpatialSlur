using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Vec2d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec2d UnitX
        {
            get { return new Vec2d(1.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d UnitY
        {
            get { return new Vec2d(0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator +(Vec2d v0, Vec2d v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator -(Vec2d v0, Vec2d v1)
        {
            v0.x -= v1.x;
            v0.y -= v1.y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec2d operator -(Vec2d v)
        {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator *(Vec2d v, double t)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator *(double t, Vec2d v)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator *(Vec2d v0, Vec2d v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator ^(Vec2d v0, Vec2d v1)
        {
            return -v0.y * v1.x + v0.x * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d operator /(Vec2d v, double t)
        {
            t = 1.0 / t;
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Max(Vec2d v0, Vec2d v1)
        {
            return new Vec2d(Math.Max(v0.x, v1.x), Math.Max(v0.y, v1.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Min(Vec2d v0, Vec2d v1)
        {
            return new Vec2d(Math.Min(v0.x, v1.x), Math.Min(v0.y, v1.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec2d Abs(Vec2d v)
        {
            return new Vec2d(Math.Abs(v.x), Math.Abs(v.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec2d v0, Vec2d v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec2d v0, Vec2d v1)
        {
            return Math.Abs(v0.x * v1.x) + Math.Abs(v0.y * v1.y);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Cross(Vec2d v0, Vec2d v1)
        {
            return -v0.y * v1.x + v0.x * v1.y;
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// If either vector is zero length, Double.NaN is returned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec2d v0, Vec2d v1)
        {
            double d = v0.Length * v1.Length;

            if (d > 0.0)
                return Math.Acos(SlurMath.Clamp(v0 * v1 / d, -1.0, 1.0)); // clamp dot product to remove noise

            return double.NaN;
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Project(Vec2d v0, Vec2d v1)
        {
            return (v0 * v1) / v1.SquareLength * v1;
        }


        /// <summary>
        /// Returns the rejection of v0 onto v1.
        /// This is the perpendicular component of v0 with respect to v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Reject(Vec2d v0, Vec2d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Reflect(Vec2d v0, Vec2d v1)
        {
            //return Project(v0, v1) * 2.0 - v0;
            return v1 * ((v0 * v1) / v1.SquareLength * 2.0) - v0;
        }

 
        /// <summary>
        /// Returns a vector parallel to v0 whos projection onto v1 equals v1
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d MatchProjection(Vec2d v0, Vec2d v1)
        {
            return v1.SquareLength / (v0 * v1) * v0;
        }


        /// <summary>
        /// Returns a vector parallel to v0 whose projection onto v2 equals the projection of v1 onto v2
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vec2d MatchProjection(Vec2d v0, Vec2d v1, Vec2d v2)
        {
            return (v1 * v2) / (v0 * v2) * v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Lerp(Vec2d v0, Vec2d v1, double t)
        {
            v0.x += (v1.x - v0.x) * t;
            v0.y += (v1.y - v0.y) * t;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Slerp(Vec2d v0, Vec2d v1, double t)
        {
            return Slerp(v0, v1, Angle(v0, v1), t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="theta"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Slerp(Vec2d v0, Vec2d v1, double theta, double t)
        {
            double st = 1.0 / Math.Sin(theta);
            return v0 * (Math.Sin((1.0 - t) * theta) * st) + v1 * (Math.Sin(t * theta) * st);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec2d(Vec3d v)
        {
            return new Vec2d(v.x, v.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec2d(Vec4d v)
        {
            return new Vec2d(v.x, v.y);
        }

        #endregion


        public double x, y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn clockwise.
        /// </summary>
        public Vec2d PerpCW
        {
            get { return new Vec2d(y, -x); }
        }

        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vec2d PerpCCW
        {
            get { return new Vec2d(-y, x); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Length
        {
            get { return Math.Sqrt(SquareLength); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double SquareLength
        {
            get { return x * x + y * y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y); }
        }



        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return x + y; }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(x, y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(x, y); }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get
            {
                const double inv3 = 1.0 / 3.0;
                return (x + y) * inv3;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double epsilon)
        {
            return (Math.Abs(x) < epsilon) && (Math.Abs(y) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double epsilon)
        {
            return Math.Abs(SquareLength - 1.0) < epsilon;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0},{1})", x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Converts from euclidean to polar coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToPolar()
        {
            return new Vec2d(Length, Math.Atan(y / x));
        }


        /// <summary>
        /// Converts from polar to euclidean coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToEuclidean()
        {
            return new Vec2d(Math.Cos(y) * x, Math.Sin(y) * x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec2d other, double epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon) && (Math.Abs(other.y - y) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec2d other, Vec2d epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon.x) && (Math.Abs(other.y - y) < epsilon.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec2d other)
        {
            other.x -= x;
            other.y -= y;
            return other.ManhattanLength;
        }


        /// <summary>
        /// Unitizes the vector in place.
        /// Returns false if the vector is zero length.
        /// </summary>
        public bool Unitize()
        {
            double d = SquareLength;

            if (d > 0.0)
            {
                d = 1.0 / Math.Sqrt(d);
                x *= d;
                y *= d;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec2d LerpTo(Vec2d other, double factor)
        {
            return new Vec2d(
                x + (other.x - x) * factor,
                y + (other.y - y) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { x, y };
        }
    }
}
