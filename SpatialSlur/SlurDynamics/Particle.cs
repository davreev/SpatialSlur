using System;
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
    public class Particle : IBody
    {
        #region Static

        private const string _rotationErrorMessage = "This implementation of IBody does not support rotation.";

        #endregion

        
        private Vec3d _position;
        private Vec3d _velocity;
        public Vec3d _forceSum;
        private Vec3d _moveSum;
        private double _moveWeightSum;
        private double _mass = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public Particle()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Particle(Vec3d position)
        {
            _position = position;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Particle(Particle other)
        {
            _position = other._position;
            _velocity = other._velocity;
            _mass = other._mass;
        }


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
        /// <param name="delta"></param>
        public void ApplyForce(Vec3d delta)
        {
            _forceSum += delta;
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
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        public double UpdatePosition(double timeStep, double damping)
        {
            _velocity *= (1.0 - damping);
            _velocity += _forceSum * (timeStep / _mass);

            if (_moveWeightSum > 0.0)
                _velocity += _moveSum * (timeStep / _moveWeightSum);

            Position += _velocity * timeStep;

            _forceSum = _moveSum = Vec3d.Zero;
            _moveWeightSum = 0.0;

            return _velocity.SquareLength;
        }


        #region Explicit interface implementations
        
        /// <summary>
        /// 
        /// </summary>
        Quaterniond IBody.Rotation
        {
            get { return Quaterniond.Identity; }
            set { throw new NotSupportedException(_rotationErrorMessage); }
        }


        /// <summary>
        /// 
        /// </summary>
        Vec3d IBody.AngularVelocity
        {
            get { return Vec3d.Zero; }
            set { throw new NotSupportedException(_rotationErrorMessage); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void IBody.ApplyTorque(Vec3d delta)
        {
            throw new NotSupportedException(_rotationErrorMessage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void IBody.ApplyRotate(Vec3d delta, double weight)
        {
            throw new NotSupportedException(_rotationErrorMessage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        double IBody.UpdateRotation(double timeStep, double damping)
        {
            return 0.0;
        }


        /// <summary>
        /// 
        /// </summary>
        bool IBody.HasRotation
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBody IBody.Duplicate()
        {
            return new Particle(this);
        }

        #endregion
    }
}
