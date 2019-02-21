/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Rhino.Geometry;

using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnMesh : Impl.OnTarget<OnMesh.Target>
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
                Weight = 1.0
            };

            /// <summary></summary>
            public Mesh Mesh;

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
                    ref var t = ref targets[indices[i]];
                    ref var p = ref positions[particles[i].PositionIndex].Current;
                    var d = (Vector3d)t.Mesh.ClosestPoint(p) - p;
                    deltas[i] = new Vector4d(d, 1.0) * t.Weight;
                }
            }
        }
    }
}

#endif