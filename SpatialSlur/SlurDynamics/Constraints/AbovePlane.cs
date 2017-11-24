using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = AbovePlane.Handle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AbovePlane : MultiConstraint<H>, IConstraint
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
        public Vec3d Origin;
        /// <summary></summary>
        public Vec3d Normal;
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public AbovePlane(Vec3d origin, Vec3d normal, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Origin = origin;
            Normal = normal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="weight"></param>
        public AbovePlane(IEnumerable<int> indices, Vec3d origin, Vec3d normal, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Origin = origin;
            Normal = normal;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
            {
                double d = Vec3d.Dot(Origin - particles[h].Position, Normal);

                if (d <= 0.0)
                {
                    h.Skip = true;
                    continue;
                }

                h.Delta = (d / Normal.SquareLength * Normal);
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
