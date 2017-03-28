using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
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
    public sealed class HeMeshHalfedgeList : HalfedgeList<HeMesh, HeMeshHalfedgeList, HeMeshVertexList, HeMeshFaceList, HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        public HeMeshHalfedgeList(HeMesh owner, int capacity)
        {
            Initialize(owner, capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeMeshHalfedge CreateElement()
        {
            return new HeMeshHalfedge();
        }


        #region Topology Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        public void Remove(HeMeshHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);
            Owner.Faces.MergeFacesImpl(halfedge);
        }


        /// <summary>
        /// Splits the given edge at the specified paramter creating a new vertex and halfedge pair.
        /// Returns the new halfedge which starts at the new vertex.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public HeMeshHalfedge SplitEdge(HeMeshHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            return SplitEdgeImpl(halfedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal HeMeshHalfedge SplitEdgeImpl(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = Owner.Vertices.Add();
            var v1 = he1.Start;

            var he2 = AddPair(v0, v1);
            var he3 = he2.Twin;

            // update halfedge->vertex refs
            he1.Start = v0;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update vertex->halfedge refs if necessary
            if (v1.First == he1)
            {
                v1.First = he3;
                v0.First = he1;
            }
            else
            {
                v0.First = he2;
            }

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he2, he0.Next);
            HeMeshHalfedge.MakeConsecutive(he0, he2);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he3);
            HeMeshHalfedge.MakeConsecutive(he3, he1);

            return he2;
        }


        /*
        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal HeMeshHalfedge SplitEdgeImpl(HeMeshHalfedge halfedge, double t)
        {
            HeMeshHalfedge he0 = halfedge;
            HeMeshHalfedge he1 = he0.Twin;

            HeMeshVertex v0 = he0.Start;
            HeMeshVertex v1 = Parent.Vertices.Add(he0.PointAt(t));

            HeMeshHalfedge he2 = AddPair(v0, v1);
            HeMeshHalfedge he3 = he2.Twin;

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
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he2, he0);
            HeMeshHalfedge.MakeConsecutive(he3, he1.Next);
            HeMeshHalfedge.MakeConsecutive(he1, he3);

            return he2;
        }
        */


        /// <summary>
        /// Splits an edge by adding a new vertex in the middle. 
        /// Faces adjacent to the given edge are also split at the new vertex.
        /// Returns the new halfedge outgoing from the new vertex or null on failure.
        /// Assumes triangle mesh.
        /// </summary>
        public HeMeshHalfedge SplitEdgeFace(HeMeshHalfedge halfedge)
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
        internal HeMeshHalfedge SplitEdgeFaceImpl(HeMeshHalfedge halfedge)
        {
            var faces = Owner.Faces;

            // split edge
            var he0 = SplitEdgeImpl(halfedge);
            var he1 = he0.Twin;

            // split left face if it exists
            if (he0.Face != null)
                faces.SplitFaceImpl(he0, he0.Next.Next);

            // split right face if it exists
            if (he1.Face != null)
                faces.SplitFaceImpl(he1, he1.Next.Next);

            return he0;
        }


        /// <summary>
        /// Collapses the given halfedge by merging the vertices at either end.
        /// The start vertex of the given halfedge is removed.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public void CollapseEdge(HeMeshHalfedge halfedge)
        {
            halfedge.UsedCheck();
            OwnsCheck(halfedge);

            /*
            // avoids creation of degenerate faces
            // OBSOLETE degenerate faces are now handled in implementation
            Halfedge he0 = halfedge;
            Halfedge he1 = he0.Twin;
   
            int allow = 0;
            if (he0.IsInTri()) allow++;
            if (he1.IsInTri()) allow++;
            if (Mesh.Vertices.CountCommonNeighbours(he0.Start, he1.Start) > allow)
                return false;
            */

            CollapseEdgeImpl(halfedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal void CollapseEdgeImpl(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge; // to be removed
            var he1 = he0.Twin; // to be removed

            var he2 = he0.Next;
            var he3 = he1.Next;

            var v0 = he0.Start; // to be removed
            var v1 = he1.Start;

            var f0 = he0.Face;
            var f1 = he1.Face;

            // if edge is on a degree 2 hole, clean it up before collapsing
            if (f0 == null && he2.Next == he0)
            {
                CleanupDegree2Hole(he2);
                he2 = he0.Next;
                f0 = he0.Face;
            }
            else if (f1 == null && he3.Next == he1)
            {
                CleanupDegree2Hole(he3);
                he3 = he1.Next;
                f1 = he1.Face;
            }

            // update start vertex of all halfedges starting at v0
            foreach (var he in he0.CirculateStart.Skip(1))
                he.Start = v1;

            // update halfedge ref of v1 if necessary
            if (v1.First == he1)
                v1.First = he3; // maintains vertex boundary condition

            // update halfedge-halfedge refs
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he3);

            // update face->halfedge refs or handle collapsed faces/holes
            if (f0 == null)
            {
                if (he2.IsInDegree1)
                    CleanupDegree1Hole(he2);
            }
            else
            {
                if (he2.IsInDegree2)
                    CleanupDegree2Face(he2);
                else if (f0.First == he0)
                    f0.First = he2;
            }

            if (f1 == null)
            {
                if (he3.IsInDegree1)
                    CleanupDegree1Hole(he3);
            }
            else
            {
                if (he3.IsInDegree2)
                    CleanupDegree2Face(he3);
                else if (f1.First == he1)
                    f1.First = he3;
            }

            // flag elements for removal
            v0.MakeUnused();
            he0.MakeUnused();
        }


        /*
        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// This implementation does not allow for degree 2 holes.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool CollapseEdgeImpl(E halfedge, double t = 0.5)
        {
            HeMeshHalfedge he0 = halfedge; // to be removed
            HeMeshHalfedge he1 = he0.Twin; // to be removed
            HeMeshHalfedge he2 = he0.Next;
            HeMeshHalfedge he3 = he1.Next;

            HeMeshVertex v0 = he0.Start; // to be removed
            HeMeshVertex v1 = he1.Start;

            HeMeshFace f0 = he0.Face;
            HeMeshFace f1 = he1.Face;

            // update vertex refs of all halfedges starting at v0
            foreach (HeMeshHalfedge he in he0.CirculateStart.Skip(1))
                he.Start = v1;

            // update halfedge ref of v1 if necessary
            if (v1.First == he1)
                v1.First = he3; // maintains vertex boundary condition

            // update halfedge-halfedge refs
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he3);

            // handle potential degenerate faces or update face->halfedge refs
            if(f0 == null)
            {
                if (he2.IsInDegree2) 
                    CleanupDegree2Hole(he2);
            }
            else
            {
                if (he2.IsInDegree2) 
                    CleanupDegree2Face(he2);
                else if (f0.First == he0) 
                    f0.First = he2;
            }

            if(f1 == null)
            {
                if (he3.IsInDegree2)
                    CleanupDegree2Hole(he3);
            }
            else
            {
                if (he3.IsInDegree2)
                    CleanupDegree2Face(he3);
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
        */


        /// <summary>
        /// 
        /// </summary>
        internal void CleanupDegree2Face(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge; // to be removed
            var he1 = he0.Twin; // to be removed
            var he2 = he0.Next;
            var he3 = he1.Next;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var f0 = he0.Face; // to be removed
            var f1 = he1.Face;

            // update vertex->halfedge refs
            if (v0.First == he0) v0.First = he3;
            if (v1.First == he1) v1.First = he2;

            // update face->halfedge refs
            if (f1 != null && f1.First == he1) f1.First = he2;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he2, he3);

            // update halfedge->face ref
            he2.Face = f1;

            // handle potential invalid edge
            if (!he2.IsValid) he2.Remove();

            // flag for removal
            f0.MakeUnused();
            he0.MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        internal void CleanupDegree2Hole(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge; // to be removed
            var he1 = he0.Twin; // to be removed
            var he2 = he0.Next;
            var he3 = he1.Next;

            var v0 = he0.Start;
            var v1 = he1.Start;

            var f1 = he1.Face;

            // update vertex->halfedge refs
            // must look for another faceless halfedge to maintain boundary invariant for v0 and v1
            if (v0.First == he0)
            {
                var he = he0.NextBoundaryAtStart;
                v0.First = (he == null) ? he3 : he;
            }

            if (v1.First == he2)
            {
                var he = he2.NextBoundaryAtStart;
                v1.First = (he == null) ? he2 : he;
            }

            // update face->halfedge refs
            if (f1.First == he1) f1.First = he2;

            // update halfedge->face refs
            he2.Face = f1;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he2, he3);

            // flag elements for removal
            he0.MakeUnused();
        }


        /// <summary>
        ///
        /// </summary>
        internal void CleanupDegree1Hole(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge; // to be removed
            var he1 = he0.Twin; // to be removed

            var v0 = he0.Start;
            var f1 = he1.Face;

            // update vertex->halfedge refs
            // must look for another boundary halfedge to maintain boundary invariant for v0
            if (v0.First == he0)
            {
                var he = he0.NextBoundaryAtStart;
                v0.First = (he == null) ? he1.Next : he;
            }

            // update face->halfedge refs
            if (f1.First == he1) f1.First = he1.Next;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he1.Next);

            // flag elements for removal
            he0.MakeUnused();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public bool SpinEdge(HeMeshHalfedge halfedge)
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
        internal void SpinEdgeImpl(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge;
            var he1 = he0.Twin;

            var he2 = he0.Next;
            var he3 = he1.Next;

            var v0 = he0.Start;
            var v1 = he1.Start;

            // update vertex->halfedge refs if necessary
            if (he0 == v0.First) v0.First = he3;
            if (he1 == v1.First) v1.First = he2;

            // update halfedge->vertex refs
            he0.Start = he3.End;
            he1.Start = he2.End;

            var f0 = he0.Face;
            var f1 = he1.Face;

            // update face->halfedge refs if necessary
            if (he2 == f0.First) f0.First = he2.Next;
            if (he3 == f1.First) f1.First = he3.Next;

            // update halfedge->face refs
            he2.Face = f1;
            he3.Face = f0;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he0, he2.Next);
            HeMeshHalfedge.MakeConsecutive(he1, he3.Next);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he3);
            HeMeshHalfedge.MakeConsecutive(he2, he1);
            HeMeshHalfedge.MakeConsecutive(he3, he0);
        }


        /// <summary>
        /// Returns the new boundary halfedge created at the detach interface.
        /// The twin of the given halfedge will have a null face reference after detaching.
        /// </summary>
        /// <param name="halfedge"></param>
        public HeMeshHalfedge DetachEdge(HeMeshHalfedge halfedge)
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
        internal HeMeshHalfedge DetachEdgeImpl(HeMeshHalfedge halfedge)
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
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal HeMeshHalfedge DetachEdgeImpl(HeMeshHalfedge halfedge, out int mask)
        {
            var he0 = halfedge;
            var he1 = he0.Twin;

            mask = 0;
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
        internal HeMeshHalfedge DetachEdgeInterior(HeMeshHalfedge halfedge)
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
            HeMeshHalfedge.MakeConsecutive(he2, he1.Next);
            HeMeshHalfedge.MakeConsecutive(he1, he3);

            // update halfedge-halfedge refs @ v1
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he3, he1);

            // update vertex-halfedge refs
            v0.First = he3;
            v1.First = he1;

            return he3;
        }


        /// <summary>
        /// Both vertices are on the mesh boundary
        /// </summary>
        internal HeMeshHalfedge DetachEdgeBoundary(HeMeshHalfedge halfedge)
        {
            var verts = Owner.Vertices;

            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = verts.Add();
            var v3 = verts.Add();

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
            HeMeshHalfedge.MakeConsecutive(he2, he1.Next);
            HeMeshHalfedge.MakeConsecutive(he4.Previous, he3);
            HeMeshHalfedge.MakeConsecutive(he1, he4);

            // update halfedge-halfedge refs @ v1
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he5.Previous, he1);
            HeMeshHalfedge.MakeConsecutive(he3, he5);

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
        internal HeMeshHalfedge DetachEdgeMixed(HeMeshHalfedge halfedge)
        {
            var verts = Owner.Vertices;

            var he0 = halfedge;
            var he1 = he0.Twin;

            var v0 = he0.Start;
            var v1 = he1.Start;
            var v2 = verts.Add();

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
            HeMeshHalfedge.MakeConsecutive(he2, he1.Next);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he2);

            // update halfedge-halfedge refs @ v1
            HeMeshHalfedge.MakeConsecutive(he4.Previous, he1);
            HeMeshHalfedge.MakeConsecutive(he1, he3);
            HeMeshHalfedge.MakeConsecutive(he3, he4);

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
        /// 
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


        #region Attributes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="result"></param>
        public void GetEdgeDepths(IEnumerable<HeMeshHalfedge> sources, IList<int> result)
        {
            var queue = new Queue<HeMeshHalfedge>();
            result.SetRange(Int32.MaxValue, 0, Count >> 1);

            // enqueue sources and set to 0
            foreach (var he in sources)
            {
                OwnsCheck(he);
                if (he.IsUnused) continue;

                result[he.Index >> 1] = 0;
                queue.Enqueue(he);
            }

            // bfs
            while (queue.Count > 0)
            {
                var he0 = queue.Dequeue();
                int t0 = result[he0.Index >> 1] + 1;

                foreach (var he1 in he0.ConnectedPairs)
                {
                    int i1 = he1.Index >> 1;
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        if (he1.Face != null) queue.Enqueue(he1); // only enqueue the halfedge if it has a face
                    }
                }
            }
        }

        #endregion
    }
}
