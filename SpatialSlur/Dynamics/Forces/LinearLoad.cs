/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearLoad : Impl.PositionForce
    {
        private DynamicArray<Vector3d> _loadForces;


        /// <summary>
        /// Per-segment load forces
        /// </summary>
        public DynamicArray<Vector3d> LoadForces
        {
            get => _loadForces;
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            base.Calculate(positions, rotations);
            var loadForces = _loadForces;

            if (Parallel)
                ForEach(Partitioner.Create(0, loadForces.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, loadForces.Count);

            void Calculate(int from, int to)
            {
                var particles = Particles;
                var deltas = Deltas;

                for (int i = from; i < to; i++)
                {
                    int j = i << 1;
                    ref var p0 = ref positions[particles[j].PositionIndex].Current;
                    ref var p1 = ref positions[particles[j + 1].PositionIndex].Current;

                    deltas[j] = deltas[j + 1] = loadForces[i] * (p0.DistanceTo(p1) * 0.5);
                }
            }
        }
    }
}
