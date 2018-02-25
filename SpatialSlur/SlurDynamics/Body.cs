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
    public class Body : Particle, IBody
    {
        private Quaterniond _rotation = Quaterniond.Identity;
        private Vec3d _angleVeloctiy;
        private Vec3d _torqueSum;
        private Vec3d _rotateSum;
        private double _rotateWeightSum;
  

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
            : base(position)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public Body(Vec3d position, Quaterniond rotation)
            :base(position)
        {
            _rotation = rotation;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Body(Body other)
            :base(other)
        {
            _rotation = other._rotation;
            _angleVeloctiy = other._angleVeloctiy;
        }


        /// <summary>
        /// 
        /// </summary>
        public Quaterniond Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vec3d AngularVelocity
        {
            get { return _angleVeloctiy; }
            set { _angleVeloctiy = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void ApplyTorque(Vec3d delta)
        {
            _torqueSum += delta;
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
        public double UpdateRotation(double timeStep, double damping)
        {
            _angleVeloctiy *= (1.0 - damping);
            _angleVeloctiy += _torqueSum * (timeStep / Mass);

            if (_rotateWeightSum > 0.0)
                _angleVeloctiy += _rotateSum * (timeStep / _rotateWeightSum);

            _rotation = new Quaterniond(_angleVeloctiy * timeStep) * _rotation;

            _torqueSum = _rotateSum = Vec3d.Zero;
            _rotateWeightSum = 0.0;

            return _angleVeloctiy.SquareLength;
        }


        /*
        // TODO consider inertia tensor?
        // http://www.cs.unc.edu/~lin/COMP768-F07/LEC/rbd1.pdf
        // https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch29.html

        private Vec3d _invInertia = new Vec3d(1.0, 1.0, 1.0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        private double UpdateRotation2(double timeStep, double damping)
        {
            _angleVeloctiy *= (1.0 - damping);
            _angleVeloctiy += _rotation.Apply(_rotation.Inverse.Apply(_torqueSum * (timeStep * InverseMass)) * _inertiaInv);
 
            if (_rotateWeightSum > 0.0)
                _angleVeloctiy += _rotateSum * (timeStep / _rotateWeightSum);

            _rotation = new Quaterniond(_angleVeloctiy * timeStep) * _rotation;

            _rotateSum = _torqueSum = Vec3d.Zero;
            _rotateWeightSum = 0.0;

            return _angleVeloctiy.SquareLength;
        }
        */

        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        bool IBody.HasRotation
        {
            get { return true; }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBody IBody.Duplicate()
        {
            return new Body(this);
        }

        #endregion
    }
}
