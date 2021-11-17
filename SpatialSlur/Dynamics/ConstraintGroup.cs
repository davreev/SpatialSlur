/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

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
        private DynamicArray<IConstraint> _constraints = new DynamicArray<IConstraint>();
        private bool _parallel;
        
        
        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<IConstraint> Constraints
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
        /// <param name="positions"></param>
        /// <param name="rotations"></param>
        /// <param name="linearCorrectSums"></param>
        /// <param name="angularCorrectSums"></param>
        public void Apply(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations,
            ArrayView<Vector4d> linearCorrectSums, 
            ArrayView<Vector4d> angularCorrectSums)
        {
            var constraints = _constraints;

            if (_parallel)
            {
                ForEach(Partitioner.Create(0, constraints.Count), range => Calculate(range.Item1, range.Item2));
                
                void Calculate(int from, int to)
                {
                    for (int i = from; i < to; i++)
                        constraints[i].Calculate(positions, rotations);
                }

                // Accumulate serially to avoid race conditions
                foreach (var c in constraints)
                    c.Accumulate(linearCorrectSums, angularCorrectSums);
            }
            else
            {
                foreach (var c in constraints)
                {
                    c.Calculate(positions, rotations);
                    c.Accumulate(linearCorrectSums, angularCorrectSums);
                }
            }
        }
    }
}
