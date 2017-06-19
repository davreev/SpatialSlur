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
    public abstract class HeVertexField<T> : HeElementField<HeMeshVertex, T>
        where T : struct
    {
        private HeMeshVertexList _vertices;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        protected HeVertexField(HeMeshVertexList vertices)
            :base(vertices)
        {
            _vertices = vertices;
        }


        /// <summary>
        /// Returns the vertex list associated with this field.
        /// </summary>
        public HeMeshVertexList Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// Returns the value at the given barycentric coordinates within the given triangle.
        /// Assumes weights sum to 1.0.
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public abstract T ValueAt(int i0, int i1, int i2, double w0, double w1, double w2);
    }
}
