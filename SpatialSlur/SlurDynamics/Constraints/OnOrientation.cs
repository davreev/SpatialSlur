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
    public class OnOrientation : Constraint, IConstraint
    {
        private H _handle = new H();
        private Quaterniond _rotation;
        private Vec3d _position;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public OnOrientation(int index, Vec3d position, Quaterniond rotation, double weight = 1.0)
        {
            _handle.Index = index;
            _rotation = rotation;
            _position = position;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _handle; }
            set { _handle = value; }
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
        public Quaterniond Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.PositionRotation; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            var p = bodies[_handle];
            _handle.Delta = _position - p.Position;
            _handle.AngleDelta = Quaterniond.CreateFromTo(p.Rotation, _rotation).ToAxisAngle();
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            var b = bodies[_handle];
            b.ApplyMove(_handle.Delta, Weight);
            b.ApplyRotate(_handle.AngleDelta, Weight);
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
