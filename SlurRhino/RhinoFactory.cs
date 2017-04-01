using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
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
            var result = new HeMesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices
            newVerts.Add(mesh.Vertices.Count);

            // add faces
            var faces = mesh.Faces;
            for (int i = 0; i < faces.Count; i++)
            {
                MeshFace f = faces[i];
                if (f.IsQuad)
                    newFaces.Add(f.A, f.B, f.C, f.D);
                else
                    newFaces.Add(f.A, f.B, f.C);
            }

            return result;
        }


        /// <summary>
        /// Creates a HeMesh instance from a Rhino Mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(Mesh mesh, out Vec3d[] vertexPositions)
        {
            vertexPositions = mesh.Vertices.Select(p => p.ToVec3d()).ToArray();
            return CreateHeMesh(mesh);
        }


        /// <summary>
        /// Creates a HeMesh instance from a Rhino Mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(Mesh mesh, out Vec3d[] vertexPositions, out Vec3d[] vertexNormals)
        {
            vertexPositions = mesh.Vertices.Select(p => p.ToVec3d()).ToArray();
            vertexNormals = mesh.Normals.Select(n => n.ToVec3d()).ToArray();
            return CreateHeMesh(mesh);
        }


        /// <summary>
        /// Creates a HeMesh instance from a collection of Polylines.
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateHeMesh(IEnumerable<Polyline> polylines, double tolerance, out List<Vec3d> vertexPositions)
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

            return HeMesh.CreateFromPolygons(points, sides, tolerance, out vertexPositions);
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
        public static HeGraph CreateHeGraph(IReadOnlyList<Line> lines, double epsilon, bool allowMultiEdges, bool allowLoops, out List<Vec3d> vertexPositions)
        {
            Vec3d[] endPts = new Vec3d[lines.Count << 1];

            for (int i = 0; i < endPts.Length; i += 2)
            {
                Line ln = lines[i >> 1];
                endPts[i] = ln.From.ToVec3d();
                endPts[i + 1] = ln.To.ToVec3d();
            }

            return HeGraph.CreateFromLineSegments(endPts, epsilon, allowMultiEdges, allowLoops, out vertexPositions);
        }
    }
}
