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
    public struct Rotation2d
    {
        #region Static

        /// <summary></summary>
        public static readonly Rotation2d Identity = new Rotation2d(1.0, 0.0);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d operator *(Rotation2d rotation, Vec2d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Rotation2d operator *(Rotation2d r0, Rotation2d r1)
        {
            return r0.Apply(r1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        public static Vec2d Multiply(Rotation2d rotation, Vec2d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Rotation2d Multiply(Rotation2d r0, Rotation2d r1)
        {
            return r0.Apply(r1);
        }


        /// <summary>
        /// Creates a relative rotation from t0 to t1.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Rotation2d CreateRelative(Rotation2d r0, Rotation2d r1)
        {
            return r1.Apply(r0.Inverse);
        }

        #endregion


        private Vec2d _x;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public Rotation2d(Vec2d x)
        {
            _x = x.Direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public Rotation2d(double angle)
            : this()
        {
            Set(angle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        private Rotation2d(double cosAngle, double sinAngle)
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
        public Rotation2d Inverse
        {
            get { return new Rotation2d(_x.X, -_x.Y); }
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
        public Rotation2d Apply(Rotation2d other)
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
        public Rotation2d ApplyInverse(Rotation2d other)
        {
            other._x = ApplyInverse(other._x);
            return other;
        }
    }
}
