using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Axis angle representation of a 3 dimensional rotation.
    /// </summary>
    [Serializable]
    public struct AxisAngle3d
    {
        #region Static

        /// <summary></summary>
        public static readonly AxisAngle3d Identity = new AxisAngle3d()
        {
            _axis = Vec3d.UnitZ,
            _cosAngle = 1.0
        };


        /// <summary>Describes a half rotation around the X axis</summary>
        public static readonly AxisAngle3d HalfX = new AxisAngle3d()
        {
            _axis = Vec3d.UnitX,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>Describes a half rotation around the Y axis</summary>
        public static readonly AxisAngle3d HalfY = new AxisAngle3d()
        {
            _axis = Vec3d.UnitY,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>Describes a half rotation around the Z axis</summary>
        public static readonly AxisAngle3d HalfZ = new AxisAngle3d()
        {
            _axis = Vec3d.UnitZ,
            _angle = Math.PI,
            _cosAngle = -1.0
        };


        /// <summary>
        /// Applies this rotation to the given vector.
        /// </summary>
        /// <param name="quatern"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vec3d operator *(AxisAngle3d rotation, Vec3d vector)
        {
            return rotation.Apply(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static AxisAngle3d CreateFromTo(Vec3d from, Vec3d to)
        {
            if (!from.Unitize() || !to.Unitize())
                return Identity;

            var ct = Vec3d.Dot(from, to);

            // parallel check
            if (1.0 - Math.Abs(ct) < SlurMath.ZeroTolerance)
            {
                // opposite check
                if (ct < 0.0)
                {
                    var perp = from.X < 1.0 ? from.CrossX() : from.CrossY();
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
            var axis = Vec3d.Cross(from, to);
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


        private Vec3d _axis;
        private double _angle;
        private double _cosAngle;
        private double _sinAngle;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public AxisAngle3d(Vec3d axis, double angle)
            : this()
        {
            Axis = axis;
            Angle = angle;
        }


        /// <summary>
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="rotation"></param>
        public AxisAngle3d(Vec3d rotation)
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
            Set(rotation);
        }


        /// <summary>
        /// Unit vector indicating the axis of rotation.
        /// </summary>
        public Vec3d Axis
        {
            get { return _axis; }
            set
            {
                double d = value.SquareLength;

                if (d > 0.0)
                    _axis = value / Math.Sqrt(d);
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
        /// 
        /// </summary>
        internal double CosAngle
        {
            get { return _cosAngle; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal double SinAngle
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
                return new AxisAngle3d()
                {
                    _axis = _axis,
                    _angle = -_angle,
                    _cosAngle = _cosAngle,
                    _sinAngle = -_sinAngle
                };
            }
        }


        /// <summary>
        /// Returns false if the axis is undefined.
        /// </summary>
        public bool IsValid
        {
            get { return _axis.SquareLength > 0.0; }
        }


        /// <summary>
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(Vec3d rotation)
        {
            var d = rotation.SquareLength;

            if (d > 0.0)
            {
                d = Math.Sqrt(d);
                _axis = rotation / d;
                Angle = d;
            }
            else
            {
                _axis = new Vec3d();
                _angle = 0.0;
                _cosAngle = 1.0;
                _sinAngle = 0.0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(Quaterniond rotation)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(OrthoBasis3d rotation)
        {
            Set(ref rotation);
        }


        /// <summary>
        /// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToAngle/index.htm
        /// </summary>
        /// <param name="rotation"></param>
        public void Set(ref OrthoBasis3d rotation)
        {
            throw new NotImplementedException();
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
        public Vec3d Apply(Vec3d vector)
        {
            return _cosAngle * vector +  _sinAngle * Vec3d.Cross(_axis, vector) + Vec3d.Dot(_axis, vector) * (1.0 - _cosAngle) * _axis;
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
        public void RotateAxis(Quaterniond rotation)
        {
            _axis = rotation.Apply(_axis);
        }


        /// <summary>
        /// Applies the given rotation to the axis of this rotation.
        /// </summary>
        /// <param name="rotation"></param>
        public void RotateAxis(OrthoBasis3d rotation)
        {
            RotateAxis(ref rotation);
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
    }
}
