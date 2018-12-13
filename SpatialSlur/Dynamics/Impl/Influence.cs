
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// Base class for an influence that acts on a dynamic collection of particles.
    /// </summary>
    [Serializable]
    public abstract class InfluenceBase<TDelta> : IInfluence
    {
        private ParticleHandle[] _handles = Array.Empty<ParticleHandle>();
        private TDelta[] _deltas = Array.Empty<TDelta>();
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
        /// <param name="handles"></param>
        public InfluenceBase(IEnumerable<ParticleHandle> handles)
        {
            SetHandles(handles);
        }


        /// <summary>
        /// 
        /// </summary>
        public ArrayView<ParticleHandle> Handles
        {
            get { return _handles.AsView(_count); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected ArrayView<TDelta> Deltas
        {
            get { return _deltas.AsView(_count); }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void SetHandles(IEnumerable<ParticleHandle> handles)
        {
            _count = handles.ToArray(ref _handles);

            // resize other buffer(s) if necessary
            if (_deltas.Length < _count)
                _deltas = new TDelta[_handles.Length];
        }


        /// <inheritdoc />
        public virtual void Initialize(ParticleBuffer particles)
        {
        }


        /// <inheritdoc />
        public abstract void Calculate(ParticleBuffer particles);
    }
}
