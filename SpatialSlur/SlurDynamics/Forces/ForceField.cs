using System;
using System.Collections.Generic;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class ForceField : Force, IConstraint
    {
        private H _handle = new H();
        private IField3d<Vec3d> _field;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="capacity"></param>
        /// <param name="strength"></param>
        public ForceField(int index, IField3d<Vec3d> field, double strength = 1.0)
        {
            _handle.Index = index;
            Field = field;
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _handle; }
        }


        /// <summary>
        /// 
        /// </summary>
        public IField3d<Vec3d> Field
        {
            get { return _field; }
            set { _field = value ?? throw new ArgumentNullException(); }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            _handle.Delta = _field.ValueAt(bodies[_handle].Position) * Strength;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            bodies[_handle].ApplyForce(_handle.Delta);
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
