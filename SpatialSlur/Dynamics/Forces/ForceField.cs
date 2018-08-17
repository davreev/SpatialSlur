
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;
using SpatialSlur.Fields;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ForceField : Force, IConstraint
    {
        private IField3d<Vector3d> _field;
        private Vector3d _delta;
        private int _index;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public ForceField(int index, IField3d<Vector3d> field, double strength = 1.0)
        {
            _index = index;
            Field = field;
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public IField3d<Vector3d> Field
        {
            get { return _field; }
            set { _field = value ?? throw new ArgumentNullException(); }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _delta = _field.ValueAt(bodies[_index].Position.Current) * Strength;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_index].Position.AddForce(_delta);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = _delta.Length;
            angular = 0.0;
        }


        #region Explicit Interface Implementations

        bool IConstraint.AffectsPosition
        {
            get { return true; }
        }


        bool IConstraint.AffectsRotation
        {
            get { return false; }
        }


        IEnumerable<int> IConstraint.Indices
        {
            get { yield return _index; }
            set { _index = value.First(); }
        }

        #endregion
    }
}
