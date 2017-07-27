using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// Each successive pair of handles represents a line segment.
    /// </summary>
    [Serializable]
    public class EqualizeLengths : MultiParticleConstraint<H>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public EqualizeLengths(double weight = 1.0)
            :base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public EqualizeLengths(int capacity, double weight = 1.0)
            : base(capacity, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public EqualizeLengths(IEnumerable<int> indices, double weight = 1.0)
            : base( weight)
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

            // need at least 4 handles to define projections
            if(n < 4)
            {
                foreach (var h in Handles) h.Weight = 0.0;
                return;
            }

            double meanLength = 0.0;

            for(int i = 0; i < n; i+=2)
            {
                var h0 = Handles[i];
                var h1 = Handles[i + 1];

                Vec3d d = particles[h1].Position - particles[h0].Position;
                h0.Delta = d;
                
                var m = d.Length;
                meanLength += m;
                h1.Delta = new Vec3d(m); // cache length temporarily
            }
       
            meanLength /= n;

            for (int i = 0; i < n; i += 2)
            {
                var h0 = Handles[i];
                var h1 = Handles[i + 1];
                
                h0.Delta *= (1.0 - meanLength / h1.Delta.X) * 0.5;
                h1.Delta = -h0.Delta;
                h0.Weight = h1.Weight = Weight;
            }
        }
    }
}
