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
    /// 
    /// </summary>
    [Serializable]
    public class Pressure : Impl.PositionForce
    {
        private double _strength;
        
        
        /// <summary>
        /// 
        /// </summary>
        public double Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            base.Calculate(positions, rotations);
            int count = Particles.Count / 3;

            if (Parallel)
                ForEach(Partitioner.Create(0, count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, count);

            void Calculate(int from, int to)
            {
                var particles = Particles;
                var deltas = Deltas;

                const double inv6 = 1.0 / 6.0;
                var strength = _strength * inv6;

                for (int i = from; i < to; i++)
                {
                    int j = i * 3;
                    ref var p0 = ref positions[particles[j].PositionIndex].Current;
                    ref var p1 = ref positions[particles[j + 1].PositionIndex].Current;
                    ref var p2 = ref positions[particles[j + 2].PositionIndex].Current;

                    deltas[j] = deltas[j + 1] = deltas[j + 2] = Vector3d.Cross(p1 - p0, p2 - p1) * strength;
                }
            }
        }
    }
}
