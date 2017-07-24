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
    public class InsideDomain : DynamicPositionConstraint<H>
    {
        /// <summary></summary>
        public Domain3d Domain;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="weight"></param>
        public InsideDomain(Domain3d domain, double weight = 1.0)
            : base(weight)
        {
            Domain = domain;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public InsideDomain(Domain3d domain, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Domain = domain;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="domain"></param>
        /// <param name="weight"></param>
        public InsideDomain(IEnumerable<int> indices, Domain3d domain, double weight = 1.0)
            : base(indices.Select(i => new H(i)), weight)
        {
            Domain = domain;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="domain"></param>
        /// <param name="weight"></param>
        public InsideDomain(IEnumerable<H> handles, Domain3d domain, double weight = 1.0)
            : base(handles, weight)
        {
            Domain = domain;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            var d = Domain;

            foreach (var h in Handles)
            {
                var p = particles[h].Position;

                if (d.Contains(p))
                {
                    h.Weight = 0.0;
                    continue;
                }

                h.Delta = Domain.Clamp(p) - p;
                h.Weight = Weight;
            }
        }
    }
}
