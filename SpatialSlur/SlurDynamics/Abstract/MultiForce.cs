using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// Base class for forces which act on a dynamic collection of bodies.
    /// </summary>
    [Serializable]
    public abstract class MultiForce<H>: Force
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
        public MultiForce(double strength, int capacity = DefaultCapacity)
        {
            _handles = new List<H>(capacity);
            Strength = strength;
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
