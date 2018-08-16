
/*
 * Notes
 * 
 * TODO reformat as per Constraint types
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Base class for a force that acts on the positions of a collection of bodies.
    /// </summary>
    [Serializable]
    public abstract class PositionGroup : Force, IConstraint
    {
        private Vector3d[] _deltas = Array.Empty<Vector3d>();
        private int[] _indices = Array.Empty<int>();
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strength"></param>
        public PositionGroup(double strength = 1.0)
        {
            Strength = strength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="strength"></param>
        public PositionGroup(IEnumerable<int> indices, double strength = 1.0)
        {
            Indices = indices;
            Strength = strength;
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
            Calculate(bodies, _indices.AsView(_count), _deltas.AsView(_count));
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas);


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Body> bodies)
        {
            for (int i = 0; i < _count; i++)
                bodies[_indices[i]].Position.AddForce(_deltas[i]);
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
            get { return _indices; }
            set { _indices.Set(value); }
        }

        #endregion
    }
}
