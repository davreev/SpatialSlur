
/*
 * Notes
 * 
 * TODO reformat as per Constraint types
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// Base class for an influence that acts on a dynamic collection of particles.
    /// </summary>
    [Serializable]
    public abstract class InfluenceBase<TDelta> : IInfluence
    {
        private TDelta[] _deltas = Array.Empty<TDelta>();
        private int[] _indices = Array.Empty<int>();
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public InfluenceBase()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="strength"></param>
        public InfluenceBase(IEnumerable<int> indices)
        {
            SetIndices(indices);
        }


        /// <summary>
        /// 
        /// </summary>
        public ArrayView<int> Indices
        {
            get { return _indices.AsView(_count); }
        }


        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyArrayView<TDelta> Deltas
        {
            get { return _deltas.AsView(_count); }
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetIndices(IEnumerable<int> indices)
        {
            _count = indices.ToArray(ref _indices);

            // resize other buffer(s) if necessary
            if (_deltas.Length < _count)
                _deltas = new TDelta[_indices.Length];
        }


        /// <inheritdoc />
        public void Calculate(ReadOnlyArrayView<Particle> particles)
        {
            Calculate(particles, _indices.AsView(_count), _deltas.AsView(_count));
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void Calculate(
            ReadOnlyArrayView<Particle> particles, 
            ReadOnlyArrayView<int> indices, 
            ArrayView<TDelta> deltas);


        /// <inheritdoc />
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            Apply(particles, _indices.AsView(_count), _deltas.AsView(_count));
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void Apply(
            ReadOnlyArrayView<Particle> particles, 
            ReadOnlyArrayView<int> indices, 
            ReadOnlyArrayView<TDelta> deltas);
    }
}
