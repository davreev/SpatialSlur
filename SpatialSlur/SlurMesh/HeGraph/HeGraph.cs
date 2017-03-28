using System;
using System.Collections.Generic;
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
    public sealed class HeGraph : HeStructure<HeGraph, HeGraphHalfedgeList, HeGraphVertexList, HeGraphHalfedge, HeGraphVertex>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public static HeGraph CreateFromLineSegments(IReadOnlyList<Vec3d> endPoints, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> vertexPositions)
        {
            int[] indexMap;
            vertexPositions = endPoints.RemoveDuplicates(epsilon, out indexMap);

            var result = new HeGraph(endPoints.Count >> 1, vertexPositions.Count);
            var verts = result.Vertices;
            var hedges = result.Halfedges;

            // add vertices
            for (int i = 0; i < vertexPositions.Count; i++)
                verts.Add();

            // add edges
            int mask = 0;
            if (allowMultiEdges) mask |= 1;
            if (allowLoops) mask |= 2;

            // 0 - neither allowed
            // 1 - no loops allowed
            // 2 - no multi-edges allowed
            // 3 - both allowed
            switch (mask)
            {
                case 0:
                    {
                        // no multi-edges or loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1 && (v0.IsUnused || !v0.IsConnectedTo(v1))) hedges.AddPairImpl(v0, v1);
                        }
                        break;
                    }
                case 1:
                    {
                        // no loops allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0 != v1) hedges.AddPairImpl(v0, v1);
                        }
                        break;
                    }
                case 2:
                    {
                        // no multi-edges allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                        {
                            var v0 = verts[indexMap[i]];
                            var v1 = verts[indexMap[i + 1]];
                            if (v0.IsUnused || !v0.IsConnectedTo(v1)) hedges.AddPairImpl(v0, v1);
                        }
                        break;
                    }
                case 3:
                    {
                        // both allowed
                        for (int i = 0; i < indexMap.Length; i += 2)
                            hedges.AddPair(indexMap[i], indexMap[i + 1]);
                        break;
                    }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph CreateFromVertexTopology(HeMesh mesh)
        {
            var result = new HeGraph(mesh.Halfedges.Count, mesh.Vertices.Count);
            result.AppendVertexTopology(mesh);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph CreateFromFaceTopology(HeMesh mesh)
        {
            var result = new HeGraph(mesh.Halfedges.Count, mesh.Faces.Count);
            result.AppendFaceTopology(mesh);
            return result;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeGraph()
            : this(4, 4)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedgeCapacity"></param>
        /// <param name="vertexCapacity"></param>
        public HeGraph(int halfedgeCapacity, int vertexCapacity)
        {
            Initialize(
                new HeGraphHalfedgeList(this, halfedgeCapacity),
                new HeGraphVertexList(this, vertexCapacity)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public HeGraph(HeGraph other)
            :this(other.Halfedges.Count, other.Vertices.Count)
        {
            Append(other);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeGraph CreateInstance()
        {
            return new HeGraph();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeGraph (HE:{0} V:{1})", Halfedges.Count, Vertices.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendVertexTopology(HeMesh mesh)
        {
            var hedges = Halfedges;
            var verts = Vertices;

            int nhe = hedges.Count;
            int nv = verts.Count;

            var meshHedges = mesh.Halfedges;
            var meshVerts = mesh.Vertices;

            // add new halfedge pairs
            for (int i = 0; i < meshHedges.Count; i += 2)
                hedges.AddPair();

            // link new halfedges to eachother
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var heB = meshHedges[i];
                var heA = hedges[i + nhe];
           
                if (heB.IsUnused) continue;
                HeGraphHalfedge.MakeConsecutive(heA, hedges[heB.Twin.Next.Index + nhe]);
            }

            // add new vertices and link to new halfedges
            for (int i = 0; i < meshVerts.Count; i++)
            {
                var vB = meshVerts[i];
                var vA = Vertices.Add();
                if (vB.IsUnused) continue;

                var heA = Halfedges[vB.First.Index + nhe];
                vA.First = heA;

                foreach (var he in heA.CirculateStart) he.Start = vA;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public void AppendFaceTopology(HeMesh mesh)
        {
            var hedges = Halfedges;
            var verts = Vertices;

            int nhe = hedges.Count;
            int nv = verts.Count;

            var meshHedges = mesh.Halfedges;
            var meshFaces = mesh.Faces;

            // add new halfedge pairs
            for (int i = 0; i < meshHedges.Count; i += 2)
                hedges.AddPair();

            // add new vertices
            for (int i = 0; i < meshFaces.Count; i++)
                verts.Add();

            // link new halfedges to eachother
            for (int i = 0; i < meshHedges.Count; i++)
            {
                var heB0 = meshHedges[i];
                var heA0 = hedges[i + nhe];
                if (heB0.IsUnused || heB0.IsBoundary) continue; // skip boundary edges

                var heB1 = heB0.Next;
                while (heB1.Twin.Face == null && heB1 != heB0) heB1 = heB1.Next; // advance to next interior halfedge in the face

                heA0.Start = verts[heB0.Face.Index + nv];
                HeGraphHalfedge.MakeConsecutive(heA0, hedges[heB1.Index + nhe]);
            }

            // set vertex->halfedge refs
            for (int i = 0; i < meshFaces.Count; i++)
            {
                var fB = meshFaces[i];
                var vA = verts[i + nv];
                if (fB.IsUnused) continue;

                // find first interior halfedge in fB
                var heB = fB.First;
                while (heB.Twin.Face == null)
                {
                    heB = heB.Next;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                }

                vA.First = hedges[heB.Index + nhe];
            EndFor:;
            }
        }
    }
}
