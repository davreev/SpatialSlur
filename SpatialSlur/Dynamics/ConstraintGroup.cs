
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ConstraintGroup
    {
        private List<IConstraint> _constraints;
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        public ConstraintGroup()
        {
            _constraints = new List<IConstraint>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public ConstraintGroup(int capacity)
        {
            _constraints = new List<IConstraint>(capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IConstraint> Constraints
        {
            get => _constraints;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool Parallel
        {
            get => _parallel;
            set => _parallel = value;
        }


        /// <summary>
        /// Calculates and accumulates all constraints in this group
        /// </summary>
        /// <param name="particles"></param>
        /// <param name="linearSum"></param>
        /// <param name="angularSum"></param>
        public void Apply(
            ParticleBuffer particles, 
            ArrayView<(Vector3d Delta, double Weight)> linearSum, 
            ArrayView<(Vector3d Delta, double Weight)> angularSum)
        {
            if(_parallel)
            {
                ForEach(Partitioner.Create(0, _constraints.Count), range => Calculate(range.Item1, range.Item2));
                
                void Calculate(int from, int to)
                {
                    for (int i = from; i < to; i++)
                        _constraints[i].Calculate(particles);
                }

                // Must accumulate serially to avoid race conditions
                foreach (var c in _constraints)
                    c.Accumulate(linearSum, angularSum);
            }
            else
            {
                foreach (var c in _constraints)
                {
                    c.Calculate(particles);
                    c.Accumulate(linearSum, angularSum);
                }
            }
        }
    }
}
