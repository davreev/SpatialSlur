
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnPosition : Constraint, IConstraint
    {
        private Vector3d _delta;
        private int _index;

        private Vector3d _target;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="weight"></param>
        public OnPosition(int index, Vector3d position, double weight = 1.0)
        {
            _index = index;
            _target = position;
            Weight = weight;
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
        public Vector3d Target
        {
            get { return _target; }
            set { _target = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _delta = _target - bodies[_index].Position.Current;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_index].Position.AddDelta(_delta, Weight);
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
