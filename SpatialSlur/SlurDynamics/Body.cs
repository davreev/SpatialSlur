using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * TODO
 * Revise rotation representation as per
 * http://www.cg.informatik.uni-mainz.de/files/2016/06/Position-and-Orientation-Based-Cosserat-Rods.pdf
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
        private Vec3d _angleVelocity;
        private Vec3d _rotateSum;
        private double _rotateWeightSum;

        // TODO consider inertia tensor?
        // http://www.cs.unc.edu/~lin/COMP768-F07/LEC/rbd1.pdf
        // https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch29.html


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
            _angleVelocity *= damping;

            if (_rotateWeightSum > 0.0)
                _angleVelocity += _rotateSum * (timeStep / (_rotateWeightSum * Mass)); // TODO assumes uniform inertia tensor

            //_rotation.Rotate(new AxisAngle3d(_angleVelocity * timeStep));
            _rotation = new Quaterniond(_angleVelocity * timeStep) * _rotation;

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


        /// <summary>
        /// 
        /// </summary>
        Quaterniond IBody.Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        Vec3d IBody.AngularVelocity
        {
            get { return _angleVelocity; }
            set { _angleVelocity = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        void IBody.ApplyRotate(Vec3d delta, double weight)
        {
            ApplyRotate(delta, weight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        double IBody.UpdateRotation(double timeStep, double damping)
        {
            return UpdateRotation(timeStep, damping);
        }

        #endregion
    }
}
