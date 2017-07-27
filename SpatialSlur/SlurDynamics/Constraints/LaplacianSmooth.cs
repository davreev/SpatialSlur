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
    public class LaplacianSmooth : MultiParticleConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public LaplacianSmooth(double weight = 1.0)
            : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(IEnumerable<int> indices, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            int n = Handles.Count;

            // need at least 3 handles to define projection
            if (n < 3)
            {
                foreach (var h in Handles) h.Weight = 0.0;
                return;
            }

            Vec3d sum = new Vec3d();
            double nInv = 1.0 / (n - 1);

            foreach (var h in Handles.Skip(1))
                sum += particles[h].Position;

            var h0 = Handles[0];
            var d = (sum * nInv - particles[h0].Position) * 0.5;

            // apply to central particle
            h0.Delta = d;
            h0.Weight = Weight;

            // distribute reverse among neighbours
            d *= -nInv;
            foreach (var h in Handles.Skip(1))
            {
                h.Delta = d;
                h.Weight = Weight;
            }
        }
    }
}
