using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OnPlane : MultiConstraint<H>, IConstraint
    {
        /// <summary></summary>
        public Vec3d Origin;
        /// <summary></summary>
        public Vec3d Normal;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <param name="weight"></param>
        public OnPlane(Vec3d origin, Vec3d normal, double weight = 1.0, int capacity = DefaultCapacity)
            :base(weight, capacity)
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
        public OnPlane(IEnumerable<int> indices, Vec3d origin, Vec3d normal, double weight = 1.0, int capacity = DefaultCapacity)
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
                h.Delta = Vec3d.Project(Origin - particles[h].Position, Normal);
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        public void Apply(IReadOnlyList<IBody> bodies)
        {
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
