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
    /// <summary>
    /// Empty topology-only implementation
    /// </summary>
    [Serializable]
    public class HeMesh : HeMeshBase<HeMesh.Vertex, HeMesh.Halfedge, HeMesh.Face>
    {
        /// <summary></summary>
        public static readonly HeMeshFactory Factory;


        /// <summary>
        /// 
        /// </summary>
        static HeMesh()
        {
            Factory = new HeMeshFactory();
        }


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
        protected sealed override Vertex NewVertex()
        {
            return new Vertex();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override Halfedge NewHalfedge()
        {
            return new Halfedge();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected sealed override Face NewFace()
        {
            return new Face();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh Duplicate()
        {
            return Factory.CreateCopy(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeMesh other)
        {
            Append(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh GetDual()
        {
            return Factory.CreateDual(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void AppendDual(HeMesh other)
        {
            AppendDual(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeMesh[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public HeMesh[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, delegate { }, out componentIndices, out edgeIndices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public HeMesh[] SplitDisjoint(Property<Halfedge, int> componentIndex, Property<Halfedge, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, delegate { }, componentIndex, edgeIndex);
        }


        #region Derived element types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge, Face>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Halfedge : Halfedge<Vertex, Halfedge, Face>
        {
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public class Face : HeFace<Vertex, Halfedge, Face>
        {
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory : HeMeshFactoryBase<HeMesh, HeMesh.Vertex, HeMesh.Halfedge, HeMesh.Face>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeMesh Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new HeMesh(vertexCapacity, hedgeCapacity, faceCapacity);
        }
    }


    /// <summary>
    /// Generic implementation for user-definable attributes.
    /// </summary>
    [Serializable]
    public class HeMesh<TV, TE, TF> : HeMeshBase<HeMesh<TV, TE, TF>.Vertex, HeMesh<TV, TE, TF>.Halfedge, HeMesh<TV, TE, TF>.Face>
    {
        /// <summary></summary>
        public static readonly HeMeshFactory<TV, TE, TF> Factory;


        /// <summary>
        /// 
        /// </summary>
        static HeMesh()
        {
            Factory = new HeMeshFactory<TV, TE, TF>();
        }


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
        protected override Vertex NewVertex()
        {
            return new Vertex();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Halfedge NewHalfedge()
        {
            return new Halfedge();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Face NewFace()
        {
            return new Face();
        }


        #region Derived element types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge, Face>
        {
            /// <summary></summary>
            public TV Value { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class Halfedge : Halfedge<Vertex, Halfedge, Face>
        {
            /// <summary></summary>
            public TE Value { get; set; }
        }


        /// <summary>
        ///
        /// </summary>
        [Serializable]
        public class Face : HeFace<Vertex, Halfedge, Face>
        {
            /// <summary></summary>
            public TF Value { get; set; }
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeMeshFactory<TV, TE, TF> : HeMeshFactoryBase<HeMesh<TV, TE, TF>, HeMesh<TV, TE, TF>.Vertex, HeMesh<TV, TE, TF>.Halfedge, HeMesh<TV, TE, TF>.Face>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeMesh<TV, TE, TF> Create(int vertexCapacity, int hedgeCapacity, int faceCapacity)
        {
            return new HeMesh<TV, TE, TF>(vertexCapacity, hedgeCapacity, faceCapacity);
        }
    }
}
