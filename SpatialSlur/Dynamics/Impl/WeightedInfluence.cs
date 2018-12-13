
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
    public abstract class WeightedInfluence<TDelta> : InfluenceBase<TDelta>
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
