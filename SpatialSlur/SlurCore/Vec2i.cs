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
    public struct Vec2i
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        public static Vec2i UnitX
        {
            get { return new Vec2i(1, 0); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2i UnitY
        {
            get { return new Vec2i(0, 1); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator ==(Vec2i v0, Vec2i v1)
        {
            return (v0.x == v1.x) && (v0.y == v1.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static bool operator !=(Vec2i v0, Vec2i v1)
        {
            return (v0.x != v1.x) || (v0.y != v1.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i operator +(Vec2i v0, Vec2i v1)
        {
            v0.x += v1.x;
            v0.y += v1.y;
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
            v0.x -= v1.x;
            v0.y -= v1.y;
            return v0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec2i operator -(Vec2i v)
        {
            v.x = -v.x;
            v.y = -v.y;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2i operator *(Vec2i v, int t)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vec2i operator *(int t, Vec2i v)
        {
            v.x *= t;
            v.y *= t;
            return v;
        }


        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static int operator *(Vec2i v0, Vec2i v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vec2i Abs(Vec2i v)
        {
            return new Vec2i(Math.Abs(v.x), Math.Abs(v.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i Max(Vec2i v0, Vec2i v1)
        {
            return new Vec2i(Math.Max(v0.x, v1.x), Math.Max(v0.y, v1.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec2i Min(Vec2i v0, Vec2i v1)
        {
            return new Vec2i(Math.Min(v0.x, v1.x), Math.Min(v0.y, v1.y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static double Dot(Vec2i v0, Vec2i v1)
        {
            return v0.x * v1.x + v0.y * v1.y;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static implicit operator Vec2i(Vec3i v)
        {
            return new Vec2i(v.x, v.y);
        }

        #endregion


        public int x, y;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn clockwise.
        /// </summary>
        public Vec2i PerpCW
        {
            get { return new Vec2i(y, -x); }
        }


        /// <summary>
        /// Returns the perpendicular vector rotated a quarter turn counter clockwise.
        /// </summary>
        public Vec2i PerpCCW
        {
            get { return new Vec2i(-y, x); }
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
            get { return x * x + y * y; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ManhattanLength
        {
            get { return Math.Abs(x) + Math.Abs(y); }
        }


        /// <summary>
        /// Returns the largest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMax
        {
            get { return Math.Max(x, y); }
        }


        /// <summary>
        /// Returns the smallest component in the vector.
        /// </summary>
        /// <returns></returns>
        public int ComponentMin
        {
            get { return Math.Min(x, y); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ComponentSum
        {
            get { return x + y; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsZero
        {
            get { return x == 0 && y == 0; }
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
            return String.Format("({0},{1})", x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }


        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/336aedhh(v=VS.71).aspx
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }


        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/336aedhh(v=VS.71).aspx
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Vec2i && this == (Vec2i)obj;
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
        /// <returns></returns>
        public int[] ToArray()
        {
            return new int[] { x, y };
        }
    }
}
