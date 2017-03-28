using System;

/*
 * Notes
 * 
 * Inheritance follows the curiously recurring template pattern
 * https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern
 * https://blogs.msdn.microsoft.com/ericlippert/2011/02/03/curiouser-and-curiouser/
 * http://eli.thegreenplace.net/2013/12/05/the-cost-of-dynamic-virtual-calls-vs-static-crtp-dispatch-in-c
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class HeElement
    {
        private int _index = -1;
        private int _tag; // tag for topological searches, validation etc.


        /// <summary>
        /// Returns the element's position within its parent list.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }

        
        /// <summary>
        /// 
        /// </summary>
        internal int Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
 

        /// <summary>
        /// Returns true if the element is not in use.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsUnused { get; }


        /// <summary>
        /// Flags the element for removal.
        /// </summary>
        internal abstract void MakeUnused();


        /// <summary>
        /// Throws an exception for unused elements.
        /// </summary>
        internal void UsedCheck()
        {
            if (IsUnused)
                throw new ArgumentException("The given element must be in use for this operation.");
        }
    }
}
