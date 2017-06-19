using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class HeFace<TV, TE, TF, TC> : HeElement, IHeFace<TV, TE, TF, TC>
        where TV : HeVertex<TV, TE, TF, TC>
        where TE : Halfedge<TV, TE, TF, TC>
        where TF : HeFace<TV, TE, TF, TC>
        where TC : HeCell<TV, TE, TF, TC>
    {
        private TE _first;


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public TF Twin
        {
            get { return _first.Adjacent.Face; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get { return _first == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsBoundary
        {
            get { return _first.Cell == null || _first.Adjacent.Cell == null; }
        }
    }
}
