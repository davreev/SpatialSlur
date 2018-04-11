
/*
 * Notes
 */ 

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
    [Serializable]
    public abstract class HeVolume<TV, TE, TF, TG> : HeStructure<TV, TE, TF, TG>
        where TV : HeVolume<TV, TE, TF, TG>.Vertex
        where TE : HeVolume<TV, TE, TF, TG>.Halfedge
        where TF : HeVolume<TV, TE, TF, TG>.Face
        where TG : HeVolume<TV, TE, TF, TG>.Group
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Vertex : HeVertex<TV, TE, TF, TG>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Halfedge : Halfedge<TV, TE, TF, TG>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Face : HeFace<TV, TE, TF, TG>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Group : HeNode<TG, TE>
        {
        }
        
        #endregion
    }
}
