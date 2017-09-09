
/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Determines what happens when a field is evaluated beyond its bounds.
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
