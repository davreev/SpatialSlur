


/*
 * Notes
 */

using System;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeStructure<E>
        where E : Halfedge<E>
    {
        #region static

        protected const int DefaultCapacity = 4;

        #endregion
        

        private HalfedgeList<E> _hedges;
        private EdgeList<E> _edges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int hedgeCapacity = DefaultCapacity)
        {
            _hedges = new HalfedgeList<E>(hedgeCapacity);
            _edges = new EdgeList<E>(_hedges);
        }


        /// <summary>
        /// 
        /// </summary>
        public HalfedgeList<E> Halfedges
        {
            get { return _hedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public EdgeList<E> Edges
        {
            get { return _edges; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract E NewHalfedge();


        /// <summary>
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Owns(E hedge)
        {
            return _hedges.Owns(hedge);
        }


        /// <summary>
        /// Creates a new pair of halfedges and adds them to the list.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <returns></returns>
        internal E AddEdge()
        {
            var he0 = NewHalfedge();
            var he1 = NewHalfedge();

            he0.Twin = he1;
            he1.Twin = he0;

            _hedges.Add(he0);
            _hedges.Add(he1);

            return he0;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        protected void Append<T, U>(HeNodeList<T, E> nodes, HeNodeList<U, E> other, Action<T, U> setNode)
            where T : HeNode<T, E>
            where U : HeNode<U, E>
        {
            // TODO
            throw new NotImplementedException();
        }
        */
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E> : HeStructure<E>
        where V : HeVertex<V, E>
        where E : Halfedge<V, E>
    {
        private HeElementList<V> _vertices;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity)
            : base(hedgeCapacity)
        {
            _vertices = new HeNodeList<V, E>(vertexCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<V> Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract V NewVertex();


        /// <summary>
        /// Returns true if the given vertex belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Owns(V vertex)
        {
            return _vertices.Owns(vertex);
        }


        /// <summary>
        /// 
        /// </summary>
        public V AddVertex()
        {
            var v = NewVertex();
            _vertices.Add(v);
            return v;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E, F> : HeStructure<V, E>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        private HeElementList<F> _faces;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity, int faceCapacity = DefaultCapacity)
            :base(vertexCapacity, hedgeCapacity)
        {
            _faces = new HeNodeList<F, E>(faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<F> Faces
        {
            get { return _faces; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract F NewFace();


        /// <summary>
        /// Returns true if the given face belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Owns(F face)
        {
            return _faces.Owns(face);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal F AddFace()
        {
            var f = NewFace();
            _faces.Add(f);
            return f;
        }
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E, F, G> : HeStructure<V, E, F>
        where V : HeVertex<V, E, F, G>
        where E : Halfedge<V, E, F, G>
        where F : HeFace<V, E, F, G>
        where G : HeNode<G, E>
    {
        private HeElementList<G> _clusters;
        private HeElementList<G> _bundles;
        private HeElementList<G> _cells;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity, int faceCapacity = DefaultCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
            _clusters = new HeNodeList<G, E>(vertexCapacity >> 2); // assumes 4 verts per cluster
            _bundles = new HeNodeList<G, E>(hedgeCapacity >> 3); // assumes 4 edges per bundle
            _cells = new HeNodeList<G, E>(faceCapacity >> 2); // assumes 4 faces per cell
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<G> Clusters
        {
            get { return _bundles; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<G> Bundles
        {
            get { return _bundles; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<G> Cells
        {
            get { return _cells; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract G NewGroup();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal G AddCluster()
        {
            var g = NewGroup();
            _bundles.Add(g);
            return g;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal G AddBundle()
        {
            var g = NewGroup();
            _bundles.Add(g);
            return g;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal G AddCell()
        {
            var g = NewGroup();
            _cells.Add(g);
            return g;
        }
    }
}