using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = BodyHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnRotation : Constraint, IConstraint
    {
        private H _handle = new H();
        private Quaterniond _rotation;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnRotation(int index, Quaterniond rotation, double weight = 1.0)
        {
            _handle.Index = index;
            _rotation = rotation;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public Quaterniond Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Rotation; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            _handle.AngleDelta = Quaterniond.CreateFromTo(bodies[_handle].Rotation, _rotation).ToAxisAngle();
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_handle].ApplyRotate(_handle.AngleDelta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { yield return _handle; }
        }

        #endregion
    }
}
