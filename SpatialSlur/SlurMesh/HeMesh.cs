using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

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
    public class HeMesh : HeMeshBase<V, E, F>
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Vertex : HeMeshBase<V, E, F>.Vertex
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new class Halfedge : HeMeshBase<V, E, F>.Halfedge
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public new class Face : HeMeshBase<V, E, F>.Face
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
        /// <returns></returns>
        protected sealed override V NewVertex()
        {
            return new V();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override E NewHalfedge()
        {
            return new E();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override F NewFace()
        {
            return new F();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh (V:{0} E:{1} F:{2})", Vertices.Count, Halfedges.Count >> 1, Faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M Duplicate()
        {
            return Factory.CreateCopy(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(M other)
        {
            Append(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M GetDual()
        {
            return Factory.CreateDual(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(M other)
        {
            AppendDual(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public M[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public M[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, delegate { }, delegate { }, delegate { });
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory : HeMeshBaseFactory<M, V, E, F>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override M Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new M(vertexCapacity, hedgeCapacity, faceCapacity);
        }
    }
}
