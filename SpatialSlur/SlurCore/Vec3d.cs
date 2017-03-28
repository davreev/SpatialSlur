using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
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
    public struct Vec3d
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitX
        {
            get { return new Vec3d(1.0, 0.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitY
        {
            get { return new Vec3d(0.0, 1.0, 0.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d UnitZ
        {
            get { return new Vec3d(0.0, 0.0, 1.0); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator +(Vec3d v0, Vec3d v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
            v0.z += v1.z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d v0, Vec3d v1)
        {
            v0.x -= v1.x;
            v0.y -= v1.y;
            v0.z -= v1.z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d v)
        {
            v.x = -v.x;
            v.y = -v.y;
            v.z = -v.z;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator *(Vec3d v, double t)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator *(double t, Vec3d v)
        {
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double operator *(Vec3d v0, Vec3d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
        }


        /// <summary>
        /// Returns the cross product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator ^(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(
               v0.y * v1.z - v0.z * v1.y,
               v0.z * v1.x - v0.x * v1.z,
               v0.x * v1.y - v0.y * v1.x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d operator /(Vec3d v, double t)
        {
            t = 1.0 / t;
            v.x *= t;
            v.y *= t;
            v.z *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Max(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(Math.Max(v0.x, v1.x), Math.Max(v0.y, v1.y), Math.Max(v0.z, v1.z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Min(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(Math.Min(v0.x, v1.x), Math.Min(v0.y, v1.y), Math.Min(v0.z, v1.z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec3d Abs(Vec3d v)
        {
            return new Vec3d(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec3d v0, Vec3d v1)
        {
            return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec3d v0, Vec3d v1)
        {
            return Math.Abs(v0.x * v1.x) + Math.Abs(v0.y * v1.y) + Math.Abs(v0.z * v1.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Cross(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(
                v0.y * v1.z - v0.z * v1.y,
                v0.z * v1.x - v0.x * v1.z,
                v0.x * v1.y - v0.y * v1.x);
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// If either vector is zero length, Double.NaN is returned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec3d v0, Vec3d v1)
        {
            double d = v0.Length * v1.Length;

            if (d > 0.0)
                return Math.Acos(SlurMath.Clamp(v0 * v1 / d, -1.0, 1.0)); // clamp dot product to remove noise

            return double.NaN;
        }


        /// <summary>
        /// Returns the cotangent of the angle between 2 vectors as per http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Cotangent(Vec3d v0, Vec3d v1)
        {
            return v0 * v1 / Cross(v0, v1).Length;
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Project(Vec3d v0, Vec3d v1)
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
        public static Vec3d Reject(Vec3d v0, Vec3d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Reflect(Vec3d v0, Vec3d v1)
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
        public static Vec3d MatchProjection(Vec3d v0, Vec3d v1)
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
        public static Vec3d MatchProjection(Vec3d v0, Vec3d v1, Vec3d v2)
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
        public static Vec3d Lerp(Vec3d v0, Vec3d v1, double t)
        {
            v0.x += (v1.x - v0.x) * t;
            v0.y += (v1.y - v0.y) * t;
            v0.z += (v1.z - v0.z) * t;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double t)
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
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double theta, double t)
        {
            double st = 1.0 / Math.Sin(theta);
            return v0 * (Math.Sin((1.0 - t) * theta) * st) + v1 * (Math.Sin(t * theta) * st);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec3d(Vec2d v)
        {
            return new Vec3d(v.x, v.y, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec3d(Vec4d v)
        {
            return new Vec3d(v.x, v.y, v.z);
        }

        #endregion

        public double x, y, z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        public Vec3d(Vec2d other, double z)
        {
            x = other.x;
            y = other.y;
            this.z = z;
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
            get { return x * x + y * y + z * z; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y) + Math.Abs(z); }
        }


        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return x + y + z; }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get
            {
                const double inv3 = 1.0 / 3.0;
                return (x + y + z) * inv3;
            }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(x, Math.Max(y, z)); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(x, Math.Min(y, z)); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double epsilon)
        {
            return (Math.Abs(x) < epsilon) && (Math.Abs(y) < epsilon) && (Math.Abs(z) < epsilon);
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
            return String.Format("({0},{1},{2})", x, y, z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        /// <summary>
        /// Converts from euclidean to spherical coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToSpherical()
        {
            double r = this.Length;
            return new Vec3d(r, Math.Atan(y / x), Math.Acos(z / r));
        }


        /// <summary>
        /// Converts from spherical to euclidean coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToEuclidean()
        {
            double rxy = Math.Sin(z) * x * x;
            return new Vec3d(Math.Cos(y) * rxy, Math.Sin(y) * rxy, Math.Cos(z) * x);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec3d other, double epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon) && (Math.Abs(other.y - y) < epsilon) && (Math.Abs(other.z - z) < epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec3d other, Vec3d epsilon)
        {
            return (Math.Abs(other.x - x) < epsilon.x) && (Math.Abs(other.y - y) < epsilon.y) && (Math.Abs(other.z - z) < epsilon.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec3d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec3d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec3d other)
        {
            other.x -= x;
            other.y -= y;
            other.z -= z;
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
        public Vec3d LerpTo(Vec3d other, double factor)
        {
            return new Vec3d(
                x + (other.x - x) * factor,
                y + (other.y - y) * factor,
                z + (other.z - z) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { x, y, z };
        }
    }
}
