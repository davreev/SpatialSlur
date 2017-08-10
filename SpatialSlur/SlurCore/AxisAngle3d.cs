using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Axis angle representation of a 3d rotation.
    /// </summary>
    [Serializable]
    public struct AxisAngle3d
    {
        #region Static

        /// <summary></summary>
        public static readonly AxisAngle3d Zero = new AxisAngle3d(Vec3d.UnitZ, 0.0, 1.0, 0.0);

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
        /// <param name="vector"></param>
        public AxisAngle3d(Vec3d vector)
            : this()
        {
            Set(vector);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="cosAngle"></param>
        /// <param name="sinAngle"></param>
        private AxisAngle3d(Vec3d axis, double angle, double cosAngle, double sinAngle)
        {
            _axis = axis;
            _angle = angle;
            _cosAngle = cosAngle;
            _sinAngle = sinAngle;
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
        public double CosAngle
        {
            get { return _cosAngle; }
        }


        /// <summary>
        /// 
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
            get { return new AxisAngle3d(_axis, -_angle, _cosAngle, -_sinAngle); }
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
        /// <param name="vector"></param>
        public void Set(Vec3d vector)
        {
            var d = vector.SquareLength;

            if (d > 0.0)
            {
                d = Math.Sqrt(d);
                _axis = vector / d;
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
        /// Inverts this rotation in place.
        /// </summary>
        public void Invert()
        {
            _angle = -_angle;
            _sinAngle = -_sinAngle;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d Rotate(Vec3d vector)
        {
            return vector * _cosAngle + (_axis ^ vector) * _sinAngle + _axis * (_axis * vector) * (1.0 - _cosAngle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d RotateInverse(Vec3d vector)
        {
            return vector * _cosAngle - (_axis ^ vector) * _sinAngle + _axis * (_axis * vector) * (1.0 - _cosAngle);
        }
    }
}
