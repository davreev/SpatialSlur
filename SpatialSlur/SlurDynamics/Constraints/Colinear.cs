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
    public class Colinear : DynamicPositionConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Colinear(double weight = 1.0)
          : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public Colinear(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public Colinear(IEnumerable<H> handles, double weight = 1.0)
            :base(handles,weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            // TODO solve best fit line
            throw new NotImplementedException();
        }
    }
}
