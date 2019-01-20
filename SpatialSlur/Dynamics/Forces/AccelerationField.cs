/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SpatialSlur.Collections;
using SpatialSlur.Fields;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force to each particle proportional to its mass
    /// </summary>
    [Serializable]
    public class AccelerationField : ForceField
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="strength"></param>
        public AccelerationField(IField3d<Vector3d> field, double strength = 1.0)
            :base(field, strength)
        {
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var particles = Particles;

            if (Parallel)
                ForEach(Partitioner.Create(0, particles.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, particles.Count);

            void Calculate(int from, int to)
            {
                var deltas = Deltas;
                var field = Field;
                var strength = Strength;

                for (int i = from; i < to; i++)
                {
                    ref var p = ref positions[particles[i].PositionIndex];
                    deltas[i] = field.ValueAt(p.Current) * (strength / p.InverseMass);
                }
            }
        }
    }
}
