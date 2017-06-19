using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Provides common and non-public implementations.
    /// Should be inherited alongside IHeElement.
    /// </summary>
    [Serializable]
    public abstract class HeElement
    {
        #region Static

        /// <summary>
        /// Implicitly converts an element to its index for convenience.
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator int(HeElement element)
        {
            return element.Index;
        }

        #endregion


        private int _index = -1;
        private int _tag;


        /// <summary>
        /// Returns this element's position within its parent list.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// General purpose tag used for topological searches and validation.
        /// </summary>
        internal int Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}
