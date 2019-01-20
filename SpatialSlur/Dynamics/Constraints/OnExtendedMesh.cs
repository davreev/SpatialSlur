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
    public class OnExtendedMesh : Impl.OnTarget<OnMesh.Target>
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
                    ref var tg = ref targets[indices[i]];
                    var mesh = tg.Mesh;

                    ref var p = ref positions[particles[i].PositionIndex].Current;
                    var mp = mesh.ClosestMeshPoint(p, 0.0);

                    var d = Vector3d.Project(
                        (Vector3d)mesh.PointAt(mp) - p, 
                        mesh.NormalAt(mp));
            
                    deltas[i] = new Vector4d(d, 1.0) * tg.Weight;
                }
            }
        }
    }
}

#endif