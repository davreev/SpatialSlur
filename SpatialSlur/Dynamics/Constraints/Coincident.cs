
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Coincident : Impl.PositionConstraint
    {
        bool _accumulate;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Coincident(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public Coincident(IEnumerable<ParticleHandle> handles, double weight = 1.0)
        {
            SetHandles(handles);
            Weight = weight;
        }


        /// <inheritdoc />
        public override void Calculate(
            ArrayView<ParticlePosition> positions,
            ArrayView<ParticleRotation> rotations)
        {
            var handles = Handles;
            var deltas = Deltas;

            if (handles.Count < 2)
            {
                deltas.Clear();
                _accumulate = false;
                return;
            }

            Vector3d cen = GetCentroid(positions, handles);
            
            for (int i = 0; i < handles.Count; i++)
                deltas[i] = cen - positions[handles[i].PositionIndex].Current;

            _accumulate = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private static Vector3d GetCentroid(ArrayView<ParticlePosition> positions, ArrayView<ParticleHandle> handles)
        {
            Vector3d sum = Vector3d.Zero;
            double wsum = 0.0;

            for (int i = 0; i < handles.Count; i++)
            {
                ref var p = ref positions[handles[i].PositionIndex];
                sum += p.Current / p.InverseMass;
                wsum += p.InverseMass;
            }

            return sum * wsum;
        }


        /// <inheritdoc />
        public override void Accumulate(
            ArrayView<Vector4d> linearCorrectSums,
            ArrayView<Vector4d> angularCorrectSums)
        {
            if (_accumulate)
                base.Accumulate(linearCorrectSums, angularCorrectSums);
        }
    }
}
