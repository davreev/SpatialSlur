
/*
 * Notes
 */

using System;
using SpatialSlur;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
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
        #region Nested Types

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


        #region Static Members

        /// <summary></summary>
        public static readonly HeGraphFactory Factory = new HeGraphFactory();

        #endregion


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
        /// <param name="other"></param>
        public HeGraph(G other)
            : base(other.Vertices.Capacity, other.Halfedges.Capacity)
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
            Append(other, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public G[] SplitDisjoint()
        {
            return Factory.CreateConnectedComponents(this, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndices"></param>
        /// <param name="edgeIndices"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(out int[] componentIndices, out int[] edgeIndices)
        {
            return Factory.CreateConnectedComponents(this, out componentIndices, out edgeIndices, null, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentIndex"></param>
        /// <param name="edgeIndex"></param>
        /// <returns></returns>
        public G[] SplitDisjoint(Property<E, int> componentIndex, Property<E, int> edgeIndex)
        {
            return Factory.CreateConnectedComponents(this, componentIndex, edgeIndex, null, null);
        }
    }
}
