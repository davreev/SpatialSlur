
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics.Forces
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Force
    {
        private double _strength;


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public double Strength
        {
            get { return _strength; }
            set { _strength = value; }
        }
    }
}
