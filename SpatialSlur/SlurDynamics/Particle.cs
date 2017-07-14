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
    public class Particle : IBody
    {
        private Vec3d _position;
        private Vec3d _velocity;

        private Vec3d _forceSum;
        private double _forceWeightSum;

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
        /// <param name="move"></param>
        /// <param name="weight"></param>
        public void ApplyMove(Vec3d move, double weight)
        {
            _forceSum += move * weight;
            _forceWeightSum += weight;
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

            if (_forceWeightSum > 0.0)
                _velocity += _forceSum * (timeStep / (_forceWeightSum * _mass));

            _position += _velocity * timeStep;

            _forceSum.Set(0.0);
            _forceWeightSum = 0.0;

            return _velocity.SquareLength;
        }


        #region Explicit interface implementations

        private static readonly string _message = "This implementation of IParticle does not support rotation.";


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
        Rotation3d IBody.Rotation
        {
            get { throw new NotSupportedException(_message); }
            set { throw new NotSupportedException(_message); }
        }


        /// <summary>
        /// 
        /// </summary>
        Vec3d IBody.AngularVelocity
        {
            get { throw new NotSupportedException(_message); }
            set { throw new NotSupportedException(_message); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void IBody.ApplyRotate(Vec3d delta, double weight)
        {
            throw new NotSupportedException(_message);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        double IUpdatable.UpdateRotation(double timeStep, double damping)
        {
            return 0.0;
        }

        #endregion
    }
}
