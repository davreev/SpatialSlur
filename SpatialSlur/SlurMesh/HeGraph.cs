using System;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    using G = HeGraph;
    using V = HeGraph.Vertex;
    using E = HeGraph.Halfedge;

    /// <summary>
    /// Empty topology-only implementation
    /// </summary>
    [Serializable]
    public class HeGraph : HeGraph<V, E>
    {
        #region Nested types

        /// <summary>
        /// Default empty vertex
        /// </summary>
        [Serializable]
        public new class Vertex : HeGraph<V, E>.Vertex
        {
        }


        /// <summary>
        /// Default empty halfedge
        /// </summary>
        [Serializable]
        public new class Halfedge : HeGraph<V, E>.Halfedge
        {
        }

        #endregion


        #region Static

        /// <summary></summary>
        public static readonly HeGraphFactory Factory = new HeGraphFactory();

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="halfedgeCapacity"></param>
        public HeGraph(int vertexCapacity = DefaultCapacity, int halfedgeCapacity = DefaultCapacity)
            : base(vertexCapacity, halfedgeCapacity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public HeGraph(G other)
            : base(other.Vertices.Capacity, other.Halfedges.Capacity)
        {
            Append(other);
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
        public override string ToString()
        {
            return String.Format("HeGraph (V:{0} E:{1})", Vertices.Count, Edges.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(G other)
        {
            Append(other, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public G[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, delegate { }, delegate { });
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HeGraphFactory : HeGraphFactory<G, V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override G Create(int vertexCapacity, int hedgeCapacity)
        {
            return new G(vertexCapacity, hedgeCapacity);
        }
    }
}
