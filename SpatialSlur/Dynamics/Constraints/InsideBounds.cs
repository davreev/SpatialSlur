
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
    public class InsideBounds : Constraint, IConstraint
    {
        private Vector3d _delta;
        private int _index;

        private Interval3d _bounds;
        private bool _apply;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="bounds"></param>
        /// <param name="weight"></param>
        public InsideBounds(int index, Interval3d bounds, double weight = 1.0)
        {
            _index = index;
            _bounds = bounds;
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
        public Interval3d Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            var p = bodies[_index].Position.Current;

            if (_bounds.Contains(p))
            {
                _delta = Vector3d.Zero;
                _apply = false;
                return;
            }

            _delta = _bounds.Clamp(p) - p;
            _apply = true;
        }


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            if (_apply)
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
