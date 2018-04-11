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
    public struct Vec3i : IEquatable<Vec3i>
    {
        #region Static

        /// <summary></summary>
        public static readonly Vec3i Zero = new Vec3i();
        /// <summary></summary>
        public static readonly Vec3i UnitX = new Vec3i(1, 0, 0);
        /// <summary></summary>
        public static readonly Vec3i UnitY = new Vec3i(0, 1, 0);
        /// <summary></summary>
        public static readonly Vec3i UnitZ = new Vec3i(0, 0, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vec3i vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec3i(Vec2i vector)
        {
            return new Vec3i(vector.X, vector.Y, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static explicit operator Vec3i(Vec3d vector)
        {
            return new Vec3i((int)vector.X, (int)vector.Y, (int)vector.Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator ==(Vec3i v0, Vec3i v1)
        {
            return v0.X == v1.X && v0.Y == v1.Y && v0.Z == v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator !=(Vec3i v0, Vec3i v1)
        {
            return v0.X != v1.X || v0.Y != v1.Y || v0.Z != v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3i operator +(Vec3i v0, Vec3i v1)
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
        public static Vec3i operator -(Vec3i v0, Vec3i v1)
        {
            v0.X -= v1.X;
            v0.Y -= v1.Y;
            v0.Z -= v1.Z;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec3i operator -(Vec3i v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3i operator *(Vec3i v, int t)
        {
            v.X *= t;
            v.Y *= t;
            v.Z *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec3i operator *(int t, Vec3i v)
        {
            v.X *= t;
            v.Y *= t;
            v.Z *= t;
            return v;
        }


        /// <summary>
        /// Component-wise multiplication.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3i operator *(Vec3i v0, Vec3i v1)
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
        public static Vec3i Abs(Vec3i v)
        {
            return new Vec3i(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3i Max(Vec3i v0, Vec3i v1)
        {
            return new Vec3i(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y), Math.Max(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3i Min(Vec3i v0, Vec3i v1)
        {
            return new Vec3i(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y), Math.Min(v0.Z, v1.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec3i v0, Vec3i v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y + v0.Z * v1.Z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3i Cross(Vec3i v0, Vec3i v1)
        {
            return new Vec3i(
                v0.Y * v1.Z - v0.Z * v1.Y, 
                v0.Z * v1.X - v0.X * v1.Z, 
                v0.X * v1.Y - v0.Y * v1.X);
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
        public Vec3i(int xyz)
        {
            X = Y = Z = xyz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
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
        public bool Equals(Vec3i other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }


        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Vec3i && Equals((Vec3i)obj);
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
        public double DistanceTo(Vec3i other)
        {
            other -= this;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int SquareDistanceTo(Vec3i other)
        {
            other -= this;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int ManhattanDistanceTo(Vec3i other)
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
