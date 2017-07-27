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
    /// Applies a force proportional to the mass of each particle.
    /// </summary>
    [Serializable]
    public class Weight : MultiParticleConstraint<H>
    {
        /// <summary>Describes the direction and magnitude of the applied weight</summary>
        public Vec3d Vector;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="weight"></param>
        public Weight(Vec3d vector, double weight = 1.0)
            : base(weight)
        {
            Vector = vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public Weight(Vec3d vector, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Vector = vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vector"></param>
        /// <param name="weight"></param>
        public Weight(IEnumerable<int> indices, Vec3d vector, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Vector = vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                h.Delta = Vector * particles[h].Mass;
                h.Weight = Weight;
            }
        }
    }
}
