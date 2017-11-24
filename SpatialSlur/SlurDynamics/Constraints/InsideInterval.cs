using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = InsideInterval.Handle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InsideInterval : MultiConstraint<H>, IConstraint
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Handle : ParticleHandle
        {
            /// <summary></summary>
            internal bool Skip = false;


            /// <summary>
            /// 
            /// </summary>
            public Handle(int index)
                : base(index)
            {
            }
        }

        #endregion


        /// <summary></summary>
        public Interval3d Interval;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="weight"></param>
        public InsideInterval(Interval3d interval, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Interval = interval;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="interval"></param>
        /// <param name="weight"></param>
        public InsideInterval(IEnumerable<int> indices, Interval3d interval, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Interval = interval;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            var d = Interval;

            foreach (var h in Handles)
            {
                var p = particles[h].Position;

                if (d.Contains(p))
                {
                    h.Skip = true;
                    continue;
                }

                h.Delta = Interval.Clamp(p) - p;
                h.Skip = false;
            }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
            foreach (var h in Handles)
                if (!h.Skip) bodies[h].ApplyMove(h.Delta, Weight);
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


        /// <inheritdoc/>
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
