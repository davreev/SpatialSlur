using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurCore;
using Rhino.Geometry;
using Grasshopper.Kernel;


namespace SpatialSlur.SlurMesh
{
    public static class GhExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="args"></param>
        public static void DisplayFaces(this HeFaceList faces, Color color, IGH_PreviewArgs args)
        {
            foreach (HeFace f in faces)
            {
                if (f.IsUnused) continue;

                List<Point3d> points = new List<Point3d>();
                foreach (HeVertex v in f.Vertices)
                    points.Add(v.Position.ToPoint3d());

                args.Display.DrawPolygon(points, color, true);
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="args"></param>
        public static void DisplayFaces(this HeFaceList faces, IList<Color> colors, IGH_PreviewArgs args)
        {
            faces.SizeCheck(colors);

            for(int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                List<Point3d> points = new List<Point3d>();
                foreach (HeVertex v in f.Vertices)
                    points.Add(v.Position.ToPoint3d());

                args.Display.DrawPolygon(points, colors[i], true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayEdges(this HeEdgeList edges, Color color, int thickness, IGH_PreviewArgs args)
        {
            List<Line> lines = new List<Line>();
   
            for (int i = 0; i < edges.Count; i+=2)
            {
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                lines.Add(e.ToLine());
            }

            args.Display.DrawLines(lines.ToArray(), color, thickness);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayEdges(this HeEdgeList edges, IList<Color> colors, int thickness, IGH_PreviewArgs args)
        {
            edges.SizeCheck(colors);
            List<Line> lines = new List<Line>();

            for (int i = 0; i < edges.Count; i += 2)
            {
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                args.Display.DrawLine(e.ToLine(), colors[i], thickness);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayHalfEdges(this HeEdgeList edges, double scale, Color color, Color boundaryColor, IGH_PreviewArgs args)
        {
            DisplayHalfEdges(edges, edges.Mesh.Faces.GetFaceCenters(), scale, color, boundaryColor, args);
        }


        /// <summary>
        /// v1.0
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayHalfEdges(this HeEdgeList edges, IList<Vec3d> faceCenters, double scale, Color color, Color boundaryColor, IGH_PreviewArgs args)
        {
            HeFaceList faces = edges.Mesh.Faces;
            faces.SizeCheck(faceCenters);

            List<Line> interior = new List<Line>();
            List<Line> boundary = new List<Line>();

            for (int i = 0; i < edges.Count; i++)
            {
                HeEdge e = edges[i];
                if (e.IsUnused) continue;

                Line ln = e.ToLine();
                if (e.Face == null)
                {
                    boundary.Add(ln);
                }
                else
                {
                    Transform xform = Transform.Scale(faceCenters[e.Face.Index].ToPoint3d(), scale);
                    ln.Transform(xform);
                    interior.Add(ln);
                }
            }

            args.Display.DrawArrows(interior, color);
            args.Display.DrawArrows(boundary, boundaryColor);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayVertices(this HeVertexList vertices, Color color, int size, IGH_PreviewArgs args)
        {
            List<Point3d> points = new List<Point3d>();

            foreach (HeVertex v in vertices)
            {
                if (v.IsUnused) continue;
                points.Add(v.Position.ToPoint3d());
            }

            args.Display.DrawPoints(points, Rhino.Display.PointStyle.Simple, size, color);
        }
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        /// <param name="boundaryColor"></param>
        /// <param name="args"></param>
        public static void DisplayVertices(this HeVertexList vertices, IList<Color> colors, int size, IGH_PreviewArgs args)
        {
            vertices.SizeCheck(colors);
            PointCloud cloud = new PointCloud();

            for (int i = 0; i < vertices.Count; i++)
            {
                HeVertex v = vertices[i];
                cloud.Add(v.Position.ToPoint3d(), colors[i]);
            }

            args.Display.DrawPointCloud(cloud, size);
        }

    }
}
