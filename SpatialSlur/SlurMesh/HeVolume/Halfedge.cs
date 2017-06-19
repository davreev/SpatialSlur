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
    public class Halfedge<TV, TE, TF, TC>: HeElement, IHalfedge<TV, TE, TF, TC>
        where TV : HeVertex<TV, TE, TF, TC>
        where TE : Halfedge<TV, TE, TF, TC>
        where TF : HeFace<TV, TE, TF, TC>
        where TC : HeCell<TV, TE, TF, TC>
    {
        private TE _self; // cached downcasted ref of this instance (TODO test impact)
        private TE _prev;
        private TE _next;
        private TE _twin;
        private TE _adj;

        private TV _start;
        private TF _face;
        private TC _cell;


        /// <summary>
        /// 
        /// </summary>
        public Halfedge()
        {
            _self = (TE)this;
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE Adjacent
        {
            get { return _adj; }
            internal set { _adj = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE PrevInFace
        {
            get { return _prev; }
            internal set { _prev = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE NextInFace
        {
            get { return _next; }
            internal set { _next = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE PrevAtStart
        {
            get { return _prev._twin; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TE NextAtStart
        {
            get { return _twin._next; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TV Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TV End
        {
            get { return _twin._start; }
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        public TF Face
        {
            get { return _face; }
            internal set { _face = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public TC Cell
        {
            get { return _cell; }
            internal set { _cell = value; }
        }
    }
}
