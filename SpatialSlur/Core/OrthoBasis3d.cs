
/*
 * Notes
 */

using System;
using static SpatialSlur.Utilities;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Orthonormal basis representation of a 3 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct OrthoBasis3d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly OrthoBasis3d Identity = new OrthoBasis3d
        {
            _x = Vector3d.UnitX,
            _y = Vector3d.UnitY,
            _z = Vector3d.UnitZ
        };


        /// <summary>
        /// Applies the given rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d operator *(OrthoBasis3d rotation, Vector3d vector)
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
            return r0.Apply(ref r1);
        }


        /// <summary>
        /// Creates a relative rotation from r0 to r1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static OrthoBasis3d CreateFromTo(OrthoBasis3d from, OrthoBasis3d to)
        {
            return CreateFromTo(ref from, ref to);
        }


        /// <summary>
        /// Creates a relative rotation from r0 to r1.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static OrthoBasis3d CreateFromTo(ref OrthoBasis3d from, ref OrthoBasis3d to)
        {
            var inv = from.Inverse;
            return to.Apply(ref inv);
        }


        /// <summary>
        /// Creates the rotation between v0 and v1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromTo(Vector3d from, Vector3d to)
        {
            // impl ref
            // https://math.stackexchange.com/questions/180418/calculate-rotation-matrix-to-align-vector-a-to-vector-b-in-3d

            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var v = Vector3d.Cross(from, to);
            var c = Vector3d.Dot(from, to);
            var k = 1.0 / (1.0 + c);

            return new OrthoBasis3d
            {
                _x = new Vector3d(v.X * v.X * k + c, v.X * v.Y * k + v.Z, v.X * v.Z * k - v.Y),
                _y = new Vector3d(v.Y * v.X * k - v.Z, v.Y * v.Y * k + c, v.Y * v.Z * k + v.X),
                _z = new Vector3d(v.Z * v.X * k + v.Y, v.Z * v.Y * k - v.X, v.Z * v.Z * k + c)
            };
        }


        /// <summary>
        /// The y axis of the returned basis is aligned with the given direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateLookAt(Vector3d direction, Vector3d up)
        {
            Geometry.Orthonormalize(ref direction, ref up, out Vector3d right);

            return new OrthoBasis3d
            {
                _x = right,
                _y = direction,
                _z = up
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromXY(Vector3d x, Vector3d xy)
        {
            OrthoBasis3d result = default;
            result.SetXY(x, xy);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromYX(Vector3d y, Vector3d xy)
        {
            OrthoBasis3d result = default;
            result.SetYX(y, xy);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="yz"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromYZ(Vector3d y, Vector3d yz)
        {
            OrthoBasis3d result = default;
            result.SetYZ(y, yz);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="yz"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromZY(Vector3d z, Vector3d yz)
        {
            OrthoBasis3d result = default;
            result.SetZY(z, yz);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="xz"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromZX(Vector3d z, Vector3d xz)
        {
            OrthoBasis3d result = default;
            result.SetZX(z, xz);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xz"></param>
        /// <returns></returns>
        public static OrthoBasis3d CreateFromXZ(Vector3d x, Vector3d xz)
        {
            OrthoBasis3d result = default;
            result.SetXY(x, xz);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal static OrthoBasis3d CreateFrom2d(OrthoBasis2d other)
        {
            return new OrthoBasis3d
            {
                _x = other.X.As3d,
                _y = other.Y.As3d,
                _z = Vector3d.UnitZ
            };
        }

        #endregion


        private Vector3d _x;
        private Vector3d _y;
        private Vector3d _z;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public OrthoBasis3d(Vector3d rotation)
            :this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        public OrthoBasis3d(AxisAngle3d rotation)
            : this()
        {
            Set(rotation);
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
        public Vector3d X
        {
            get { return _x; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Y
        {
            get { return _y; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Z
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
                return new OrthoBasis3d
                {
                    _x = new Vector3d(_x.X, _y.X, _z.X),
                    _y = new Vector3d(_x.Y, _y.Y, _z.Y),
                    _z = new Vector3d(_x.Z, _y.Z, _z.Z)
                };
            }
        }


        /// <summary>
        /// Returns true if this rotation has non-zero axes.
        /// </summary>
        public bool IsValid
        {
            get { return _x.SquareLength > 0.0; }
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        public bool SetXY(Vector3d x, Vector3d xy)
        {
            Vector3d z = Vector3d.Cross(x, xy);
            double m = z.SquareLength;

            if (m > 0.0)
            {
                SetOrthoZX(z / Math.Sqrt(m), x / x.Length);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        public bool SetYX(Vector3d y, Vector3d xy)
        {
            Vector3d z = Vector3d.Cross(xy, y);
            double m = z.SquareLength;

            if (m > 0.0)
            {
                SetOrthoYZ(y / y.Length, z / Math.Sqrt(m));
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="yz"></param>
        /// <returns></returns>
        public bool SetYZ(Vector3d y, Vector3d yz)
        {
            Vector3d x = Vector3d.Cross(y, yz);
            double m = x.SquareLength;

            if (m > 0.0)
            {
                SetOrthoXY(x / Math.Sqrt(m), y / y.Length);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="yz"></param>
        /// <returns></returns>
        public bool SetZY(Vector3d z, Vector3d yz)
        {
            Vector3d x = Vector3d.Cross(yz, z);
            double m = x.SquareLength;

            if (m > 0.0)
            {
                SetOrthoZX(z / z.Length, x / Math.Sqrt(m));
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="xz"></param>
        /// <returns></returns>
        public bool SetZX(Vector3d z, Vector3d xz)
        {
            Vector3d y = Vector3d.Cross(z, xz);
            double m = y.SquareLength;

            if (m > 0.0)
            {
                SetOrthoYZ(y / Math.Sqrt(m), z / z.Length);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if this basis was successfully set.
        /// </summary>
        public bool SetXZ(Vector3d x, Vector3d xz)
        {
            Vector3d y = Vector3d.Cross(xz, x);
            double m = y.SquareLength;

            if (m > 0.0)
            {
                SetOrthoXY(x / x.Length, y / Math.Sqrt(m));
                return true;
            }

            return false;
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SetOrthoXY(Vector3d x, Vector3d y)
        {
            _x = x;
            _y = y;
            _z = Vector3d.Cross(x, y);
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void SetOrthoYZ(Vector3d y, Vector3d z)
        {
            _x = Vector3d.Cross(y, z);
            _y = y;
            _z = z;
        }


        /// <summary>
        /// Assumes the given vectors are orthonormal.
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        private void SetOrthoZX(Vector3d z, Vector3d x)
        {
            _x = x;
            _y = Vector3d.Cross(z, x);
            _z = z;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(Vector3d rotation)
        {
            var angle = rotation.SquareLength;

            if (angle > 0.0)
            {
                angle = Math.Sqrt(angle);
                SetImpl(rotation / angle, Math.Cos(angle), Math.Sin(angle));
                return;
            }

            this = Identity;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public bool Set(AxisAngle3d rotation)
        {
            if (rotation.IsValid)
            {
                SetImpl(rotation.Axis, rotation.CosAngle, rotation.SinAngle);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Assumes the given axis is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        /// <returns></returns>
        private void SetImpl(Vector3d axis, double cosAngle, double sinAngle)
        {
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
        public Vector3d Apply(Vector3d vector)
        {
            return vector.X * _x + vector.Y * _y + vector.Z * _z;
        }


        /// <summary>
        /// Applies this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public OrthoBasis3d Apply(OrthoBasis3d other)
        {
            return Apply(ref other);
        }


        /// <summary>
        /// Applies this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public OrthoBasis3d Apply(ref OrthoBasis3d other)
        {
            var x = Apply(other._x);
            var y = Apply(other._y);

            return new OrthoBasis3d
            {
                _x = x,
                _y = y,
                _z = Vector3d.Cross(x, y)
            };
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3d ApplyInverse(Vector3d vector)
        {
            return new Vector3d(Vector3d.Dot(vector, _x), Vector3d.Dot(vector, _y), Vector3d.Dot(vector, _z));
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public OrthoBasis3d ApplyInverse(OrthoBasis3d other)
        {
            return ApplyInverse(ref other);
        }


        /// <summary>
        /// Applies the inverse of this rotation to the given rotation in place.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public OrthoBasis3d ApplyInverse(ref OrthoBasis3d other)
        {
            var x = ApplyInverse(other._x);
            var y = ApplyInverse(other._y);

            return new OrthoBasis3d
            {
                _x = x,
                _y = y,
                _z = Vector3d.Cross(x, y)
            };
        }


        /// <summary>
        /// Applies the given rotation to this object.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(AxisAngle3d rotation)
        {
            SetOrthoXY(rotation.Apply(_x), rotation.Apply(_y));
        }


        /// <summary>
        /// Applies the given rotation to this object.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Quaterniond rotation)
        {
            SetOrthoXY(rotation.Apply(_x), rotation.Apply(_y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(OrthoBasis3d other, double epsilon = D.ZeroTolerance)
        {
            return ApproxEquals(ref other, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(ref OrthoBasis3d other, double epsilon = D.ZeroTolerance)
        {
            return
                _x.ApproxEquals(other._x, epsilon) &&
                _y.ApproxEquals(other._y, epsilon);
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
        public void Deconstruct(out Vector3d x, out Vector3d y, out Vector3d z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
