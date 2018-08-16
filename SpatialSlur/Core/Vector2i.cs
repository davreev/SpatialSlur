
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
    public struct Vector2i : IEquatable<Vector2i>
    {
        #region Static members

        /// <summary></summary>
        public static readonly Vector2i Zero = new Vector2i();
        /// <summary></summary>
        public static readonly Vector2i UnitX = new Vector2i(1, 0);
        /// <summary></summary>
        public static readonly Vector2i UnitY = new Vector2i(0, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator string(Vector2i vector)
        {
            return vector.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2i v0, Vector2i v1)
        {
            return v0.X == v1.X && v0.Y == v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2i v0, Vector2i v1)
        {
            return v0.X != v1.X || v0.Y != v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2i operator +(Vector2i v0, Vector2i v1)
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
        public static Vector2i operator -(Vector2i v0, Vector2i v1)
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
        public static Vector2i operator -(Vector2i vector)
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
        public static Vector2i operator *(Vector2i vector, int scalar)
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
        public static Vector2i operator *(int scalar, Vector2i vector)
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
        public static Vector2i operator *(Vector2i v0, Vector2i v1)
        {
            v0.X *= v1.X;
            v0.Y *= v1.Y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2i Abs(Vector2i vector)
        {
            return new Vector2i(Math.Abs(vector.X), Math.Abs(vector.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2i Max(Vector2i vector, int value)
        {
            return new Vector2i(Math.Max(vector.X, value), Math.Max(vector.Y, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2i Max(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2i Min(Vector2i vector, int value)
        {
            return new Vector2i(Math.Min(vector.X, value), Math.Min(vector.Y, value));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2i Min(Vector2i v0, Vector2i v1)
        {
            return new Vector2i(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static int Dot(Vector2i v0, Vector2i v1)
        {
            return v0.X * v1.X + v0.Y * v1.Y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static int AbsDot(Vector2i v0, Vector2i v1)
        {
            return Math.Abs(v0.X * v1.X) + Math.Abs(v0.Y * v1.Y);
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
        public Vector2i(int xy)
        {
            X = Y = xy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2i(int x, int y)
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
        public Vector2i PerpCW
        {
            get { return new Vector2i(Y, -X); }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vector2i PerpCCW
        {
            get { return new Vector2i(-Y, X); }
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
            get => new Vector3i(X, Y, 0);
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
        public bool Equals(Vector2i other)
        {
            return X == other.X && Y == other.Y;
        }


        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Vector2i && Equals((Vector2i)obj);
        }


        /// <inheritdoc />
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
        public double DistanceTo(Vector2i other)
        {
            other -= this;
            return other.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int SquareDistanceTo(Vector2i other)
        {
            other -= this;
            return other.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int ManhattanDistanceTo(Vector2i other)
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
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
