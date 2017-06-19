
/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Determines what happens when a field is evaluated outside of its domain.
    /// </summary>
    public enum WrapMode
    {
        /// <summary></summary>
        Clamp,
        /// <summary></summary>
        Repeat,
        /// <summary></summary>
        MirrorRepeat
    }
}
