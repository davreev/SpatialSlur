
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
    public class Colinear : Impl.PositionConstraint
    {
        bool _accumulate;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Colinear(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="weight"></param>
        public Colinear(IEnumerable<ParticleHandle> handles, double weight = 1.0)
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

            if (handles.Count < 3 || !Geometry.FitLineToPoints(handles.Select(h => positions[h.PositionIndex].Current), out Vector3d p, out Vector3d d))
            {
                deltas.Clear();
                _accumulate = false;
                return;
            }

            for (int i = 0; i < handles.Count; i++)
                deltas[i] = Vector3d.Reject(p - positions[handles[i].PositionIndex].Current, d);

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
