using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
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


        /// <summary>
        /// Need at least 4 handles to define projections.
        /// </summary>
        private bool IsValid
        {
            get { return Handles.Count > 3; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            if (!IsValid) return;
            
            double meanLength = 0.0;
            int n = Handles.Count;
            
            for (int i = 0; i < n; i += 2)
            {
                var h0 = Handles[i];
                var h1 = Handles[i + 1];

                Vec3d d = particles[h1].Position - particles[h0].Position;
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            if (!IsValid) return;

            foreach (var h in Handles)
                bodies[h].ApplyMove(h.Delta, Weight);
        }


        #region Explicit interface implementations

        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        bool IConstraint.AppliesRotation
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IHandle> IConstraint.Handles
        {
            get { return Handles; }
        }

        #endregion
    }
}
