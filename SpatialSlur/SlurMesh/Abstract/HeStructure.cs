using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 * 
 * Provides common implementations for all derived Halfedge structures
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E>
        where V : HeElement<V, E>, IHeVertex<V, E>
        where E : Halfedge<V, E>, IHalfedge<V, E>
    {
        #region static

        protected const int DefaultCapacity = 4;

        #endregion


        private HeElementList<V> _vertices;
        private HalfedgeList<E> _hedges;
        private HeEdgeList<E> _edges;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity)
        {
            _vertices = new HeElementList<V>(vertexCapacity);
            _hedges = new HalfedgeList<E>(hedgeCapacity);
            _edges = new HeEdgeList<E>(_hedges);
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
        public HalfedgeList<E> Halfedges
        {
            get { return _hedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeEdgeList<E> Edges
        {
            get { return _edges; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract E NewHalfedge();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract V NewVertex();


        /// <summary>
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Contains(E hedge)
        {
            return _hedges.Contains(hedge);
        }


        /// <summary>
        /// Returns true if the given vertex belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(V vertex)
        {
            return _vertices.Contains(vertex);
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

        
        /// <summary>
        /// Creates a new pair of halfedges and adds them to the list.
        /// Returns the first halfedge in the pair.
        /// </summary>
        /// <returns></returns>
        internal E AddHalfedges()
        {
            var he0 = NewHalfedge();
            var he1 = NewHalfedge();

            he0.Twin = he1;
            he1.Twin = he0;

            _hedges.Add(he0);
            _hedges.Add(he1);

            return he0;
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
        where V : HeElement<V, E>, IHeVertex<V, E, F>
        where E : Halfedge<V, E, F>, IHalfedge<V, E, F>
        where F : HeElement<F, E>, IHeFace<V, E, F>
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
            _faces = new HeElementList<F>(faceCapacity);
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
        public bool Contains(F face)
        {
            return _faces.Contains(face);
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
    [Serializable]
    public abstract class HeStructure<V, E, F, C> : HeStructure<V, E, F>
        where V : HeElement<V, E>, IHeVertex<V, E, F, C>
        where E : Halfedge<V, E, F, C>, IHalfedge<V, E, F, C>
        where F : HeElement<F, E>, IHeFace<V, E, F, C>
        where C : HeElement<C, E>, IHeCell<V, E, F, C>
    {
        private HeElementList<E> _bundles;
        private HeElementList<C> _cells;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity = DefaultCapacity, int hedgeCapacity = DefaultCapacity, int faceCapacity = DefaultCapacity, int cellCapacity = DefaultCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
            _bundles = new HeElementList<E>(hedgeCapacity >> 2);
            _cells = new HeElementList<C>(cellCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<E> Bundles
        {
            get { return _bundles; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<C> Cells
        {
            get { return _cells; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract C NewCell();


        /// <summary>
        /// Returns true if the given cell belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(C cell)
        {
            return _cells.Contains(cell);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal C AddCell()
        {
            var c = NewCell();
            _cells.Add(c);
            return c;
        }
    }
}
