
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
    /// Each successive pair of handles represents a line segment.
    /// </summary>
    [Serializable]
    public class EqualizeLengths : PositionGroup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public EqualizeLengths(double weight = 1.0)
        {
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public EqualizeLengths(IEnumerable<int> indices, double weight = 1.0)
            : base(indices, weight)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="indices"></param>
        /// <param name="deltas"></param>
        /// <returns></returns>
        protected override bool Calculate(ReadOnlyArrayView<Body> bodies, ReadOnlyArrayView<int> indices, ArrayView<Vector3d> deltas)
        {
            int n = indices.Count;

            // check if sufficient number of bodies
            if (n < 4)
            {
                deltas.Clear();
                return false;
            }

            var target = 0.0;

            // calculate target as mean length
            for (int i = 0; i < n; i += 2)
            {
                var d = bodies[indices[i + 1]].Position.Current - bodies[indices[i]].Position.Current;
                var m = d.Length;
                target += m;

                // cache to avoid redundant calcs
                deltas[i] = d;
                deltas[i + 1].X = m;
            }

            target /= n >> 1;

            // calculate deltas
            for (int i = 0; i < n; i += 2)
            {
                var d = deltas[i];
                var m = deltas[i + 1].X;

                d *= (1.0 - target / m) * 0.5;
                deltas[i] = d;
                deltas[i + 1] = -d;
            }
            
            return true;
        }
    }
}
