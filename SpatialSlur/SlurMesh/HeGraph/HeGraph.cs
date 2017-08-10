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
    public class HeGraph : HeGraphBase<HeGraph.Vertex, HeGraph.Halfedge>
    {
        /// <summary></summary>
        public static readonly HeGraphFactory Factory;


        /// <summary>
        /// Static constructor to initialize factory instance.
        /// </summary>
        static HeGraph()
        {
            Factory = new HeGraphFactory();
        }


        /// <summary>
        /// 
        /// </summary>
        public HeGraph()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="halfedgeCapacity"></param>
        public HeGraph(int vertexCapacity, int halfedgeCapacity)
            : base(vertexCapacity, halfedgeCapacity)
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
        public HeGraph Duplicate()
        {
            return Factory.CreateCopy(this, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeGraph other)
        {
            Append(other, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HeGraph[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public HeGraph[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, out componentIndices, out edgeIndices);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public HeGraph[] SplitDisjoint(Property<Halfedge, int> componentIndex, Property<Halfedge, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { }, componentIndex, edgeIndex);
        }


        #region Derived element types

        /// <summary>
        /// Default empty vertex
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge>
        {
        }


        /// <summary>
        /// Default empty halfedge
        /// </summary>
        [Serializable]
        public class Halfedge : Halfedge<Vertex, Halfedge>
        {
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraphFactory : HeGraphFactoryBase<HeGraph, HeGraph.Vertex, HeGraph.Halfedge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeGraph Create(int vertexCapacity, int hedgeCapacity)
        {
            return new HeGraph(vertexCapacity, hedgeCapacity);
        }
    }


    /// <summary>
    /// Empty topology-only implementation
    /// </summary>
    [Serializable]
    public class HeGraph<TV,TE> : HeGraphBase<HeGraph<TV,TE>.Vertex, HeGraph<TV,TE>.Halfedge>
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<TV,TE> Factory;


        /// <summary>
        /// Static constructor to initialize factory instance.
        /// </summary>
        static HeGraph()
        {
            Factory = new HeGraphFactory<TV, TE>();
        }


        /// <summary>
        /// 
        /// </summary>
        public HeGraph()
            : base()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="halfedgeCapacity"></param>
        public HeGraph(int vertexCapacity, int halfedgeCapacity)
            : base(vertexCapacity, halfedgeCapacity)
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


        #region Derived element types

        /// <summary>
        /// Default empty vertex
        /// </summary>
        [Serializable]
        public class Vertex : HeVertex<Vertex, Halfedge>
        {
            /// <summary></summary>
            public TV Value { get; set; }
        }


        /// <summary>
        /// Default empty halfedge
        /// </summary>
        [Serializable]
        public class Halfedge : Halfedge<Vertex, Halfedge>
        {
            /// <summary></summary>
            public TE Value { get; set; }
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraphFactory<TV,TE> : HeGraphFactoryBase<HeGraph<TV,TE>, HeGraph<TV, TE>.Vertex, HeGraph<TV, TE>.Halfedge>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override HeGraph<TV, TE> Create(int vertexCapacity, int hedgeCapacity)
        {
            return new HeGraph<TV, TE>(vertexCapacity, hedgeCapacity);
        }
    }
}
