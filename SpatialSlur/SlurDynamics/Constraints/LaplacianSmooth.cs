using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
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
        private H _center = new H();


        /// <summary>
        /// 
        /// </summary>
        public H Handle
        {
            get { return _center; }
        }


        /// <summary>
        /// 
        /// </summary>
        public List<H> Neighbors
        {
            get { return _neighbors; }
        }


        /// <summary>
        /// Need at least 2 neighbors to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Neighbors.Count > 1; }
        }


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
        public LaplacianSmooth(int index, IEnumerable<int> neighbors, double weight = 1.0, int capacity = DefaultCapacity)
            : this(weight, capacity)
        {
            _center.Index = index;
            _neighbors.AddRange(neighbors.Select(i => new H(i)));
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            if (!IsValid) return;

            Vec3d sum = new Vec3d();

            foreach (var h in _neighbors)
                sum += particles[h].Position;

            double nInv = 1.0 / (_neighbors.Count - 1);
            var d = (sum * nInv - particles[_center].Position) * 0.5;

            // apply to center
            _center.Delta = d;
            d *= -nInv;

            // distribute reverse among neighbors
            foreach (var h in _neighbors)
                h.Delta = d;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            foreach (var h in Neighbors)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return false; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get
            {
                yield return _center;
                foreach (var h in _neighbors) yield return h;
            }
        }

        #endregion
    }
}
