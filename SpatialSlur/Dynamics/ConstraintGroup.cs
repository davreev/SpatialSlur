
/*
 * Notes
 */

using System;
using System.Collections;
using System.Collections.Generic;

using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstraintGroup : Impl.InfluenceGroup, IEnumerable<IConstraint>
    {
        private List<IConstraint> _constraints;

        /// <summary>
        /// 
        /// </summary>
        public ConstraintGroup()
        {
            _constraints = new List<IConstraint>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public ConstraintGroup(int capacity)
        {
            _constraints = new List<IConstraint>(capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IConstraint> Constraints
        {
            get => _constraints;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            Apply(_constraints, particles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<IConstraint>.Enumerator GetEnumerator()
        {
            return _constraints.GetEnumerator();
        }


        #region Explicit interface implementations

        IEnumerator<IConstraint> IEnumerable<IConstraint>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
