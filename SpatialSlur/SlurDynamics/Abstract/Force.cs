using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
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
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _strength = value;
            }
        }
    }
}
