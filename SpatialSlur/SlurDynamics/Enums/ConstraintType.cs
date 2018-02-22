/*
 * Notes
 */

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    public enum ConstraintType
    {
        /// <summary>Constraint only affects the position of given bodies.</summary>
        Position,
        /// <summary>Constraint only affects the rotation of given bodies.</summary>
        Rotation,
        /// <summary>Constraint affects both the position and rotation of given bodies.</summary>
        PositionRotation
    }
}
