using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Constraint<P, H> : IConstraint<P>
        where P : IParticle
        where H : ParticleHandle
    {
        private double _weight;


        /// <summary>
        /// Allows iteration over all handles used by this constraint.
        /// </summary>
        public abstract IEnumerable<H> Handles { get; }


        /// <summary>
        /// 
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("Weight cannot be negative.");

                _weight = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public abstract void Calculate(IReadOnlyList<P> particles);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Apply(IReadOnlyList<P> particles)
        {
            foreach (var h in Handles)
                particles[h].ApplyForce(h.Delta, Weight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        public void SetHandles(IEnumerable<int> indices)
        {
            var itr = indices.GetEnumerator();

            foreach (var h in Handles)
            {
                h.Index = itr.Current;
                itr.MoveNext();
            }
        }
    }
}
