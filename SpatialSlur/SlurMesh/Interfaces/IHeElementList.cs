using System;
using System.Collections.Generic;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHeElementList<T> : IReadOnlyList<T>
        where T : IHeElement
    {
        /// <summary>
        /// Removes all attributes corresponding with elements which have been flagged for removal.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        void CompactAttributes<U>(List<U> attributes);


        /// <summary>
        /// Moves all attributes corresponding with unflagged elements to the front of the given list.
        /// Returns the number of unflagged elements in the list.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        /// <returns></returns>
        int SwimAttributes<U>(IList<U> attributes);


        /// <summary>
        /// Returns true if this list contains the given element.
        /// Note this is an O(1) operation.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Contains(T element);


        /// <summary>
        /// Returns the number of elements that have been flagged for removal.
        /// </summary>
        /// <returns></returns>
        int CountRemoved();


        /// <summary>
        /// Returns the set difference of the given sequences.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        IEnumerable<T> GetDifference(IEnumerable<T> elementsA, IEnumerable<T> elementsB);


        /// <summary>
        /// Returns distinct elements from the given sequence.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<T> GetDistinct(IEnumerable<T> elements);


        /// <summary>
        /// Returns the set intersection of the given sequences.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        IEnumerable<T> GetIntersection(IEnumerable<T> elementsA, IEnumerable<T> elementsB);


        /// <summary>
        /// Returns the set union of the given sequences.
        /// </summary>
        /// <param name="elementsA"></param>
        /// <param name="elementsB"></param>
        /// <returns></returns>
        IEnumerable<T> GetUnion(IEnumerable<T> elementsA, IEnumerable<T> elementsB);
    }
}