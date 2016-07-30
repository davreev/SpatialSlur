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
    public class HeVertexList : HeElementList<HeVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        internal HeVertexList(HeMesh mesh, int capacity = 2)
            : base(mesh, capacity)
        {
        }
      

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public HeVertex Add(double x, double y, double z)
        {
            HeVertex v = new HeVertex(x, y, z);
            Add(v);
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public HeVertex Add(Vec3d position)
        {
            HeVertex v = new HeVertex(position);
            Add(v);
            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void SetVertexPositions(IList<Vec3d> points, bool parallel = false)
        {
            SizeCheck(points);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        this[i].Position = points[i];
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    this[i].Position = points[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void SetVertexPositions(HeVertexList other, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        this[i].Position = other[i].Position;
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    this[i].Position = other[i].Position;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="parallel"></param>
        public void Translate(Vec3d delta, bool parallel = false)
        {
            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        this[i].Position += delta;
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    this[i].Position += delta;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltas"></param>
        /// <param name="parallel"></param>
        public void TranslateEach(IList<Vec3d> deltas, bool parallel = false)
        {
            SizeCheck(deltas);

            if (parallel)
            {
                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        this[i].Position += deltas[i];
                });
            }
            else
            {
                for (int i = 0; i < Count; i++)
                    this[i].Position += deltas[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<HeVertex> GetBreadthFirstOrder(HeVertex start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var queue = new Queue<HeVertex>();
            int currTag = NextTag;

            queue.Enqueue(start);
            start.Tag = currTag;

            while (queue.Count > 0)
            {
                HeVertex v0 = queue.Dequeue();
                yield return v0;

                foreach (HeVertex v1 in v0.ConnectedVertices)
                {
                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        queue.Enqueue(v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public IEnumerable<HeVertex> GetDepthFirstOrder(HeVertex start)
        {
            OwnsCheck(start);

            if (start.IsUnused)
                yield break;

            var stack = new Stack<HeVertex>();
            int currTag = NextTag;

            stack.Push(start);
            start.Tag = currTag;

            while (stack.Count > 0)
            {
                HeVertex v0 = stack.Pop();
                yield return v0;

                foreach (HeVertex v1 in v0.ConnectedVertices)
                {
                    if (v1.Tag != currTag)
                    {
                        v1.Tag = currTag;
                        stack.Push(v1);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            int n = 0;

            for (int i = 0; i < Count; i++)
            {
                var v = this[i];
                if (!v.IsUnused && v.IsBoundary) n++;
            }

            return n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<HeVertex> GetBoundaryVertices()
        {
            List<HeVertex> result = new List<HeVertex>();

            for (int i = 0; i < Count; i++)
            {
                var v = this[i];
                if (!v.IsUnused && v.IsBoundary) result.Add(v);
            }

            return result;
        }


        /// <summary>
        /// Returns the number of common neigbours shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonNeighbours(HeVertex v0, HeVertex v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return 0;

            return CountCommonNeighboursImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal int CountCommonNeighboursImpl(HeVertex v0, HeVertex v1)
        {
            int currTag = NextTag;

            // flag neighbours of v1
            foreach (HeVertex v in v1.ConnectedVertices)
                v.Tag = currTag;

            // count flagged neighbours of v0
            int count = 0;
            foreach (HeVertex v in v0.ConnectedVertices)
                if (v.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common neigbours shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public List<HeVertex> GetCommonNeighbours(HeVertex v0, HeVertex v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return new List<HeVertex>();

            return GetCommonNeighboursImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal List<HeVertex> GetCommonNeighboursImpl(HeVertex v0, HeVertex v1)
        {
            int currTag = NextTag;

            // flag neighbours of v1
            foreach (HeVertex v in v1.ConnectedVertices)
                v.Tag = currTag;

            // collect flagged neighbours of v0
            List<HeVertex> result = new List<HeVertex>();
            foreach (HeVertex v in v0.ConnectedVertices)
                if (v.Tag == currTag) result.Add(v);

            return result;
        }


        /// <summary>
        /// Returns the number of common adjacent faces shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonFaces(HeVertex v0, HeVertex v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return 0;

            return CountCommonFacesImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal int CountCommonFacesImpl(HeVertex v0, HeVertex v1)
        {
            int currTag = Mesh.Faces.NextTag;

            // flag neighbours of v1
            foreach (HeFace f in v1.SurroundingFaces)
                f.Tag = currTag;

            // count flagged neighbours of v0
            int count = 0;
            foreach (HeFace f in v0.SurroundingFaces)
                if (f.Tag == currTag) count++;

            return count;
        }


        /// <summary>
        /// Returns all common adjacent faces shared between v0 and v1.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public List<HeFace> GetCommonFaces(HeVertex v0, HeVertex v1)
        {
            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0.IsUnused || v1.IsUnused)
                return new List<HeFace>();

            return GetCommonFacesImpl(v0, v1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        internal List<HeFace> GetCommonFacesImpl(HeVertex v0, HeVertex v1)
        {
            int currTag = Mesh.Faces.NextTag;

            // flag neighbours of v1
            foreach (HeFace f in v1.SurroundingFaces)
                f.Tag = currTag;

            // collect flagged neighbours of v0
            List<HeFace> result = new List<HeFace>();
            foreach (HeFace f in v0.SurroundingFaces)
                if (f.Tag == currTag) result.Add(f);

            return result;
        }

        #region Euler Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public void Remove(HeVertex vertex)
        {
            vertex.UsedCheck();
            OwnsCheck(vertex);

            RemoveImpl(vertex);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal void RemoveImpl(HeVertex vertex)
        {
            // simplified removal for interior degree 2 vertices
            if (vertex.IsDegree2 && !vertex.IsBoundary)
                RemoveSimple(vertex); 

            var faces = Mesh.Faces;
            foreach (HeFace f in vertex.SurroundingFaces)
                faces.RemoveImpl(f);
        }


        /// <summary>
        /// Simplified removal method for interior degree 2 verts.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        internal void RemoveSimple(HeVertex vertex)
        {
            Halfedge he0 = vertex.First;
            Halfedge he1 = he0.Twin;
            Halfedge he2 = he1.Next;

            HeVertex v0 = vertex; // to be removed
            HeVertex v1 = he1.Start;

            // update vertex->halfedge refs if necesasry
            if (he1 == v1.First) v1.First = he2;
            he2.Start = v1;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he0.Next);
            Halfedge.MakeConsecutive(he1.Previous, he2);

            // flag for removal
            v0.MakeUnused();
            he0.MakeUnused();
            he1.MakeUnused();
        }


        /// <summary>
        /// Merges a pair of boundary vertices.
        /// The first vertex is flagged as unused.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool MergeVertices(HeVertex v0, HeVertex v1, double t = 0.5)
        {
            v0.UsedCheck();
            v1.UsedCheck();

            OwnsCheck(v0);
            OwnsCheck(v1);

            if (v0 == v1)
                return false;

            if (!(v0.IsBoundary && v1.IsBoundary)) 
                return false;

            return MergeVerticesImpl(v0, v1, t);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        internal bool MergeVerticesImpl(HeVertex v0, HeVertex v1, double t = 0.5)
        {
            Halfedge he0 = v0.First;
            Halfedge he1 = v1.First;

            Halfedge he2 = he0.Previous;
            Halfedge he3 = he1.Previous;

            // if vertices are consecutive, just collapse the halfedge between them
            if (he0 == he3)
                return Mesh.Halfedges.CollapseEdgeImpl(he0, t);
            else if (he1 == he2)
                return Mesh.Halfedges.CollapseEdgeImpl(he1, 1.0 - t);

            // update halfedge->vertex refs for all edges emanating from v1
            foreach (Halfedge he in v0.OutgoingHalfedges)
                he.Start = v1;

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he3, he0);
            Halfedge.MakeConsecutive(he2, he1);

            // deal with potential collapse of boundary loops on either side of the merge
            if (he1.Next == he2)
            {
                Mesh.Faces.MergeInvalidFace(he1);
                v1.First = he0; // maintain boundary status of v1
            }

            if (he0.Next == he3)
            {
                Mesh.Faces.MergeInvalidFace(he0);
            }

            // flag elements for removal
            v0.MakeUnused();

            // update postition of remaining vertex
            v1.Position = Vec3d.Lerp(v0.Position, v1.Position, t);
            return true;
        }


        /// <summary>
        /// Splits a vertex in 2 connected by a new edge.
        /// Returns the new edge on success and null on failure.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public Halfedge SplitVertex(Halfedge he0, Halfedge he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();
         
            var hedges = Mesh.Halfedges;
            hedges.OwnsCheck(he0);
            hedges.OwnsCheck(he1);

            if (he0.Start != he1.Start) 
                return null;

            return SplitVertexImpl(he0, he1);
        }

      
        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal Halfedge SplitVertexImpl(Halfedge he0, Halfedge he1)
        {
            // if the same edge or vertex is degree 2 then just split the edge
            if (he0 == he1 || he0.IsFromDegree2)
                return Mesh.Halfedges.SplitEdgeImpl(he0, 0.0);

            HeVertex v0 = he0.Start;
            HeVertex v1 = Add(v0.Position);

            Halfedge he2 = Mesh.Halfedges.AddPair(v0, v1);
            Halfedge he3 = he2.Twin;

            // update halfedge->face refs
            he2.Face = he0.Face;
            he3.Face = he1.Face;

            // update start vertex of all outoging edges between he0 and he1
            Halfedge he = he0;
            do
            {
                he.Start = v1;
                he = he.Twin.Next;
            } while (he != he1);

            // update vertex->halfedge refs if necessary
            if (v0.First.Start == v1)
            {
                // if v0's outgoing halfedge now starts at v1, can assume v1 is now on the boundary if v0 was originally
                v1.First = v0.First;
                v0.First = he2;
            }
            else
            {
                v1.First = he3;
            }

            // update halfedge->halfedge refs
            Halfedge.MakeConsecutive(he0.Previous, he2);
            Halfedge.MakeConsecutive(he2, he0);
            Halfedge.MakeConsecutive(he1.Previous, he3);
            Halfedge.MakeConsecutive(he3, he1);

            return he2;
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        public bool DetatchVertex(Halfedge he0, Halfedge he1)
        {
            he0.UsedCheck();
            he1.UsedCheck();

            var hedges = Mesh.Halfedges;
            hedges.OwnsCheck(he0);
            hedges.OwnsCheck(he1);

            if (he0 == he1) 
                return false;

            if (he0.Start != he1.Start)
                return false;

            return DetatchVertexImpl(he0, he1);
        }


        /// <summary>
        /// Assumes the given elements are valid for the operation.
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal bool DetatchVertexImpl(Halfedge he0, Halfedge he1)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

    }
}
