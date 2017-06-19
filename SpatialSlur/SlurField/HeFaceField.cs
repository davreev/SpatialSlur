using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurCore;

/*
* Notes
*/

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HeFaceFieldT<T> : HeElementField<HeMeshFace, T>
        where T : struct
    {
        private HeMeshFaceList _faces;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        protected HeFaceFieldT(HeMeshFaceList faces)
            :base(faces)
        {
            _faces = faces;
        }


        /// <summary>
        /// Returns the face list associated with this field.
        /// </summary>
        public HeMeshFaceList Faces
        {
            get { return _faces; }
        }
    }
}

