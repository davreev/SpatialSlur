
/*
 * Notes
 */

using System.Collections.Generic;
using SpatialSlur.Collections;

namespace SpatialSlur.Dynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Gets or sets the indices of the bodies affected by this constraint.
        /// </summary>
        IEnumerable<int> Indices { get; set; }


        /// <summary>
        /// Returns true if this constraint affects a body's position.
        /// </summary>
        bool AffectsPosition { get; }


        /// <summary>
        /// Returns true if this constraint affects a body's rotation.
        /// </summary>
        bool AffectsRotation { get; }


        /// <summary>
        /// Calculates all forces and projections associated with this constraint.
        /// </summary>
        /// <param name="bodies"></param>
        void Calculate(ReadOnlyArrayView<Body> bodies);


        /// <summary>
        /// Applies calculated forces and projections to the affected bodies.
        /// </summary>
        /// <param name="bodies"></param>
        void Apply(ReadOnlyArrayView<Body> bodies);


        /// <summary>
        /// Returns the energy that this constraint is trying to minimize.
        /// The constraint is satisfied when this equals zero.
        /// </summary>
        void GetEnergy(out double linear, out double angular);
    }
}
