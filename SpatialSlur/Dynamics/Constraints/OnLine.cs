
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
    public class OnLine : Constraint, IConstraint
    {
        private Vector3d _delta;
        private int _index;

        private Vector3d _start;
        private Vector3d _direction;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public OnLine(int index, Vector3d start, Vector3d direction, double weight = 1.0)
        {
            _index = index;
            _start = start;
            _direction = direction;
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
        public Vector3d Start
        {
            get { return _start; }
            set { _start = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Vector3d Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _delta = Vector3d.Reject(_start - bodies[_index].Position.Current, _direction);
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


        #region Explicit interface implementations

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
