/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AbovePlane : Impl.OnTarget<OnPlane.Target>
    {
        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            base.Calculate(positions, rotations);
            var indices = TargetIndices;

            if (Parallel)
                ForEach(Partitioner.Create(0, indices.Count), range => Calculate(range.Item1, range.Item2));
            else
                Calculate(0, indices.Count);

            void Calculate(int from, int to)
            {
                var particles = Particles;
                var deltas = Deltas;
                var targets = Targets;

                for (int i = from; i < to; i++)
                {
                    ref var t = ref targets[indices[i]];
                    var d = Vector3d.Dot(t.Origin - positions[particles[i].PositionIndex].Current, t.Normal);

                    deltas[i] = d > 0.0 ?
                        new Vector4d(t.Normal * (d / t.Normal.SquareLength), 1.0) * t.Weight : Vector4d.Zero;
                }
            }
        }
    }
}
