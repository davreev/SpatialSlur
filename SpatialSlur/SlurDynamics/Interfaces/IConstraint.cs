using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        ConstraintType Type { get; }


        /// <summary>
        /// Handles to all bodies used by this constraint.
        /// </summary>
        IEnumerable<IHandle> Handles { get; }


        /// <summary>
        /// This method is responsible for calculating all deltas and weights associated with the constraint.
        /// </summary>
        /// <param name="bodies"></param>
        void Calculate(IReadOnlyList<IBody> bodies);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodies"></param>
        void Apply(IReadOnlyList<IBody> bodies);
    }
}
