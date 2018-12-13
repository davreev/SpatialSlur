
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
    public class ForceGroup
    {
        private List<IForce> _forces;
        private bool _parallel;


        /// <summary>
        /// 
        /// </summary>
        public ForceGroup()
        {
            _forces = new List<IForce>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public ForceGroup(int capacity)
        {
            _forces = new List<IForce>(capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IForce> Forces
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
        /// <param name="particles"></param>
        public void Apply(ParticleBuffer particles, ArrayView<Vector3d> forceSum, ArrayView<Vector3d> torqueSum)
        {
            if (_parallel)
            {
                ForEach(Partitioner.Create(0, _forces.Count), range => Calculate(range.Item1, range.Item2));

                void Calculate(int from, int to)
                {
                    for (int i = from; i < to; i++)
                        _forces[i].Calculate(particles);
                }

                // Must accumulate serially to avoid race conditions
                foreach (var f in _forces)
                    f.Accumulate(forceSum, torqueSum);
            }
            else
            {
                foreach (var f in _forces)
                {
                    f.Calculate(particles);
                    f.Accumulate(forceSum, torqueSum);
                }
            }
        }
    }
}
