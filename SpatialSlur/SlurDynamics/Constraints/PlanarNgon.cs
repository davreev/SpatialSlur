using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.GeometryUtil;

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
    public class PlanarNgon : MultiConstraint<H>, IConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public PlanarNgon(double weight = 1.0, int capacity = DefaultCapacity)
            :base(weight, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public PlanarNgon(IEnumerable<int> indices, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// Need at least 4 neighbors to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Handles.Count > 3; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;
            
            int n = Handles.Count;
            
            // quad case
            if (n == 4)
            {
                var h0 = Handles[0];
                var h1 = Handles[1];
                var h2 = Handles[2];
                var h3 = Handles[3];

                var d = LineLineShortestVector(
                    bodies[h0].Position,
                    bodies[h2].Position,
                    bodies[h1].Position,
                    bodies[h3].Position) * 0.5;

                h0.Delta = h2.Delta = d;
                h1.Delta = h3.Delta = -d;
                return;
            }

            // general case
            foreach (var h in Handles)
                h.Delta = new Vec3d();
      
            for (int i = 0; i < n; i++)
            {
                var h0 = Handles[i];
                var h1 = Handles[(i + 1) % n];
                var h2 = Handles[(i + 2) % n];
                var h3 = Handles[(i + 3) % n];

                var d = LineLineShortestVector(
                    bodies[h0].Position,
                    bodies[h2].Position,
                    bodies[h1].Position,
                    bodies[h3].Position) * 0.125; // 0.5 / 4 (4 deltas applied per index)

                h0.Delta += d;
                h2.Delta += d;
                h1.Delta -= d;
                h3.Delta -= d;
            }
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            foreach (var h in Handles)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
