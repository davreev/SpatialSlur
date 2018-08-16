
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
    public class Coplanar : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public Coplanar(double weight = 1.0)
            : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public Coplanar(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <inheritdoc />
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            int n = indices.Count;
            
            if(n < 4 || !Geometry.FitPlaneToPoints(Indices.Select(i => bodies[i].Position.Current), out Vector3d p, out Vector3d z))
            {
                deltas.Clear();
                return false;
            }

            for (int i = 0; i < n; i++)
                deltas[i] = Vector3d.Project(p - bodies[indices[i]].Position.Current, z);

            return true;
        }
    }
}
