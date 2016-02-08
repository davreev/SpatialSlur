using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class HeElement
    {
        private int _index = -1;


        /// <summary>
        /// Reeturns the element's position within the collection of the parent mesh.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// Returns true if the element lies on the mesh boundary.
        /// </summary>
        public abstract bool IsBoundary { get; }


        /// <summary>
        /// Returns true if the element has been flagged for removal.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsUnused { get; }


        /// <summary>
        /// Returns false for non-manifold elements.
        /// </summary>
        internal abstract bool IsValid { get; }


        /// <summary>
        /// Flags the element for removal.
        /// </summary>
        internal abstract void MakeUnused();
    }
}
