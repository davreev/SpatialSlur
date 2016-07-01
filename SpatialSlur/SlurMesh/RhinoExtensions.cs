using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class RhinoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh ToHeMesh(this Mesh mesh)
        {
            return RhinoFactory.CreateHeMesh(mesh);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public static Line ToLine(this Halfedge2 halfedge)
        {
            Vec3d p0 = halfedge.Start.Position;
            Vec3d p1 = halfedge.End.Position;
            return new Line(p0.x, p0.y, p0.z, p1.x, p1.y, p1.z);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Polyline ToPolyline(this HeFace face)
        {
            Polyline result = new Polyline();

            foreach (HeVertex v in face.Vertices)
                result.Add(v.Position.ToPoint3d());
        
            result.Add(result.First);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this HalfedgeList hedges, bool parallel = false)
        {
            Line[] result = new Line[hedges.Count >> 1];
            hedges.UpdateEdgeLines(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedges"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateEdgeLines(this HalfedgeList hedges, IList<Line> result, bool parallel = false)
        {
            hedges.HalfSizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, hedges.Count >> 1), range =>
                    hedges.UpdateEdgeLines(result, range.Item1, range.Item2));
            else
                hedges.UpdateEdgeLines(result, 0, hedges.Count >> 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateEdgeLines(this HalfedgeList hedges, IList<Line> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                Halfedge2 he = hedges[i << 1];
                if (he.IsUnused) continue;
                result[i] = he.ToLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Polyline[] GetFacePolylines(this HeFaceList faces, bool parallel = false)
        {
            Polyline[] result = new Polyline[faces.Count];
            faces.UpdateFacePolylines(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFacePolylines(this HeFaceList faces, IList<Polyline> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFacePolylines(result, range.Item1, range.Item2));
            else
                faces.UpdateFacePolylines(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFacePolylines(this HeFaceList faces, IList<Polyline> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;
                result[i] = f.ToPolyline();
            }
        }


        /// <summary>
        /// Assumes triangular faces.
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static Circle[] GetFaceCircumcircles(this HeFaceList faces, bool parallel = false)
        {
            Circle[] result = new Circle[faces.Count];
            faces.UpdateFaceCircumcircles(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void UpdateFaceCircumcircles(this HeFaceList faces, IList<Circle> result, bool parallel = false)
        {
            faces.SizeCheck(result);

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, faces.Count), range =>
                    faces.UpdateFaceCircumcircles(result, range.Item1, range.Item2));
            else
                faces.UpdateFaceCircumcircles(result, 0, faces.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void UpdateFaceCircumcircles(this HeFaceList faces, IList<Circle> result, int i0, int i1)
        {
            for (int i = i0; i < i1; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                Halfedge2 he = f.First;
                HeVertex v0 = he.Start;
                he = he.Next;
                HeVertex v1 = he.Start;
                he = he.Next;
                HeVertex v2 = he.Start;

                result[i] = new Circle(v0.Position.ToPoint3d(), v1.Position.ToPoint3d(), v2.Position.ToPoint3d());
            }
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Circle[] GetFaceIncircles(this HeFaceList list, IList<double> edgeLengths)
        {
            if (edgeLengths.Count != list.Mesh.Edges.Count)
                throw new ArgumentException("the number of edge lengths provided must match the number of edges in the associated mesh.");

            int nf = list.Count;
            Circle[] result = new Circle[nf];

            for (int i = 0; i < nf; i++)
            {
                HeFace f = list[i];
                if (f.IsUnused) continue;

                HeEdge e0 = f.First;
                HeEdge e1 = e0.Next;
                HeEdge e2 = e1.Next;

                ///result[i] = Circle.TryFitCircleTTT();
            }

            throw new System.NotImplementedException();    
            return result;
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this HeMesh mesh)
        {
            List<Point3d> points = new List<Point3d>();

            foreach (HeVertex v in mesh.Vertices)
            {
                if (v.IsUnused) continue;
                points.Add(v.Position.ToPoint3d());
            }

            return new BoundingBox(points);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="xform"></param>
        /// <param name="parallel"></param>
        public static void Transform(this HeMesh mesh, Transform xform, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, mesh.Vertices.Count), range =>
                   mesh.Transform(xform, range.Item1, range.Item2));
            else
                mesh.Transform(xform, 0, mesh.Vertices.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void Transform(this HeMesh mesh, Transform xform, int i0, int i1)
        {
            HeVertexList verts = mesh.Vertices;

            for (int i = i0; i < i1; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                Point3d p = xform * v.Position.ToPoint3d();
                v.Position = p.ToVec3d();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMesh(this HeMesh mesh)
        {
            // TODO support different triangulation schemes for n-gons.
            Mesh result = new Mesh();

            // add vertices
            foreach (HeVertex v in mesh.Vertices)
                result.Vertices.Add(v.Position.ToPoint3d());

            // add faces
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = f.Degree;

                if (ne == 3)
                {
                    Halfedge2 he = f.First;
                    result.Faces.AddFace(he.Start.Index, he.Next.Start.Index, he.Previous.Start.Index);
                }
                else if (ne == 4)
                {
                    Halfedge2 he = f.First;
                    result.Faces.AddFace(he.Start.Index, he.Next.Start.Index, he.Next.Next.Start.Index, he.Previous.Start.Index);
                }
                else
                {
                    // TODO triangulate face
                }
            }

            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMeshUnwelded(this HeMesh mesh)
        {
            // TODO support different triangulation schemes for n-gons.
            Mesh result = new Mesh();

            // add vertices per face
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = 0;

                // add face vertices
                foreach(HeVertex v in f.Vertices)
                {
                    result.Vertices.Add(v.Position.ToPoint3d());
                    ne++;
                }

                // add face(s)
                if (ne == 3)
                {
                    int n = result.Vertices.Count;
                    result.Faces.AddFace(n - 3, n - 2, n - 1);
                }
                else if (ne == 4)
                {
                    int n = result.Vertices.Count;
                    result.Faces.AddFace(n - 4, n - 3, n - 2, n - 1);
                }
                else
                {
                    // TODO triangulate face
                }
            }

            return result;
        }

    }
}
