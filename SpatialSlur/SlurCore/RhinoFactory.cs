using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurGraph;
using Rhino.Geometry;

/*
 * Notes
 * 
 */ 

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// Static methods for creating SpatialSlur objects from Rhino objects.
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
            HeMesh result = new HeMesh();
            var vertList = result.Vertices;
            var faceList = result.Faces;

            // add vertices
            var verts = mesh.Vertices;
            for (int i = 0; i < verts.Count; i++)
                vertList.Add(verts[i].ToVec3d());

            // add faces
            var faces = mesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                MeshFace f = faces[i];
                if (f.IsQuad)
                    faceList.Add(f.A, f.B, f.C, f.D);
                else
                    faceList.Add(f.A, f.B, f.C);
            }

            return result;
        }


        /// <summary>
        /// Creates a HeMesh instance from a collection of Polylines.
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(IEnumerable<Polyline> polylines, double tolerance)
        {
            List<Vec3d> points = new List<Vec3d>();
            List<int> sides = new List<int>();

            // get all polyline points
            foreach (Polyline p in polylines)
            {
                int n = p.Count - 1;
                if (!p.IsClosed || n < 3) continue;  // skip open or invalid loops

                // collect all points in the loop
                for (int i = 0; i < n; i++)
                    points.Add(p[i].ToVec3d());

                sides.Add(n);
            }

            return HeMesh.CreateFromPolygons(points, sides, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static Graph CreateGraph(IList<Line> lines, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> nodePositions)
        {
            Vec3d[] endPts = new Vec3d[lines.Count << 1];

            for (int i = 0; i < endPts.Length; i += 2)
            {
                Line ln = lines[i >> 1];
                endPts[i] = ln.From.ToVec3d();
                endPts[i + 1] = ln.To.ToVec3d();
            }

            return Graph.CreateFromLineSegments(endPts, epsilon, allowMultiEdges, allowLoops, out nodePositions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <param name="nodePositions"></param>
        /// <returns></returns>
        public static DiGraph CreateDiGraph(IList<Line> lines, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> nodePositions)
        {
            Vec3d[] endPts = new Vec3d[lines.Count << 1];

            for (int i = 0; i < endPts.Length; i += 2)
            {
                Line ln = lines[i >> 1];
                endPts[i] = ln.From.ToVec3d();
                endPts[i + 1] = ln.To.ToVec3d();
            }

            return DiGraph.CreateFromLineSegments(endPts, epsilon, allowMultiEdges, allowLoops, out nodePositions);
        }
    }
}
