
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// Base class for a constraint that acts on the positions of a collection of bodies.
    /// </summary>
    [Serializable]
    public abstract class PositionGroup : Constraint, IConstraint
    {
        private Vector3d[] _deltas = Array.Empty<Vector3d>();
        private int[] _indices = Array.Empty<int>();
        private int _count;
        private bool _apply = true;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public PositionGroup(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public PositionGroup(IEnumerable<int> indices, double weight = 1.0)
        {
            Indices = indices;
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> Indices
        {
            get { return _indices.TakeRange(0, _count); }
            set
            {
                _count = value.ToArray(ref _indices);

                // resize delta buffer if necessary
                if (_deltas.Length < _count)
                    _deltas = new Vector3d[_indices.Length];
            }
        }

        
        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Body> bodies)
        {
            _apply = Calculate(bodies, _indices.AsView(_count), _deltas.AsView(_count));
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas);


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            if (!_apply) return;

            for (int i = 0; i < _count; i++)
                bodies[_indices[i]].Position.AddDelta(_deltas[i], Weight);
        }


        /// <inheritdoc />
        public virtual void GetEnergy(out double linear, out double angular)
        {
            var sum = 0.0;

            for (int i = 0; i < _count; i++)
                sum += _deltas[i].Length;

            linear = sum;
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

        #endregion
    }
}
