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
    public partial class HalfedgeList : HeElementList<Halfedge>
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
        /// Creates a pair of halfedges between the given vertices and add them to the list.
        /// Returns the halfedge starting from v0.
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
            Add(he0);
            Add(he1);

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
            Halfedge he0 = halfedge; // to be removed
            Halfedge he1 = he0.Twin; // to be removed

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;

            if (he0.IsAtDegree1)
            {
                v0.MakeUnused(); // flag degree 1 vertex as unused
            }
            else
            {
                Halfedge.MakeConsecutive(he0.Previous, he1.Next); // update halfedge->halfedge refs
                if (he0 == v0.First) v0.First = he1.Next; // update vertex->halfedge ref if necessary
            }

            if (he1.IsAtDegree1)
            {
                v1.MakeUnused(); // flag degree 1 vertex as unused
            }
            else
            {
                Halfedge.MakeConsecutive(he1.Previous, he0.Next); // update halfedge->halfedge refs
                if (he1 == v1.First) v1.First = he0.Next; // update vertex->halfedge ref if necessary
            }

            // flag elements for removal
            he0.MakeUnused();
        }


        /// <summary>
        /// Splits the given edge at the specified paramter.
        /// Creates a new halfedge pair and a new vertex.
        /// Returns the new halfedge belonging to the same face as the given one.
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

            // update halfedge->vertex refs
            he0.Start = v1;

            // update halfedge->face refs
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
        /// Returns the new halfedge outgoing from the new vertex or null on failure.
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

            /*
            // avoids creation of degenerate faces
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;
   
            int allow = 0;
            if (he0.IsInTri()) allow++;
            if (he1.IsInTri()) allow++;
            if (Mesh.Vertices.CountCommonNeighbours(he0.Start, he1.Start) > allow)
                return false;
            */

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
            Halfedge he0 = halfedge; // to be removed
            Halfedge he1 = he0.Twin; // to be removed
            Halfedge he2 = he0.Next;
            Halfedge he3 = he1.Next;

            HeVertex v0 = he0.Start; // to be removed
            HeVertex v1 = he1.Start;

            HeFace f0 = he0.Face;
            HeFace f1 = he1.Face;

            // update vertex refs of all halfedges starting at v0
            foreach (Halfedge he in he0.CirculateStart.Skip(1))
                he.Start = v1;

            // update halfedge ref of v1 if necessary
            if (v1.First == he1)
                v1.First = he3; // maintains vertex boundary condition

            // update halfedge-halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he2);
            Halfedge.MakeConsecutive(he1.Previous, he3);

            // handle potential degenerate faces or update face->halfedge refs
            if(f0 == null)
            {
                if (he2.IsInDegenerate) 
                    CleanupDegenerateHole(he2);
            }
            else
            {
                if (he2.IsInDegenerate) 
                    CleanupDegenerateFace(he2);
                else if (f0.First == he0) 
                    f0.First = he2;
            }

            if(f1 == null)
            {
                if (he3.IsInDegenerate)
                    CleanupDegenerateHole(he3);
            }
            else
            {
                if (he3.IsInDegenerate)
                    CleanupDegenerateFace(he3);
                else if (f1.First == he1)
                    f1.First = he3;
            }

            // flag elements for removal
            v0.MakeUnused();
            he0.MakeUnused();

            // update position of remaining vertex
            v1.Position = Vec3d.Lerp(v0.Position, v1.Position, t);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        internal void CleanupDegenerateFace(Halfedge halfedge)
        {
            Halfedge he0 = halfedge; // to be removed
            Halfedge he1 = he0.Twin; // to be removed
            Halfedge he2 = he0.Next;
            Halfedge he3 = he1.Next;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;

            HeFace f0 = he0.Face; // to be removed
            HeFace f1 = he1.Face;

            // update face->halfedge refs
            if (f1 != null && f1.First == he1) f1.First = he2;

            // update vertex->halfedge refs
            if (v1.First == he1) v1.First = he2;
            if (v0.First == he0) v0.First = he3;

            // update halfedge->face refs
            he2.Face = f1;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he1.Previous, he2);
            Halfedge.MakeConsecutive(he2, he3);

            // handle potential invalid edge
            if (!he2.IsValid)
                RemovePair(he2); 
            
            // flag for removal
            f0.MakeUnused();
            he0.MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        internal void CleanupDegenerateHole(Halfedge halfedge)
        {
            Halfedge he0 = halfedge; // to be removed
            Halfedge he1 = he0.Twin; // to be removed
            Halfedge he2 = he0.Next;
            Halfedge he3 = he1.Next;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;

            HeFace f1 = he1.Face;

            // update face->halfedge refs
            if (f1.First == he1) f1.First = he2;

            // update vertex->halfedge refs
            // must look for another boundary halfedge to maintain boundary invariant for v0 and v1
            if (v0.First == he0)
            {
                Halfedge he = he0.NextBoundaryAtStart(); // search for another boundary halfedge from v0
                v0.First = (he == null) ? he3 : he;
            }

            if (v1.First == he2)
            {
                Halfedge he = he2.NextBoundaryAtStart();
                v1.First = (he == null) ? he2 : he;
            }

            // update halfedge->face refs
            he2.Face = f1;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he1.Previous, he2);
            Halfedge.MakeConsecutive(he2, he3);

            // flag elements for removal
            he0.MakeUnused();
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
            if (halfedge.IsAtDegree2 || halfedge.Twin.IsAtDegree2)
                return false;

            SpinEdgeImpl(halfedge);
            return true;
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
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


        /// <summary>
        /// Returns the new boundary halfedge created at the detach interface.
        /// </summary>
        /// <param name="halfedge"></param>
        public Halfedge DetachEdge(Halfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            // halfedge must be adjacent to 2 faces
            if (halfedge.IsBoundary)
                return null;

            return DetachEdgeImpl(halfedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal Halfedge DetachEdgeImpl(Halfedge halfedge)
        {
            var he0 = halfedge;
            var he1 = he0.Twin;

            int mask = 0;
            if (he0.Start.IsBoundary) mask |= 1;
            if (he1.Start.IsBoundary) mask |= 2;

            switch (mask)
            {
                case 0:
                    return DetachEdgeInterior(he0);
                case 1:
                    return DetachEdgeMixed(he1);
                case 2:
                    return DetachEdgeMixed(he0);
                case 3:
                    return DetachEdgeBoundary(he0);
            }

            return null;
        }


        /// <summary>
        /// Neither vertex is on the mesh boundary
        /// </summary>
        internal Halfedge DetachEdgeInterior(Halfedge halfedge)
        {
            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var he2 = AddPair(v1, v0);
            var he3 = he2.Twin;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            Halfedge.MakeConsecutive(he2, he1.Next);
            Halfedge.MakeConsecutive(he1, he3);

            // update halfedge-halfedge refs @ v1
            Halfedge.MakeConsecutive(he1.Previous, he2);
            Halfedge.MakeConsecutive(he3, he1);

            // update vertex-halfedge refs
            v0.First = he3;
            v1.First = he1;

            return he3;
        }


        /// <summary>
        /// Both vertices are on the mesh boundary
        /// </summary>
        internal Halfedge DetachEdgeBoundary(Halfedge halfedge)
        {
            var verts = Mesh.Vertices;

            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = verts.Add(v0.Position);
            var v3 = verts.Add(v1.Position);

            var he2 = AddPair(v3, v2);
            var he3 = he2.Twin;
            var he4 = v0.First;
            var he5 = v1.First;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            Halfedge.MakeConsecutive(he2, he1.Next);
            Halfedge.MakeConsecutive(he4.Previous, he3);
            Halfedge.MakeConsecutive(he1, he4);

            // update halfedge-halfedge refs @ v1
            Halfedge.MakeConsecutive(he1.Previous, he2);
            Halfedge.MakeConsecutive(he5.Previous, he1);
            Halfedge.MakeConsecutive(he3, he5);

            // update vertex-halfedge refs
            v1.First = he1;
            v2.First = he3;
            v3.First = he5;

            //update halfedge-vertex refs around each new vert
            var he = he2;
            do
            {
                he.Start = v3;
                he = he.Twin.Next;
            } while (he != he2);

            he = he3;
            do
            {
                he.Start = v2;
                he = he.Twin.Next;
            } while (he != he3);

            return he3;
        }


        /// <summary>
        /// Vertex at the end of the given halfedge is on the boundary.
        /// </summary>
        internal Halfedge DetachEdgeMixed(Halfedge halfedge)
        {
            var verts = Mesh.Vertices;

            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = verts.Add(v1.Position);

            var he2 = AddPair(v2, v0);
            var he3 = he2.Twin;
            var he4 = v1.First;

            var f1 = he1.Face;

            // update halfedge-face refs
            he2.Face = f1;
            he3.Face = null;
            he1.Face = null;

            //update face-halfedge ref if necessary
            if (f1.First == he1)
                f1.First = he2;

            // update halfedge-halfedge refs @ v0
            Halfedge.MakeConsecutive(he2, he1.Next);
            Halfedge.MakeConsecutive(he1.Previous, he2);

            // update halfedge-halfedge refs @ v1
            Halfedge.MakeConsecutive(he4.Previous, he1);
            Halfedge.MakeConsecutive(he1, he3);
            Halfedge.MakeConsecutive(he3, he4);

            // update vertex-halfedge refs
            v0.First = he3;
            v1.First = he1;
            v2.First = he4;

            //update halfedge-vertex refs around each new vert
            var he = he2;
            do
            {
                he.Start = v2;
                he = he.Twin.Next;
            } while (he != he2);

            return he3;
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

        /*
      /// <summary>
      /// TODO replace with merge vertex
      /// </summary>
      /// <param name="he0"></param>
      /// <param name="he1"></param>
      /// <returns></returns>
      public bool MergeEdges(Halfedge he0, Halfedge he1)
      {
          he0.UsedCheck();
          he1.UsedCheck();
          OwnsCheck(he0);
          OwnsCheck(he1);

          // both halfedges must be on boundary
          if (he0.Face != null || he1.Face != null)
              return false;

          // can't merge edges which belong to the same face
          if (he0.Twin.Face == he1.Twin.Face)
              return false;

          // TODO doesn't consider edges 
          Halfedge he2 = he0.Next;
          Halfedge he3 = he1.Next;

          if (he2 == he1)
              ZipEdgeImpl(he0);
          else if (he1.Next == he0)
              ZipEdgeImpl(he1);
          else
              MergeEdgesImpl(he0, he1);

          return true;
      }


      
      /// <summary>
      /// 
      /// </summary>
      internal void MergeEdgesImpl(Halfedge he0, Halfedge he1)
      {
          Halfedge he2 = he0.Twin;
          Halfedge he3 = he1.Twin;
            
          HeVertex v0 = he0.Start;
          HeVertex v1 = he1.Start;
          HeVertex v2 = he2.Start;
          HeVertex v3 = he3.Start;

          HeFace f3 = he3.Face;

          // update vertex refs for all halfedges around v1
          foreach (Halfedge he in he1.CirculateStart.Skip(1))
              he.Start = v2;

          // update vertex refs for all halfedges around v3
          foreach (Halfedge he in he3.CirculateStart.Skip(1))
              he.Start = v0;

          // update vertex->halfedge refs
          v0.First = he1.Next;
          v1.First = he0.Next;

          // update halfedge->face refs
          if (f3.First == he3) f3.First = he0;
          he0.Face = f3;
   
          // update halfedge->halfedge refs
          Halfedge.MakeConsecutive(he1.Previous, he0.Next);
          Halfedge.MakeConsecutive(he0.Previous, he1.Next);
          Halfedge.MakeConsecutive(he0, he3.Next);
          Halfedge.MakeConsecutive(he3.Previous, he0);

          // flag elements for removal
          he1.MakeUnused();
          he3.MakeUnused();
          v1.MakeUnused();
          v3.MakeUnused();
      }
  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        public bool ZipEdge(Halfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            // halfedge must be on boundary
            if (halfedge.Face != null)
                return false;

            // can't zip from valence 2 vertex
            if (halfedge.Next.IsFromDegree2)
                return false;

            ZipEdgeImpl(halfedge);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        internal void ZipEdgeImpl(Halfedge halfedge)
        {
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Next;
            Halfedge he2 = he1.Twin;

            HeVertex v0 = he0.Start;
            HeVertex v1 = he1.Start;
            HeVertex v2 = he2.Start;

            HeFace f2 = he2.Face;

            // update vertex refs for all halfedges around v2
            foreach (Halfedge he in he2.CirculateStart.Skip(1))
                he.Start = v0;

            // update vertex->halfedge refs
            v0.First = he1.Next;
            Halfedge he3 = he2.Next.FindBoundaryAtStart(); // check for another boundary edge at v1
            v1.First = (he3 == he1) ? he0.Twin : he3;

            // update halfedge->face refs
            if (f2.First == he2) f2.First = he0;
            he0.Face = f2;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he1.Next);
            Halfedge.MakeConsecutive(he2.Previous, he0);
            Halfedge.MakeConsecutive(he0, he2.Next);

            // flag elements as unused
            he1.MakeUnused();
            he2.MakeUnused();
            v2.MakeUnused();
        }
        */

        #endregion

    }
}
