using System;
using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Orthogonal matrix representation of a 3 dimensional rotation.
    /// </summary>
    [Obsolete("Renamed to Ortho3d")]
    [Serializable]
    public struct Rotation3d
    {
        #region Static

        /// <summary></summary>
        public static readonly Rotation3d Identity = new Rotation3d(Vec3d.UnitX, Vec3d.UnitY, Vec3d.UnitZ);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator Rotation3d(Rotation2d rotation)
        {
            return new Rotation3d(rotation.X, rotation.Y, Vec3d.UnitZ);
        }


        /// <summary>
        /// Applies the given rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator *(Rotation3d rotation, Vec3d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// Concatenates the given rotations as per rules of matrix multiplication.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Rotation3d operator *(Rotation3d r0, Rotation3d r1)
        {
            r0.Apply(ref r1);
            return r1;
        }


        /// <summary>
        /// Creates a relative rotation from r0 to r1.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static Rotation3d CreateRelative(Rotation3d r0, Rotation3d r1)
        {
            return r1.Apply(r0.Inverse);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        public static Rotation3d CreateRelative(ref Rotation3d r0, ref Rotation3d r1)
        {
            return r1.Apply(r0.Inverse);
        }

        #endregion


        private Vec3d _x;
        private Vec3d _y;
        private Vec3d _z;


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Vec3d x, Vec3d y)
            : this()
        {
            Set(x, y);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(AxisAngle3d rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotation3d(Quaterniond quaternion)
            : this()
        {
            Set(quaternion);
        }


        /// <summary>
        /// Assumes the given axes are orthonormal.
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
        public Rotation3d Inverse
        {
            get
            {
                return new Rotation3d(
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
        internal void SetXY(Vec3d x, Vec3d y)
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
        internal void SetYZ(Vec3d y, Vec3d z)
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
        internal void SetZX(Vec3d z, Vec3d x)
        {
            _x = x;
            _y = Vec3d.Cross(z, x);
            _z = z;
        }
        

        /// <summary>
        /// Swaps the x and y axes.
        /// Also flips the z axis to preserve handedness.
        /// </summary>
        public void SwapXY()
        {
            var temp = _x;
            _x = _y;
            _y = temp;
            _z = -_z;
        }


        /// <summary>
        /// Swaps the y and z axes.
        /// Also flips the x axis to preserve handedness.
        /// </summary>
        public void SwapYZ()
        {
            var temp = _y;
            _y = _z;
            _z = temp;
            _x = -_x;
        }


        /// <summary>
        /// Swaps the z and x axes
        /// Also flips the y axis to preserve handedness.
        /// </summary>
        public void SwapZX()
        {
            var temp = _z;
            _z = _x;
            _x = temp;
            _y = -_y;
        }


        /// <summary>
        /// Reverses all axes.
        /// </summary>
        public void Flip()
        {
            _x.Negate();
            _y.Negate();
            _z.Negate();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public void Set(AxisAngle3d rotation)
        {
            var axis = rotation.Axis;
            var cos = rotation.CosAngle;
            var sin = rotation.SinAngle;
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
        /// 
        /// </summary>
        /// <param name="quaternion"></param>
        public void Set(Quaterniond quaternion)
        {
            throw new NotImplementedException();
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
        public Rotation3d Apply(Rotation3d other)
        {
            Apply(ref other);
            return other;
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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
            return new Vec3d(Vec3d.Dot(vector, _x), Vec3d.Dot(vector, _y), Vec3d.Dot(vector, _z));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Rotation3d ApplyInverse(Rotation3d other)
        {
            ApplyInverse(ref other);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void ApplyInverse(ref Rotation3d other)
        {
            other.SetXY(ApplyInverse(other._x), ApplyInverse(other._y));
        }
    }
}
