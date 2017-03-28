using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class HeMeshFace : HeFace<HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        internal override HeMeshFace Self
        {
            get { return this; }
        }


        /// <summary>
        /// Returns true if the face has more than 2 edges.
        /// </summary>
        internal bool IsValid
        {
            get { return !(IsDegree1 || IsDegree2); }
        }


        /// <summary>
        /// Returns true if the face has 1 edge.
        /// </summary>
        internal bool IsDegree1
        {
            get { return First.IsInDegree1; }
        }


        /// <summary>
        /// Returns true if the face has 2 edges.
        /// This state is invalid
        /// </summary>
        internal bool IsDegree2
        {
            get { return First.IsInDegree2; }
        }


        /// <summary>
        /// Returns true if the face has 3 edges.
        /// </summary>
        public bool IsDegree3
        {
            get { return First.IsInDegree3; }
        }
    }
}
