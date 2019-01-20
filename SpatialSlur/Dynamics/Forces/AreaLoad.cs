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
    /// Applies a force proportional to the area of the triangle defined by 3 particles.
    /// </summary>
    [Serializable]
    public class AreaLoad : Impl.PositionForce
    {
        private SlurList<Vector3d> _loadForces;


        /// <summary>
        /// Per-segment load forces
        /// </summary>
        public SlurList<Vector3d> LoadForces
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
                    int j = i * 3;
                    ref var p0 = ref positions[particles[j].PositionIndex].Current;
                    ref var p1 = ref positions[particles[j + 1].PositionIndex].Current;
                    ref var p2 = ref positions[particles[j + 2].PositionIndex].Current;
                    
                    const double inv6 = 1.0 / 6.0;
                    deltas[j] = deltas[j + 1] = deltas[j + 2] = loadForces[i] * (Vector3d.Cross(p1 - p0, p2 - p1).Length * inv6);
                }
            }
        }
    }
}
