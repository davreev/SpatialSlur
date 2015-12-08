using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// abstract base class for all HeMesh elements
    /// </summary>
    public abstract class HeElement
    {
        private int _index = -1;


        /// <summary>
        /// element's position within the collection of the parent mesh
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// true if the element lies on the mesh boundary
        /// </summary>
        public abstract bool IsBoundary { get; }


        /// <summary>
        /// true if element has been flagged for removal
        /// </summary>
        /// <returns></returns>
        public abstract bool IsUnused { get; }


        /// <summary>
        /// returns false for non-manifold elements
        /// assumes element is used
        /// </summary>
        internal abstract bool IsValid { get; }


        /// <summary>
        /// 
        /// </summary>
        internal abstract void MakeUnused();
    }
}
