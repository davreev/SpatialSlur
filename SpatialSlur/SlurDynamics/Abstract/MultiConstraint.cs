using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Base class for constraints which act on a dynamic collection of bodies.
    /// </summary>
    [Serializable]
    public abstract class MultiConstraint<H>: Constraint
        where H : IHandle
    {
        #region Static

        protected const int DefaultCapacity = 4;

        #endregion


        private List<H> _handles;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="capacity"></param>
        public MultiConstraint(double weight, int capacity = DefaultCapacity)
        {
            _handles = new List<H>(capacity);
            Weight = weight;
        }


        /// <summary>
        /// 
        /// </summary>
        public List<H> Handles
        {
            get { return _handles; }
        }
    }
}
