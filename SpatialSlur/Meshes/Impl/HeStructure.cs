
/*
 * Notes
 */

using System;
using System.Collections.Generic;

namespace SpatialSlur.Meshes.Impl
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E>
        where V : HeStructure<V, E>.Vertex
        where E : HeStructure<V, E>.Halfedge
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Halfedge : Halfedge<E>
        {
            private V _start;


            /// <summary>
            /// Returns the vertex at the start of this halfedge.
            /// </summary>
            public V Start
            {
                get { return _start; }
                internal set { _start = value; }
            }


            /// <summary>
            /// Returns the vertex at the end of this halfedge.
            /// </summary>
            public V End
            {
                get { return Twin._start; }
            }


            /// <summary>
            /// Returns true if this halfedge is the first at its start vertex.
            /// </summary>
            public bool IsFirstAtStart
            {
                get { return this == _start.First; }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Vertex : Node<V, E>
        {
            /// <summary>
            /// Circulates through all halfedges starting at this vertex.
            /// </summary>
            public IEnumerable<E> OutgoingHalfedges
            {
                get { return First.CirculateStart; }
            }


            /// <summary>
            /// Circulates through all halfedges ending at this vertex.
            /// </summary>
            public IEnumerable<E> IncomingHalfedges
            {
                get { return First.Twin.CirculateEnd; }
            }


            /// <summary>
            /// Circulates through all vertices connected to this vertex.
            /// </summary>
            public IEnumerable<V> ConnectedVertices
            {
                get
                {
                    var he0 = First.Twin;
                    var he1 = he0;

                    do
                    {
                        yield return he1.Start;
                        he1 = he1.NextAtEnd;
                    } while (he1 != he0);
                }
            }


            /// <summary>
            /// Returns true if this vertex has 1 outgoing halfedge.
            /// </summary>
            public bool IsDegree1
            {
                get { return First.IsAtDegree1; }
            }


            /// <summary>
            /// Returns true if the vertex has 2 outgoing halfedges.
            /// </summary>
            public bool IsDegree2
            {
                get { return First.IsAtDegree2; }
            }


            /// <summary>
            /// Returns true if the vertex has 3 outgoing halfedges.
            /// </summary>
            public bool IsDegree3
            {
                get { return First.IsAtDegree3; }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public int GetDegree()
            {
                return First.CountAtStart();
            }


            /// <summary>
            /// Returns true if the number of edges at this vertex is equal to the given value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool IsDegree(int value)
            {
                return First.IsAtDegree(value);
            }


            /// <summary>
            /// Returns true if this vertex is connected to the given vertex.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool IsConnectedTo(V other)
            {
                return FindHalfedgeTo(other) != null;
            }


            /// <summary>
            /// Returns a halfedge from this vertex to another or null if none exists.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public E FindHalfedgeTo(V other)
            {
                var he0 = First.Twin;
                var he1 = he0;

                do
                {
                    if (he1.Start == other) return he1.Twin;
                    he1 = he1.NextAtEnd;
                } while (he1 != he0);

                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static class Edge
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="values"></param>
            /// <returns></returns>
            public static Property<E, U> CreateProperty<U>(U[] values)
            {
                return new Property<E, U>(he => values[he >> 1], (he, u) => values[he >> 1] = u);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="values"></param>
            /// <returns></returns>
            public static Property<E, U> CreateProperty<U>(IList<U> values)
            {
                return new Property<E, U>(he => values[he >> 1], (he, u) => values[he >> 1] = u);
            }
        }

        #endregion


        private NodeList<V> _vertices;
        private HalfedgeList<E> _hedges;


        /// <summary>
        /// 
        /// </summary>
        public HeStructure()
        {
            _vertices = new NodeList<V, E>();
            _hedges = new HalfedgeList<E>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        public HeStructure(int vertexCapacity, int hedgeCapacity)
        {
            _vertices = new NodeList<V, E>(vertexCapacity);
            _hedges = new HalfedgeList<E>(hedgeCapacity);
        }

      
        /// <summary>
        /// 
        /// </summary>
        public NodeList<V> Vertices
        {
            get => _vertices;
        }

        
        /// <summary>
        /// 
        /// </summary>
        public HalfedgeList<E> Halfedges
        {
            get => _hedges;
        }


        /// <summary>
        /// 
        /// </summary>
        public EdgeListView<E> Edges
        {
            get => new EdgeListView<E>(_hedges);
        }


        /// <summary>
        /// Significantly faster than using a new() constraint on the type parameter
        /// https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
        /// </summary>
        /// <returns></returns>
        protected abstract V NewVertex();

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract E NewHalfedge();


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
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public bool Owns(E hedge)
        {
            return _hedges.Owns(hedge);
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
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E, F> : HeStructure<V, E>
        where V : HeStructure<V, E, F>.Vertex
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Halfedge : HeStructure<V, E>.Halfedge
        {
            private F _face;


            /// <summary>
            /// Returns the face adjacent to this halfedge.
            /// If this halfedge is adjacent to a hole, null is returned.
            /// </summary>
            public F Face
            {
                get { return _face; }
                internal set { _face = value; }
            }


            /// <summary>
            /// Returns true if this halfedge is the first in its face.
            /// </summary>
            public bool IsFirstInFace
            {
                get { return this == Face.First; }
            }


            /// <summary>
            /// Returns true if this halfedge or its twin is adjacent to a hole.
            /// </summary>
            public bool IsBoundary
            {
                get { return Face == null || Twin.Face == null; }
            }


            /// <summary>
            /// Returns true if this halfedge is adjacent to a hole.
            /// </summary>
            public bool IsHole
            {
                get { return Face == null; }
            }


            /// <summary>
            /// Returns true if the halfedge and its twin have different faces.
            /// </summary>
            internal bool IsManifold
            {
                get { return Face != Twin.Face; }
            }


            /// <summary>
            /// Returns true this halfedge spans between non-consecutive boundary vertices.
            /// </summary>
            public bool IsBridge
            {
                get { return Start.IsBoundary && End.IsBoundary && !IsBoundary; }
            }


            /// <summary>
            /// Returns the next boundary halfedge encountered when circulating the loop of this halfedge.
            /// If no such halfedge is found, null is returned.
            /// </summary>
            /// <returns></returns>
            public E NextBoundary
            {
                get
                {
                    var he1 = Next;

                    do
                    {
                        if (he1.Twin.Face == null) return he1;
                        he1 = he1.Next;
                    } while (he1 != this);

                    return null;
                }
            }


            /// <summary>
            /// Returns the next faceless halfedge encountered when circulating around the start vertex of this halfedge.
            /// If no such halfedge is found, null is returned.
            /// </summary>
            /// <returns></returns>
            public E NextBoundaryAtStart
            {
                get
                {
                    var he = NextAtStart;

                    do
                    {
                        if (he.Face == null) return he;
                        he = he.NextAtStart;
                    } while (he != this);

                    return null;
                }
            }


            /// <summary>
            /// Returns the next faceless halfedge encountered when circulating around the end vertex of this halfedge.
            /// If no such halfedge is found, null is returned.
            /// </summary>
            /// <returns></returns>
            public E NextBoundaryAtEnd
            {
                get
                {
                    var he = NextAtEnd;

                    do
                    {
                        if (he.Face == null) return he;
                        he = he.NextAtEnd;
                    } while (he != this);

                    return null;
                }
            }


            /// <summary>
            /// Sets this halfedge to be the first in its face.
            /// </summary>
            public void MakeFirstInFace()
            {
                Face.First = Self;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Vertex : HeStructure<V, E>.Vertex
        {
            /// <summary>
            /// Circulates through all faces surrounding this vertex.
            /// Note that if multiple outgoing halfedges lie on the same face, that face will be returned multiple times.
            /// </summary>
            public IEnumerable<F> SurroundingFaces
            {
                get
                {
                    var he0 = First;
                    var he1 = he0;

                    do
                    {
                        var f = he1.Face;
                        if (f != null) yield return f;
                        he1 = he1.NextAtStart;
                    } while (he1 != he0);
                }
            }


            /// <summary>
            /// Returns true if this vertex is on the mesh boundary.
            /// </summary>
            public bool IsBoundary
            {
                get { return First.Face == null; }
            }


            /// <summary>
            /// Returns false if the vertex has more than one boundary edge (i.e. bowtie condition).
            /// </summary>
            public bool IsManifold
            {
                get
                {
                    var he = First;

                    // interior vertex, can assume manifold
                    if (he.Face != null)
                        return true;

                    // boundary vertex, check for second boundary
                    he = he.NextAtStart;
                    do
                    {
                        if (he.Face == null) return false;
                    } while (he != First);

                    return true;
                }
            }


            /// <summary>
            /// Return true if this vertex is degree 2 and on the mesh boundary. 
            /// </summary>
            public bool IsCorner
            {
                get { return IsBoundary && IsDegree2; }
            }


            /// <summary>
            /// Sets the first halfedge of this vertex to the first boundary halfedge encountered during circulation.
            /// Returns true if a boundary halfedge was found.
            /// </summary>
            /// <returns></returns>
            internal bool SetFirstToBoundary()
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    if (he1.Face == null)
                    {
                        First = he1;
                        return true;
                    }

                    he1 = he1.NextAtStart;
                } while (he1 != he0);

                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Face : Node<F, E>
        {
            /// <summary>
            /// Circulates through all halfedges in this face.
            /// </summary>
            public IEnumerable<E> Halfedges
            {
                get { return First.Circulate; }
            }


            /// <summary>
            /// Circulates through all vertices in this face.
            /// </summary>
            public IEnumerable<V> Vertices
            {
                get
                {
                    var he0 = First;
                    var he1 = he0;

                    do
                    {
                        yield return he1.Start;
                        he1 = he1.Next;
                    } while (he1 != he0);
                }
            }


            /// <summary>
            /// Circulates through all faces adjacent to this face.
            /// Note that if multiple edges are shared with an adjacent face, then that face will be returned multiple times.
            /// </summary>
            public IEnumerable<F> AdjacentFaces
            {
                get
                {
                    var he0 = First;
                    var he1 = he0;

                    do
                    {
                        var f = he1.Twin.Face;
                        if (f != null) yield return f;
                        he1 = he1.Next;
                    } while (he1 != he0);
                }
            }


            /// <summary>
            /// Returns true if this face has 1 edge.
            /// </summary>
            internal bool IsDegree1
            {
                get { return First.IsInDegree1; }
            }


            /// <summary>
            /// Returns true if this face has 2 edges.
            /// </summary>
            internal bool IsDegree2
            {
                get { return First.IsInDegree2; }
            }


            /// <summary>
            /// Returns true if this face has 3 edges.
            /// </summary>
            public bool IsDegree3
            {
                get { return First.IsInDegree3; }
            }


            /// <summary>
            /// Returns true if the face has one or more boundary edges.
            /// </summary>
            public bool IsBoundary
            {
                get
                {
                    var he0 = First;
                    var he1 = he0;

                    do
                    {
                        if (he1.Twin.Face == null) return true;
                        he1 = he1.Next;
                    } while (he1 != he0);

                    return false;
                }
            }


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public int GetDegree()
            {
                return First.Count();
            }


            /// <summary>
            /// Returns true if the number of edges in this face it equal to the given value.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public bool IsDegree(int value)
            {
                return First.IsInDegree(value);
            }


            /// <summary>
            /// Sets the first halfedge in this face to the first boundary halfedge encountered during circulation.
            /// Returns true if a boundary halfedge was found.
            /// </summary>
            /// <returns></returns>
            public bool SetFirstToBoundary()
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    if (he1.Twin.Face == null)
                    {
                        First = he1;
                        return true;
                    }

                    he1 = he1.Next;
                } while (he1 != he0);

                return false;
            }


            /// <summary>
            /// Returns the first halfedge between this face and another or null if none exists.
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public E FindHalfedge(F other)
            {
                var he0 = First;
                var he1 = he0;

                do
                {
                    if (he1.Twin.Face == other) return he1;
                    he1 = he1.Next;
                } while (he1 != he0);

                return null;
            }


            /// <summary>
            /// Returns the number of boundary edges in this face.
            /// </summary>
            /// <returns></returns>
            public int CountBoundaryEdges()
            {
                var he0 = First;
                var he1 = he0;
                int count = 0;

                do
                {
                    if (he1.Twin.Face == null) count++;
                    he1 = he1.Next;
                } while (he1 != he0);

                return count;
            }


            /// <summary>
            /// Returns the number of boundary vertices in this face.
            /// </summary>
            /// <returns></returns>
            public int CountBoundaryVertices()
            {
                var he0 = First;
                var he1 = he0;
                int count = 0;

                do
                {
                    if (he1.Start.IsBoundary) count++;
                    he1 = he1.Next;
                } while (he1 != he0);

                return count;
            }
        }

        #endregion


        private NodeList<F> _faces;


        /// <summary>
        /// 
        /// </summary>
        public HeStructure()
            :base()
        {
            _faces = new NodeList<F, E>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeStructure(int vertexCapacity, int hedgeCapacity, int faceCapacity)
            :base(vertexCapacity, hedgeCapacity)
        {
            _faces = new NodeList<F, E>(faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList<F> Faces
        {
            get => _faces;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract F NewFace();


        /// <summary>
        /// Returns true if the given face belongs to this mesh.
        /// </summary>
        /// <param name="face"></param>
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
    /// <typeparam name="VG"></typeparam>
    /// <typeparam name="EG"></typeparam>
    /// <typeparam name="FG"></typeparam>
    [Serializable]
    public abstract class HeStructure<V, E, F, VG, EG, FG> : HeStructure<V, E, F>
        where V : HeStructure<V, E, F, VG, EG, FG>.Vertex
        where E : HeStructure<V, E, F, VG, EG, FG>.Halfedge
        where F : HeStructure<V, E, F, VG, EG, FG>.Face
        where VG : HeStructure<V, E, F, VG, EG, FG>.Cluster
        where EG : HeStructure<V, E, F, VG, EG, FG>.Bundle
        where FG : HeStructure<V, E, F, VG, EG, FG>.Cell
    {
        #region Nested types

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Halfedge : HeStructure<V, E, F>.Halfedge
        {
            private E _adjacent;
            private VG _bundle;


            /// <summary>
            /// 
            /// </summary>
            public E Adjacent
            {
                get => _adjacent;
                internal set => _adjacent = value;
            }


            /// <summary>
            /// 
            /// </summary>
            public VG Bundle
            {
                get => _bundle;
                internal set => _bundle = value;
            }


            /// <summary>
            /// 
            /// </summary>
            public E PreviousInBundle
            {
                get => Twin._adjacent;
            }


            /// <summary>
            /// 
            /// </summary>
            public E NextInBundle
            {
                get => _adjacent.Twin;
            }


            /// <summary>
            /// 
            /// </summary>
            public bool IsFirstInBundle
            {
                get => this == _bundle.First;
            }


            /// <summary>
            /// 
            /// </summary>
            public IEnumerable<E> CirculateBundle
            {
                get
                {
                    var he = Self;

                    do
                    {
                        yield return he;
                        he = he.NextInBundle;
                    } while (he != this);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Vertex : HeStructure<V, E, F>.Vertex
        {
            private VG _cluster;


            /// <summary>
            /// 
            /// </summary>
            public VG Cluster
            {
                get => _cluster;
                internal set => _cluster = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public new abstract class Face : HeStructure<V, E, F>.Face
        {
            private VG _cell;


            /// <summary>
            /// 
            /// </summary>
            public F Twin
            {
                get => First.Adjacent.Face;
            }


            /// <summary>
            /// 
            /// </summary>
            public VG Cell
            {
                get => _cell;
                internal set => _cell = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Cluster : Node<VG, E>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Bundle : Node<EG, E>
        {
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public abstract class Cell : Node<FG, E>
        {
        }

        #endregion


        private NodeList<VG> _clusters;
        private NodeList<EG> _bundles;
        private NodeList<FG> _cells;
        // private protected NodeList<FP> _pairs; // face pairs

        
        /// <summary>
        /// 
        /// </summary>
        public HeStructure()
            :base()
        {
            _clusters = new NodeList<VG, E>(); // assumes 4 verts per cluster
            _bundles = new NodeList<EG, E>(); // assumes 4 edges per bundle
            _cells = new NodeList<FG, E>(); // assumes 4 faces per cell
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <param name="clusterCapacity"></param>
        /// <param name="bundleCapacity"></param>
        /// <param name="cellCapacity"></param>
        public HeStructure(int vertexCapacity, int hedgeCapacity, int faceCapacity, int clusterCapacity, int bundleCapacity, int cellCapacity)
            : base(vertexCapacity, hedgeCapacity, faceCapacity)
        {
            // TODO
            // can consecutive pairs be maintained in the face list similar to the halfedge list?
            // assumes faces are added in pairs... would this work?

            _clusters = new NodeList<VG, E>(clusterCapacity); // assumes 4 verts per cluster
            _bundles = new NodeList<EG, E>(bundleCapacity); // assumes 4 edges per bundle
            _cells = new NodeList<FG, E>(cellCapacity); // assumes 4 faces per cell
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList<VG> Clusters
        {
            get => _clusters;
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList<EG> Bundles
        {
            get => _bundles;
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList<FG> Cells
        {
            get => _cells;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract VG NewCluster();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract EG NewBundle();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract FG NewCell();


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal VG AddCluster()
        {
            var g = NewCluster();
            _clusters.Add(g);
            return g;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal EG AddBundle()
        {
            var b = NewBundle();
            _bundles.Add(b);
            return b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal FG AddCell()
        {
            var c = NewCell();
            _cells.Add(c);
            return c;
        }
    }
}