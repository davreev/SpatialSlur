
/*
 * Notes
 */

using System;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur
{
    /// <summary>
    /// Axis angle representation of a 3 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct AxisAngle3d
    {
        #region Static Members

        /// <summary></summary>
        public static readonly AxisAngle3d Identity = new AxisAngle3d {
            _axis = Vector3d.UnitZ,
            _cosAngle = 1.0
        };


        /// <summary>Describes a half rotation around the X axis</summary>
        public static readonly AxisAngle3d HalfX = new AxisAngle3d {
            _axis = Vector3d.UnitX,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>Describes a half rotation around the Y axis</summary>
        public static readonly AxisAngle3d HalfY = new AxisAngle3d {
            _axis = Vector3d.UnitY,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>Describes a half rotation around the Z axis</summary>
        public static readonly AxisAngle3d HalfZ = new AxisAngle3d {
            _axis = Vector3d.UnitZ,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3d operator *(AxisAngle3d rotation, Vector3d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static AxisAngle3d CreateFromTo(Vector3d from, Vector3d to)
        {
            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var ct = Vector3d.Dot(from, to);

            // parallel check
            if (1.0 - Math.Abs(ct) < D.ZeroTolerance)
            {
                // opposite check
                if (ct < 0.0)
                {
                    var perp = from.X < 1.0 ? from.CrossX : from.CrossY;
                    return new AxisAngle3d()
                    {
                        _axis = perp / perp.Length,
                        _angle = Math.PI,
                        _cosAngle = -1.0,
                        _sinAngle = 0.0
                    };
                }

                return Identity;
            }

            // can assume axis is valid
            var axis = Vector3d.Cross(from, to);
            var st = axis.Length;

            return new AxisAngle3d()
            {
                _axis = axis / st,
                _angle = Math.Acos(ct),
                _sinAngle = st,
                _cosAngle = ct
            };
        }

        #endregion


        private Vector3d _axis;
        private double _angle;
        private double _cosAngle;
        private double _sinAngle;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public AxisAngle3d(Vector3d axis, double angle)
            : this()
        {
            Axis = axis;
            Angle = angle;
        }


        /// <summary>
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="rotation"></param>
        public AxisAngle3d(Vector3d rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public AxisAngle3d(Quaterniond rotation)
            : this()
        {
            Set(rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public AxisAngle3d(OrthoBasis3d rotation)
            : this()
        {
            Set(ref rotation);
        }


        /// <summary>
        /// Unit vector indicating the axis of rotation.
        /// </summary>
        public Vector3d Axis
        {
            get { return _axis; }
            set
            {
                if (value.Unitize())
                    _axis = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                _cosAngle = Math.Cos(value);
                _sinAngle = Math.Sin(value);
            }
        }


        /// <summary>
        /// Returns the cached cosine of the angle
        /// </summary>
        public double CosAngle
        {
            get { return _cosAngle; }
        }


        /// <summary>
        /// Returns the cached sine of the angle
        /// </summary>
        public double SinAngle
        {
            get { return _sinAngle; }
        }


        /// <summary>
        /// Returns the inverse of this rotation
        /// </summary>
        public AxisAngle3d Inverse
        {
            get
            {
                var r = this;
                r.Invert();
                return r;
            }
        }


        /// <summary>
        /// Returns true if this rotation has a non-zero axis.
        /// </summary>
        public bool IsValid
        {
            get { return _axis.SquareLength > 0.0; }
        }


        /// <summary>
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="rotation"></param>
        public bool Set(Vector3d rotation)
        {
            var d = rotation.SquareLength;

            if (d > 0.0)
            {
                d = Math.Sqrt(d);
                _axis = rotation / d;
                Angle = d;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public bool Set(Quaterniond rotation)
        {
            if (!rotation.Unitize())
                return false;

            var c2 = rotation.W;
            var s2 = 1.0 - c2 * c2; // pythag's identity

            if (s2 > 0.0)
            {
                s2 = Math.Sqrt(s2);
                var s2Inv = 1.0 / s2;

                _axis.X = rotation.X * s2Inv;
                _axis.Y = rotation.Y * s2Inv;
                _axis.Z = rotation.Z * s2Inv;
                _angle = 2.0 * Math.Acos(c2);

                // double-angle identities
                _cosAngle = 2.0 * c2 * c2 - 1.0;
                _sinAngle = 2.0 * c2 * s2;
                return true;
            }
            
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public bool Set(OrthoBasis3d rotation)
        {
            return Set(ref rotation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public bool Set(ref OrthoBasis3d rotation)
        {
            return Set(new Quaterniond(rotation));
        }


        /// <summary>
        /// Inverts this rotation in place.
        /// </summary>
        public void Invert()
        {
            _angle = -_angle;
            _sinAngle = -_sinAngle;
        }


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3d Apply(Vector3d vector)
        {
            return _cosAngle * vector +  _sinAngle * Vector3d.Cross(_axis, vector) + Vector3d.Dot(_axis, vector) * (1.0 - _cosAngle) * _axis;
        }
        

        /// <summary>
        /// Applies the given rotation to the axis of this rotation.
        /// </summary>
        /// <param name="rotation"></param>
        public void RotateAxis(AxisAngle3d rotation)
        {
            _axis = rotation.Apply(_axis);
        }


        /// <summary>
        /// Applies the given rotation to the axis of this rotation.
        /// </summary>
        /// <param name="rotation"></param>
        public void RotateAxis(ref OrthoBasis3d rotation)
        {
            _axis = rotation.Apply(_axis);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool ApproxEquals(AxisAngle3d other, double epsilon = D.ZeroTolerance)
        {
            return
                SlurMath.ApproxEquals(_angle, other._angle, epsilon) &&
                _axis.ApproxEquals(other._axis, epsilon);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix3d ToMatrix()
        {
            var t = 1.0 - _cosAngle;

            var cxy = _axis.X * _axis.Y * t;
            var sz = _axis.Z * _sinAngle;

            var cxz = _axis.X * _axis.Z * t;
            var sy = _axis.Y * _sinAngle;

            var cyz = _axis.Y * _axis.Z * t;
            var sx = _axis.X * _sinAngle;
            
            return new Matrix3d(
                _cosAngle + _axis.X * _axis.X * t, cxy - sz, cxz + sy,
                cxy + sz, _cosAngle + _axis.Y * _axis.Y * t, cyz - sx,
                cxz - sy, cyz + sx, _cosAngle + _axis.Z * _axis.Z * t
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Deconstruct(out Vector3d axis, out double angle)
        {
            axis = _axis;
            angle = _angle;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        public void Deconstruct(out Vector3d axis, out double angle, out double cosAngle, out double sinAngle)
        {
            axis = _axis;
            angle = _angle;
            cosAngle = _cosAngle;
            sinAngle = _sinAngle;
        }
    }
}
