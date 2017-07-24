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
    public class Weight : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Vec3d Direction;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public Weight(Vec3d direction, double weight = 1.0)
            : base(weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public Weight(Vec3d direction, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public Weight(IEnumerable<int> indices, Vec3d direction, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="direction"></param>
        /// <param name="weight"></param>
        public Weight(IEnumerable<H> handles, Vec3d direction, double weight = 1.0)
            : base(handles, weight)
        {
            Direction = direction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                h.Delta = Direction * particles[h].Mass;
                h.Weight = Weight;
            }
        }
    }
}
