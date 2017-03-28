using System;
using System.Collections.Generic;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="EE"></typeparam>
    /// <typeparam name="VV"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public abstract class HeStructure<S, EE, VV, E, V>
        where S : HeStructure<S, EE, VV, E, V>
        where EE : HalfedgeList<S, EE, VV, E, V>
        where VV : HeVertexList<S, EE, VV, E, V>
        where E : Halfedge<E, V>
        where V : HeVertex<E, V>
    {
        private EE _halfedges;
        private VV _vertices;


        /// <summary>
        /// 
        /// </summary>
        public EE Halfedges
        {
            get { return _halfedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public VV Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract S CreateInstance();


        /// <summary>
        /// Call from concrete constructor to assign private fields.
        /// </summary>
        /// <param name="halfedges"></param>
        /// <param name="vertices"></param>
        protected void Initialize(EE halfedges, VV vertices)
        {
            _halfedges = halfedges;
            _vertices = vertices;
        }


        /// <summary>
        /// Removes all unused elements in the mesh.
        /// </summary>
        public virtual void Compact()
        {
            _halfedges.Compact();
            _vertices.Compact();
        }


        /// <summary>
        /// Returns true if the given vertex belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Owns(V vertex)
        {
            return Vertices.Owns(vertex);
        }


        /// <summary>
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public bool Owns(E halfedge)
        {
            return Halfedges.Owns(halfedge);
        }


        /// <summary>
        /// Returns the first halfedge from each connected component in the structure.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<E> GetConnectedComponents()
        {
            var hedges = Halfedges;

            var stack = new Stack<E>();
            int currTag = Halfedges.NextTag;

            // dfs
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var he = hedges[i];
                if (he.IsUnused || he.Tag == currTag) continue;

                // flag all connected halfedges as visited
                he.Tag = currTag;
                stack.Push(he);

                while (stack.Count > 0)
                {
                    var he0 = stack.Pop();

                    // add unvisited neighbours to the stack
                    foreach (var he1 in he0.ConnectedPairs)
                    {
                        if (he1.Tag == currTag) continue;
                        he1.Tag = he1.Twin.Tag = currTag; // tag halfedge pair as visited
                        stack.Push(he1);
                    }
                }

                yield return he;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public virtual void Append(S other)
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("Cannot append an HeStructure to itself.");

            var hedges = Halfedges;
            var verts = Vertices;

            int nhe = Halfedges.Count;
            int nv = Vertices.Count;

            var otherHedges = other.Halfedges;
            var otherVerts = other.Vertices;

            // append new elements
            for (int i = 0; i < otherVerts.Count; i++)
                verts.Add();

            for (int i = 0; i < otherHedges.Count; i += 2)
                hedges.AddPair();

            // link new vertices to new halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var v0 = otherVerts[i];
                if (v0.IsUnused) continue;

                var v1 = verts[i + nv];
                v1.First = hedges[v0.First.Index + nhe];
            }

            // link new halfedges to new vertices and other new halfedges
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var he0 = otherHedges[i];
                if (he0.IsUnused) continue;

                var he1 = hedges[i + nhe];
                he1.Previous = hedges[he0.Previous.Index + nhe];
                he1.Next = hedges[he0.Next.Index + nhe];
                he1.Start = verts[he0.Start.Index + nv];
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public virtual void Append(S other)
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("Cannot append an HeStructure to itself.");

            var hedges = Halfedges;
            var verts = Vertices;
            int nhe = hedges.Count;

            var otherHedges = other.Halfedges;
            var otherVerts = other.Vertices;

            // add new halfedge pairs
            for (int i = 0; i < otherHedges.Count; i += 2)
                hedges.AddPair();

            // link new halfedges to eachother
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var heB = otherHedges[i];
                var heA = hedges[i + nhe];
                if (!heB.IsUnused) Halfedge<E, V>.MakeConsecutive(heA, hedges[heB.Next.Index + nhe]);
            }

            // add new vertices and link to new halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var vB = otherVerts[i];
                var vA = verts.Add();
                if (vB.IsUnused) continue;

                var heA = hedges[vB.First.Index + nhe];
                vA.First = heA;

                foreach (var he in heA.CirculateStart) he.Start = vA;
            }
        }
        */


        /// <summary>
        /// Returns an array of connected components.
        /// </summary>
        /// <returns></returns>
        public S[] SplitDisjoint()
        {
            int[] componentMap, halfedgeMap;
            return SplitDisjoint(out componentMap, out halfedgeMap);
        }


        /// <summary>
        /// Returns an array of connected components.
        /// For each edge in the mesh, returns the index of the component to which it belongs.
        /// For each halfedge in the mesh, returns the index of the corresponding component halfedge.
        /// </summary>
        /// <param name="componentMap"></param>
        /// <param name="halfedgeMap"></param>
        /// <returns></returns>
        public virtual S[] SplitDisjoint(out int[] componentMap, out int[] halfedgeMap)
        {
            var hedges = Halfedges;

            componentMap = new int[hedges.Count >> 1];
            halfedgeMap = new int[hedges.Count];

            int ncomp = hedges.GetEdgeComponentMap(componentMap);
            var components = new S[ncomp];

            // initialize components
            for (int i = 0; i < components.Length; i++)
                components[i] = CreateInstance();

            // create component halfedges
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var heA = hedges[i];
                if (heA.IsUnused) continue;

                var compHedges = components[componentMap[i >> 1]].Halfedges;
                int nhe = compHedges.Count;

                compHedges.AddPair();
                halfedgeMap[i] = nhe;
                halfedgeMap[i + 1] = nhe + 1;
            }

            // set component halfedge->halfedge refs
            for (int i = 0; i < hedges.Count; i++)
            {
                var heA = hedges[i];
                if (heA.IsUnused) continue;

                var compHedges = components[componentMap[i >> 1]].Halfedges;
                var heB = compHedges[halfedgeMap[i]];

                Halfedge<E, V>.MakeConsecutive(heB, compHedges[halfedgeMap[heA.Next.Index]]);
            }

            // create component vertices
            foreach (var vA in Vertices)
            {
                if (vA.IsUnused) continue;
                var heA = vA.First;

                // add new vertex 
                var comp = components[componentMap[heA.Index >> 1]];
                var vB = comp.Vertices.Add();

                // set vertex->halfedge ref
                var heB = comp.Halfedges[halfedgeMap[heA.Index]];
                vB.First = heB;

                // set halfedge->vertex refs
                foreach (var he in heB.CirculateStart) he.Start = vB;
            }

            return components;
        }


        /// <summary>
        /// Returns an array of connected components.
        /// For each edge in the mesh, returns the index of the component to which it belongs.
        /// For each halfedge in the mesh, returns the index of the corresponding component halfedge.
        /// For each halfedge in each component, returns the index of the corresponding halfedge in the source mesh.
        /// </summary>
        /// <param name="componentMap"></param>
        /// <param name="halfedgeMap"></param>
        /// <param name="reverseHalfedgeMaps"></param>
        /// <returns></returns>
        public S[] SplitDisjoint(out int[] componentMap, out int[] halfedgeMap, out int[][] reverseHalfedgeMaps)
        {
            var components = SplitDisjoint(out componentMap, out halfedgeMap);

            var hedges = Halfedges;
            reverseHalfedgeMaps = new int[components.Length][];

            // initialize reverse halfedge maps (1 per component)
            for (int i = 0; i < components.Length; i++)
            {
                var comp = components[i];
                reverseHalfedgeMaps[i] = new int[comp.Halfedges.Count];
            }

            // populate reverse halfedge maps
            for (int i = 0; i < hedges.Count; i += 2)
            {
                var heA = hedges[i];
                if (heA.IsUnused) continue;

                var compHeMap = reverseHalfedgeMaps[componentMap[i >> 1]];
                var heBi = halfedgeMap[i];

                compHeMap[heBi] = i;
                compHeMap[heBi + 1] = i + 1;
            }

            return components;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="EE"></typeparam>
    /// <typeparam name="VV"></typeparam>
    /// <typeparam name="FF"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeStructure<S, EE, VV, FF, E, V, F> : HeStructure<S, EE, VV, E, V>
        where S : HeStructure<S, EE, VV, FF, E, V, F>
        where EE : HalfedgeList<S, EE, VV, FF, E, V, F>
        where VV : HeVertexList<S, EE, VV, FF, E, V, F>
        where FF : HeFaceList<S, EE, VV, FF, E, V, F>
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        private FF _faces;


        /// <summary>
        /// 
        /// </summary>
        public FF Faces
        {
            get { return _faces; }
        }


        /// <summary>
        /// Call from concrete constructor to assign private fields.
        /// </summary>
        /// <param name="halfedges"></param>
        /// <param name="vertices"></param>
        /// <param name="faces"></param>
        protected void Initialize(EE halfedges, VV vertices, FF faces)
        {
            Initialize(halfedges, vertices);
            _faces = faces;
        }


        /// <summary>
        /// Returns false if there are any boundary halfedges in the mesh.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                var hedges = Halfedges;

                for (int i = 0; i < hedges.Count; i += 2)
                {
                    var he = hedges[i];
                    if (!he.IsUnused && he.IsBoundary) return false;
                }

                return true;
            }
        }


        /// <summary>
        /// Removes all unused elements in the mesh.
        /// </summary>
        public override void Compact()
        {
            base.Compact();
            _faces.Compact();
        }


        /// <summary>
        /// Returns true if the given face belongs to this mesh.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public bool Owns(F face)
        {
            return Faces.Owns(face);
        }


        /// <summary>
        /// Returns the first halfedge from each hole in the mesh.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<E> GetHoles()
        {
            int currTag = Halfedges.NextTag;

            for (int i = 0; i < Halfedges.Count; i++)
            {
                var he = Halfedges[i];
                if (he.IsUnused || he.Face != null || he.Tag == currTag) continue;

                do
                {
                    he.Tag = currTag;
                    he = he.Next;
                } while (he.Tag != currTag);

                yield return he;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void Append(S other)
        {
            var hedges = Halfedges;
            var faces = Faces;
            int nhe = hedges.Count;
            int nf = faces.Count;

            var otherFaces = other.Faces;
            var otherHedges = other.Halfedges;

            base.Append(other);

            // append new faces
            for (int i = 0; i < otherFaces.Count; i++)
                faces.Add();

            // link new faces to new halfedges
            for (int i = 0; i < otherFaces.Count; i++)
            {
                var f0 = otherFaces[i];
                if (f0.IsUnused) continue;

                var f1 = faces[i + nf];
                f1.First = hedges[f0.First.Index + nhe];
            }

            // link new halfedges to new faces
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var he0 = otherHedges[i];

                if (he0.Face != null)
                    hedges[i + nhe].Face = faces[he0.Face.Index + nf];
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public override void Append(S other)
        {
            var hedges = Halfedges;
            int nhe = hedges.Count;

            // append other's halfedges and vertices
            base.Append(other);

            var faces = Faces;
            var otherFaces = other.Faces;

            // append other's faces and link to appended halfedges
            for (int i = 0; i < otherFaces.Count; i++)
            {
                var fB = otherFaces[i];
                var fA = faces.Add();
                if (fB.IsUnused) continue;

                var heA = hedges[fB.First.Index + nhe];
                fA.First = heA;

                foreach (var he in heA.CirculateFace) he.Face = fA;
            }
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentMap"></param>
        /// <param name="halfedgeMap"></param>
        /// <returns></returns>
        public override S[] SplitDisjoint(out int[] componentMap, out int[] halfedgeMap)
        {
            var hedges = Halfedges;

            // create components, component halfedges, and component vertices
            var components = base.SplitDisjoint(out componentMap, out halfedgeMap);

            // create component faces
            foreach (var fA in Faces)
            {
                if (fA.IsUnused) continue;
                var heA = fA.First;

                // add new face
                var comp = components[componentMap[heA.Index >> 1]];
                var fB = comp.Faces.Add();

                // set face->halfedge ref
                var heB = comp.Halfedges[halfedgeMap[heA.Index]];
                fB.First = heB;

                // set halfedge->face refs
                foreach (var he in heB.CirculateFace) he.Face = fB;
            }

            return components;
        }


        /// <summary>
        /// Reverses the winding direction of all faces in the mesh
        /// </summary>
        public void Flip()
        {
            foreach(var he in Halfedges)
            {
                var prev = he.Previous;
                he.Previous = he.Next;
                he.Next = prev;
            }
        }
    }
}

