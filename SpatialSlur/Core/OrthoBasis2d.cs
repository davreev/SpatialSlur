
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Orthonormal basis representation of a 2 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct OrthoBasis2d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly OrthoBasis2d Identity = new OrthoBasis2d(1.0, 0.0);


        /// <summary>
        /// Applies the given rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2d operator *(OrthoBasis2d rotation, Vector2d vector)
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


        private Vector2d _x;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public OrthoBasis2d(Vector2d x)
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
            _x = new Vector2d(cosAngle, sinAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d X
        {
            get { return _x; }
            set { _x = value.Unit; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector2d Y
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
        /// Returns true if this rotation has non-zero axes.
        /// </summary>
        public bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OrthoBasis3d As3d
        {
            get => OrthoBasis3d.CreateFrom2d(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        public void Set(double angle)
        {
            _x = new Vector2d(Math.Cos(angle), Math.Sin(angle));
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
        public Vector2d Apply(Vector2d vector)
        {
            return vector.X * X + vector.Y * Y;
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
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2d ApplyInverse(Vector2d vector)
        {
            return new Vector2d(Vector2d.Dot(vector, X), Vector2d.Dot(vector, Y));
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
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(OrthoBasis2d other, double epsilon = D.ZeroTolerance)
        {
            return _x.ApproxEquals(other._x, epsilon);
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
        public void Deconstruct(out Vector2d x, out Vector2d y)
        {
            x = X;
            y = Y;
        }
    }
}
