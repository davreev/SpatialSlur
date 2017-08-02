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

        /// <summary></summary>
        public static Vec4d Zero = new Vec4d();
        /// <summary></summary>
        public static Vec4d UnitX = new Vec4d(1.0, 0.0, 0.0, 0.0);
        /// <summary></summary>
        public static Vec4d UnitY = new Vec4d(0.0, 1.0, 0.0, 0.0);
        /// <summary></summary>
        public static Vec4d UnitZ = new Vec4d(0.0, 0.0, 1.0, 0.0);


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
            v0.X += v1.X;
            v0.Y += v1.Y;
            v0.Z += v1.Z;
            v0.W += v1.W;
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
            v0.X -= v1.X;
            v0.Y -= v1.Y;
            v0.Z -= v1.Z;
            v0.W -= v1.W;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec4d operator -(Vec4d v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            v.W = -v.W;
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
            v.X *= t;
            v.Y *= t;
            v.Z *= t;
            v.W *= t;
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
            v.X *= t;
            v.Y *= t;
            v.Z *= t;
            v.W *= t;
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
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z + v0.W * v1.W;
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
            v.X *= t;
            v.Y *= t;
            v.Z *= t;
            v.W *= t;
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
            return new Vec4d(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y), Math.Max(v0.Z, v1.Z), Math.Max(v0.W, v1.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec4d Min(Vec4d v0, Vec4d v1)
        {
            return new Vec4d(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y), Math.Min(v0.Z, v1.Z), Math.Min(v0.W, v1.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec4d Abs(Vec4d v)
        {
            return new Vec4d(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z), Math.Abs(v.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec4d v0, Vec4d v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z + v0.W * v1.W;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec4d v0, Vec4d v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y) + Math.Abs(v0.Z * v1.Z) + Math.Abs(v0.W * v1.W);
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
            v0.X += (v1.X - v0.X) * t;
            v0.Y += (v1.Y - v0.Y) * t;
            v0.Z += (v1.Z - v0.Z) * t;
            v0.W += (v1.W - v0.W) * t;
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
            return new Vec4d(v.X, v.Y, 0.0, 0.0);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec4d(Vec3d v)
        {
            return new Vec4d(v.X, v.Y, v.Z, 0.0);
        }


        #endregion


        /// <summary></summary>
        public double X;
        /// <summary></summary>
        public double Y;
        /// <summary></summary>
        public double Z;
        /// <summary></summary>
        public double W;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyzw"></param>
        public Vec4d(double xyzw)
        {
            X = Y = Z = W = xyzw;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vec4d(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vec4d(Vec2d other, double z, double w)
        {
            X = other.X;
            Y = other.Y;
            this.Z = z;
            this.W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="w"></param>
        public Vec4d(Vec3d other, double w)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            this.W = w;
        }


        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vec4d Direction
        {
            get
            {
                double d = SquareLength;
                return (d > 0.0) ? this / Math.Sqrt(d) : Zero;
            }
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
            get { return X * X + Y * Y + Z * Z + W * W; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z) + Math.Abs(W); }
        }


        /// <summary>
        /// 
        /// </summary>
        public (double, double, double, double) Components
        {
            get { return (X, Y, Z, W); }
        }


        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return X + Y + Z + W; }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get { return (X + Y + Z + W) * 0.25; }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(X, Math.Max(Y, Math.Max(Z, W))); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(X, Math.Min(Y, Math.Min(Z, W))); }
        }



        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double tolerance)
        {
            return (Math.Abs(X) < tolerance) && (Math.Abs(Y) < tolerance) && (Math.Abs(Z) < tolerance) && (Math.Abs(W) < tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double tolerance)
        {
            return Math.Abs(SquareLength - 1.0) < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("({0},{1},{2},{3})", X, Y, Z, W);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyzw"></param>
        public void Set(double xyzw)
        {
            X = Y = Z = W = xyzw;
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
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec4d other, double tolerance)
        {
            return (Math.Abs(other.X - X) < tolerance) && (Math.Abs(other.Y - Y) < tolerance) && (Math.Abs(other.Z - Z) < tolerance) && (Math.Abs(other.W - W) < tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec4d other, Vec4d tolerance)
        {
            return (Math.Abs(other.X - X) < tolerance.X) && (Math.Abs(other.Y - Y) < tolerance.Y) && (Math.Abs(other.Z - Z) < tolerance.Z) && (Math.Abs(other.W - W) < tolerance.W);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec4d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
            other.W -= W;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec4d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
            other.W -= W;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec4d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
            other.W -= W;
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
                X *= d;
                Y *= d;
                Z *= d;
                W *= d;
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
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor,
                Z + (other.Z - Z) * factor,
                W + (other.W - W) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new double[] { X, Y, Z, W };
        }
    }
}
