using System;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    using M = HeMesh;
    using V = HeMesh.Vertex;
    using E = HeMesh.Halfedge;
    using F = HeMesh.Face;

    /// <summary>
    /// Empty topology-only implementation
    /// </summary>
    [Serializable]
    public class HeMesh : HeMesh<V, E, F>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMesh<V, E, F>.Vertex
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMesh<V, E, F>.Halfedge
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public new class Face : HeMesh<V, E, F>.Face
        {
        }

        #endregion


        #region Static

        /// <summary></summary>
        public static readonly HeMeshFactory Factory = new HeMeshFactory();

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeMesh()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeMesh(int vertexCapacity, int hedgeCapacity, int faceCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public HeMesh(M other)
            : base(other.Vertices.Capacity, other.Halfedges.Capacity, other.Faces.Capacity)
        {
            Append(other);
        }


        /// <inheritdoc />
        protected sealed override V NewVertex()
        {
            return new V();
        }


        /// <inheritdoc />
        protected sealed override E NewHalfedge()
        {
            return new E();
        }


        /// <inheritdoc />
        protected sealed override F NewFace()
        {
            return new F();
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("HeMesh (V:{0} E:{1} F:{2})", Vertices.Count, Edges.Count, Faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(M other)
        {
            Append(other, null, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M GetDual()
        {
            return Factory.CreateDual(this, null, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(M other)
        {
            AppendDual(other, null, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, null, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, null, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, null, null, null);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory : HeMeshFactory<M, V, E, F>
    {
        /// <inheritdoc />
        public sealed override M Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new M(vertexCapacity, hedgeCapacity, faceCapacity);
        }
    }
}
