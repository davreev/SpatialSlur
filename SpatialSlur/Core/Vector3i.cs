
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
    public struct Vector3i : IEquatable<Vector3i>
    {
        #region Static Members

        /// <summary></summary>
        public static readonly Vector3i Zero = new Vector3i();
        /// <summary></summary>
        public static readonly Vector3i UnitX = new Vector3i(1, 0, 0);
        /// <summary></summary>
        public static readonly Vector3i UnitY = new Vector3i(0, 1, 0);
        /// <summary></summary>
        public static readonly Vector3i UnitZ = new Vector3i(0, 0, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vector3i vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator ==(Vector3i v0, Vector3i v1)
        {
            return v0.X == v1.X && v0.Y == v1.Y && v0.Z == v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator !=(Vector3i v0, Vector3i v1)
        {
            return v0.X != v1.X || v0.Y != v1.Y || v0.Z != v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3i operator +(Vector3i v0, Vector3i v1)
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
        public static Vector3i operator -(Vector3i v0, Vector3i v1)
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
        public static Vector3i operator -(Vector3i vector)
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
        public static Vector3i operator *(Vector3i vector, int scalar)
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
        public static Vector3i operator *(int scalar, Vector3i vector)
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
        public static Vector3i operator *(Vector3i v0, Vector3i v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            v0.Z *= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector3i Abs(Vector3i vector)
        {
            return new Vector3i(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3i Max(Vector3i vector, int value)
        {
            return new Vector3i(Math.Max(vector.X, value), Math.Max(vector.Y, value), Math.Max(vector.Z, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3i Max(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y), Math.Max(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3i Min(Vector3i vector, int value)
        {
            return new Vector3i(Math.Min(vector.X, value), Math.Min(vector.Y, value), Math.Min(vector.Z, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3i Min(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y), Math.Min(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static int Dot(Vector3i v0, Vector3i v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static int AbsDot(Vector3i v0, Vector3i v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y) + Math.Abs(v0.Z * v1.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector3i Cross(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(
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
        public static int ScalarTriple(Vector3i u, Vector3i v, Vector3i w)
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
        public static Vector3i VectorTriple(Vector3i u, Vector3i v, Vector3i w)
        {
            return v * Cross(u, w) - w * Cross(u, v);
        }

        #endregion


        /// <summary></summary>
        public int X;
        /// <summary></summary>
        public int Y;
        /// <summary></summary>
        public int Z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xyz"></param>
        public Vector3i(int xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2i XY
        {
            get { return new Vector2i(X, Y); }
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
        public int SquareLength
        {
            get { return X * X + Y * Y + Z * Z; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z); }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMax
        {
            get { return Math.Max(X, Math.Max(Y, Z)); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMin
        {
            get { return Math.Min(X, Math.Min(Y, Z)); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ComponentSum
        {
            get { return X + Y + Z; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero
        {
            get { return X == 0 && Y == 0 && Z == 0; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsUnit
        {
            get { return SquareLength == 1; }
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
        public Vector3f As3f
        {
            get => new Vector3f(X, Y, Z);
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
        public void Set(int xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Vector3i other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }


        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Vector3i && Equals((Vector3i)obj);
        }


        /// <inheritdoc />
        public override int GetHashCode()
        {
            const int p0 = 73856093;
            const int p1 = 19349663;
            const int p2 = 83492791;
            return X * p0 ^ Y * p1 ^ Z * p2;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vector3i other)
        {
            other -= this;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int SquareDistanceTo(Vector3i other)
        {
            other -= this;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int ManhattanDistanceTo(Vector3i other)
        {
            other -= this;
            return other.ManhattanLength;
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
        /// <returns></returns>
        public int[] ToArray()
        {
            var result = new int[3];
            ToArray(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void ToArray(int[] result)
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
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
