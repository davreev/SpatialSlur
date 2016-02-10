using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class HeFaceList:HeElementList<HeFace>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        internal HeFaceList(HeMesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="capacity"></param>
        internal HeFaceList(HeMesh mesh, int capacity)
            : base(mesh, capacity)
        {
        }


        /// <summary>
        /// Adds a new face to the mesh.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public HeFace Add(int v0, int v1, int v2)
        {
            HeVertexList verts = Mesh.Vertices;
            return Add(new HeVertex[] { verts[v0], verts[v1], verts[v2]});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public HeFace Add(int v0, int v1, int v2, int v3)
        {
            HeVertexList verts = Mesh.Vertices;
            return Add(new HeVertex[] { verts[v0], verts[v1], verts[v2], verts[v3]});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public HeFace Add(IList<int> indices)
        {
            HeVertexList verts = Mesh.Vertices;
            HeVertex[] fv = new HeVertex[indices.Count];

            for (int i = 0; i < indices.Count; i++)
                fv[i] = verts[indices[i]];

            return Add(fv);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        private HeFace Add(IList<HeVertex> vertices)
        {
            // don't allow degenerate faces
            if (vertices.Count < 3) return null;

            // all vertices must be unused or on boundary
            foreach (HeVertex v in vertices)
                if (!(v.IsUnused || v.IsBoundary)) return null;

            // edges of the new face
            int n = vertices.Count;
            HalfEdge[] faceLoop = new HalfEdge[n];

            // gather any existing edges in the face loop
            for (int i = 0, j = 1; i < n; i++, j++)
            {
                if (j == n) j = 0; // wrap j 

                // search for existing edge between vertices
                HalfEdge e = vertices[i].FindHalfEdgeTo(vertices[j]);

                // if the edge does exist, it can't already have a face
                if (e == null)
                    continue;
                else if (e.Face == null)
                    faceLoop[i] = e;
                else
                    return null;
            }

            /*
            // avoids creation of non-manifold vertices
            // if two consecutive new edges share a used vertex then that vertex will be non-manifold upon adding the face
            for (int i = 0, j = 1; i < n; i++, j++)
            {
                if (j == n) j = 0; // wrap j
                if (faceLoop[i] == null && faceLoop[j] == null && !vertices[j].IsUnused) 
                    return null;
            }
            */

            // create any missing edge pairs in the face loop and assign the new face
            HeFace result = new HeFace();
            for (int i = 0, j = 1; i < n; i++, j++)
            {
                if (j == n) j = 0; // wrap j
                HalfEdge e = faceLoop[i];

                // if missing an edge, add a new edge pair between the vertices
                if (e == null)
                {
                    e = Mesh.HalfEdges.AddPair(vertices[i], vertices[j]);
                    faceLoop[i] = e;
                }

                e.Face = result; // assign the new face
            }

            // link consecutive edges
            for (int i = 0, j = 1; i < n; i++, j++)
            {
                if (j == n) j = 0; // wrap j
                HalfEdge e0 = faceLoop[i];
                HalfEdge e1 = faceLoop[j];
                HeVertex v0 = e0.Start;
                HeVertex v1 = e1.Start;

                // check if edges are newly created (new edges will have null refs to previous and/or next)
                int mask = 0;
                if (e0.Next == null) mask |= 1; // e0 is new
                if (e1.Previous == null) mask |= 2; // e1 is new

                if (mask == 0)
                {
                    // neither edge is new
                    // update v1's outgoing edge if necessary 
                    HalfEdge e = null;
                    if (e1.IsFirstFromVertex)
                    {
                        e = e1.FindBoundary(); // find the next boundary edge around v1
                        if (e != null) v1.First = e;
                    }

                    // if the two existing edges aren't consecutive then deal with non-manifold vertex http://www.pointclouds.org/blog/nvcs/
                    if (e0.Next != e1)
                    {
                        // find the next boundary edge around v1 if it hasn't been done already
                        if (e == null)
                            e = e1.FindBoundary();

                        // if no other boundary edge was found then something went horribly wrong
                        if (e == null)
                            throw new InvalidOperationException(String.Format("No second boundary edge was found around vertex {0}. Face creation failed.", v1.Index));

                        HalfEdge.MakeConsecutive(e.Previous, e0.Next);
                        HalfEdge.MakeConsecutive(e1.Previous, e);
                        HalfEdge.MakeConsecutive(e0, e1);
                    }
                }
                else
                {
                    // at least one edge is new
                    // update edge-edge refs for outer edges depending on case
                    switch (mask)
                    {
                        case 1:
                            // e0 is new, e1 is old
                            HalfEdge.MakeConsecutive(e1.Previous, e0.Twin);
                            v1.First = e0.Twin;
                            break;
                        case 2:
                            // e1 is new, e0 is old
                            HalfEdge.MakeConsecutive(e1.Twin, e0.Next);
                            break;
                        case 3:
                            // both edges are new
                            if (v1.IsUnused)
                            {
                                // if v1 has no outgoing edge
                                HalfEdge.MakeConsecutive(e1.Twin, e0.Twin);
                            }
                            else
                            {
                                // if v1 is already in use (has an outgoing edge)
                                // deal with non-manifold case
                                HalfEdge.MakeConsecutive(v1.First.Previous, e0.Twin);
                                HalfEdge.MakeConsecutive(e1.Twin, v1.First);
                            }
                            v1.First = e0.Twin;
                            break;
                    }

                    // update edge-edge refs for inner edges
                    HalfEdge.MakeConsecutive(e0, e1);
                }
            }

            // set face-edge ref
            result.First = faceLoop[0];

            // add to face list and return
            Add(result);
            return result;
        }


        /// <summary>
        /// Turns all faces to create consistent directionality across the mesh where possible.
        /// This is intended for use on quad meshes.
        /// http://page.math.tu-berlin.de/~bobenko/MinimalCircle/minsurftalk.pdf
        /// </summary>
        public void UnifyOrientation()
        {
            // find an appropriate start edge
            bool[] visited = new bool[Count];
            Stack<HalfEdge> stack = new Stack<HalfEdge>();

            for (int i = 0; i < Count; i++)
            {
                // skip if unused or already visited
                HeFace f = this[i];
                if (f.IsUnused || visited[i]) continue;

                // conduct a depth first search from face i, orienting adjacent faces along the way
                stack.Push(f.First);

                do
                {
                    HalfEdge e = stack.Pop();
                    f = e.Face;
                    if (f == null || visited[f.Index]) continue; // skip boundary edges or those whose face has already been visited

                    // turn face and flag as visited
                    e.MakeFirstInFace();
                    visited[f.Index] = true;

                    // add next edges to stack (preference one direction over other for consistent uv directionality where possible)
                    stack.Push(e.Twin.Next.Next); // down
                    stack.Push(e.Next.Next.Twin); // up
                    stack.Push(e.Previous.Twin.Previous); // left
                    stack.Push(e.Next.Twin.Next); // right
                } while (stack.Count > 0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void UnifyOrientation(HalfEdge start)
        {
            Mesh.HalfEdges.Validate(start);

            bool[] visited = new bool[Count];
            Stack<HalfEdge> stack = new Stack<HalfEdge>();
            stack.Push(start);

            // conduct a depth first search from face i, orienting adjacent faces along the way
            do
            {
                HalfEdge e = stack.Pop();
                HeFace f = e.Face;
                if (f == null || visited[f.Index]) continue; // skip boundary edges or those whose face has already been visited

                // turn face and flag as visited
                e.MakeFirstInFace();
                visited[f.Index] = true;

                // add next edges to stack (preference one direction over other for consistent uv directionality where possible)
                stack.Push(e.Twin.Next.Next); // down
                stack.Push(e.Next.Next.Twin); // up
                stack.Push(e.Previous.Twin.Previous); // left
                stack.Push(e.Next.Twin.Next); // right
            } while (stack.Count > 0);
        }


        /// <summary>
        /// Returns lists of half-edges whose faces form stips.
        /// This is intended for use on quad meshes.
        /// </summary>
        public List<List<HalfEdge>> GetStrips(HeFace start)
        {
            Mesh.HalfEdges.Validate(start);

            List<List<HalfEdge>> strips = new List<List<HalfEdge>>();
            bool[] visited = new bool[Count];

            Stack<HalfEdge> stack = new Stack<HalfEdge>();
            stack.Push(start.First);

            // breadth first search
            do
            {
                HalfEdge first = stack.Pop();
                HeFace f = first.Face;
                if (f == null || visited[f.Index]) continue; // don't start from boundary edges or those with visited faces

                // backtrack to first encountered visited face or boundary
                HalfEdge e = first;
                f = e.Twin.Face;
                while (f != null && !visited[f.Index])
                {
                    e = e.Twin.Next.Next; // down
                    f = e.Twin.Face;
                    if (e == first) break; // break if back at first edge
                }

                // collect edges in strip
                List<HalfEdge> strip = new List<HalfEdge>();
                f = e.Face;
                do
                {
                    // add left/right neighbours to stack
                    stack.Push(e.Previous.Twin.Previous);
                    stack.Push(e.Next.Twin.Next);

                    // add current edge to strip and flag face as visited
                    strip.Add(e);
                    visited[f.Index] = true;

                    // advance to next edge
                    e = e.Next.Next.Twin;
                    f = e.Face;
                } while (f != null && !visited[f.Index]);

                strips.Add(strip);
            } while (stack.Count > 0);

            return strips;
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        public List<List<HeEdge>> GetStrips(HeEdge start)
        {
            Mesh.Edges.Validate(start);
            List<List<HeEdge>> strips = new List<List<HeEdge>>();
            bool[] visited = new bool[Count];

            Queue<HeEdge> q = new Queue<HeEdge>();
            q.Enqueue(start);

            // modified breadth first search from start edge
            List<HeEdge> strip = new List<HeEdge>();
            do
            {
                HeEdge first = q.Dequeue();

                // march up strip
                HeEdge e = first;
                HeFace f = e.Face;
                while (f != null && !visited[f.Index])
                {
                    strip.Add(e);
                    visited[f.Index] = true; // flag face as visited

                    q.Enqueue(e.Prev.Twin.Prev); // add left to queue
                    q.Enqueue(e.Next.Twin.Next); // add right to queue
                    e = e.Next.Next.Twin; // move up
                    f = e.Face;
                }
                strip.Reverse();

                // march down strip
                e = first.Twin.Next.Next;
                f = e.Face;
                while (f != null && !visited[f.Index])
                {
                    strip.Add(e);
                    visited[f.Index] = true; // flag face as visited

                    q.Enqueue(e.Prev.Twin.Prev); // add left to queue
                    q.Enqueue(e.Next.Twin.Next); // add right to queue
                    e = e.Twin.Next.Next; // move down
                    f = e.Face;
                }

                // add strip if not empty
                if (strip.Count > 0)
                {
                    strips.Add(strip);
                    strip = new List<HeEdge>();
                }

            } while (q.Count > 0);

            return strips;
        }
        */


        /// <summary>
        /// Orients each quad such that the first half-edge has the shortest diagonal in the face.
        /// This is intended for use on quad meshes.
        /// </summary>
        public void OrientQuadsToShortestDiagonal()
        {
            for (int i = 0; i < Count; i++)
            {
                HeFace f = this[i];
                if (f.IsUnused) continue;

                HalfEdge e0 = f.First;
                HalfEdge e1 = e0.Next.Next;
                if (e0.Previous != e1.Next) continue; // quad check

                // compare diagonals
                Vec3d p0 = e0.Start.Position;
                Vec3d p1 = e0.End.Position;
                Vec3d p2 = e1.Start.Position;
                Vec3d p3 = e1.End.Position;

                if (p0.SquareDistanceTo(p2) > p1.SquareDistanceTo(p3))
                    e0.Next.MakeFirstInFace();
            }
        }


        /// <summary>
        /// Counts the number of faces adjacent to both given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public int CountCommonNeighbours(HeFace f0, HeFace f1)
        {
            Validate(f0);
            Validate(f1);

            List<int> indices = new List<int>();

            // flag faces around f0 by setting their index to a unique integer
            foreach (HeFace f in f0.AdjacentFaces)
            {
                indices.Add(f.Index); // cache face indices for reset
                f.Index = -2;
            }

            // count flagged faces around f1
            int count = 0;
            foreach (HeFace f in f1.AdjacentFaces)
                if (f.Index == -2) count++;

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Faces[i].Index = i;

            return count;
        }


        /// <summary>
        /// Returns all faces adjacent to both given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public IList<HeFace> GetCommonNeighbours(HeFace f0, HeFace f1)
        {
            Validate(f0);
            Validate(f1);

            List<HeFace> result = new List<HeFace>();
            List<int> indices = new List<int>();

            // flag faces around f0 by setting their index to a unique integer
            foreach (HeFace f in f0.AdjacentFaces)
            {
                indices.Add(f.Index); // cache face indices for reset
                f.Index = -2;
            }

            // cache flagged faces around f1
            foreach (HeFace f in f1.AdjacentFaces)
                if (f.Index == -2) result.Add(f);

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Faces[i].Index = i;

            return result;
        }


        /// <summary>
        /// Counts the number of vertices shared by the two given faces.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public int CountCommonVertices(HeFace f0, HeFace f1)
        {
            Validate(f0);
            Validate(f1);

            List<int> indices = new List<int>();

            // flag vertices around f0 by setting their index to a unique integer
            foreach (HeVertex v in f0.Vertices)
            {
                indices.Add(v.Index); // cache vertex indices for reset
                v.Index = -2;
            }

            // count flagged faces around f1
            int count = 0;
            foreach (HeVertex v in f1.Vertices)
                if (v.Index == -2) count++;

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Vertices[i].Index = i;

            return count;
        }


        /// <summary>
        /// Returns all vertices shared by both given faces.
        /// TODO use visited mask instead of changing indices
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public IList<HeVertex> GetCommonVertices(HeFace f0, HeFace f1)
        {
            Validate(f0);
            Validate(f1);

            List<HeVertex> result = new List<HeVertex>();
            List<int> indices = new List<int>();

            // flag vertices around f0 by setting their index to a unique integer
            foreach (HeVertex v in f0.Vertices)
            {
                indices.Add(v.Index); // cache vertex indices for reset
                v.Index = -2;
            }

            // cache flagged faces around f1
            foreach (HeVertex v in f1.Vertices)
                if (v.Index == -2) result.Add(v);

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Vertices[i].Index = i;

            return result;
        }


        #region Element Attributes


        /// <summary>
        /// Returns the number of boundary edges in each face.
        /// </summary>
        /// <returns></returns>
        public int[] GetFaceBoundaryStatus()
        {
            int[] result = new int[Count];
            UpdateFaceBoundaryStatus(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateFaceBoundaryStatus(IList<int> result)
        {
            SizeCheck(result);

            HalfEdgeList edges = Mesh.HalfEdges;
            for (int i = 0; i < edges.Count; i += 2)
            {
                HalfEdge e = edges[i];
                if (e.IsUnused) continue;

                if (e.IsBoundary)
                {
                    result[e.Face.Index]++;
                    result[e.Twin.Face.Index]++;
                }
            }
        }


        /// <summary>
        /// Returns the number of edges in each face.
        /// </summary>
        /// <returns></returns>
        public int[] GetFaceDegrees()
        {
            int[] result = new int[Count];
            UpdateFaceDegrees(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateFaceDegrees(IList<int> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = this[i].CountEdges();
            });
        }


        /// <summary>
        /// Returns the topological depth of all faces connected to a set of sources.
        /// </summary>
        /// <returns></returns>
        public int[] GetFaceDepths(IList<int> sources)
        {
            int[] result = new int[Count];
            UpdateFaceDepths(sources, result);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public void UpdateFaceDepths(IList<int> sources, IList<int> result)
        {
            SizeCheck(result);

            Queue<int> queue = new Queue<int>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (int i in sources)
            {
                queue.Enqueue(i);
                result[i] = 0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                int t0 = result[i0] + 1;

                foreach (HeFace f in this[i0].AdjacentFaces)
                {
                    int i1 = f.Index;
              
                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(i1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the topological distance of all faces connected to a set of sources.
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceDepths(IList<int> sources, IList<double> halfEdgeLengths)
        {
            double[] result = new double[Count];
            UpdateFaceDepths(sources, halfEdgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateFaceDepths(IList<int> sources, IList<double> halfEdgeLengths, IList<double> result)
        {
            SizeCheck(result);
            Mesh.HalfEdges.SizeCheck(halfEdgeLengths);

            Queue<int> queue = new Queue<int>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (int i in sources)
            {
                queue.Enqueue(i);
                result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                double t0 = result[i0];

                foreach (HalfEdge e in this[i0].HalfEdges)
                {
                    HeFace f = e.Twin.Face;
                    if (f == null) continue;

                    int i1 = f.Index;
                    double t1 = t0 + halfEdgeLengths[e.Index];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(i1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec3d[] GetFaceCenters()
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateFaceCenters(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateFaceCenters(IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;
                    result[i] = f.GetCenter();
                }
            });
        }


        /*
        /// <summary>
        /// Returns distances between the start of each edge and the end of the next within each face.
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceDiagonalLengths()
        {
            double[] result = new double[Mesh.Edges.Count];
            UpdateFaceDiagonalLengths(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateFaceDiagonalLengths(IList<double> result)
        {
            Mesh.Edges.SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = List[i];
                    if (f.IsUnused) continue;
          
                    HeEdge e0 = f.First;
                    HeEdge e1 = e0.Next.Next;

                    if (e1.Next == e0)
                    {
                        // tri case
                        continue;
                    }
                    else if (e1.Next == e0.Prev)
                    {
                        // quad case
                        for (int j = 0; j < 2; j++)
                        {
                            double d = e0.Start.VectorTo(e1.Start).Length;
                            result[e0.Index] = d;
                            result[e1.Index] = d;
                            e0 = e0.Next;
                            e1 = e1.Next;
                        }
                    }
                    else
                    {
                        // general ngon case
                        foreach (HeEdge e in f.Edges)
                            result[e.Index] = e.Start.VectorTo(e.Next.End).Length;
                    }
                }
            });
        }
        */


        /// <summary>
        /// Calculates face normals as the area-weighted sum of half-edge normals in each face.
        /// Face normals are unitized by default.
        /// </summary>
        /// <returns></returns>
        public Vec3d[] GetFaceNormals()
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateFaceNormals(result);
            return result;
        }


        /// <summary>
        /// Calculates face normals as the sum of half-edge normals in each face.
        /// Half-edge normals can be scaled in advance for custom weighting.
        /// Face normals are unitized by default.
        /// </summary>
        /// <param name="halfEdgeNormals"></param>
        /// <returns></returns>
        public Vec3d[] GetFaceNormals(IList<Vec3d> halfEdgeNormals)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateFaceNormals(halfEdgeNormals, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateFaceNormals(IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    if (f.IsTri)
                    {
                        // simplified tri case
                        Vec3d v = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                        v.Unitize();
                        result[i] = v;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d sum = new Vec3d();

                        foreach (HalfEdge e in f.HalfEdges)
                            sum += Vec3d.Cross(e.Previous.Span, e.Span);

                        sum.Unitize();
                        result[i] = sum;
                    }
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfEdgeNormals"></param>
        /// <param name="result"></param>
        public void UpdateFaceNormals(IList<Vec3d> halfEdgeNormals, IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    if (f.IsTri)
                    {
                        // simplified unitized tri case
                        Vec3d v = halfEdgeNormals[f.First.Index];
                        v.Unitize();
                        result[i] = v / v.Length;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d sum = new Vec3d();
              
                        foreach (HalfEdge e in f.HalfEdges)
                            sum += halfEdgeNormals[e.Index];

                        sum.Unitize();
                        result[i] = sum;
                    }
                }
            });
        }


        /// <summary>
        /// Calculates face normals as the normal of the first half-edge in the face.
        /// Face normals are unitized by default.
        /// This method assumes all faces are triangular.
        /// </summary>
        /// <returns></returns>
        public Vec3d[] GetFaceNormalsTri()
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateFaceNormalsTri(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateFaceNormalsTri(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    Vec3d v = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceAreas()
        {
            double[] result = new double[Count];
            UpdateFaceAreas(result);
            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceAreas(IList<Vec3d> faceCenters)
        {
            double[] result = new double[Count];
            UpdateFaceAreas(faceCenters, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateFaceAreas(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    if (f.IsTri)
                    {
                        // simplified tri case
                        Vec3d norm = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                        result[i] = norm.Length * 0.5;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d cen = f.GetCenter();
                        double sum = 0.0;

                        foreach (HalfEdge e in f.HalfEdges)
                            sum += Vec3d.Cross(e.Start.Position - cen, e.Span).Length * 0.5;

                        result[i] = sum;
                    }
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        public void UpdateFaceAreas(IList<Vec3d> faceCenters, IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    if (f.IsTri)
                    {
                        // simplified tri case
                        Vec3d norm = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                        result[i] = norm.Length * 0.5;
                    }
                    else
                    {
                        // general ngon case
                        Vec3d cen = faceCenters[i];
                        double sum = 0.0;

                        foreach (HalfEdge e in f.HalfEdges)
                            sum += Vec3d.Cross(e.Start.Position - cen, e.Span).Length * 0.5;

                        result[i] = sum;
                    }
                }
            });
        }


        /// <summary>
        /// Returns the area of each face.
        /// This method assumes all faces are triangular.
        /// </summary>
        /// <returns></returns>
        public double[] GetFaceAreasTri()
        {
            double[] result = new double[Count];
            UpdateFaceAreasTri(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateFaceAreasTri(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    // simplified tri case
                    Vec3d norm = Vec3d.Cross(f.First.Span, f.First.Next.Span);
                    result[i] = norm.Length * 0.5;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetFacePlanarity()
        {
            double[] result = new double[Count];
            UpdateFacePlanarity(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void UpdateFacePlanarity(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = this[i];
                    if (f.IsUnused) continue;

                    HalfEdge e0 = f.First;
                    HalfEdge e1 = e0.Next;
                    HalfEdge e2 = e1.Next;
                    HalfEdge e3 = e2.Next;
                    if (e3 == e0) continue; // ensure face has at least 4 edges

                    if (e3.Next == e0)
                    {
                        // simplified quad case
                        Vec3d span = GeometryUtil.GetShortestVector(e0.Start.Position, e2.Start.Position, e1.Start.Position, e3.Start.Position);
                        result[i] = span.Length;
                    }
                    else
                    {
                        // general ngon case
                        double sum = 0.0;
                        do
                        {
                            Vec3d span = GeometryUtil.GetShortestVector(e0.Start.Position, e2.Start.Position, e1.Start.Position, e3.Start.Position);
                            sum += span.Length;

                            // advance to next set of 4 edges
                            e0 = e0.Next;
                            e1 = e1.Next;
                            e2 = e2.Next;
                            e3 = e3.Next;
                        } while (e0 != f.First);

                        result[i] = sum;
                    }
                }
            });
        }


        #endregion


        #region Euler Operators

        /// <summary>
        /// Removes a face from the mesh as well as any invalid elements created in the process.
        /// Returns true on success.
        /// </summary>
        /// <param name="face"></param>
        public bool Remove(HeFace face)
        {
            Validate(face);

            /*
            // avoids creatiion of non-manifold vertices
            foreach (HeVertex v in face.Vertices)
                if (v.IsBoundary && v.Outgoing.Twin.Face != face) return false;
            */

            // update edge-face refs
            HalfEdge ef = face.First;
            HeVertex vf = ef.Start;

            // can't just circulate face since edge connectivity may be changed within loop
            HalfEdgeList edges = Mesh.HalfEdges;
            do
            {
                if (ef.Twin.Face == null)
                    edges.RemovePair(ef);
                else
                {
                    ef.Face = null;
                    ef.MakeFirstFromVertex();
                }
                ef = ef.Next;
            } while (!ef.IsUnused && ef.Start != vf);

            // flag as unused
            face.MakeUnused();
            return true;
        }


        /// <summary>
        /// Removes a half-edge pair, merging their two adajcent faces.
        /// The face adjacent to the given halfedge is retained.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool MergeFaces(HalfEdge edge)
        {
            HalfEdgeList edges = Mesh.HalfEdges;
            edges.Validate(edge);
     
            HalfEdge e0 = edge;
            HalfEdge e1 = e0.Twin;

            HeFace f0 = e0.Face;
            HeFace f1 = e1.Face; // face to be removed
    
            // if edge is on boundary, just remove the existing face
            if (f0 == null)
                return Remove(f1);
            else if (f1 == null)
                return Remove(f0);

            // update edge ref for f0 if necessary
            if (e0.IsFirstInFace) 
                e0.Next.MakeFirstInFace();

            // update face refs for all edges in f1
            foreach (HalfEdge e in e1.CirculateFace.Skip(1)) 
                e.Face = f0;

            // remove edge pair shared between the two faces 
            edges.RemovePair(e0);

            // clean up any valence 1 vertices created in the process
            e0 = e0.Next;
            while (e0.IsFromDegree1)
            {
                edges.RemovePair(e0);
                e0 = e0.Next;
            }

            e1 = e1.Next;
            while (e1.IsFromDegree1)
            {
                edges.RemovePair(e1);
                e1 = e1.Next;
            }

            // flag elements for removal
            f1.MakeUnused();
            return true;
        }


        /// <summary>
        /// Splits a face by creating a new half-edge pair between the start vertices of the given halfedges.
        /// Returns the new edge adjacent to the new face on success or null on failure.
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <returns></returns>
        public HalfEdge SplitFace(HalfEdge e0, HalfEdge e1)
        {
            HalfEdgeList edges = Mesh.HalfEdges;
            edges.Validate(e0);
            edges.Validate(e1);
         
            // edges must be on the same face which can't be null
            if (e0.Face == null || e0.Face != e1.Face) 
                return null;

            // edges can't be consecutive
            if (HalfEdge.AreConsecutive(e0, e1)) 
                return null;
      
            HeFace f0 = e0.Face;
            HeFace f1 = new HeFace();
            Add(f1);

            HalfEdge e2 = Mesh.HalfEdges.AddPair(e0.Start, e1.Start);
            HalfEdge e3 = e2.Twin;

            // set edge-face refs
            e3.Face = f0;
            e2.Face = f1;

            // set new edges as first in respective faces
            f0.First = e3;
            f1.First = e2;
  
            // update edge-edge refs
            HalfEdge.MakeConsecutive(e0.Previous, e2);
            HalfEdge.MakeConsecutive(e1.Previous, e3);
            HalfEdge.MakeConsecutive(e3, e0);
            HalfEdge.MakeConsecutive(e2, e1);

            // update face references of all edges in new loop
            HalfEdge e = e2.Next;
            do
            {
                e.Face = f1;
                e = e.Next;
            } while (e != e2);

            return e2; // return edge adjacent to new face
        }


        /// <summary>
        /// Returns the new vertex created at the center of the given face.
        /// </summary>
        /// <param name="face"></param>
        public HeVertex Stellate(HeFace face)
        {
            Validate(face);

            // add new vertex at face center
            HeVertex fc = Mesh.Vertices.Add(face.GetCenter());

            // hold first edge and vertex in the face
            HalfEdge e = face.First;
            HeVertex v = e.Start;

            // create new edges and connect to existing ones
            HalfEdgeList edges = Mesh.HalfEdges;
            do
            {
                HalfEdge e0 = edges.AddPair(e.Start, fc);
                HalfEdge.MakeConsecutive(e.Previous, e0);
                HalfEdge.MakeConsecutive(e0.Twin, e);
                e = e.Next;
            } while (e.Start != v);

            e = face.First; // reset to first edge in face
            fc.First = e.Previous; // set outgoing edge for the central vertex

            // connect new edges to eachother and create new faces where necessary
            do
            {
                HalfEdge e0 = e.Previous;
                HalfEdge e1 = e.Next;
                HalfEdge.MakeConsecutive(e1, e0);

                // create new face if necessary
                if (face == null)
                {
                    face = new HeFace();
                    Add(face);
                    face.First = e;
                    e.Face = face;
                }
           
                // assign edge-face refs
                e0.Face = face;
                e1.Face = face;
                face = null;
                e = e1.Twin.Next;
            } while (e.Start != v);

            return fc;
        }


        /// <summary>
        /// TODO 
        /// Triangulates a given face without adding any vertices.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public HalfEdge Triangulate(HeFace face)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
