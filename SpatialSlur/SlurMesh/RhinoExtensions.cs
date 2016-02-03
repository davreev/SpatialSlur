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
    public static class RhinoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Line ToLine(this HalfEdge e)
        {
            return new Line(e.Start.Position.ToPoint3d(), e.Span.ToVector3d());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Polyline ToPolyline(this HeFace f)
        {
            Polyline result = new Polyline();

            foreach (HeVertex v in f.Vertices)
                result.Add(v.Position.ToPoint3d());
        
            result.Add(result.First);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Line[] GetEdgeLines(this HalfEdgeList list)
        {
            Line[] result = new Line[list.Count];

            Parallel.ForEach(Partitioner.Create(0, list.Count >> 1), range =>
            {
                int i0 = range.Item1 << 1;
                int i1 = range.Item2 << 1;
                for (int i = i0; i < i1; i+=2)
                {
                    HalfEdge e = list[i];
                    if (e.IsUnused) continue;
                    result[i] = e.ToLine();
                }
            });

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Polyline[] GetFacePolylines(this HeFaceList list)
        {
            Polyline[] result = new Polyline[list.Count];

            Parallel.ForEach(Partitioner.Create(0, list.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeFace f = list[i];
                    if (f.IsUnused) continue;
                    result[i] = f.ToPolyline();
                }
            });

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Circle[] GetFaceCircumcircles(this HeFaceList list)
        {
            Circle[] result = new Circle[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                HeFace f = list[i];
                if (f.IsUnused) continue;

                HalfEdge e = f.First;

                HeVertex v0 = e.Start;
                e = e.Next;
                HeVertex v1 = e.Start;
                e = e.Next;
                HeVertex v2 = e.Start;
               
                result[i] = new Circle(v0.Position.ToPoint3d(), v1.Position.ToPoint3d(), v2.Position.ToPoint3d());
            }

            return result;
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
        public static void Transform(this HeMesh mesh, Transform xform)
        {
            HeVertexList verts = mesh.Vertices;

            Parallel.ForEach(Partitioner.Create(0, verts.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = verts[i];
                    if (v.IsUnused) continue;

                    Point3d p = xform * v.Position.ToPoint3d();
                    v.Position = p.ToVec3d();
                }
            });
        }


        /// <summary>
        /// TODO support different triangulation schemes for n-gons.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMesh(this HeMesh mesh)
        {
            Mesh result = new Mesh();

            // add vertices
            foreach (HeVertex v in mesh.Vertices)
                result.Vertices.Add(v.Position.ToPoint3d());

            // add faces
            foreach (HeFace f in mesh.Faces)
            {
                if (f.IsUnused) continue;
                int ne = f.CountEdges();

                if (ne == 3)
                {
                    HalfEdge e = f.First;
                    result.Faces.AddFace(e.Start.Index, e.Next.Start.Index, e.Prev.Start.Index);
                }
                else if (ne == 4)
                {
                    HalfEdge e = f.First;
                    result.Faces.AddFace(e.Start.Index, e.Next.Start.Index, e.Next.Next.Start.Index, e.Prev.Start.Index);
                }
                else
                {
                    // TODO triangulate face
                }
            }

            return result;
        }


        /// <summary>
        /// TODO support different triangulation schemes for n-gons.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh ToRhinoMeshUnwelded(this HeMesh mesh)
        {
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
                    //TODO triangulate face
                }
            }

            return result;
        }

    }
}
