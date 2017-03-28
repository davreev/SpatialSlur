using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public struct Vec4d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec4d UnitX
        {
            get { return new Vec4d(1.0, 0.0, 0.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec4d UnitY
        {
            get { return new Vec4d(0.0, 1.0, 0.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec4d UnitZ
        {
            get { return new Vec4d(0.0, 0.0, 1.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec4d UnitW
        {
            get { return new Vec4d(0.0, 0.0, 0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d operator +(Vec4d v0, Vec4d v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
            v0.z += v1.z;
            v0.w += v1.w;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d operator -(Vec4d v0, Vec4d v1)
        {
            v0.x -= v1.x;
            v0.y -= v1.y;
            v0.z -= v1.z;
            v0.w -= v1.w;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec4d operator -(Vec4d v)
        {
            v.x = -v.x;
            v.y = -v.y;
            v.z = -v.z;
            v.w = -v.w;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec4d operator *(Vec4d v, double t)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            v.w *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec4d operator *(double t, Vec4d v)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            v.w *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator *(Vec4d v0, Vec4d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z + v0.w * v1.w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec4d operator /(Vec4d v, double t)
        {
            t = 1.0 / t;
            v.x *= t;
            v.y *= t;
            v.z *= t;
            v.w *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d Max(Vec4d v0, Vec4d v1)
        {
            return new Vec4d(Math.Max(v0.x, v1.x), Math.Max(v0.y, v1.y), Math.Max(v0.z, v1.z), Math.Max(v0.w, v1.w));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d Min(Vec4d v0, Vec4d v1)
        {
            return new Vec4d(Math.Min(v0.x, v1.x), Math.Min(v0.y, v1.y), Math.Min(v0.z, v1.z), Math.Min(v0.w, v1.w));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec4d Abs(Vec4d v)
        {
            return new Vec4d(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z), Math.Abs(v.w));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec4d v0, Vec4d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z + v0.w * v1.w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec4d v0, Vec4d v1)
        {
            return Math.Abs(v0.x * v1.x) + Math.Abs(v0.y * v1.y) + Math.Abs(v0.z * v1.z) + Math.Abs(v0.w * v1.w);
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// If either vector is zero length, Double.NaN is returned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec4d v0, Vec4d v1)
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
        public static Vec4d Project(Vec4d v0, Vec4d v1)
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
        public static Vec4d Reject(Vec4d v0, Vec4d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d Reflect(Vec4d v0, Vec4d v1)
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
        public static Vec4d MatchProjection(Vec4d v0, Vec4d v1)
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
        public static Vec4d MatchProjection(Vec4d v0, Vec4d v1, Vec4d v2)
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
        public static Vec4d Lerp(Vec4d v0, Vec4d v1, double t)
        {
            v0.x += (v1.x - v0.x) * t;
            v0.y += (v1.y - v0.y) * t;
            v0.z += (v1.z - v0.z) * t;
            v0.w += (v1.w - v0.w) * t;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec4d Slerp(Vec4d v0, Vec4d v1, double t)
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
        public static Vec4d Slerp(Vec4d v0, Vec4d v1, double theta, double t)
        {
            double st = 1.0 / Math.Sin(theta);
            return v0 * (Math.Sin((1.0 - t) * theta) * st) + v1 * (Math.Sin(t * theta) * st);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec4d(Vec2d v)
        {
            return new Vec4d(v.x, v.y, 0.0, 0.0);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec4d(Vec3d v)
        {
            return new Vec4d(v.x, v.y, v.z, 0.0);
        }


        #endregion

        public double x, y, z, w;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vec4d(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vec4d(Vec2d other, double z, double w)
        {
            x = other.x;
            y = other.y;
            this.z = z;
            this.w = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="w"></param>
        public Vec4d(Vec3d other, double w)
        {
            x = other.x;
            y = other.y;
            z = other.z;
            this.w = w;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Length
        {
            get { return Math.Sqrt(SquareLength); }
        }


        /// <summary>
        /// 
        /// </summary>
        public double SquareLength
        {
            get { return x * x + y * y + z * z + w * w; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y) + Math.Abs(z) + Math.Abs(w); }
        }


        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return x + y + z + w; }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get { return (x + y + z + w) * 0.25; }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(x, Math.Max(y, Math.Max(z, w))); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(x, Math.Min(y, Math.Min(z, w))); }
        }



        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double epsilon)
        {
            return (Math.Abs(x) < epsilon) && (Math.Abs(y) < epsilon) && (Math.Abs(z) < epsilon) && (Math.Abs(w) < epsilon);
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
            return String.Format("({0},{1},{2},{3})", x, y, z, w);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public void Set(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec4d other, double epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon) && (Math.Abs(other.y - y) < epsilon) && (Math.Abs(other.z - z) < epsilon) && (Math.Abs(other.w - w) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec4d other, Vec4d epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon.x) && (Math.Abs(other.y - y) < epsilon.y) && (Math.Abs(other.z - z) < epsilon.z) && (Math.Abs(other.w - w) < epsilon.w);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec4d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
            other.w -= w;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec4d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
            other.w -= w;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec4d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
            other.w -= w;
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
                z *= d;
                w *= d;
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
        public Vec4d LerpTo(Vec4d other, double factor)
        {
            return new Vec4d(
                x + (other.x - x) * factor,
                y + (other.y - y) * factor,
                z + (other.z - z) * factor,
                w + (other.w - w) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { x, y, z, w };
        }
    }
}
