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
    /// 
    /// </summary>
    [Serializable]
    public class TangentialSmooth : Constraint, IConstraint
    {
        #region Static

        protected const int DefaultCapacity = 4;

        #endregion


        private List<H> _neighbors;
        private H _handle = new H();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(double weight = 1.0, int capacity = DefaultCapacity)
        {
            _neighbors = new List<H>(capacity);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="neighborIndices"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(int index, IEnumerable<int> neighborIndices, double weight = 1.0, int capacity = DefaultCapacity)
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
        /// Need at least 3 neighbors to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Neighbors.Count > 2; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            Vec3d sum = new Vec3d();

            foreach(var h in _neighbors)
                sum += bodies[h].Position;
            
            double nInv = 1.0 / (_neighbors.Count - 1);
            var d = Vec3d.Reject((sum * nInv - bodies[_handle].Position) * 0.5, ComputeNormal(bodies));

            // apply to central particle
            _handle.Delta = d;
            d *= -nInv;

            // distribute reverse among neighbours
            foreach (var h in  _neighbors)
                h.Delta = d;
        }


        /// <summary>
        /// Calculates the normal as the sum of triangle area gradients
        /// </summary>
        /// <returns></returns>
        private Vec3d ComputeNormal(IReadOnlyList<IBody> bodies)
        {
            var p = bodies[_handle].Position;

            var sum = new Vec3d();
            var n = _neighbors.Count;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                var p0 = bodies[_neighbors[i]].Position;
                var p1 = bodies[_neighbors[j]].Position;
                sum += GeometryUtil.GetTriAreaGradient(p, p0, p1);
            }

            return sum;
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
