﻿
/*
 * Notes
 */

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstraint : IInfluence
    {
        /// <summary>
        /// 
        /// </summary>
        double Weight { get; set; }
    }
}
