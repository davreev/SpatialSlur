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
    public sealed class HeMeshFaceList : HeFaceList<HeMesh, HeMeshHalfedgeList, HeMeshVertexList, HeMeshFaceList, HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="capacity"></param>
        public HeMeshFaceList(HeMesh owner, int capacity)
        {
            Initialize(owner, capacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeMeshFace CreateElement()
        {
            return new HeMeshFace();
        }


        /// <summary>
        /// Triangulates all non-triangular faces in the mesh.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public void Triangulate(TriangulateMode mode = TriangulateMode.Strip)
        {
            int nf = Count;

            for (int i = 0; i < nf; i++)
            {
                var f = this[i];
                if (!f.IsUnused) Triangulate(f, mode);
            }
        }


        /// <summary>
        /// Splits all n-gonal faces into quads (and tris where necessary).
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public void Quadrangulate(QuadrangulateMode mode = QuadrangulateMode.Strip)
        {
            int nf = Count;

            for (int i = 0; i < nf; i++)
            {
                var f = this[i];
                if (!f.IsUnused) Quadrangulate(f, mode);
            }
        }


        /// <summary>
        /// Turns all faces to create consistent directionality across the mesh where possible.
        /// Assumes quadrilateral faces.
        /// http://page.math.tu-berlin.de/~bobenko/MinimalCircle/minsurftalk.pdf
        /// </summary>
        public void UnifyQuadOrientation(bool flip)
        {
            var stack = new Stack<HeMeshHalfedge>();
            int currTag = NextTag;

            for (int i = 0; i < Count; i++)
            {
                var f = this[i];
                if (f.IsUnused || f.Tag == currTag) continue; // skip if unused or already visited

                // check flip
                if (flip) f.First = f.First.Next;

                // flag as visited
                f.Tag = currTag;

                // add to stack
                stack.Push(f.First);
                UnifyQuadOrientation(stack, currTag);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public void UnifyQuadOrientation(HeMeshFace start)
        {
            start.UsedCheck();
            OwnsCheck(start);

            var stack = new Stack<HeMeshHalfedge>();
            int currTag = NextTag;

            // flag as visited
            start.Tag = currTag;

            // add first halfedge to stack
            stack.Push(start.First);
            UnifyQuadOrientation(stack, currTag);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UnifyQuadOrientation(Stack<HeMeshHalfedge> stack, int currTag)
        {
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();

                foreach (var he1 in he0.AdjacentQuads)
                {
                    var f1 = he1.Face;
                    if (f1 != null && f1.Tag != currTag)
                    {
                        f1.First = he1;
                        f1.Tag = currTag;
                        stack.Push(he1);
                    }
                }
            }
        }


        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        /// <param name="flip"></param>
        /// <returns></returns>
        public List<List<HeMeshHalfedge>> GetQuadStrips(bool flip)
        {
            // TODO return as IEnumerable<IEnumerable<E>> instead
            var result = new List<List<HeMeshHalfedge>>();
            var stack = new Stack<HeMeshHalfedge>();
            int currTag = NextTag;

            for (int i = 0; i < Count; i++)
            {
                var f = this[i];
                if (f.IsUnused || f.Tag == currTag) continue; // skip if unused or already visited

                stack.Push((flip) ? f.First.Next : f.First);
                GetQuadStrips(stack, currTag, result);
            }

            return result;
        }


        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="flip"></param>
        /// <returns></returns>
        public List<List<HeMeshHalfedge>> GetQuadStrips(HeMeshFace start, bool flip)
        {
            // TODO return as IEnumerable<IEnumerable<E>> instead
            start.UsedCheck();
            OwnsCheck(start);

            var result = new List<List<HeMeshHalfedge>>();
            var stack = new Stack<HeMeshHalfedge>();

            stack.Push((flip) ? start.First.Next : start.First);
            GetQuadStrips(stack, NextTag, result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetQuadStrips(Stack<HeMeshHalfedge> stack, int currTag, List<List<HeMeshHalfedge>> result)
        {
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();
                var f0 = he0.Face;

                // don't start from boundary halfedges or those with visited faces
                if (f0 == null || f0.Tag == currTag) continue;

                // backtrack to first encountered visited face or boundary
                var he1 = he0;
                var f1 = he1.Face;
                do
                {
                    he1 = he1.Twin.Next.Next;
                    f1 = he1.Face;
                } while (f1 != null && f1.Tag != currTag && f1 != f0);

                // collect halfedges in strip
                var strip = new List<HeMeshHalfedge>();
                he1 = he1.Previous.Previous.Twin;
                f1 = he1.Face;
                do
                {
                    // add left and right neighbours to stack
                    stack.Push(he1.Previous.Twin.Previous);
                    stack.Push(he1.Next.Twin.Next);

                    // add current halfedge to strip and flag face as visited
                    strip.Add(he1);
                    f1.Tag = currTag;

                    // advance to next halfedge
                    he1 = he1.Next.Next.Twin;
                    f1 = he1.Face;
                } while (f1 != null && f1.Tag != currTag);

                strip.Add(he1); // add last halfedge
                result.Add(strip);
            }
        }


        #region Topology Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <param name="vi2"></param>
        /// <returns></returns>
        public HeMeshFace Add(int vi0, int vi1, int vi2)
        {
            return Add(new int[] { vi0, vi1, vi2 });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi0"></param>
        /// <param name="vi1"></param>
        /// <param name="vi2"></param>
        /// <param name="vi3"></param>
        /// <returns></returns>
        public HeMeshFace Add(int vi0, int vi1, int vi2, int vi3)
        {
            return Add(new int[] { vi0, vi1, vi2, vi3 });
        }


        /// <summary>
        /// Adds a new face to the mesh.
        /// </summary>
        /// <param name="vertexIndices"></param>
        /// <returns></returns>
        public HeMeshFace Add(IReadOnlyList<int> vertexIndices)
        {
            int nv = vertexIndices.Count;
            if (nv < 3) return null; // no degenerate faces allowed

            var faceVerts = new HeMeshVertex[nv];
            var verts = Owner.Vertices;
            int currTag = verts.NextTag;

            // collect and validate vertices
            for (int i = 0; i < nv; i++)
            {
                var v = verts[vertexIndices[i]];

                // ensures each vertex is only used once within a single face
                if (v.Tag == currTag) return null;
                v.Tag = currTag;

                faceVerts[i] = v;
            }

            return AddImpl(faceVerts);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public HeMeshFace Add(HeMeshVertex v0, HeMeshVertex v1, HeMeshVertex v2)
        {
            return Add(new HeMeshVertex[] { v0, v1, v2 });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public HeMeshFace Add(HeMeshVertex v0, HeMeshVertex v1, HeMeshVertex v2, HeMeshVertex v3)
        {
            return Add(new HeMeshVertex[] { v0, v1, v2, v3 });
        }


        /// <summary>
        /// Adds a new face to the mesh.
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public HeMeshFace Add(IReadOnlyList<HeMeshVertex> vertices)
        {
            if (vertices.Count < 3) return null; // no degenerate faces allowed
            var verts = Owner.Vertices;
            int currTag = verts.NextTag;

            // validate vertices
            foreach (var v in vertices)
            {
                verts.OwnsCheck(v);

                // ensures each vertex is only used once within a single face
                if (v.Tag == currTag) return null;
                v.Tag = currTag;
            }

            return AddImpl(vertices);
        }


        /// <summary>
        /// http://pointclouds.org/blog/nvcs/martin/index.php
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        internal HeMeshFace AddImpl(IReadOnlyList<HeMeshVertex> vertices)
        {
            int n = vertices.Count;
            var faceLoop = new HeMeshHalfedge[n];

            // collect all existing halfedges in the new face
            for (int i = 0; i < n; i++)
            {
                var v = vertices[i];
                if (v.IsUnused) continue;

                // can't create a new face with an interior vertex
                if (!v.IsBoundary) return null;

                // search for an existing halfedge between consecutive vertices
                var he = v.FindHalfedgeTo(vertices[(i + 1) % n]);
                if (he == null) continue; // no existing halfedge

                // can't create a new face if the halfedge already has one
                if (he.Face != null) return null;
                faceLoop[i] = he;
            }

            /*
            // avoids creation of non-manifold vertices
            // if two consecutive new halfedges share a used vertex then that vertex will be non-manifold upon adding the face
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                if (faceLoop[i] == null && faceLoop[j] == null && !vertices[j].IsUnused) 
                    return null;
            }
            */

            // create the new face
            var newFace = Add();
    
            // create any missing halfedge pairs in the face loop and assign the new face
            var hedges = Owner.Halfedges;
            for (int i = 0; i < n; i++)
            {
                var he = faceLoop[i];

                // if missing a halfedge, add a pair between consecutive vertices
                if (he == null)
                {
                    he = hedges.AddPair(vertices[i], vertices[(i + 1) % n]);
                    faceLoop[i] = he;
                }

                he.Face = newFace;
            }

            // link consecutive halfedges
            for (int i = 0; i < n; i++)
            {
                var he0 = faceLoop[i];
                var he1 = faceLoop[(i + 1) % n];

                var he2 = he0.Next;
                var he3 = he1.Previous;
                var he4 = he0.Twin;

                var v0 = he0.Start;
                var v1 = he1.Start;

                // check if halfedges are newly created
                // new halfedges will have null previous or next refs
                int mask = 0;
                if (he2 == null) mask |= 1; // e0 is new
                if (he3 == null) mask |= 2; // e1 is new

                // 0 - neither halfedge is new
                // 1 - he0 is new, he1 is old
                // 2 - he1 is new, he0 is old
                // 3 - both halfedges are new
                switch (mask)
                {
                    case 0:
                        {
                            // neither halfedge is new
                            // if he0 and he1 aren't consecutive, then deal with non-manifold vertex as per http://www.pointclouds.org/blog/nvcs/
                            // otherwise, update the first halfedge at v1
                            if (he2 != he1)
                            {
                                var he = he1.NextBoundaryAtStart; // find the next boundary halfedge around v1 (must exist if halfedges aren't consecutive)
                                v1.First = he;

                                HeMeshHalfedge.MakeConsecutive(he.Previous, he2);
                                HeMeshHalfedge.MakeConsecutive(he3, he);
                                HeMeshHalfedge.MakeConsecutive(he0, he1);
                            }
                            else
                            {
                                v1.SetFirstToBoundary();
                            }

                            break;
                        }
                    case 1:
                        {
                            // he0 is new, he1 is old
                            HeMeshHalfedge.MakeConsecutive(he3, he4);
                            v1.First = he4;
                            goto default;
                        }
                    case 2:
                        {
                            // he1 is new, he0 is old
                            HeMeshHalfedge.MakeConsecutive(he1.Twin, he2);
                            goto default;
                        }
                    case 3:
                        {
                            // both halfedges are new
                            // deal with non-manifold case if v1 is already in use
                            if (v1.IsUnused)
                            {
                                HeMeshHalfedge.MakeConsecutive(he1.Twin, he4);
                            }
                            else
                            {
                                HeMeshHalfedge.MakeConsecutive(v1.First.Previous, he4);
                                HeMeshHalfedge.MakeConsecutive(he1.Twin, v1.First);
                            }
                            v1.First = he4;
                            goto default;
                        }
                    default:
                        {
                            HeMeshHalfedge.MakeConsecutive(he0, he1); // update refs for inner halfedges
                            break;
                        }
                }
            }

            newFace.First = faceLoop[0]; // set first halfedge in the new face
            return newFace;
        }


        /// <summary>
        /// Removes a face from the mesh as well as any invalid elements created in the process.
        /// Returns true on success.
        /// </summary>
        /// <param name="face"></param>
        public void Remove(HeMeshFace face)
        {
            OwnsCheck(face);
            face.UsedCheck();
            RemoveImpl(face);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        internal void RemoveImpl(HeMeshFace face)
        {
            /*
            // avoids creation of non-manifold vertices
            foreach (HeVertex v in face.Vertices)
                if (v.IsBoundary && v.First.Twin.Face != face) return false;
            */

            // update halfedge->face refs
            var he = face.First;
            do
            {
                if (he.Twin.Face == null)
                {
                    he.Remove();
                }
                else
                {
                    he.Start.First = he;
                    he.Face = null;
                }

                he = he.Next;
            } while (he.Face == face);

            // flag for removal
            face.MakeUnused();
        }


        /// <summary>
        /// Removes a halfedge pair, merging their two adajcent faces.
        /// The face of the given halfedge is removed.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public bool MergeFaces(HeMeshHalfedge halfedge)
        {
            halfedge.UsedCheck();
            Owner.Halfedges.OwnsCheck(halfedge);
            return MergeFacesImpl(halfedge);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal bool MergeFacesImpl(HeMeshHalfedge halfedge)
        {
            if (halfedge.IsInHole)
                return MergeHoleToFace(halfedge);
            else if (halfedge.Twin.IsInHole)
                return MergeFaceToHole(halfedge);
            else
                return MergeFaceToFace(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        private bool MergeFaceToFace(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge;
            var he1 = halfedge;

            var f1 = halfedge.Twin.Face;

            // backtrack to previous non-shared halfedge in f0
            do
            {
                he0 = he0.Previous;
                if (he0 == halfedge) return false; // all edges in f0 are shared with f1, can't merge
            } while (he0.Twin.Face == f1);

            // advance to next non-shared halfedge in f0
            do
            {
                he1 = he1.Next;
            } while (he1.Twin.Face == f1);

            // ensure single string of shared edges between f0 and f1
            {
                var he = he1;
                do
                {
                    if (he.Twin.Face == f1) return false; // multiple strings of shared edges detected, can't merge
                    he = he.Next;
                } while (he != he0);
            }

            // advance to first shared halfedge
            he0 = he0.Next;

            // update halfedge->face refs
            {
                var he = he1;
                do
                {
                    he.Face = f1;
                    he = he.Next;
                } while (he != he0);
            }

            // remove shared edges
            {
                var he = he0;
                do
                {
                    he.Remove();
                    he = he.Next;
                } while (he != he1);
            }

            // update face->halfedge ref if necessary
            if (f1.First.IsUnused) f1.First = he1;

            // flag face as unused
            he0.Face.MakeUnused();
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        private bool MergeFaceToHole(HeMeshHalfedge halfedge)
        {
            Owner.Faces.RemoveImpl(halfedge.Face);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        private bool MergeHoleToFace(HeMeshHalfedge halfedge)
        {
            var he0 = halfedge; // has null face
            var he1 = halfedge;

            var f1 = halfedge.Twin.Face;

            // backtrack to previous non-shared halfedge in f0
            do
            {
                he0 = he0.Previous;
                if (he0 == halfedge) return false; // all edges in f0 are shared with f1, can't merge
            } while (he0.Twin.Face == f1);

            // advance to next non-shared halfedge in f0
            do
            {
                he1 = he1.Next;
            } while (he1.Twin.Face == f1);

            // ensure single string of shared edges between f0 and f1
            {
                var he = he1;
                do
                {
                    if (he.Twin.Face == f1) return false; // multiple strings of shared edges detected, can't merge
                    he = he.Next;
                } while (he != he0);
            }

            // advance to first shared halfedge
            he0 = he0.Next;

            // update halfedge->face refs and vertex->halfedge refs if necessary
            {
                var he = he1;
                do
                {
                    he.Face = f1;
                    he.Start.SetFirstToBoundary();
                    he = he.Next;
                } while (he != he0);
            }

            // remove shared edges
            {
                var he = he0;
                do
                {
                    he.Remove();
                    he = he.Next;
                } while (he != he1);
            }

            // update face->halfedge ref if necessary
            if (f1.First.IsUnused) f1.First = he1;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public HeMeshFace FillHole(HeMeshHalfedge halfedge)
        {
            Owner.Halfedges.OwnsCheck(halfedge);
            halfedge.UsedCheck();

            // halfedge must be in a hole with at least 3 edges
            if (!halfedge.IsInHole && halfedge.IsInDegree2)
                return null;

            return FillHoleImpl(halfedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        internal HeMeshFace FillHoleImpl(HeMeshHalfedge halfedge)
        {
            var f = Add();
            f.First = halfedge;

            foreach (var he0 in halfedge.CirculateFace)
            {
                he0.Face = f;
                he0.Start.SetFirstToBoundary();
            }

            return f;
        }


        /// <summary>
        /// Splits a face by creating a new halfedge pair between the start vertices of the given halfedges.
        /// Returns the new halfedge that shares a start vertex with he0.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public HeMeshHalfedge SplitFace(HeMeshHalfedge he0, HeMeshHalfedge he1)
        {
            var hedges = Owner.Halfedges;
            hedges.OwnsCheck(he0);
            hedges.OwnsCheck(he1);

            he0.UsedCheck();
            he1.UsedCheck();

            // halfedges must be on the same face which can't be null
            if (he0.Face == null || he0.Face != he1.Face)
                return null;

            // halfedges can't be consecutive
            if (HeMeshHalfedge.AreConsecutive(he0, he1))
                return null;

            return SplitFaceImpl(he0, he1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal HeMeshHalfedge SplitFaceImpl(HeMeshHalfedge he0, HeMeshHalfedge he1)
        {
            var f0 = he0.Face;
            var f1 = Add();

            var he2 = Owner.Halfedges.AddPair(he0.Start, he1.Start);
            var he3 = he2.Twin;

            // set halfedge->face refs
            he3.Face = f0;
            he2.Face = f1;

            // set new halfedges as first in respective faces
            f0.First = he3;
            f1.First = he2;

            // update halfedge->halfedge refs
            HeMeshHalfedge.MakeConsecutive(he0.Previous, he2);
            HeMeshHalfedge.MakeConsecutive(he1.Previous, he3);
            HeMeshHalfedge.MakeConsecutive(he3, he0);
            HeMeshHalfedge.MakeConsecutive(he2, he1);

            // update face references of all halfedges in new loop
            var he = he2.Next;
            do
            {
                he.Face = f1;
                he = he.Next;
            } while (he != he2);

            return he2; // return halfedge adjacent to new face
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        public void Triangulate(HeMeshFace face, TriangulateMode mode)
        {
            face.UsedCheck();
            OwnsCheck(face);

            switch (mode)
            {
                case TriangulateMode.Fan:
                    TriangulateFan(face);
                    break;
                case TriangulateMode.Strip:
                    TriangulateStrip(face);
                    break;
                case TriangulateMode.Poke:
                    TriangulatePoke(face);
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        internal void TriangulateFan(HeMeshFace face)
        {
            var he0 = face.First;
            var he1 = he0.Next.Next;

            while (he1.Next != he0)
            {
                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.Next;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        internal void TriangulateStrip(HeMeshFace face)
        {
            var he0 = face.First;
            var he1 = he0.Next.Next;

            while (he1.Next != he0)
            {
                he0 = SplitFaceImpl(he0, he1).Previous;
                if (he1.Next == he0) break;

                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.Next;
            }
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal HeMeshVertex TriangulatePoke(HeMeshFace face)
        {
            // add new vertex at face center
            var vc = Owner.Vertices.Add();

            // hold first halfedge and vertex in the face
            var he = face.First;
            var v = he.Start;

            // create new halfedges and connect to existing ones
            var hedges = Owner.Halfedges;
            do
            {
                var he0 = hedges.AddPair(he.Start, vc);
                HeMeshHalfedge.MakeConsecutive(he.Previous, he0);
                HeMeshHalfedge.MakeConsecutive(he0.Twin, he);
                he = he.Next;
            } while (he.Start != v);

            he = face.First; // reset to first halfedge in face
            vc.First = he.Previous; // set outgoing halfedge for the central vertex

            // connect new halfedges and create new faces where necessary
            do
            {
                var he0 = he.Previous;
                var he1 = he.Next;
                HeMeshHalfedge.MakeConsecutive(he1, he0);

                // create new face if necessary
                if (face == null)
                {
                    face = Add();
                    face.First = he;
                    he.Face = face;
                }

                // assign halfedge->face refs
                he0.Face = face;
                he1.Face = face;
                face = null;

                he = he1.Twin.Next;
            } while (he.Start != v);

            return vc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <param name="mode"></param>
        public void Quadrangulate(HeMeshFace face, QuadrangulateMode mode)
        {
            face.UsedCheck();
            OwnsCheck(face);

            switch (mode)
            {
                case QuadrangulateMode.Fan:
                    QuadrangulateFan(face);
                    break;
                case QuadrangulateMode.Strip:
                    QuadrangulateStrip(face);
                    break;
                case QuadrangulateMode.Poke:
                    QuadrangulatePoke(face);
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        internal void QuadrangulateFan(HeMeshFace face)
        {
            var he0 = face.First;
            var he1 = he0.Next.Next.Next;

            while (he1 != he0 && he1.Next != he0)
            {
                he0 = SplitFaceImpl(he0, he1);
                he1 = he1.Next.Next;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        internal void QuadrangulateStrip(HeMeshFace face)
        {
            var he0 = face.First;
            var he1 = he0.Next.Next.Next;

            while (he1 != he0 && he1.Next != he0)
            {
                he0 = SplitFaceImpl(he0, he1).Previous;
                he1 = he1.Next;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal HeMeshVertex QuadrangulatePoke(HeMeshFace face)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion
    }
}
