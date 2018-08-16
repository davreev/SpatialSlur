
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class BodyPosition
    {
        /// <summary>The current position of this body</summary>
        public Vector3d Current;

        /// <summary>The velocity of this body</summary>
        public Vector3d Velocity;
        
        private Vector3d _forceSum;
        private Vector3d _deltaSum;
        private double _weightSum;

        private double _mass = 1.0;
        private double _massInv = 1.0;


        /// <summary>
        /// 
        /// </summary>
        public BodyPosition()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public BodyPosition(Vector3d current)
        {
            Current = current;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private BodyPosition(BodyPosition other)
        {
            Current = other.Current;
            Velocity = other.Velocity;
            _mass = other._mass;
            _massInv = other._massInv;
        }


        /// <summary>
        /// Returns a deep copy of this object.
        /// </summary>
        /// <returns></returns>
        public BodyPosition Duplicate()
        {
            return new BodyPosition(this);
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
                _massInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void AddForce(Vector3d force)
        {
            _forceSum += force;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="weight"></param>
        public void AddDelta(Vector3d delta, double weight)
        {
            _deltaSum += delta * weight;
            _weightSum += weight;
        }


        /// <summary>
        /// Clears all deltas and forces applied to the position of this body.
        /// </summary>
        public void Clear()
        {
            _forceSum = _deltaSum = Vector3d.Zero;
            _weightSum = 0.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        public void Update(double timeStep, double damping)
        {
            var v = Velocity * (1.0 - damping) + _forceSum * (timeStep * _massInv);

            if (_weightSum > 0.0)
                v += _deltaSum * (timeStep * _massInv / _weightSum);
            
            Current += v * timeStep;
            Velocity = v;
            Clear();
        }
    }
}
