
/*
 * Notes
 */

using System;

using Constf = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial struct Vector2f
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Vector2f Zero = new Vector2f();
        /// <summary></summary>
        public static readonly Vector2f UnitX = new Vector2f(1.0f, 0.0f);
        /// <summary></summary>
        public static readonly Vector2f UnitY = new Vector2f(0.0f, 1.0f);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vector2f vector)
        {
            return vector.ToString();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator Vector2f(Vector2i vector)
        {
            return vector.As2f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f operator +(Vector2f v0, Vector2f v1)
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
        public static Vector2f operator -(Vector2f v0, Vector2f v1)
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
        public static Vector2f operator -(Vector2f vector)
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
        public static Vector2f operator *(Vector2f vector, float scalar)
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
        public static Vector2f operator *(float scalar, Vector2f vector)
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
        public static Vector2f operator *(Vector2f v0, Vector2f v1)
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
        public static Vector2f operator /(Vector2f vector, float scalar)
        {
            scalar = 1.0f / scalar;
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
        public static Vector2f operator /(float scalar, Vector2f vector)
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
        public static Vector2f operator /(Vector2f v0, Vector2f v1)
        {
            v0.X /= v1.X;
            v0.Y /= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2f Max(Vector2f vector, float value)
        {
            return new Vector2f(
                Math.Max(vector.X, value), 
                Math.Max(vector.Y, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f Max(Vector2f v0, Vector2f v1)
        {
            return new Vector2f(
                Math.Max(v0.X, v1.X), 
                Math.Max(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2f Min(Vector2f vector, float value)
        {
            return new Vector2f(
                Math.Min(vector.X, value), 
                Math.Min(vector.Y, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f Min(Vector2f v0, Vector2f v1)
        {
            return new Vector2f(
                Math.Min(v0.X, v1.X), 
                Math.Min(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2f Abs(Vector2f vector)
        {
            return new Vector2f(
                Math.Abs(vector.X), 
                Math.Abs(vector.Y));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2f Floor(Vector2f vector)
        {
            return new Vector2f(
               SlurMath.Floor(vector.X),
               SlurMath.Floor(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2f Ceiling(Vector2f vector)
        {
            return new Vector2f(
               SlurMath.Ceiling(vector.X),
               SlurMath.Ceiling(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2f Round(Vector2f vector)
        {
            return new Vector2f(
               SlurMath.Round(vector.X),
               SlurMath.Round(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static Vector2f Fract(Vector2f vector, out Vector2i whole)
        {
            return new Vector2f(
               SlurMath.Fract(vector.X, out whole.X),
               SlurMath.Fract(vector.Y, out whole.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Dot(Vector2f v0, Vector2f v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float AbsDot(Vector2f v0, Vector2f v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Cross(Vector2f v0, Vector2f v1)
        {
            return v0.X * v1.Y - v0.Y * v1.X;
        }


        /// <summary>
        /// Returns the angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Angle(Vector2f v0, Vector2f v1)
        {
            var d = v0.SquareLength * v1.SquareLength;
            return d > 0.0f ? SlurMath.AcosSafe(Dot(v0, v1) / SlurMath.Sqrt(d)) : 0.0f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float SignedAngle(Vector2f v0, Vector2f v1)
        {
            return SlurMath.Atan2(Cross(v0, v1), Dot(v0, v1));
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f Project(Vector2f v0, Vector2f v1)
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
        public static Vector2f Reject(Vector2f v0, Vector2f v1)
        {
            // return v0 - Project(v0, v1);
            return v0 - Dot(v0, v1) / v1.SquareLength * v1;
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f Reflect(Vector2f v0, Vector2f v1)
        {
            // return Project(v0, v1) * 2.0f - v0;
            return v1 * (Dot(v0, v1) / v1.SquareLength * 2.0f) - v0;
        }

 
        /// <summary>
        /// Returns a vector parallel to v0 whos projection onto v1 equals v1
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f MatchProjection(Vector2f v0, Vector2f v1)
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
        public static Vector2f MatchProjection(Vector2f v0, Vector2f v1, Vector2f v2)
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
        public static Vector2f Lerp(Vector2f v0, Vector2f v1, float factor)
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
        public static Vector2f Slerp(Vector2f v0, Vector2f v1, float factor)
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
        public static Vector2f Slerp(Vector2f v0, Vector2f v1, float angle, float factor)
        {
            return v0.SlerpTo(v1, angle, factor);
        }

        #endregion

        
        /// <summary></summary>
        public float X;
        /// <summary></summary>
        public float Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public Vector2f(float xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }
        

        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vector2f Unit
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
        public float Length
        {
            get { return SlurMath.Sqrt(SquareLength); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float SquareLength
        {
            get { return X * X + Y * Y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y); }
        }


        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public float ComponentSum
        {
            get { return X + Y; }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public float ComponentMax
        {
            get { return Math.Max(X, Y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public float ComponentMin
        {
            get { return Math.Min(X, Y); }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public float ComponentMean
        {
            get
            {
                const float inv3 = 1.0f / 3.0f;
                return (X + Y) * inv3;
            }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn clockwise.
        /// </summary>
        public Vector2f PerpCW
        {
            get { return new Vector2f(Y, -X); }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vector2f PerpCCW
        {
            get { return new Vector2f(-Y, X); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(float tolerance = Constf.ZeroTolerance)
        {
            return SquareLength <= tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(float tolerance = Constf.ZeroTolerance)
        {
            return SlurMath.ApproxEquals(SquareLength, 1.0f, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2i As2i
        {
            get => new Vector2i((int)X, (int)Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2d As2d
        {
            get => new Vector2d(X, Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3i As3i
        {
            get => new Vector3i((int)X, (int)Y, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3f As3f
        {
            get => new Vector3f(X, Y, 0.0f);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3d As3d
        {
            get => new Vector3d(X, Y, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector4d As4d
        {
            get => new Vector4d(X, Y, 0.0, 0.0);
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
        public void Set(float xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// Converts from euclidean to polar coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vector2f ToPolar()
        {
            return new Vector2f(Length, SlurMath.Atan(Y / X));
        }


        /// <summary>
        /// Converts from polar to euclidean coordiantes
        /// (x,y) = (radius, theta)
        /// </summary>
        /// <returns></returns>
        public Vector2f ToEuclidean()
        {
            return new Vector2f(SlurMath.Cos(Y) * X, SlurMath.Sin(Y) * X);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vector2f other, float epsilon = Constf.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, epsilon) &&
                SlurMath.ApproxEquals(Y, other.Y, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float DistanceTo(Vector2f other)
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
        public float SquareDistanceTo(Vector2f other)
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
        public float ManhattanDistanceTo(Vector2f other)
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
            float d = SquareLength;

            if (d > 0.0)
            {
                d = 1.0f / SlurMath.Sqrt(d);
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
        public Vector2f LerpTo(Vector2f other, float factor)
        {
            return new Vector2f(
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector2f SlerpTo(Vector2f other, float factor)
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
        public Vector2f SlerpTo(Vector2f other, float angle, float factor)
        {
            float sa = SlurMath.Sin(angle);
      
            // handle aligned cases
            if (sa > 0.0f)
            {
                var saInv = 1.0f / sa;
                var af = angle * factor;
                return this * SlurMath.Sin(angle - af) * saInv + other * SlurMath.Sin(af) * saInv;
            }

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            var result = new float[2];
            ToArray(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void ToArray(float[] result)
        {
            result[0] = X;
            result[1] = Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }
    }
}
