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
    public class OutsideSphere: Impl.OnTarget<OnSphere.Target>
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
                    var r = t.Radius;

                    var d = t.Origin - positions[particles[i].PositionIndex].Current;
                    var m = d.SquareLength;
                    
                    if (m < r * r)
                    {
                        d *= 1.0 - r / Math.Sqrt(m);
                        deltas[i] = new Vector4d(d, 1.0) * t.Weight;
                    }
                    else
                    {
                        deltas[i] = Vector4d.Zero;
                    }
                }
            }
        }
    }
}
