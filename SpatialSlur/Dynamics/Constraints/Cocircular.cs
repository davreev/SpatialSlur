
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
    public class Cocircular : Impl.PositionConstraint
    {
        private bool _accumulate;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Cocircular(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public Cocircular(IEnumerable<ParticleHandle> handles, double weight = 1.0)
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

            // TODO consider particle masses

            if (handles.Count < 4 || !Geometry.FitCircleToPoints(handles.Select(h => positions[h.PositionIndex].Current), out Vector3d p, out Vector3d z, out double r))
            {
                deltas.Clear();
                _accumulate = false;
                return;
            }

            for (int i = 0; i < handles.Count; i++)
            {
                var d = p - positions[handles[i].PositionIndex].Current;
                var dz = Vector3d.Project(d, z);
                var dxy = d - dz;
                deltas[i] = dz + dxy * (1.0 - r / dxy.Length);
            }

            _accumulate = true;
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