
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
    public class OnRotation : Constraint, IConstraint
    {
        private Vector3d _delta;
        private int _index;

        private Quaterniond _target;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="target"></param>
        /// <param name="weight"></param>
        public OnRotation(int index, Quaterniond target, double weight = 1.0)
        {
            _index = index;
            _target = target;
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
        public Quaterniond Target
        {
            get { return _target; }
            set { _target = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _delta = Quaterniond.CreateFromTo(bodies[_index].Rotation.Current, _target).ToAxisAngle();
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            bodies[_index].Rotation.AddDelta(_delta, Weight);
        }


        /// <inheritdoc />
        public void GetEnergy(out double linear, out double angular)
        {
            linear = 0.0;
            angular = _delta.Length;
        }


        #region Explicit interface implementations

        bool IConstraint.AffectsPosition
        {
            get { return false; }
        }


        bool IConstraint.AffectsRotation
        {
            get { return true; }
        }


        IEnumerable<int> IConstraint.Indices
        {
            get { yield return _index; }
            set { _index = value.First(); }
        }

        #endregion
    }
}
