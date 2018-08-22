
/*
 * Notes
 */

using System;

using F = SpatialSlur.SlurMath.Constantsf;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public partial struct Vector3f
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Vector3f Zero = new Vector3f();
        /// <summary></summary>
        public static readonly Vector3f UnitX = new Vector3f(1.0f, 0.0f, 0.0f);
        /// <summary></summary>
        public static readonly Vector3f UnitY = new Vector3f(0.0f, 1.0f, 0.0f);
        /// <summary></summary>
        public static readonly Vector3f UnitZ = new Vector3f(0.0f, 0.0f, 1.0f);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vector3f vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator Vector3f(Vector3i vector)
        {
            return vector.As3f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f operator +(Vector3f v0, Vector3f v1)
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
        public static Vector3f operator -(Vector3f v0, Vector3f v1)
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
        public static Vector3f operator -(Vector3f vector)
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
        public static Vector3f operator *(Vector3f vector, float scalar)
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
        public static Vector3f operator *(float scalar, Vector3f vector)
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
        public static Vector3f operator *(Vector3f v0, Vector3f v1)
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
        public static Vector3f operator /(Vector3f vector, float scalar)
        {
            scalar = 1.0f / scalar;
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
        public static Vector3f operator /(float scalar, Vector3f vector)
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
        public static Vector3f operator /(Vector3f v0, Vector3f v1)
        {
            v0.X /= v1.X;
            v0.Y /= v1.Y;
            v0.Z /= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3f Max(Vector3f vector, float value)
        {
            return new Vector3f(
                Math.Max(vector.X, value), 
                Math.Max(vector.Y, value), 
                Math.Max(vector.Z, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f Max(Vector3f v0, Vector3f v1)
        {
            return new Vector3f(
                Math.Max(v0.X, v1.X), 
                Math.Max(v0.Y, v1.Y), 
                Math.Max(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3f Min(Vector3f vector, float value)
        {
            return new Vector3f(
                Math.Min(vector.X, value), 
                Math.Min(vector.Y, value), 
                Math.Min(vector.Z, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f Min(Vector3f v0, Vector3f v1)
        {
            return new Vector3f(
                Math.Min(v0.X, v1.X), 
                Math.Min(v0.Y, v1.Y), 
                Math.Min(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f Abs(Vector3f vector)
        {
            return new Vector3f(
                Math.Abs(vector.X), 
                Math.Abs(vector.Y), 
                Math.Abs(vector.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f Floor(Vector3f vector)
        {
            return new Vector3f(
               SlurMath.Floor(vector.X),
               SlurMath.Floor(vector.Y),
               SlurMath.Floor(vector.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f Ceiling(Vector3f vector)
        {
            return new Vector3f(
               SlurMath.Ceiling(vector.X),
               SlurMath.Ceiling(vector.Y),
               SlurMath.Ceiling(vector.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3f Round(Vector3f vector)
        {
            return new Vector3f(
               SlurMath.Round(vector.X),
               SlurMath.Round(vector.Y),
               SlurMath.Round(vector.Z));
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="whole"></param>
        /// <returns></returns>
        public static Vector3f Fract(Vector3f vector, out Vector3i whole)
        {
            return new Vector3f(
               SlurMath.Fract(vector.X, out whole.X),
               SlurMath.Fract(vector.Y, out whole.Y),
               SlurMath.Fract(vector.Z, out whole.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Dot(Vector3f v0, Vector3f v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float AbsDot(Vector3f v0, Vector3f v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y) + Math.Abs(v0.Z * v1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f Cross(Vector3f v0, Vector3f v1)
        {
            return new Vector3f(
                v0.Y * v1.Z - v0.Z * v1.Y,
                v0.Z * v1.X - v0.X * v1.Z,
                v0.X * v1.Y - v0.Y * v1.X);
        }
        

        /// <summary>
        /// Returns the box product or scalar triple product of the given vectors (i.e. u x v . w)
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static float ScalarTriple(Vector3f u, Vector3f v, Vector3f w)
        {
            return Dot(Cross(u, v), w);
        }


        /// <summary>
        /// Returns the vector triple product of the given vectors (i.e. u x v x w)
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static Vector3f VectorTriple(Vector3f u, Vector3f v, Vector3f w)
        {
            return v * Cross(u, w) - w * Cross(u, v);
        }


        /// <summary>
        /// Returns the minimum angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Angle(Vector3f v0, Vector3f v1)
        {
            var d = v0.SquareLength * v1.SquareLength;
            return d > 0.0f ? SlurMath.AcosSafe(Dot(v0, v1) / SlurMath.Sqrt(d)) : 0.0f;
        }


        /// <summary>
        /// Returns the signed angle between two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static float SignedAngle(Vector3f v0, Vector3f v1, Vector3f up)
        {
            var c = Cross(v0, v1);
            return SlurMath.Atan2(c.Length * Math.Sign(Dot(c, up)), Dot(v0, v1));
        }


        /// <summary>
        /// Returns the cotangent of the angle between 2 vectors as per http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static float Cotangent(Vector3f v0, Vector3f v1)
        {
            return Dot(v0, v1) / Cross(v0, v1).Length;
        }


        /// <summary>
        /// Returns the projection of v0 onto v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f Project(Vector3f v0, Vector3f v1)
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
        public static Vector3f Reject(Vector3f v0, Vector3f v1)
        {
            return v0 - Project(v0, v1);
        }


        /// <summary>
        /// Returns the reflection of v0 about v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f Reflect(Vector3f v0, Vector3f v1)
        {
            //return Project(v0, v1) * 2.0 - v0;
            return v1 * (Dot(v0, v1) / v1.SquareLength * 2.0f) - v0;
        }


        /// <summary>
        /// Returns a vector parallel to v0 whos projection onto v1 equals v1
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3f MatchProjection(Vector3f v0, Vector3f v1)
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
        public static Vector3f MatchProjection(Vector3f v0, Vector3f v1, Vector3f v2)
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
        public static Vector3f Lerp(Vector3f v0, Vector3f v1, float factor)
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
        public static Vector3f Slerp(Vector3f v0, Vector3f v1, float factor)
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
        public static Vector3f Slerp(Vector3f v0, Vector3f v1, float angle, float factor)
        {
            return v0.SlerpTo(v1, angle, factor);
        }

        #endregion


        /// <summary></summary>
        public float X;
        /// <summary></summary>
        public float Y;
        /// <summary></summary>
        public float Z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyz"></param>
        public Vector3f(float xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }


#if false
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="z"></param>
        public Vector3f(Vector2f other, float z)
        {
            X = other.X;
            Y = other.Y;
            Z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2f XY
        {
            get { return new Vector2f(X, Y); }
        }
#endif


        /// <summary>
        /// Returns a unit length copy of this vector.
        /// Returns the zero vector if this vector is zero length.
        /// </summary>
        /// <returns></returns>
        public Vector3f Unit
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
            get { return X * X + Y * Y + Z * Z; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); }
        }
        

        /// <summary>
        /// Returns the sum of components.
        /// </summary>
        public float ComponentSum
        {
            get { return X + Y + Z; }
        }


        /// <summary>
        /// Returns the mean of components.
        /// </summary>
        public float ComponentMean
        {
            get
            {
                const float inv3 = 1.0f / 3.0f;
                return (X + Y + Z) * inv3;
            }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public float ComponentMax
        {
            get { return Math.Max(X, Math.Max(Y, Z)); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public float ComponentMin
        {
            get { return Math.Min(X, Math.Min(Y, Z)); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d As3d
        {
            get => new Vector3d(X, Y, Z);
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3i As3i
        {
            get => new Vector3i((int)X, (int)Y, (int)Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector4d As4d
        {
            get => new Vector4d(X, Y, Z, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero(float tolerance = F.ZeroTolerance)
        {
            return SquareLength < tolerance;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit(float tolerance = F.ZeroTolerance)
        {
            return SlurMath.ApproxEquals(SquareLength, 1.0f, tolerance);
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
        public void Set(float xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(float x, float y, float z)
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
        public Vector3f ToSpherical()
        {
            float r = Length;
            return new Vector3f(r, SlurMath.Atan(Y / X), SlurMath.AcosSafe(Z / r));
        }


        /// <summary>
        /// Converts from spherical to euclidean coordiantes.
        /// (x,y,z) = (radius, azimuth, polar)
        /// </summary>
        /// <returns></returns>
        public Vector3f ToEuclidean()
        {
            float rxy = SlurMath.Sin(Z) * X * X;
            return new Vector3f(SlurMath.Cos(Y) * rxy, SlurMath.Sin(Y) * rxy, SlurMath.Cos(Z) * X);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(Vector3f other, float epsilon = F.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(X, other.X, epsilon) &&
                SlurMath.ApproxEquals(Y, other.Y, epsilon) &&
                SlurMath.ApproxEquals(Z, other.Z, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float DistanceTo(Vector3f other)
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
        public float SquareDistanceTo(Vector3f other)
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
        public float ManhattanDistanceTo(Vector3f other)
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
            float d = SquareLength;

            if (d > 0.0f)
            {
                d = 1.0f / SlurMath.Sqrt(d);
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
        public Vector3f LerpTo(Vector3f other, float factor)
        {
            return new Vector3f(
                X + (other.X - X) * factor,
                Y + (other.Y - Y) * factor,
                Z + (other.Z - Z) * factor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector3f SlerpTo(Vector3f other, float factor)
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
        public Vector3f SlerpTo(Vector3f other, float angle, float factor)
        {
            var sa = SlurMath.Sin(angle);

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
        /// Returns the cross product of this vector vector with the x Axis
        /// </summary>
        /// <returns></returns>
        public Vector3f CrossX()
        {
            return new Vector3f(0.0f, Z, -Y);
        }


        /// <summary>
        /// Returns the cross product of this vector with the Y Axis
        /// </summary>
        /// <returns></returns>
        public Vector3f CrossY()
        {
            return new Vector3f(-Z, 0.0f, X);
        }


        /// <summary>
        /// Returns the cross product of this vector with the Y Axis
        /// </summary>
        /// <returns></returns>
        public Vector3f CrossZ()
        {
            return new Vector3f(Y, -X, 0.0f);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            var result = new float[3];
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
            result[2] = Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
