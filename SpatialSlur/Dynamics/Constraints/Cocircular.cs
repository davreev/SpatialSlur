
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
    public class Cocircular : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Cocircular(double weight = 1.0)
            : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public Cocircular(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <inheritdoc />
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            int n = indices.Count;

            if (n < 4 || !Geometry.FitCircleToPoints(Indices.Select(i => bodies[i].Position.Current), out Vector3d p, out Vector3d z, out double r))
            {
                deltas.Clear();
                return false;
            }

            for (int i = 0; i < n; i++)
            {
                var d = Vector3d.Project(p - bodies[indices[i]].Position.Current, z);
                deltas[i] = d * (1.0 - r / d.Length);
            }

            return true;
        }
    }
}