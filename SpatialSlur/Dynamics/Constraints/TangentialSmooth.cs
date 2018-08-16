
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
    public class TangentialSmooth : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public TangentialSmooth(double weight = 1.0)
        {
            Weight = weight;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public TangentialSmooth(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <inheritdoc />
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            var n = indices.Count;

            // check if sufficient number of bodies
            if (n < 4)
            {
                deltas.Clear();
                return false;
            }

            var sum = new Vector3d();

            for (int i = 1; i < n; i++)
                sum += bodies[indices[i]].Position.Current;

            var t = 1.0 / (n - 1);
            var d = (sum * t - bodies[indices[0]].Position.Current) * 0.5;

            // reject onto mean curvature normal if valid
            if (GetNormal(bodies, indices, out Vector3d norm))
                d = Vector3d.Reject(d, norm);
            
            // apply projection to center
            deltas[0] = d;
            d *= -t;

            // distribute reverse among 1 ring
            for (int i = 1; i < n; i++)
                deltas[i] = d;

            return true;
        }


        /// <summary>
        /// Calculates the normal as the sum of triangle area gradients with respect to the first body.
        /// </summary>
        /// <returns></returns>
        private bool GetNormal(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, out Vector3d result)
        {
            var p0 = bodies[indices[0]].Position.Current;
            var last = indices.Count - 1;

            result = Vector3d.Zero;

            for (int i = 1; i < last; i++)
                result += GetGradient(i, i + 1);

            result += GetGradient(last, 1);
            return result.SquareLength > 0.0;
            
            Vector3d GetGradient(int i, int j)
            {
                var p1 = bodies[indices[i]].Position.Current;
                var p2 = bodies[indices[j]].Position.Current;
                return Geometry.GetAreaGradient(p0, p1, p2);
            }
        }
    }
}
