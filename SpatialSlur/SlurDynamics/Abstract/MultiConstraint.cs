using System;
using System.Collections.Generic;
using System.Linq;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Base class for constraints on a dynamic collection of particles.
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
        public List<H> Handles
        {
            get { return _handles; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="capacity"></param>
        public MultiConstraint(double weight = 1.0, int capacity = DefaultCapacity)
        {
            _handles = new List<H>(capacity);
            Weight = weight;
        }
    }
}
