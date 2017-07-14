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
    public class Body : IBody
    {
        private Vec3d _position;
        private Vec3d _velocity;
        private Vec3d _forceSum;
        private double _forceWeightSum;

        private Rotation3d _rotation;
        private Vec3d _angleVelocity;
        private Vec3d _torqueSum; // direction = rotation axis, length = rotation angle
        private double _torqueWeightSum;

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
        public Rotation3d Rotation
        {
            get { return _rotation; }
            set { _rotation = value ?? throw new ArgumentNullException(); }
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
            _rotation = new Rotation3d();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Body(Vec3d position)
        {
            _position = position;
            Rotation = new Rotation3d();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Body(Vec3d position, Rotation3d rotation)
        {
            _position = position;
            Rotation = rotation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotationX"></param>
        /// <param name="rotationY"></param>
        public Body(Vec3d position, Vec3d rotationX, Vec3d rotationY)
        {
            _position = position;
            _rotation = new Rotation3d(rotationX, rotationY);
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
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        public void ApplyRotate(Vec3d delta, double weight)
        {
            _torqueSum += delta * weight;
            _torqueWeightSum += weight;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        public double UpdateRotation(double timeStep, double damping)
        {
            _angleVelocity *= damping;

            if (_torqueWeightSum > 0.0)
                _angleVelocity += _torqueSum  * (timeStep / (_torqueWeightSum * Mass));

            _rotation.Rotate(_angleVelocity * timeStep);

            _torqueSum.Set(0.0);
            _torqueWeightSum = 0.0;

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
