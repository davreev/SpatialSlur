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
    /// Applies a force proportional to the mass of each particle.
    /// </summary>
    [Serializable]
    public class Weight : MultiConstraint<H>, IConstraint
    {
        /// <summary>Describes the direction and magnitude of the applied weight</summary>
        public Vec3d Force;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public Weight(Vec3d vector, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Force = vector;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="force"></param>
        /// <param name="weight"></param>
        public Weight(IEnumerable<int> indices, Vec3d force, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Force = force;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
                h.Delta = Force * particles[h].Mass;
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
