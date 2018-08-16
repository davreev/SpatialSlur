
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LaplacianSmooth : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public LaplacianSmooth(double weight = 1.0)
            : base(weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public LaplacianSmooth(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <inheritdoc />
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            int n = indices.Count;

            // check if sufficient number of bodies
            if (n < 3)
            {
                deltas.Clear();
                return false;
            }
            
            var sum = new Vector3d();

            for (int i = 1; i < n; i++)
                sum += bodies[indices[i]].Position.Current;

            var t = 1.0 / (n - 1);
            var d = (sum * t - bodies[indices[0]].Position.Current) * 0.5;

            // apply projection to center
            deltas[0] = d;
            d *= -t;

            // distribute reverse among 1 ring
            for (int i = 1; i < n; i++)
                deltas[i] = d;

            return true;
        }
    }
}
