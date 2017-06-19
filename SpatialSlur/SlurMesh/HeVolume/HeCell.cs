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
    public class HeCell<TV, TE, TF, TC> : HeElement, IHeCell<TV, TE, TF, TC>
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


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public bool IsRemoved
        {
            get { return _first == null; }
        }
    }
}
