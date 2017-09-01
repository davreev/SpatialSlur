using System;
using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents an arbitrary rotation in 3 dimensions as a right-handed orthonormal basis.
    /// </summary>
    [Serializable]
    public struct Rotate3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Rotate3d Identity = new Rotate3d(Vec3d.UnitX, Vec3d.UnitY, Vec3d.UnitZ);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        public static implicit operator Rotate3d(Rotate2d rotate)
        {
            return new Rotate3d(rotate.X, rotate.Y, Vec3d.UnitZ);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator *(Rotate3d rotate, Vec3d vector)
        {
            return rotate.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Rotate3d operator *(Rotate3d t0, Rotate3d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotate"></param>
        /// <param name="vector"></param>
        public static Vec3d Multiply(ref Rotate3d rotate, Vec3d vector)
        {
            return rotate.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        public static Rotate3d Multiply(ref Rotate3d t0, ref Rotate3d t1)
        {
            return t0.Apply(t1);
        }


        /// <summary>
        /// Creates a change of basis rotation.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Rotate3d CreateChangeBasis(Rotate3d from, Rotate3d to)
        {
            return to.Apply(from.Inverse);
        }


        /// <summary>
        /// Creates a change of basis rotation.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static Rotate3d CreateChangeBasis(ref Rotate3d from, ref Rotate3d to)
        {
            return to.Apply(from.Inverse);
        }

        #endregion


        private Vec3d _x;
        private Vec3d _y;
        private Vec3d _z;


        /// <summary>
        /// 
        /// </summary>
        public Rotate3d(Vec3d x, Vec3d y)
            : this()
        {
            Set(x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotate3d(AxisAngle3d axisAngle)
            : this()
        {
            Set(axisAngle);
        }

        
        /// <summary>
        /// Assumes the given axes are orthonormal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private Rotate3d(Vec3d x, Vec3d y, Vec3d z)
        {
            _x = x;
            _y = y;
            _z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d X
        {
            get { return _x; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Y
        {
            get { return _y; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Z
        {
            get { return _z; }
        }


        /// <summary>
        /// Returns the inverse of this rotation
        /// </summary>
        public Rotate3d Inverse
        {
            get
            {
                return new Rotate3d(
                    new Vec3d(_x.X, _y.X, _z.X),
                    new Vec3d(_x.Y, _y.Y, _z.Y),
                    new Vec3d(_x.Z, _y.Z, _z.Z)
                    );
            }
        }


        /// <summary>
        /// Return false if this rotation is undefined.
        /// </summary>
        public bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }


        /// <summary>
        /// Returns true if the given vectors form a valid orthonormal basis i.e. they aren't parallel or zero length.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Set(Vec3d x, Vec3d y)
        {
            Vec3d z = Vec3d.Cross(x, y);
            double m = z.SquareLength;

            if (m > 0.0)
            {
                SetZX(z / Math.Sqrt(m), x / x.Length);
                return true;
            }

            return false;
        }

        
        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetXY(Vec3d x, Vec3d y)
        {
            _x = x;
            _y = y;
            _z = Vec3d.Cross(x, y);
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void SetYZ(Vec3d y, Vec3d z)
        {
            _x = Vec3d.Cross(y, z);
            _y = y;
            _z = z;
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        private void SetZX(Vec3d z, Vec3d x)
        {
            _x = x;
            _y = Vec3d.Cross(z, x);
            _z = z;
        }
        

        /// <summary>
        /// Swaps the x and y axes
        /// </summary>
        public void SwapXY()
        {
            var temp = _x;
            _x = _y;
            _y = temp;
        }


        /// <summary>
        /// Swaps the y and z axes
        /// </summary>
        public void SwapYZ()
        {
            var temp = _y;
            _y = _z;
            _z = temp;
        }


        /// <summary>
        /// Swaps the z and x axes
        /// </summary>
        public void SwapZX()
        {
            var temp = _z;
            _z = _x;
            _x = temp;
        }


        /// <summary>
        /// Flips the x and y axes
        /// </summary>
        public void FlipXY()
        {
            _x = -_x;
            _y = -_y;
        }


        /// <summary>
        /// Flips the y and z axes
        /// </summary>
        public void FlipYZ()
        {
            _y = -_y;
            _z = -_z;
        }


        /// <summary>
        /// Flips the z and x axes
        /// </summary>
        public void FlipZX()
        {
            _z = -_z;
            _x = -_x;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisAngle"></param>
        /// <returns></returns>
        public void Set(AxisAngle3d axisAngle)
        {
            var axis = axisAngle.Axis;
            var cos = axisAngle.CosAngle;
            var sin = axisAngle.SinAngle;
            double t = 1.0 - cos;

            _x.X = cos + axis.X * axis.X * t;
            _y.Y = cos + axis.Y * axis.Y * t;
            _z.Z = cos + axis.Z * axis.Z * t;

            double p0 = axis.X * axis.Y * t;
            double p1 = axis.Z * sin;

            _x.Y = p0 + p1;
            _y.X = p0 - p1;

            p0 = axis.X * axis.Z * t;
            p1 = axis.Y * sin;

            _x.Z = p0 - p1;
            _z.X = p0 + p1;

            p0 = axis.Y * axis.Z * t;
            p1 = axis.X * sin;

            _y.Z = p0 + p1;
            _z.Y = p0 - p1;
        }


        /// <summary>
        /// Inverts this rotation in place.
        /// </summary>
        public void Invert()
        {
            Swap(ref _x.Y, ref _y.X);
            Swap(ref _x.Z, ref _z.X);
            Swap(ref _y.Z, ref _z.Y);
        }


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d Apply(Vec3d vector)
        {
            return vector.X * _x + vector.Y * _y + vector.Z * _z;
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Rotate3d Apply(Rotate3d other)
        {
            other.SetXY(Apply(other._x), Apply(other._y));
            return other;
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d ApplyInverse(Vec3d vector)
        {
            return new Vec3d(Vec3d.Dot(vector, _x), Vec3d.Dot(vector, _y), Vec3d.Dot(vector, _z));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Rotate3d ApplyInverse(Rotate3d other)
        {
            other.SetXY(ApplyInverse(other._x), ApplyInverse(other._y));
            return other;
        }


        /// <summary>
        /// Rotates this basis around the given axis by the specified angle.
        /// </summary>
        /// <param name="axisAngle"></param>
        public void Rotate(AxisAngle3d axisAngle)
        {
            SetXY(axisAngle.Rotate(_x), axisAngle.Rotate(_y));
        }
    }
}
