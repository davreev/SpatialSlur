
/*
 * Notes
 */

using System;

namespace SpatialSlur.Dynamics.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDelta"></typeparam>
    public abstract class WeightedInfluence<TDelta> : Influence<TDelta>
    {
        private double _weight = 1.0;

        
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
