using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial struct Vec3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Vec3d Zero = new Vec3d();
        /// <summary></summary>
        public static readonly Vec3d UnitX = new Vec3d(1.0, 0.0, 0.0);
        /// <summary></summary>
        public static readonly Vec3d UnitY = new Vec3d(0.0, 1.0, 0.0);
        /// <summary></summary>
        public static readonly Vec3d UnitZ = new Vec3d(0.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vec3d vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec3d(Vec2d vector)
        {
            return new Vec3d(vector.X, vector.Y, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec3d(Vec4d vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator Vec3d(Vec3i vector)
        {
            return new Vec3d(vector.X, vector.Y, vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator +(Vec3d v0, Vec3d v1)
        {
            v0.X += v1.X;
            v0.Y += v1.Y;
            v0.Z += v1.Z;
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
            v0.X -= v1.X;
            v0.Y -= v1.Y;
            v0.Z -= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            vector.Z = -vector.Z;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3d operator *(Vec3d vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3d operator *(double scalar, Vec3d vector)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }


        /// <summary>
        /// Component-wise multiplication.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator *(Vec3d v0, Vec3d v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            v0.Z *= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3d operator /(Vec3d vector, double scalar)
        {
            scalar = 1.0 / scalar;
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec3d operator /(double scalar, Vec3d vector)
        {
            vector.X = scalar / vector.X;
            vector.Y = scalar / vector.Y;
            vector.Z = scalar / vector.Z;
            return vector;
        }


        /// <summary>
        /// Component-wise division.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d operator /(Vec3d v0, Vec3d v1)
        {
            v0.X /= v1.X;
            v0.Y /= v1.Y;
            v0.Z /= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Max(Vec3d v, double t)
        {
            return new Vec3d(Math.Max(v.X, t), Math.Max(v.Y, t), Math.Max(v.Z, t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Max(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y), Math.Max(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3d Min(Vec3d v, double t)
        {
            return new Vec3d(Math.Min(v.X, t), Math.Min(v.Y, t), Math.Min(v.Z, t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Min(Vec3d v0, Vec3d v1)
        {
            return new Vec3d(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y), Math.Min(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d Abs(Vec3d vector)
        {
            return new Vec3d(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec3d v0, Vec3d v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec3d v0, Vec3d v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y) + Math.Abs(v0.Z * v1.Z);
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
                v0.Y * v1.Z - v0.Z * v1.Y,
                v0.Z * v1.X - v0.X * v1.Z,
                v0.X * v1.Y - v0.Y * v1.X);
        }
        

        /// <summary>
        /// Returns the minimum angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec3d v0, Vec3d v1)
        {
            var d = v0.SquareLength * v1.SquareLength;
            return d > 0.0 ? SlurMath.AcosSafe(Dot(v0, v1) / Math.Sqrt(d)) : 0.0;
        }


        /// <summary>
        /// Returns the signed angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double SignedAngle(Vec3d v0, Vec3d v1, Vec3d up)
        {
            var c = Cross(v0, v1);
            return Math.Atan2(c.Length * Math.Sign(Dot(c, up)), Dot(v0, v1));
        }


        /// <summary>
        /// Returns the cotangent of the angle between 2 vectors as per http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Cotangent(Vec3d v0, Vec3d v1)
        {
            return Dot(v0, v1) / Cross(v0, v1).Length;
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d Project(Vec3d v0, Vec3d v1)
        {
            return Dot(v0, v1) / v1.SquareLength * v1;
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
            return v1 * (Dot(v0, v1) / v1.SquareLength * 2.0) - v0;
        }


        /// <summary>
        /// Returns a vector parallel to v0 whos projection onto v1 equals v1
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d MatchProjection(Vec3d v0, Vec3d v1)
        {
            return v1.SquareLength / Dot(v0, v1) * v0;
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
            return Dot(v1, v2) / Dot(v0, v2) * v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Vec3d Lerp(Vec3d v0, Vec3d v1, double factor)
        {
            return v0.LerpTo(v1, factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double factor)
        {
            return v0.SlerpTo(v1, Angle(v0, v1), factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="angle"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static Vec3d Slerp(Vec3d v0, Vec3d v1, double angle, double factor)
        {
            return v0.SlerpTo(v1, angle, factor);
        }

        #endregion


        /// <summary></summary>
        public double X;
        /// <summary></summary>
        public double Y;
        /// <summary></summary>
        public double Z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyz"></param>
        public Vec3d(double xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        public Vec3d(Vec2d other, double z)
        {
            X = other.X;
            Y = other.Y;
            Z = z;
        }


        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vec3d Unit
        {
            get
            {
                var v = this;
                return v.Unitize() ? v : Zero;
            }
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
            get { return X * X + Y * Y + Z * Z; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); }
        }
        

        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return X + Y + Z; }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get
            {
                const double inv3 = 1.0 / 3.0;
                return (X + Y + Z) * inv3;
            }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(X, Math.Max(Y, Z)); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(X, Math.Min(Y, Z)); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(double tolerance = SlurMath.ZeroTolerance)
        {
            return SquareLength < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double tolerance = SlurMath.ZeroTolerance)
        {
            return SlurMath.ApproxEquals(SquareLength, 1.0, tolerance);
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyz"></param>
        public void Set(double xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// Converts from euclidean to spherical coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToSpherical()
        {
            double r = Length;
            return new Vec3d(r, Math.Atan(Y / X), SlurMath.AcosSafe(Z / r));
        }


        /// <summary>
        /// Converts from spherical to euclidean coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vec3d ToEuclidean()
        {
            double rxy = Math.Sin(Z) * X * X;
            return new Vec3d(Math.Cos(Y) * rxy, Math.Sin(Y) * rxy, Math.Cos(Z) * X);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec3d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, tolerance) &&
                SlurMath.ApproxEquals(Y, other.Y, tolerance) &&
                SlurMath.ApproxEquals(Z, other.Z, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec3d other, Vec3d tolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, tolerance.X) &&
                SlurMath.ApproxEquals(Y, other.Y, tolerance.Y) &&
                SlurMath.ApproxEquals(Z, other.Z, tolerance.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec3d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec3d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec3d other)
        {
            other.X -= X;
            other.Y -= Y;
            other.Z -= Z;
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
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
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
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor,
                Z + (other.Z - Z) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec3d SlerpTo(Vec3d other, double factor)
        {
            return SlerpTo(other, Angle(this, other), factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="angle"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec3d SlerpTo(Vec3d other, double angle, double factor)
        {
            var sa = Math.Sin(angle);

            // handle aligned cases
            if (sa > 0.0)
            {
                var saInv = 1.0 / sa;
                var af = angle * factor;
                return this * Math.Sin(angle - af) * saInv + other * Math.Sin(af) * saInv;
            }

            return this;
        }


        /// <summary>
        /// Returns the cross product of this vector vector with the x Axis
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public Vec3d CrossX()
        {
            return new Vec3d(0.0, Z, -Y);
        }


        /// <summary>
        /// Returns the cross product of this vector with the Y Axis
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public Vec3d CrossY()
        {
            return new Vec3d(-Z, 0.0, X);
        }


        /// <summary>
        /// Returns the cross product of this vector with the Y Axis
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public Vec3d CrossZ()
        {
            return new Vec3d(Y, -X, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[3];
            ToArray(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void ToArray(double[] result)
        {
            result[0] = X;
            result[1] = Y;
            result[2] = Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
