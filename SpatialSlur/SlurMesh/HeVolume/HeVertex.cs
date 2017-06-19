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
    public class HeVertex<TV, TE, TF, TC> : HeElement, IHeVertex<TV, TE, TF, TC>
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
        public TE FirstOut
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE FirstIn
        {
            get { return _first.Twin; }
        }

    }
}
