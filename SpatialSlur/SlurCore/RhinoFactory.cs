using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurGraph;
using Rhino.Geometry;


namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Static methods for creating SpatialSlur objects from Rhino objects
    /// </summary>
    public static class RhinoFactory
    {
        /// <summary>
        /// Creates a HeMesh instance from a Rhino Mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(Mesh mesh)
        {
            HeMesh hm = new HeMesh();
            var hmv = hm.Vertices;
            var hmf = hm.Faces;

            // add vertices
            var mv = mesh.Vertices;
            for (int i = 0; i < mv.Count; i++)
                hmv.Add(mv[i].ToVec3d());

            // add faces
            var mf = mesh.Faces;
            for (int i = 0; i < mf.Count; i++)
            {
                MeshFace f = mf[i];
                if (f.IsQuad)
                    hmf.Add(f.A, f.B, f.C, f.D);
                else
                    hmf.Add(f.A, f.B, f.C);
            }

            return hm;
        }


        /// <summary>
        /// Creates a HeMesh instance from a collection of Polylines.
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(IEnumerable<Polyline> polylines, double tolerance)
        {
            List<Vec3d> faceVerts = new List<Vec3d>();
            List<int> nSides = new List<int>();

            // get all polyline vertices
            foreach (Polyline p in polylines)
            {
                int n = p.Count - 1;
                if (!p.IsClosed || n < 3) continue;  // skip open or invalid loops

                // collect all points in the loop
                for (int i = 0; i < n; i++)
                    faceVerts.Add(p[i].ToVec3d());

                nSides.Add(n);
            }

            // remove duplicate points
            int[] faceIndices;
            List<Vec3d> verts = Vec3d.RemoveDuplicates(faceVerts, tolerance, out faceIndices);
            IList<int>[] faces = new IList<int>[nSides.Count];

            // get face arrays
            int marker = 0;
            for (int i = 0; i < nSides.Count; i++)
            {
                int n = nSides[i];
                faces[i] = new ArraySegment<int>(faceIndices, marker, n);
                marker += n;
            }

            // create from face vertex data
            return HeMesh.CreateFromFaceVertexData(verts, faces);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowDupEdges"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static Graph CreateGraph(IList<Line> lines, double epsilon, bool allowDupEdges, out List<Vec3d> nodePositions)
        {
            Vec3d[] endPts = new Vec3d[lines.Count << 1];

            for (int i = 0; i < endPts.Length; i += 2)
            {
                Line ln = lines[i >> 1];
                endPts[i] = ln.From.ToVec3d();
                endPts[i + 1] = ln.To.ToVec3d();
            }

            return Graph.CreateFromLineSegments(endPts, epsilon, allowDupEdges, out nodePositions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowDupEdges"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static DiGraph CreateDiGraph(IList<Line> lines, double epsilon, bool allowDupEdges, out List<Vec3d> nodePositions)
        {
            Vec3d[] endPts = new Vec3d[lines.Count << 1];

            for (int i = 0; i < endPts.Length; i += 2)
            {
                Line ln = lines[i >> 1];
                endPts[i] = ln.From.ToVec3d();
                endPts[i + 1] = ln.To.ToVec3d();
            }

            return DiGraph.CreateFromLineSegments(endPts, epsilon, allowDupEdges, out nodePositions);
        }
    }
}
