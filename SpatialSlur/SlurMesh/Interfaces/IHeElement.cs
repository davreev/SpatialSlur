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


    /// <summary>
    /// 
    /// </summary>
    public static class IHeElementExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        internal static void RemovedCheck(this IHeElement element)
        {
            if (element.IsRemoved)
                throw new ArgumentException("The given element has been flagged for removal. The operation cannot be performed.");
        }
    }
}
