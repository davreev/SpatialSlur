using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Represents a 3d right-handed orthonormal basis.
    /// </summary>
    public class Rotation3d
    {
        private Vec3d _x = Vec3d.UnitX;
        private Vec3d _y = Vec3d.UnitY;
        private Vec3d _z = Vec3d.UnitZ;


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
        /// 
        /// </summary>
        public Rotation3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Vec3d x, Vec3d y)
        {
            Set(x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(AxisAngle3d axisAngle)
        {
            Set(axisAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Rotation3d other)
        {
            _x = other._x;
            _x = other._x;
            _x = other._x;
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
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="vector"></param>
        public bool Set(Vec3d vector)
        {
            double t = vector.SquareLength;

            if (t > 0.0)
            {
                t = Math.Sqrt(t);
                SetImpl(vector / t, Math.Cos(t), Math.Sin(t));
                return true;
            }

            return false;
        }


        /// <summary>
        /// The angle of rotation is taken from the length of the given axis.
        /// </summary>
        /// <param name="axisAngle"></param>
        /// <returns></returns>
        public bool Set(AxisAngle3d axisAngle)
        {
            if(axisAngle.IsValid)
            {
                SetImpl(axisAngle.Axis, axisAngle.CosAngle, axisAngle.SinAngle);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        private void SetImpl(Vec3d axis, double cosAngle, double sinAngle)
        {
            double t = 1.0 - cosAngle;

            _x.X = cosAngle + axis.X * axis.X * t;
            _y.Y = cosAngle + axis.Y * axis.Y * t;
            _z.Z = cosAngle + axis.Z * axis.Z * t;

            double p0 = axis.X * axis.Y * t;
            double p1 = axis.Z * sinAngle;

            _x.Y = p0 + p1;
            _y.X = p0 - p1;

            p0 = axis.X * axis.Z * t;
            p1 = axis.Y * sinAngle;

            _x.Z = p0 - p1;
            _z.X = p0 + p1;

            p0 = axis.Y * axis.Z * t;
            p1 = axis.X * sinAngle;

            _y.Z = p0 + p1;
            _z.Y = p0 - p1;
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
        public void Apply(Rotation3d other)
        {
            Apply(other, other);
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void Apply(Rotation3d other, Rotation3d result)
        {
            result.SetXY(Apply(other._x), Apply(other._y));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d ApplyInv(Vec3d vector)
        {
            return new Vec3d(vector * _x, vector * _y, vector * _z);
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        public void ApplyInv(Rotation3d other)
        {
            ApplyInv(other, other);
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result"></param>
        public void ApplyInv(Rotation3d other, Rotation3d result)
        {
            result.SetXY(ApplyInv(other._x), ApplyInv(other._y));
        }


        /// <summary>
        /// Rotates this basis around the given axis by the specified angle.
        /// Returns false if the given axis is a zero vector.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public bool Rotate(Vec3d axis, double angle)
        {
            if (axis.Unitize())
            {
                RotateImpl(axis, angle);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Rotates this basis around the given axis.
        /// The angle is taken from the length of the given axis.
        /// </summary>
        /// <param name="axisAngle"></param>
        public void Rotate(Vec3d axisAngle)
        {
            double d = axisAngle.SquareLength;

            if (d > 0.0)
            {
                d = Math.Sqrt(d);
                RotateImpl(axisAngle / d, d);
            }
        }


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
    }
}
