
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
    public class Cospherical : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Cospherical(double weight = 1.0)
            : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public Cospherical(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <inheritdoc />
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            int n = indices.Count;

            if(n < 4 || !Geometry.FitSphereToPoints(Indices.Select(i => bodies[i].Position.Current), out Vector3d p, out double r))
            {
                deltas.Clear();
                return false;
            }

            for (int i = 0; i < n; i++)
            {
                var d = p - bodies[indices[i]].Position.Current;
                deltas[i] = d * (1.0 - r / d.Length);
            }

            return true;
        }
    }
}

