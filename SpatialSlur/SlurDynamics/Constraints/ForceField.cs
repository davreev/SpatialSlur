using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics.Constraints
{
    using H = ParticleHandle;

    /// <summary>
    /// 
    /// </summary>
    public class ForceField : MultiConstraint<H>, IConstraint
    {
        private IField3d<Vec3d> _field;


        /// <summary>
        /// 
        /// </summary>
        public IField3d<Vec3d> Field
        {
            get { return _field; }
            set { _field = value ?? throw new ArgumentNullException(); }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="capacity"></param>
        /// <param name="weight"></param>
        public ForceField(IField3d<Vec3d> field, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Field = field;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="field"></param>
        /// <param name="weight"></param>
        public ForceField(IEnumerable<int> indices, IField3d<Vec3d> field, double weight = 1.0, int capacity = DefaultCapacity)
            : base(weight, capacity)
        {
            Handles.AddRange(indices.Select(i => new H(i)));
            Field = field;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Calculate(IReadOnlyList<IBody> particles)
        {
            foreach (var h in Handles)
                h.Delta = _field.ValueAt(particles[h].Position);
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
