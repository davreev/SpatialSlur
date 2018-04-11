using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// The first handle represents the central vertex, remaining handles represent neighbours.
    /// </summary>
    [Serializable]
    public class LaplacianSmooth : Constraint, IConstraint
    {
        #region Static

        protected const int DefaultCapacity = 4;

        #endregion


        private List<H> _neighbors;
        private H _handle = new H();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="capacity"></param>
        public LaplacianSmooth(double weight = 1.0, int capacity = DefaultCapacity)
        {
            _neighbors = new List<H>(capacity);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(int index, IEnumerable<int> neighborIndices, double weight = 1.0, int capacity = DefaultCapacity)
            : this(weight, capacity)
        {
            _handle.Index = index;
            _neighbors.AddRange(neighborIndices.Select(i => new H(i)));
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
        public List<H> Neighbors
        {
            get { return _neighbors; }
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// Need at least 2 neighbors to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Neighbors.Count > 1; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            Vec3d sum = new Vec3d();

            foreach (var h in _neighbors)
                sum += bodies[h].Position;

            double nInv = 1.0 / (_neighbors.Count - 1);
            var d = (sum * nInv - bodies[_handle].Position) * 0.5;

            // apply to center
            _handle.Delta = d;
            d *= -nInv;

            // distribute reverse among neighbors
            foreach (var h in _neighbors)
                h.Delta = d;
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            foreach (var h in Neighbors)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _handle;

                foreach (var h in _neighbors)
                    yield return h;
            }
        }

        #endregion
    }
}
