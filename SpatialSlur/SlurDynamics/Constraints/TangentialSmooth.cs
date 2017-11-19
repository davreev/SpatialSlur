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
    public class TangentialSmooth : MultiParticleConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(IEnumerable<int> indices, double weight = 1.0, int capacity = DefaultCapacity)
            :base(weight, capacity)
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

            foreach(var h in Handles.Skip(1))
                sum += particles[h].Position;
     
            var h0 = Handles[0];
            var d = Vec3d.Reject((sum * nInv - particles[h0].Position) * 0.5, ComputeNormal(particles));

            // apply to central particle
            h0.Delta = d;
            h0.Weight = Weight;

            // distribute reverse among neighbours
            d *= -nInv;
            foreach(var h in Handles.Skip(1))
            {
                h.Delta = d;
                h.Weight = Weight;
            }
        }


        /// <summary>
        /// Calculates the normal as the sum of triangle area gradients
        /// </summary>
        /// <returns></returns>
        private Vec3d ComputeNormal(IReadOnlyList<IBody> particles)
        {
            var p = particles[Handles[0]].Position;

            var sum = new Vec3d();
            var n = Handles.Count - 1;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                var p0 = particles[Handles[i + 1]].Position;
                var p1 = particles[Handles[j + 1]].Position;
                sum += GeometryUtil.GetTriAreaGradient(p, p0, p1);
            }

            return sum;
        }
    }
}
