using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class HeMesh : HeStructure<HeMesh, HeMeshHalfedgeList, HeMeshVertexList, HeMeshFaceList, HeMeshHalfedge, HeMeshVertex, HeMeshFace>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public static HeMesh CreateFromObj(string path, out List<Vec3d> vertexPositions)
        {
            return HeMeshIO.ReadObj(path, out vertexPositions);
        }


        /// <summary>
        /// Creates a new HeMesh instance from face-vertex information.
        /// Note that this method assumes consistent faces windings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vertexCount"></param>
        /// <param name="faces"></param>
        /// <returns></returns>
        public static HeMesh CreateFromFaceVertexData<T>(int vertexCount, IEnumerable<T> faces)
            where T : IReadOnlyList<int>
        {
            var result = new HeMesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices
            newVerts.Add(vertexCount);

            // add faces
            foreach (var f in faces)
                newFaces.Add(f);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="polygons"></param>
        /// <param name="tolerance"></param>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolygons<T>(IEnumerable<T> polygons, double tolerance, out List<Vec3d> vertexPositions)
            where T : IEnumerable<Vec3d>
        {
            List<Vec3d> points = new List<Vec3d>();
            List<int> partitions = new List<int>();

            // get all polygon points
            foreach (var poly in polygons)
            {
                int n = points.Count;
                points.AddRange(poly);
                partitions.Add(points.Count - n);
            }

            return CreateFromPolygons(points, partitions, tolerance, out vertexPositions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="partitionSizes"></param>
        /// <param name="tolerance"></param>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolygons(IReadOnlyList<Vec3d> points, IEnumerable<int> partitionSizes, double tolerance, out List<Vec3d> vertexPositions)
        {
            int[] indexMap;
            vertexPositions = points.RemoveDuplicates(tolerance, out indexMap);
            return CreateFromFaceVertexData(vertexPositions.Count, indexMap.Segment(partitionSizes));
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public HeMesh()
            : this(4, 4, 4)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh(int halfedgeCapacity, int vertexCapacity, int faceCapacity)
        {
            Initialize(
               new HeMeshHalfedgeList(this, halfedgeCapacity),
               new HeMeshVertexList(this, vertexCapacity),
               new HeMeshFaceList(this, faceCapacity)
               );
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh(HeMesh other)
            :this(other.Halfedges.Count, other.Vertices.Count, other.Faces.Count)
        {
            Append(other);
        }


        /// <summary>
        /// Returns the Euler number of the struct.
        /// </summary>
        public int EulerNumber
        {
            get { return Vertices.Count - (Halfedges.Count >> 1) + Faces.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override HeMesh CreateInstance()
        {
            return new HeMesh();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh (HE:{0} V:{1} F:{2})", Halfedges.Count, Vertices.Count, Faces.Count);
        }


        /// <summary>
        /// Returns the dual of the mesh.
        /// Note this method preserves indexical correspondance between primal and dual mesh elements.
        /// </summary>
        /// <returns></returns>
        public HeMesh GetDual()
        {
            var result = new HeMesh(Halfedges.Count, Faces.Count, Vertices.Count);
            result.AppendDual(this);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        internal void AppendDual(HeMesh other)
        {
            if (ReferenceEquals(this, other))
                throw new ArgumentException("Cannot append the dual of an HeMesh to itself.");

            var hedges = Halfedges;
            var verts = Vertices;
            var faces = Faces;

            int nhe = hedges.Count;
            int nv = verts.Count;
            int nf = faces.Count;

            var otherHedges = other.Halfedges;
            var otherVerts = other.Vertices;
            var otherFaces = other.Faces;

            // add new vertices (1 per face in primal mesh)
            for (int i = 0; i < otherFaces.Count; i++)
                verts.Add();

            // add new faces (1 per vertex in primal mesh)
            for (int i = 0; i < otherVerts.Count; i++)
                faces.Add();

            // add halfedges and set their faces/vertices
            // spin each halfedge such that its face in the primal mesh corresponds with its start vertex in the dual
            for (int i = 0; i < otherHedges.Count; i += 2)
            {
                var heB0 = otherHedges[i];
                var heA0 = hedges.AddPair();
                if (heB0.IsUnused || heB0.IsBoundary) continue; // skip boundary edges

                var heB1 = otherHedges[i + 1];
                var vB0 = heB0.Start;
                var vB1 = heB1.Start;

                int mask = 0;
                if (vB0.IsBoundary) mask |= 1;
                if (vB1.IsBoundary) mask |= 2;
                if (mask == 3) continue; // skip non-manifold dual edges 

                var heA1 = heA0.Twin;
                heA0.Start = verts[heB0.Face.Index + nv];
                heA1.Start = verts[heB1.Face.Index + nv];

                if ((mask & 1) == 0) heA1.Face = faces[vB0.Index + nf]; // vB0 is interior
                if ((mask & 2) == 0) heA0.Face = faces[vB1.Index + nf]; // vB1 is interior
            }

            // set halfedge->halfedge refs
            for (int i = 0; i < otherHedges.Count; i++)
            {
                var heB0 = otherHedges[i];
                var heA0 = hedges[i + nhe];
      
                if (heA0.IsUnused) continue;
                var heB1 = heB0.Next;
                var heA1 = hedges[heB1.Index + nhe];

                // backtrack around primal face, until dual halfedge is valid
                while (heA1.IsUnused)
                {
                    heB1 = heB1.Next;
                    heA1 = hedges[heB1.Index + nhe];
                }

                HeMeshHalfedge.MakeConsecutive(heA1.Twin, heA0);
            }

            // set dual face->halfegde refs 
            // must be set before vertex refs to check for boundary invariant
            for (int i = 0; i < otherVerts.Count; i++)
            {
                var vB = otherVerts[i];
                var fA = faces[i + nf];
       
                if (vB.IsUnused || vB.IsBoundary) continue;
                fA.First = hedges[vB.First.Twin.Index + nhe]; // can assume dual edge around interior vertex is valid
            }

            // set dual vertex->halfedge refs
            for (int i = 0; i < otherFaces.Count; i++)
            {
                var fB = otherFaces[i];
                var vA = verts[i + nv];

                if (fB.IsUnused) continue;
                var heB = fB.First; // primal halfedge
                var heA = hedges[heB.Index + nhe]; // corresponding dual halfedge

                // find first used dual halfedge
                while (heA.IsUnused)
                {
                    heB = heB.Next;
                    if (heB == fB.First) goto EndFor; // dual vertex has no valid halfedges
                    heA = hedges[heB.Index + nhe];
                }

                vA.First = heA;
                vA.SetFirstToBoundary();

            EndFor:;
            }

            // cleanup any appended degree 2 faces
            for (int i = nf; i < faces.Count; i++)
            {
                var f = faces[i];
                if (!f.IsUnused && f.IsDegree2)
                    hedges.CleanupDegree2Face(f.First);
            }
        }
    }
}

