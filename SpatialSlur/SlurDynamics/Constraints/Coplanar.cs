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
    /// 
    /// </summary>
    public class Coplanar<P> : DynamicConstraint<P, H>
        where P : IParticle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Coplanar(double weight = 1.0)
           : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public Coplanar(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public Coplanar(IEnumerable<H> handles, double weight = 1.0)
            :base(handles,weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override void Calculate(IReadOnlyList<P> particles)
        {
            // TODO solve best fit plane
            throw new NotImplementedException();
        }
    }
}
