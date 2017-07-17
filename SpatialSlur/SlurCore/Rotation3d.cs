using System;
using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a 3 dimensional right-handed orthonormal basis.
    /// </summary>
    [Serializable]
    public struct Rotation3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Rotation3d Identity = new Rotation3d(Vec3d.UnitX, Vec3d.UnitY, Vec3d.UnitZ);

        #endregion


        private Vec3d _x;
        private Vec3d _y;
        private Vec3d _z;


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
        /// Return false if this rotation is undefined.
        /// </summary>
        bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }



        /// <summary>
        /// Assumes the given axes are orthonormal
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private Rotation3d(Vec3d x, Vec3d y, Vec3d z)
        {
            _x = x;
            _y = y;
            _z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Vec3d x, Vec3d y)
            :this()
        {
            Set(x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(AxisAngle3d axisAngle)
            :this()
        {
            Set(axisAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Rotation3d other)
        {
            _x = other._x;
            _y = other._y;
            _z = other._z;
        }


        /// <summary>
        /// Returns true if the given vectors form a valid orthonormal basis i.e. they aren't parallel or zero length.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Set(Vec3d x, Vec3d y)
        {
            Vec3d z = x ^ y;
            double m = z.SquareLength;

            if (m > 0.0)
            {
                SetXZ(x / x.Length, z / Math.Sqrt(m));
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
            _z = x ^ y;
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        private void SetXZ(Vec3d x, Vec3d z)
        {
            _x = x;
            _y = z ^ x;
            _z = z;
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void SetYZ(Vec3d y, Vec3d z)
        {
            _x = y ^ z;
            _y = y;
            _z = z;
        }


        /// <summary>
        /// Returns false if the given axis is undefined.
        /// </summary>
        /// <param name="axisAngle"></param>
        /// <returns></returns>
        public bool Set(AxisAngle3d axisAngle)
        {
            if (!axisAngle.IsValid) return false;

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

            return true;
        }


        /// <summary>
        /// Inverts the rotation in place.
        /// </summary>
        public void Invert()
        {
            Swap(ref _x.Y, ref _y.X);
            Swap(ref _x.Z, ref _z.X);
            Swap(ref _y.Z, ref _z.Y);
        }


        /// <summary>
        /// Returns an inverted copy of this rotation.
        /// </summary>
        /// <returns></returns>
        public Rotation3d Inverted()
        {
            return new Rotation3d(
                new Vec3d(_x.X, _y.X, _z.X),
                new Vec3d(_x.Y, _y.Y, _z.Y),
                new Vec3d(_x.Z, _y.Z, _z.Z)
                );
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
        public void Apply(ref Rotation3d other)
        {
            other.SetXY(Apply(other._x), Apply(other._y));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d ApplyInverse(Vec3d vector)
        {
            return new Vec3d(vector * _x, vector * _y, vector * _z);
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public void ApplyInverse(ref Rotation3d other)
        {
            other.SetXY(ApplyInverse(other._x), ApplyInverse(other._y));
        }


        /// <summary>
        /// Rotates this basis around the given axis by the specified angle.
        /// </summary>
        /// <param name="axisAngle"></param>
        public void Rotate(AxisAngle3d axisAngle)
        {
            _x = axisAngle.Rotate(_x);
            _y = axisAngle.Rotate(_y);
            _z = _x ^ _y;
        }


        /*
        /// <summary>
        /// https://en.wikipedia.org/wiki/Rodrigues%27_rotation_formula
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        private void RotateImpl(Vec3d axis, double angle)
        {
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1.0 - c;

            _x = Rotate(_x);
            _y = Rotate(_y);
            _z = _x ^ _y;

            Vec3d Rotate(Vec3d v)
            {
                return v * c + (axis ^ v) * s + axis * (axis * v) * t;
            }
        }
        */


        /// <summary>
        /// Returns the axis angle representation of this rotation
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm
        /// </summary>
        /// <returns></returns>
        public AxisAngle3d ToAxisAngle()
        {
            throw new NotImplementedException();
        }
    }
}
