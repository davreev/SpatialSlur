
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class BodyRotation
    {
        /// <summary>The current rotation of this body</summary>
        public Quaterniond Current = Quaterniond.Identity;

        /// <summary>The rotational velocity of this body</summary>
        public Vector3d Velocity;
        
        private Vector3d _torqueSum;
        private Vector3d _deltaSum;
        private double _weightSum;

        private Vector3d _inertia = new Vector3d(1.0);
        private Vector3d _inertiaInv = new Vector3d(1.0);


        /// <summary>
        /// 
        /// </summary>
        public BodyRotation()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public BodyRotation(Quaterniond current)
        {
            Current = current;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private BodyRotation(BodyRotation other)
        {
            Current = other.Current;
            Velocity = other.Velocity;
            _inertia = other._inertia;
            _inertiaInv = other._inertiaInv;
        }


        /// <summary>
        /// Returns a deep copy of this object.
        /// </summary>
        /// <returns></returns>
        public BodyRotation Duplicate()
        {
            return new BodyRotation(this);
        }


        /// <summary>
        /// Gets/sets the rotational mass of the body.
        /// Assumes the current rotation is aligned with the body's principal axes of inertia.
        /// </summary>
        public Vector3d Inertia
        {
            get { return _inertia; }
            set
            {
                if (value.X <= 0.0 || value.Y <= 0.0 || value.Z <= 0.0)
                    throw new ArgumentException("The value must be greater than zero in all dimensions.");

                _inertia = value;
                _inertiaInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="torque"></param>
        public void AddTorque(Vector3d torque)
        {
            _torqueSum += torque;
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
        /// Clears all deltas and torque applied to the rotation of this body.
        /// </summary>
        public void Clear()
        {
            _torqueSum = _deltaSum = Vector3d.Zero;
            _weightSum = 0.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        public void Update(double timeStep, double damping)
        {
            var r = Current.ToMatrix();
            var v = Velocity * (1.0 - damping) + r.Apply(_inertiaInv * r.ApplyTranspose(_torqueSum)) * timeStep;

            if (_weightSum > 0.0)
                v += r.Apply(r.ApplyTranspose(_deltaSum) * _inertiaInv) * (timeStep / _weightSum);

            Current = new Quaterniond(v * timeStep) * Current;
            Velocity = v;
            Clear();
        }
    }
}
