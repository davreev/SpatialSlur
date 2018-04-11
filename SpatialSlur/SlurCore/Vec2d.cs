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
    public partial struct Vec2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Vec2d Zero = new Vec2d();
        /// <summary></summary>
        public static readonly Vec2d UnitX = new Vec2d(1.0, 0.0);
        /// <summary></summary>
        public static readonly Vec2d UnitY = new Vec2d(0.0, 1.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vec2d vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec2d(Vec3d vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec2d(Vec4d vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator Vec2d(Vec2i vector)
        {
            return new Vec2d(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator +(Vec2d v0, Vec2d v1)
        {
            v0.X += v1.X;
            v0.Y += v1.Y;
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
            v0.X -= v1.X;
            v0.Y -= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d operator -(Vec2d vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec2d operator *(Vec2d vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec2d operator *(double scalar, Vec2d vector)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            return vector;
        }


        /// <summary>
        /// Component-wise multiplication.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator *(Vec2d v0, Vec2d v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec2d operator /(Vec2d vector, double scalar)
        {
            scalar = 1.0 / scalar;
            vector.X *= scalar;
            vector.Y *= scalar;
            return vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vec2d operator /(double scalar, Vec2d vector)
        {
            vector.X = scalar / vector.X;
            vector.Y = scalar / vector.Y;
            return vector;
        }


        /// <summary>
        /// Component-wise division.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d operator /(Vec2d v0, Vec2d v1)
        {
            v0.X /= v1.X;
            v0.Y /= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Max(Vec2d v, double t)
        {
            return new Vec2d(Math.Max(v.X, t), Math.Max(v.Y, t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Max(Vec2d v0, Vec2d v1)
        {
            return new Vec2d(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2d Min(Vec2d v, double t)
        {
            return new Vec2d(Math.Min(v.X, t), Math.Min(v.Y, t));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Min(Vec2d v0, Vec2d v1)
        {
            return new Vec2d(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d Abs(Vec2d vector)
        {
            return new Vec2d(Math.Abs(vector.X), Math.Abs(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec2d v0, Vec2d v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double AbsDot(Vec2d v0, Vec2d v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Cross(Vec2d v0, Vec2d v1)
        {
            return v0.X * v1.Y - v0.Y * v1.X;
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Angle(Vec2d v0, Vec2d v1)
        {
            var d = v0.SquareLength * v1.SquareLength;
            return d > 0.0 ? SlurMath.AcosSafe(Dot(v0, v1) / Math.Sqrt(d)) : 0.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double SignedAngle(Vec2d v0, Vec3d v1)
        {
            return Math.Atan2(Cross(v0, v1), Dot(v0, v1));
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d Project(Vec2d v0, Vec2d v1)
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
            return v1 * (Dot(v0, v1) / v1.SquareLength * 2.0) - v0;
        }

 
        /// <summary>
        /// Returns a vector parallel to v0 whos projection onto v1 equals v1
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2d MatchProjection(Vec2d v0, Vec2d v1)
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
        public static Vec2d MatchProjection(Vec2d v0, Vec2d v1, Vec2d v2)
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
        public static Vec2d Lerp(Vec2d v0, Vec2d v1, double factor)
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
        public static Vec2d Slerp(Vec2d v0, Vec2d v1, double factor)
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
        public static Vec2d Slerp(Vec2d v0, Vec2d v1, double angle, double factor)
        {
            return v0.SlerpTo(v1, angle, factor);
        }

#endregion


        /// <summary></summary>
        public double X;
        /// <summary></summary>
        public double Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public Vec2d(double xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec2d(double x, double y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vec2d Unit
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
            get { return X * X + Y * Y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y); }
        }


        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public double ComponentSum
        {
            get { return X + Y; }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMax
        {
            get { return Math.Max(X, Y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public double ComponentMin
        {
            get { return Math.Min(X, Y); }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public double ComponentMean
        {
            get
            {
                const double inv3 = 1.0 / 3.0;
                return (X + Y) * inv3;
            }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn clockwise.
        /// </summary>
        public Vec2d PerpCW
        {
            get { return new Vec2d(Y, -X); }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vec2d PerpCCW
        {
            get { return new Vec2d(-Y, X); }
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
            return Math.Abs(SquareLength - 1.0) < tolerance;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X}, {Y})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public void Set(double xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(double x, double y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// Converts from euclidean to polar coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToPolar()
        {
            return new Vec2d(Length, Math.Atan(Y / X));
        }


        /// <summary>
        /// Converts from polar to euclidean coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vec2d ToEuclidean()
        {
            return new Vec2d(Math.Cos(Y) * X, Math.Sin(Y) * X);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec2d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, tolerance) &&
                SlurMath.ApproxEquals(Y, other.Y, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vec2d other, Vec2d tolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, tolerance.X) &&
                SlurMath.ApproxEquals(Y, other.Y, tolerance.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec2d other)
        {
            other.X -= X;
            other.Y -= Y;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double SquareDistanceTo(Vec2d other)
        {
            other.X -= X;
            other.Y -= Y;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double ManhattanDistanceTo(Vec2d other)
        {
            other.X -= X;
            other.Y -= Y;
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
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec2d SlerpTo(Vec2d other, double factor)
        {
            return SlerpTo(other, Angle(this, other), factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vec2d SlerpTo(Vec2d other, double angle, double factor)
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
            var result = new double[2];
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }
    }
}
