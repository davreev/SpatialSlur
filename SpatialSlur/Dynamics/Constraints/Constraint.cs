
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics.Constraints
{
    /// <summary>
    /// Contains any implementation details common to ALL constraints.
    /// </summary>
    [Serializable]
    public abstract class Constraint
    {
        private double _weight;


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                _weight = value;
            }
        }
    }
}
