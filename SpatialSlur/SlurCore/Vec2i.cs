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
    public struct Vec2i : IEquatable<Vec2i>
    {
        #region Static

        /// <summary></summary>
        public static readonly Vec2i Zero = new Vec2i();
        /// <summary></summary>
        public static readonly Vec2i UnitX = new Vec2i(1, 0);
        /// <summary></summary>
        public static readonly Vec2i UnitY = new Vec2i(0, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vec2i vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static implicit operator Vec2i(Vec3i vector)
        {
            return new Vec2i(vector.X, vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static explicit operator Vec2i(Vec2d vector)
        {
            return new Vec2i((int)vector.X, (int)vector.Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator ==(Vec2i v0, Vec2i v1)
        {
            return v0.X == v1.X && v0.Y == v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator !=(Vec2i v0, Vec2i v1)
        {
            return v0.X != v1.X || v0.Y != v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i operator +(Vec2i v0, Vec2i v1)
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
        public static Vec2i operator -(Vec2i v0, Vec2i v1)
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
        public static Vec2i operator -(Vec2i vector)
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
        public static Vec2i operator *(Vec2i vector, int scalar)
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
        public static Vec2i operator *(int scalar, Vec2i vector)
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
        public static Vec2i operator *(Vec2i v0, Vec2i v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec2i Abs(Vec2i vector)
        {
            return new Vec2i(Math.Abs(vector.X), Math.Abs(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i Max(Vec2i v0, Vec2i v1)
        {
            return new Vec2i(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i Min(Vec2i v0, Vec2i v1)
        {
            return new Vec2i(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec2i v0, Vec2i v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y;
        }

        #endregion


        /// <summary></summary>
        public int X;
        /// <summary></summary>
        public int Y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public Vec2i(int xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec2i(int x, int y)
        {
            X = x;
            Y = y;
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
            get { return X * X + Y * Y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ManhattanLength
        {
            get { return Math.Abs(X) + Math.Abs(Y); }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMax
        {
            get { return Math.Max(X, Y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMin
        {
            get { return Math.Min(X, Y); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ComponentSum
        {
            get { return X + Y; }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn clockwise.
        /// </summary>
        public Vec2i PerpCW
        {
            get { return new Vec2i(Y, -X); }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vec2i PerpCCW
        {
            get { return new Vec2i(-Y, X); }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero
        {
            get { return X == 0 && Y == 0; }
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
        /// <returns></returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public void Set(int xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Vec2i other)
        {
            return X == other.X && Y == other.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Vec2i && Equals((Vec2i)obj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            const int p0 = 73856093;
            const int p1 = 19349663;
            return X * p0 ^ Y * p1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(Vec2i other)
        {
            other -= this;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int SquareDistanceTo(Vec2i other)
        {
            other -= this;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int ManhattanDistanceTo(Vec2i other)
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            var result = new int[2];
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
