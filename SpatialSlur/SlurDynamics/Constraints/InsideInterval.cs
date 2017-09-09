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
    public class InsideInterval : MultiParticleConstraint<H>
    {
        /// <summary></summary>
        public Interval3d Interval;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="weight"></param>
        public InsideInterval(Interval3d interval, double weight = 1.0)
            : base(weight)
        {
            Interval = interval;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public InsideInterval(Interval3d interval, int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
            Interval = interval;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="interval"></param>
        /// <param name="weight"></param>
        public InsideInterval(IEnumerable<int> indices, Interval3d interval, double weight = 1.0)
            : base(weight)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Interval = interval;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public override sealed void Calculate(IReadOnlyList<IBody> particles)
        {
            var d = Interval;

            foreach (var h in Handles)
            {
                var p = particles[h].Position;

                if (d.Contains(p))
                {
                    h.Weight = 0.0;
                    continue;
                }

                h.Delta = Interval.Clamp(p) - p;
                h.Weight = Weight;
            }
        }
    }
}
