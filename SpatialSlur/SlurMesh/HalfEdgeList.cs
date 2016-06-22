using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HalfedgeList : HeElementList<Halfedge>
    {
        /// <summary>
        /// 
        /// </summary>
        internal HalfedgeList(HeMesh mesh, int capacity = 2)
            : base(mesh, capacity)
        {
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        internal void HalfSizeCheck<U>(IList<U> attributes)
        {
            if (attributes.Count != Count >> 1)
                throw new ArgumentException("The number of attributes provided does not match the number of edges in the mesh.");
        }


        /// <summary>
        /// Adds an edge and its twin to the list.
        /// </summary>
        /// <param name="halfedge"></param>
        internal void AddPair(Halfedge halfedge)
        {
            Add(halfedge);
            Add(halfedge.Twin);
        }


        /// <summary>
        /// Creates a pair of halfedges between the given vertices and add them to the list.
        /// Note that the face, previous, and next references of the new halfedges are left unassigned.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        internal Halfedge AddPair(HeVertex v0, HeVertex v1)
        {
            Halfedge he0 = new Halfedge();
            Halfedge he1 = new Halfedge();

            he0.Start = v0;
            he1.Start = v1;
            Halfedge.MakeTwins(he0, he1);

            AddPair(he0);
            return he0;
        }

        #region Euler Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        public void Remove(Halfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            Mesh.Faces.MergeFacesImpl(halfedge);
        }


        /// <summary>
        /// Removes a pair of halfedges from the mesh.
        /// Note that this method does not update face->halfedge references.
        /// </summary>
        /// <param name="halfedge"></param>
        internal void RemovePair(Halfedge halfedge)
        {
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;

            if (he0.IsFromDegree1)
            {
                v0.MakeUnused(); // flag degree 1 vertex as unused
            }
            else
            {
                Halfedge.MakeConsecutive(he0.Previous, he1.Next); // update halfedge-halfedge refs
                if (he0 == v0.First) v0.First = he1.Next; // update vertex-halfedge ref if necessary
            }

            if (he1.IsFromDegree1)
            {
                v1.MakeUnused(); // flag degree 1 vertex as unused
            }
            else
            {
                Halfedge.MakeConsecutive(he1.Previous, he0.Next); // update halfedge-halfedge refs
                if (he1 == v1.First) v1.First = he0.Next; // update vertex-halfedge ref if necessary
            }

            // flag elements for removal
            he0.MakeUnused();
            he1.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge at the specified paramter.
        /// Creates a new halfedge pair and a new vertex.
        /// Returns the new halfedge that starts from the new vertex.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Halfedge SplitEdge(Halfedge halfedge, double t = 0.5)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            return SplitEdgeImpl(halfedge, t);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal Halfedge SplitEdgeImpl(Halfedge halfedge, double t = 0.5)
        {
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = Mesh.Vertices.Add(he0.PointAt(t));

            Halfedge he2 = AddPair(v0, v1);
            Halfedge he3 = he2.Twin;

            // update halfedge->vertex references
            he0.Start = v1;

            // update halfedge->face references
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update vertex->halfedge refs if necessary
            if (v0.First == he0)
            {
                v0.First = he2;
                v1.First = he0;
            }
            else
            {
                v1.First = he3;
            }

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he2);
            Halfedge.MakeConsecutive(he2, he0);
            Halfedge.MakeConsecutive(he3, he1.Next);
            Halfedge.MakeConsecutive(he1, he3);

            return he2;
        }


        /*
        /// <summary>
        /// Old implementation
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfEdge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal HalfEdge SplitEdgeImpl(HalfEdge halfEdge, double t = 0.5)
        {
            HalfEdge he0 = halfEdge;
            HalfEdge he1 = he0.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;
            //HeVertex v2 = Mesh.Vertices.Add(v0.Position);
            HeVertex v2 = Mesh.Vertices.Add(he0.PointAt(t));

            HalfEdge he2 = AddPair(v2, v1);
            HalfEdge he3 = he2.Twin;

            // update edge-vertex references
            he1.Start = v2;

            // update edge-face references
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update vertex-edge references if necessary
            if (v1.First == he1)
            {
                v1.First = he3;
                v2.First = he1;
            }
            else
            {
                v2.First = he2;
            }

            // update edge-edge references
            HalfEdge.MakeConsecutive(he2, he0.Next);
            HalfEdge.MakeConsecutive(he1.Previous, he3);
            HalfEdge.MakeConsecutive(he0, he2);
            HalfEdge.MakeConsecutive(he3, he1);

            return he2;
        }
        */


        /// <summary>
        /// Splits an edge by adding a new vertex in the middle. 
        /// Faces adjacent to the given edge are also split at the new vertex.
        /// Returns the new edge outgoing from the new vertex or null on failure.
        /// Assumes triangle mesh.
        /// </summary>
        public Halfedge SplitEdgeFace(Halfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            return SplitEdgeFaceImpl(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal Halfedge SplitEdgeFaceImpl(Halfedge halfedge)
        {
            var faces = Mesh.Faces;

            // split edge
            SplitEdgeImpl(halfedge, 0.5);
           
            // split left face if it exists
            if (halfedge.Face != null)
                faces.SplitFaceImpl(halfedge, halfedge.Next.Next);

            halfedge = halfedge.Twin.Next;
     
            // split right face if it exists
            if (halfedge.Face != null)
                faces.SplitFaceImpl(halfedge, halfedge.Next.Next);

            return halfedge;
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// The start vertex of the given halfedge is removed.
        /// The end vertex is moved to the specified parameter along the edge.
        /// Return true on success.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CollapseEdge(Halfedge halfedge, double t = 0.5)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);
        
            return CollapseEdgeImpl(halfedge, t);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool CollapseEdgeImpl(Halfedge halfedge, double t = 0.5)
        {
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;

            HeVertex v0 = he0.Start; // to be removed
            HeVertex v1 = he1.Start;

            HeFace f0 = he0.Face;
            HeFace f1 = he1.Face;

            // avoids creation of non-manifold vertices
            // TODO move all checks outside of Impl method?
            if (!he0.IsBoundary && v0.IsBoundary && v1.IsBoundary)
                return false;

            /*
            // avoids creation of non-manifold edges
            int allow = 0; // the number of common neighbours allowed between v0 and v1
            if (f0 != null && f0.IsTri) allow++;
            if (f1 != null && f1.IsTri) allow++;
            if (Mesh.Vertices.CountCommonNeighbours(v0, v1) > allow)
                return false;
            */

            // update vertex refs of all edges emanating from v0
            foreach (Halfedge he in v0.OutgoingHalfedges) 
                he.Start = v1;

            // update halfedge ref of v1 if necessary
            if (v1.First == he1) 
                v1.First = he1.Next; // maintains vertex boundary condition

            // update halfedge-halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he0.Next);
            Halfedge.MakeConsecutive(he1.Previous, he1.Next);

            // update halfedge refs of faces if necessary and deal with potential collapse by merging
            if (f0 != null)
            {
                if (f0.First == he0) f0.First = he0.Next;
                if (!f0.IsValid) Mesh.Faces.MergeInvalidFace(he0.Next);
            }

            if (f1 != null)
            {
                if (f1.First == he1) f1.First = he1.Next;
                if (!f1.IsValid) Mesh.Faces.MergeInvalidFace(he1.Next);
            }

            // TODO cleanup potential degree 1 verts?

            // flag elements for removal
            he0.MakeUnused();
            he1.MakeUnused();
            v0.MakeUnused();

            // update position of remaining vertex
            v1.Position = Vec3d.Lerp(v0.Position, v1.Position, t);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public bool SpinEdge(Halfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            // halfedge must be adjacent to 2 faces
            if (halfedge.IsBoundary) 
                return false;

            // don't allow for the creation of valence 1 vertices
            if (halfedge.IsFromDegree2 || halfedge.Twin.IsFromDegree2) 
                return false;

            SpinEdgeImpl(halfedge);
            return true;
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal void SpinEdgeImpl(Halfedge halfedge)
        {
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;

            Halfedge he2 = he0.Next;
            Halfedge he3 = he1.Next;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;

            // update vertex->halfedge refs if necessary
            if (he0 == v0.First) v0.First = he3;
            if (he1 == v1.First) v1.First = he2;

            // update halfedge->vertex refs
            he0.Start = he3.End;
            he1.Start = he2.End;

            HeFace f0 = he0.Face;
            HeFace f1 = he1.Face;

            // update face->halfedge refs if necessary
            if (he2 == f0.First) f0.First = he2.Next;
            if (he3 == f1.First) f1.First = he3.Next;

            // update halfedge->face refs
            he2.Face = f1;
            he3.Face = f0;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0, he2.Next);
            Halfedge.MakeConsecutive(he1, he3.Next);
            Halfedge.MakeConsecutive(he1.Previous, he2);
            Halfedge.MakeConsecutive(he0.Previous, he3);
            Halfedge.MakeConsecutive(he2, he1);
            Halfedge.MakeConsecutive(he3, he0);
        }


        /*
        /// <summary>
        /// OBSOLETE degree 1 verts are now taken care of in HeEdgeList.RemovePair
        /// removes an open chain of degree 2 vertices
        /// note that this method does not update face-edge references
        /// </summary>
        /// <param name="edge"></param>
        internal HeEdge Prune(HeEdge edge)
        {
            HeEdge e0 = edge;
            HeEdge e1 = e0.Twin;

            // advance until edge pair are no longer twins
            do
            {
                // stop if found an isolated segment
                if (e0.Next == e1)
                {
                    e0.Start.MakeUnused();
                    e1.Start.MakeUnused();
                    e0.MakeUnused();
                    e1.MakeUnused();
                    return null;
                }

                // flag elements for removal
                e0.Start.MakeUnused();
                e0.MakeUnused();
                e1.MakeUnused();

                // advance to next edge pair
                e0 = e0.Next;
                e1 = e1.Previous;
            } while (e0.Twin == e1);

            // update vertex-edge refs if necessary
            HeVertex v0 = e0.Start;
            if (v0.Outgoing.IsUnused) v0.Outgoing = e0;
    
            //update edge-edge refs
            HeEdge.MakeConsecutive(e1, e0);
            return e0;
        }
        */

        #endregion

    }
}
