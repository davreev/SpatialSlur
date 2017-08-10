using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHeElement
    {
        /// <summary>
        /// Returns this element's position within its parent list.
        /// </summary>
        int Index { get; }


        /// <summary>
        /// Returns true if the element has been flagged for removal.
        /// </summary>
        bool IsRemoved { get; }
    }
}
