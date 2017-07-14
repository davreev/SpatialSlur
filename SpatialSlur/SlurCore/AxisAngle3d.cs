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
        private Vec3d _axis;
        private double _cosAngle;
        private double _sinAngle;


        /// <summary>
        /// 
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
            set
            {
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
        /// Returns false if the length of the axis is zero.
        /// </summary>
        public bool IsValid
        {
            get { return _axis.SquareLength > 0.0; }
        }


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
        /// The axis and angle of rotation are taken from the direction and length of the given vector respectively.
        /// </summary>
        /// <param name="vector"></param>
        public void Set(Vec3d vector)
        {
            var t = vector.SquareLength;

            if (t > 0.0)
            {
                t = Math.Sqrt(t);
                _axis = vector / t;
                _cosAngle = Math.Cos(t);
                _sinAngle = Math.Sin(t);
            }
            else
            {
                _axis = new Vec3d();
                _cosAngle = 1.0;
                _sinAngle = 0.0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d Rotate(Vec3d vector)
        {
            double t = 1.0 - _cosAngle;
            return vector * _cosAngle + (_axis ^ vector) * _sinAngle + _axis * (_axis * vector) * t;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vec3d RotateInv(Vec3d vector)
        {
            double t = 1.0 - _cosAngle;
            return vector * _cosAngle - (_axis ^ vector) * _sinAngle + _axis * (_axis * vector) * t;
        }
    }
}
