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
    public class ForceGroup
    {
        private DynamicArray<IForce> _forces = new DynamicArray<IForce>();
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        public DynamicArray<IForce> Forces
        {
            get => _forces;
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
        /// Calculates and accumulates all forces in this group
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="rotations"></param>
        /// <param name="forceSums"></param>
        /// <param name="torqueSums"></param>
        public void Apply(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations,
            ArrayView<Vector3d> forceSums, 
            ArrayView<Vector3d> torqueSums)
        {
            var forces = _forces;

            if (_parallel)
            {
                ForEach(Partitioner.Create(0, forces.Count), range => Calculate(range.Item1, range.Item2));

                void Calculate(int from, int to)
                {
                    for (int i = from; i < to; i++)
                        forces[i].Calculate(positions, rotations);
                }

                // Accumulate serially to avoid race conditions
                foreach (var f in forces)
                    f.Accumulate(forceSums, torqueSums);
            }
            else
            {
                foreach (var f in forces)
                {
                    f.Calculate(positions, rotations);
                    f.Accumulate(forceSums, torqueSums);
                }
            }
        }
    }
}
