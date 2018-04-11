using System;
using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Orthonormal basis representation of a 3 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct OrthoBasis3d
    {
        #region Static

        /// <summary></summary>
        public static readonly OrthoBasis3d Identity = new OrthoBasis3d {
            _x = Vec3d.UnitX,
            _y = Vec3d.UnitY,
            _z = Vec3d.UnitZ
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public static implicit operator OrthoBasis3d(OrthoBasis2d rotation)
        {
            return new OrthoBasis3d {
                _x = rotation.X,
                _y = rotation.Y,
                _z = Vec3d.UnitZ
            };
        }


        /// <summary>
        /// Applies the given rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator *(OrthoBasis3d rotation, Vec3d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// Concatenates the given transformations by applying the first to the second.
        /// </summary>
        /// <param name="r0"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static OrthoBasis3d operator *(OrthoBasis3d r0, OrthoBasis3d r1)
        {
            r0.Apply(ref r1, ref r1);
            return r1;
        }


        /// <summary>
        /// Creates a relative rotation from r0 to r1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static OrthoBasis3d CreateFromTo(ref OrthoBasis3d from, ref OrthoBasis3d to)
        {
            return to.Apply(from.Inverse);
        }


        /// <summary>
        /// Creates the rotation between v0 and v1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromTo(Vec3d from, Vec3d to)
        {
            // impl ref
            // https://math.stackexchange.com/questions/180418/calculate-rotation-matrix-to-align-vector-a-to-vector-b-in-3d

            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var v = Vec3d.Cross(from, to);
            var c = Vec3d.Dot(from, to);
            var k = 1.0 / (1.0 + c);

            return new OrthoBasis3d {
                _x = new Vec3d(v.X * v.X * k + c, v.X * v.Y * k + v.Z, v.X * v.Z * k - v.Y),
                _y = new Vec3d(v.Y * v.X * k - v.Z, v.Y * v.Y * k + c, v.Y * v.Z * k + v.X),
                _z = new Vec3d(v.Z * v.X * k + v.Y, v.Z * v.Y * k - v.X, v.Z * v.Z * k + c)
            };
        }


        /// <summary>
        /// The y axis of the returned basis is aligned with the given direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateLookAt(Vec3d direction, Vec3d up)
        {
            GeometryUtil.Orthonormalize(ref direction, ref up, out Vec3d z);

            return new OrthoBasis3d {
                _x = z,
                _y = direction,
                _z = up
            };
        }

        #endregion


        private Vec3d _x;
        private Vec3d _y;
        private Vec3d _z;


        /// <summary>
        /// 
        /// </summary>
        public OrthoBasis3d(Vec3d x, Vec3d xy)
            : this()
        {
            Set(x, xy);
        }


        /// <summary>
        /// 
        /// </summary>
        public OrthoBasis3d(AxisAngle3d rotation)
            : this()
        {
            Set(ref rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        public OrthoBasis3d(Quaterniond quaternion)
            : this()
        {
            Set(quaternion);
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
        /// Returns the inverse of this rotation.
        /// </summary>
        public OrthoBasis3d Inverse
        {
            get
            {
                return new OrthoBasis3d {
                    _x = new Vec3d(_x.X, _y.X, _z.X),
                    _y = new Vec3d(_x.Y, _y.Y, _z.Y),
                    _z = new Vec3d(_x.Z, _y.Z, _z.Z)
                };
            }
        }


        /// <summary>
        /// Returns true if this rotation has been successfully initialized.
        /// </summary>
        public bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }


        /// <summary>
        /// Returns true if the given vectors form a valid orthonormal basis i.e. they aren't parallel or zero length.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        public bool Set(Vec3d x, Vec3d xy)
        {
            Vec3d z = Vec3d.Cross(x, xy);
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
        public bool Set(ref AxisAngle3d rotation)
        {
            if (!rotation.IsValid)
                return false;

            var axis = rotation.Axis;
            var cosAngle = rotation.CosAngle;
            var sinAngle = rotation.SinAngle;

            var t = 1.0 - cosAngle;

            _x.X = cosAngle + axis.X * axis.X * t;
            _y.Y = cosAngle + axis.Y * axis.Y * t;
            _z.Z = cosAngle + axis.Z * axis.Z * t;

            var c = axis.X * axis.Y * t;
            var s = axis.Z * sinAngle;

            _x.Y = c + s;
            _y.X = c - s;

            c = axis.X * axis.Z * t;
            s = axis.Y * sinAngle;

            _x.Z = c - s;
            _z.X = c + s;

            c = axis.Y * axis.Z * t;
            s = axis.X * sinAngle;

            _y.Z = c + s;
            _z.Y = c - s;

            return true;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public bool Set(Quaterniond rotation)
        {
            // impl ref 
            // http://www.cs.ucr.edu/~vbz/resources/quatut.pdf

            var d = rotation.SquareLength;

            if(d > 0.0)
            {
                var s = 2.0 / d;

                var x2 = rotation.X * s;
                var y2 = rotation.Y * s;
                var z2 = rotation.Z * s;

                var wx2 = rotation.W * x2;
                var wy2 = rotation.W * y2;
                var wz2 = rotation.W * z2;

                var xx2 = rotation.X * x2;
                var xy2 = rotation.X * y2;
                var xz2 = rotation.X * z2;

                var yy2 = rotation.Y * y2;
                var yz2 = rotation.Y * z2;
                var zz2 = rotation.Z * z2;

                _x.X = 1.0 - yy2 - zz2;
                _x.Y = xy2 + wz2;
                _x.Z = xz2 - wy2;

                _y.X = xy2 - wz2;
                _y.Y = 1.0 - xx2 - zz2;
                _y.Z = yz2 + wx2;

                _z.X = xz2 + wy2;
                _z.Y = yz2 - wx2;
                _z.Z = 1.0 - xx2 - yy2;

                return true;
            }

            return false;
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
        public OrthoBasis3d Apply(OrthoBasis3d other)
        {
            Apply(ref other, ref other);
            return other;
        }


        /// <summary>
        /// Applies this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Apply(ref OrthoBasis3d other)
        {
            Apply(ref other, ref other);
        }


        /// <summary>
        /// Applies this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void Apply(ref OrthoBasis3d other, ref OrthoBasis3d result)
        {
            result.SetXY(
                Apply(other._x),
                Apply(other._y)
                );
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
        public OrthoBasis3d ApplyInverse(OrthoBasis3d other)
        {
            ApplyInverse(ref other, ref other);
            return other;
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void ApplyInverse(ref OrthoBasis3d other)
        {
            ApplyInverse(ref other, ref other);
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public void ApplyInverse(ref OrthoBasis3d other, ref OrthoBasis3d result)
        {
            result.SetXY(
                ApplyInverse(other._x),
                ApplyInverse(other._y)
                );
        }


        /// <summary>
        /// Applies the given rotation to this object.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(ref AxisAngle3d rotation)
        {
            SetXY(rotation.Apply(_x), rotation.Apply(_y));
        }


        /// <summary>
        /// Applies the given rotation to this object.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Quaterniond rotation)
        {
            SetXY(rotation.Apply(_x), rotation.Apply(_y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref OrthoBasis3d other, double tolerance = SlurMath.ZeroTolerance)
        {
            return
                _x.ApproxEquals(other._x, tolerance) &&
                _y.ApproxEquals(other._y, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            return new Matrix3d(X, Y, Z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out Vec3d x, out Vec3d y, out Vec3d z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
