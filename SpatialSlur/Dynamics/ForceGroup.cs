
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
    public class ForceGroup : Impl.InfluenceGroup, IEnumerable<IForce>
    {
        private List<IForce> _forces;

        /// <summary>
        /// 
        /// </summary>
        public ForceGroup()
        {
            _forces = new List<IForce>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public ForceGroup(int capacity)
        {
            _forces = new List<IForce>(capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public List<IForce> Forces
        {
            get => _forces;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        public void Apply(ReadOnlyArrayView<Particle> particles)
        {
            Apply(_forces, particles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<IForce>.Enumerator GetEnumerator()
        {
            return _forces.GetEnumerator();
        }


        #region Explicit interface implementations

        IEnumerator<IForce> IEnumerable<IForce>.GetEnumerator()
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
