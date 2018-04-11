using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    using H = ParticleHandle;

    /// <summary>
    /// Each successive pair of handles represents a line segment.
    /// </summary>
    [Serializable]
    public class EqualizeLengths : MultiConstraint<H>, IConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public EqualizeLengths(double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="weight"></param>
        public EqualizeLengths(IEnumerable<int> indices, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
        }


        /// <inheritdoc />
        public ConstraintType Type
        {
            get { return ConstraintType.Position; }
        }


        /// <summary>
        /// Need at least 4 handles to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Handles.Count > 3; }
        }


        /// <inheritdoc />
        public void Calculate(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;
            
            double meanLength = 0.0;
            int n = Handles.Count;
            
            for (int i = 0; i < n; i += 2)
            {
                var h0 = Handles[i];
                var h1 = Handles[i + 1];

                Vec3d d = bodies[h1].Position - bodies[h0].Position;
                var m = d.Length;

                h0.Delta = d;
                h1.Delta = new Vec3d(m); // cache length in h1.Delta temporarily

                meanLength += m;
            }
       
            meanLength /= n;

            for (int i = 0; i < n; i += 2)
            {
                var h0 = Handles[i];
                var h1 = Handles[i + 1];

                var d = h0.Delta;
                var m = h1.Delta.X;

                d *= (1.0 - meanLength / m) * 0.5;
                h0.Delta = d;
                h1.Delta = -d;
            }
        }


        /// <inheritdoc />
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            foreach (var h in Handles)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
