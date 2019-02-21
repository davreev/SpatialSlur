
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
    public class AlignPair : Impl.OnTarget<AlignPair.Target>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public struct Target
        {
            /// <summary>
            /// 
            /// </summary>
            public static Target Default = new Target()
            {
                Direction = Vector3d.UnitX,
                Weight = 1.0
            };

            /// <summary></summary>
            public Vector3d Direction;

            /// <summary>Relative influence of this target</summary>
            public double Weight;
        }

        #endregion


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
                    ref var tg = ref targets[indices[i]];

                    int j = i << 1;
                    ref var p0 = ref positions[particles[j].PositionIndex];
                    ref var p1 = ref positions[particles[j + 1].PositionIndex];

                    var d = Vector3d.Reject(p1.Current - p0.Current, tg.Direction);

                    var w0 = p0.InverseMass;
                    var w1 = p1.InverseMass;
                    var t = tg.Weight / (w0 + w1);

                    deltas[j] = new Vector4d(d * (w0 * t), 1.0) * tg.Weight;
                    deltas[j+1] = new Vector4d(d * -(w1 * t), 1.0) * tg.Weight;
                }
            }
        }
    }
}
