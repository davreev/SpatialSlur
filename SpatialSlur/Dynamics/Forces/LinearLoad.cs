
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SpatialSlur.Collections;

using static System.Threading.Tasks.Parallel;
using static SpatialSlur.Collections.Buffer;


namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// Applies a force proportional to the distance between 2 particles.
    /// </summary>
    [Serializable]
    public class LinearLoad : Impl.PositionForce
    {
        private RefList<Vector3d> _loadForces;


        /// <summary>
        /// 
        /// </summary>
        public RefList<Vector3d> LoadForces
        {
            get { return _loadForces; }
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
                var handles = Handles;
                var deltas = Deltas;

                for (int i = from; i < to; i++)
                {
                    int j = i << 1;
                    ref var h0 = ref handles[j];
                    ref var h1 = ref handles[j + 1];

                    var d = positions[h1.PositionIndex].Current - positions[h0.PositionIndex].Current;
                    deltas[j] = deltas[j + 1] = loadForces[i] * (d.Length * 0.5);
                }
            }
        }
    }
}
