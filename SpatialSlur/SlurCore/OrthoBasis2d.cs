using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Orthonormal basis representation of a 2 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct OrthoBasis2d
    {
        #region Static

        /// <summary></summary>
        public static readonly OrthoBasis2d Identity = new OrthoBasis2d(1.0, 0.0);


        /// <summary>
        /// Applies the given rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec2d operator *(OrthoBasis2d rotation, Vec2d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static OrthoBasis2d operator *(OrthoBasis2d r0, OrthoBasis2d r1)
        {
            return r0.Apply(r1);
        }


        /// <summary>
        /// Creates a relative rotation from t0 to t1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static OrthoBasis2d CreateFromTo(OrthoBasis2d from, OrthoBasis2d to)
        {
            return to.Apply(from.Inverse);
        }

        #endregion


        private Vec2d _x;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public OrthoBasis2d(Vec2d x)
        {
            _x = x.Unit;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public OrthoBasis2d(double angle)
            : this()
        {
            Set(angle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        private OrthoBasis2d(double cosAngle, double sinAngle)
        {
            _x = new Vec2d(cosAngle, sinAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec2d X
        {
            get { return _x; }
            set { _x = value.Unit; }
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
        public OrthoBasis2d Inverse
        {
            get { return new OrthoBasis2d(_x.X, -_x.Y); }
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
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec2d ApplyInverse(Vec2d vector)
        {
            return new Vec2d(Vec2d.Dot(vector, X), Vec2d.Dot(vector, Y));
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public OrthoBasis2d Apply(OrthoBasis2d other)
        {
            other._x = Apply(other._x);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public OrthoBasis2d ApplyInverse(OrthoBasis2d other)
        {
            other._x = ApplyInverse(other._x);
            return other;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(OrthoBasis2d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return _x.ApproxEquals(other._x, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix2d ToMatrix()
        {
            return new Matrix2d(X, Y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out Vec2d x, out Vec2d y)
        {
            x = X;
            y = Y;
        }
    }
}
