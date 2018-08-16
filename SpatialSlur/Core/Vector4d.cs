
/*
 * Notes
 */

using System;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial struct Vector4d
    {
        #region Static members

        /// <summary></summary>
        public static Vector4d Zero = new Vector4d();
        /// <summary></summary>
        public static Vector4d UnitX = new Vector4d(1.0, 0.0, 0.0, 0.0);
        /// <summary></summary>
        public static Vector4d UnitY = new Vector4d(0.0, 1.0, 0.0, 0.0);
        /// <summary></summary>
        public static Vector4d UnitZ = new Vector4d(0.0, 0.0, 1.0, 0.0);
        /// <summary></summary>
        public static Vector4d UnitW = new Vector4d(0.0, 0.0, 0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vector4d vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static implicit operator Vector4d(Quaterniond quaternion)
        {
            return new Vector4d(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d operator +(Vector4d v0, Vector4d v1)
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
        public static Vector4d operator -(Vector4d v0, Vector4d v1)
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
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector4d operator -(Vector4d vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            vector.Z = -vector.Z;
            vector.W = -vector.W;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector4d operator *(Vector4d vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            vector.W *= scalar;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector4d operator *(double scalar, Vector4d vector)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            vector.W *= scalar;
            return vector;
        }


        /// <summary>
        /// Component-wise multiplication.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d operator *(Vector4d v0, Vector4d v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            v0.Z *= v1.Z;
            v0.W *= v1.W;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector4d operator /(Vector4d vector, double scalar)
        {
            scalar = 1.0 / scalar;
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            vector.W *= scalar;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector4d operator /(double scalar, Vector4d vector)
        {
            vector.X = scalar / vector.X;
            vector.Y = scalar / vector.Y;
            vector.Z = scalar / vector.Z;
            vector.W = scalar / vector.W;
            return vector;
        }



        /// <summary>
        /// Component-wise division.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d operator /(Vector4d v0, Vector4d v1)
        {
            v0.X /= v1.X;
            v0.Y /= v1.Y;
            v0.Z /= v1.Z;
            v0.W /= v1.W;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector4d Max(Vector4d vector, double value)
        {
            return new Vector4d(
                Math.Max(vector.X, value), 
                Math.Max(vector.Y, value), 
                Math.Max(vector.Z, value), 
                Math.Max(vector.W, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d Max(Vector4d v0, Vector4d v1)
        {
            return new Vector4d(
                Math.Max(v0.X, v1.X), 
                Math.Max(v0.Y, v1.Y), 
                Math.Max(v0.Z, v1.Z), 
                Math.Max(v0.W, v1.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d Min(Vector4d v0, Vector4d v1)
        {
            return new Vector4d(
                Math.Min(v0.X, v1.X), 
                Math.Min(v0.Y, v1.Y), 
                Math.Min(v0.Z, v1.Z),
                Math.Min(v0.W, v1.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector4d Min(Vector4d vector, double value)
        {
            return new Vector4d(
                Math.Min(vector.X, value), 
                Math.Min(vector.Y, value), 
                Math.Min(vector.Z, value), 
                Math.Min(vector.W, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector4d Abs(Vector4d vector)
        {
            return new Vector4d(
                Math.Abs(vector.X), 
                Math.Abs(vector.Y), 
                Math.Abs(vector.Z), 
                Math.Abs(vector.W));
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector4d Floor(Vector4d vector)
        {
            return new Vector4d(
               Math.Floor(vector.X),
               Math.Floor(vector.Y),
               Math.Floor(vector.Z),
               Math.Floor(vector.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector4d Ceiling(Vector4d vector)
        {
            return new Vector4d(
               Math.Ceiling(vector.X),
               Math.Ceiling(vector.Y),
               Math.Ceiling(vector.Z),
               Math.Ceiling(vector.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector4d Round(Vector4d vector)
        {
            return new Vector4d(
               Math.Round(vector.X),
               Math.Round(vector.Y),
               Math.Round(vector.Z),
               Math.Round(vector.W));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vector4d v0, Vector4d v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z + v0.W * v1.W;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vector4d v0, Vector4d v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y) + Math.Abs(v0.Z * v1.Z) + Math.Abs(v0.W * v1.W);
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vector4d v0, Vector4d v1)
        {
            double d = v0.SquareLength * v1.SquareLength;
            return d > 0.0 ? SlurMath.AcosSafe(Dot(v0, v1) / Math.Sqrt(d)) : 0.0;
        }



        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d Project(Vector4d v0, Vector4d v1)
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
        public static Vector4d Reject(Vector4d v0, Vector4d v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector4d Reflect(Vector4d v0, Vector4d v1)
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
        public static Vector4d MatchProjection(Vector4d v0, Vector4d v1)
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
        public static Vector4d MatchProjection(Vector4d v0, Vector4d v1, Vector4d v2)
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
        public static Vector4d Lerp(Vector4d v0, Vector4d v1, double factor)
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
        public static Vector4d Slerp(Vector4d v0, Vector4d v1, double factor)
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
        public static Vector4d Slerp(Vector4d v0, Vector4d v1, double angle, double factor)
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
        /// <summary></summary>
        public double W;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyzw"></param>
        public Vector4d(double xyzw)
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
        public Vector4d(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public Vector4d(Vector2d other, double z, double w)
        {
            X = other.X;
            Y = other.Y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="w"></param>
        public Vector4d(Vector3d other, double w)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d XY
        {
            get { return new Vector2d(X, Y); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d XYZ
        {
            get { return new Vector3d(X, Y, Z); }
        }


        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vector4d Unit
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
        public bool IsZero(double tolerance = SlurMath.ZeroToleranced)
        {
            return SquareLength < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(double tolerance = SlurMath.ZeroToleranced)
        {
            return Math.Abs(SquareLength - 1.0) < tolerance;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
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
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vector4d other, double epsilon = SlurMath.ZeroToleranced)
        {
            return
                SlurMath.ApproxEquals(X, other.X, epsilon) &&
                SlurMath.ApproxEquals(Y, other.Y, epsilon) &&
                SlurMath.ApproxEquals(Z, other.Z, epsilon) &&
                SlurMath.ApproxEquals(W, other.W, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vector4d other)
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
        public double SquareDistanceTo(Vector4d other)
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
        public double ManhattanDistanceTo(Vector4d other)
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
        public void Negate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
            W = -W;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector4d LerpTo(Vector4d other, double factor)
        {
            return new Vector4d(
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor,
                Z + (other.Z - Z) * factor,
                W + (other.W - W) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector4d SlerpTo(Vector4d other, double factor)
        {
            return SlerpTo(other, Angle(this, other), factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="angle"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector4d SlerpTo(Vector4d other, double angle, double factor)
        {
            double sa = Math.Sin(angle);

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
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            var result = new double[4];
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
            result[3] = W;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public void Deconstruct(out double x, out double y, out double z, out double w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }
    }
}
