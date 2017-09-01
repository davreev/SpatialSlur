using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */
 
namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Body : IBody
    {
        private Vec3d _position;
        private Vec3d _velocity;
        private Vec3d _moveSum;
        private double _moveWeightSum;

        private Rotate3d _rotation = Rotate3d.Identity;
        private Vec3d _angleVelocity;
        private Vec3d _rotateSum; // direction = rotation axis, length = rotation angle
        private double _rotateWeightSum;

        private double _mass = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Position
        {
            get { return _position; }
            set { _position = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double Mass
        {
            get { return _mass; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("The value must be greater than zero.");

                _mass = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Rotate3d Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d AngularVelocity
        {
            get { return _angleVelocity; }
            set { _angleVelocity = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Body()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Body(Vec3d position)
        {
            _position = position;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Body(Vec3d position, Rotate3d rotation)
        {
            _position = position;
            _rotation = rotation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        public void ApplyMove(Vec3d delta, double weight)
        {
            _moveSum += delta * weight;
            _moveWeightSum += weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        public void ApplyRotate(Vec3d delta, double weight)
        {
            _rotateSum += delta * weight;
            _rotateWeightSum += weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        public double UpdatePosition(double timeStep, double damping)
        {
            _velocity *= damping;

            if (_moveWeightSum > 0.0)
                _velocity += _moveSum * (timeStep / (_moveWeightSum * _mass));

            _position += _velocity * timeStep;

            _moveSum.Set(0.0);
            _moveWeightSum = 0.0;

            return _velocity.SquareLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        public double UpdateRotation(double timeStep, double damping)
        {
            _angleVelocity *= damping;

            if (_rotateWeightSum > 0.0)
                _angleVelocity += _rotateSum  * (timeStep / (_rotateWeightSum * _mass));

            _rotation.Rotate(new AxisAngle3d(_angleVelocity * timeStep));

            _rotateSum.Set(0.0);
            _rotateWeightSum = 0.0;

            return _angleVelocity.SquareLength;
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        bool IBody.HasRotation
        {
            get { return true; }
        }

        #endregion
    }
}
