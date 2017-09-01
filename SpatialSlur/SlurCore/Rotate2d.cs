using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents an arbitrary rotation in 2 dimensions as a right-handed orthonormal basis.
    /// </summary>
    public struct Rotate2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Rotate2d Identity = new Rotate2d(1.0, 0.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d operator *(Rotate2d rotate, Vec2d vector)
        {
            return rotate.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Rotate2d operator *(Rotate2d t0, Rotate2d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="vector"></param>
        public static Vec2d Multiply(Rotate2d rotate, Vec2d vector)
        {
            return rotate.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Rotate2d Multiply(Rotate2d t0, Rotate2d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// Creates a change of basis rotation from r0 to r1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Rotate2d CreateChangeBasis(Rotate2d from, Rotate2d to)
        {
            return to.Apply(from.Inverse);
        }

        #endregion


        private Vec2d _x;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public Rotate2d(Vec2d x)
        {
            _x = x.Direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public Rotate2d(double angle)
            : this()
        {
            Set(angle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        private Rotate2d(double cosAngle, double sinAngle)
        {
            _x = new Vec2d(cosAngle, sinAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d X
        {
            get { return _x; }
            set { _x = value.Direction; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d Y
        {
            get { return _x.PerpCCW; }
        }


        /// <summary>
        /// Returns the inverse of this rotation
        /// </summary>
        public Rotate2d Inverse
        {
            get { return new Rotate2d(_x.X, -_x.Y); }
        }


        /// <summary>
        /// Return false if this rotation is undefined.
        /// </summary>
        public bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void Set(double angle)
        {
            _x = new Vec2d(Math.Cos(angle), Math.Sin(angle));
        }


        /// <summary>
        /// Inverts this rotation in place.
        /// </summary>
        public void Invert()
        {
            _x.Y = -_x.Y;
        }


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec2d Apply(Vec2d vector)
        {
            return vector.X * X + vector.Y * Y;
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public Rotate2d Apply(Rotate2d other)
        {
            other._x = Apply(other._x);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec2d ApplyInverse(Vec2d vector)
        {
            return new Vec2d(Vec2d.Dot(vector, X), Vec2d.Dot(vector, Y));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public Rotate2d ApplyInverse(Rotate2d other)
        {
            other._x = ApplyInverse(other._x);
            return other;
        }
    }
}
