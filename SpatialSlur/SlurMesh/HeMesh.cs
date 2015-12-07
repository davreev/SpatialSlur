using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using Rhino.Geometry;


/* Notes
 * http://webdoc.sub.gwdg.de/ebook/serien/e/IMPA_A/406.pdf
 * */


namespace SpatialSlur.SlurMesh
{
    public class HeMesh
    {
        #region Static

        /// <summary>
        /// Creates a new HeMesh instance from face vertex topology information.
        /// Note that this method assumes consistent faces windings.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="faces"></param>
        /// <returns></returns>
        public static HeMesh CreateFromFaceVertexData(IList<Vec3d> vertices, IList<IList<int>> faces)
        {
            HeMesh hm = new HeMesh();
            var hmv = hm.Vertices;
            var hmf = hm.Faces;

            // add vertices
            foreach (Vec3d v in vertices)
                hmv.Add(v);

            // add faces
            foreach(int[] f in faces)
                hmf.Add(f);
              
            return hm;
        }


        /// <summary>
        /// Creates an HeMesh instance from a Rhino mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh CreateFromRhinoMesh(Mesh mesh)
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


        /*
        /// <summary>
        /// Creates an HeMesh instance from a Rhino mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh CreateFromRhinoMeshLegacy(Mesh mesh)
        {
            // check validity of mesh
            if (!mesh.IsValid) return null;

            Vec3d[] vertices = new Vec3d[mesh.TopologyVertices.Count];
            int[][] faces = new int[mesh.Faces.Count][];

            // get vertices
            for (int i = 0; i < mesh.TopologyVertices.Count; i++)
            {
                Point3d p = mesh.TopologyVertices[i];
                vertices[i] = p.ToVec3d();
            }

            // get face indices
            for (int i = 0; i < mesh.Faces.Count; i++)
                faces[i] = mesh.TopologyVertices.IndicesFromFace(i);

            return CreateFromFaceVertexData(vertices, faces);
        }
        */


        /// <summary>
        /// creates an hemesh from a collection of polylines
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolylines(IList<Polyline> polylines, double tolerance)
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
            return CreateFromFaceVertexData(verts, faces);
        }


        /*
        /// <summary>
        /// creates a new HeMesh instance from face vertex topology information
        /// this method assumes faces windings are unified
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="faces"></param>
        /// <returns></returns>
        public static HeMesh CreateFromFaceVertexData(IList<Point3d> vertices, IList<int[]> faces)
        {
            HeMesh mesh = new HeMesh();

            // add new HeVertices
            foreach (Point3d v in vertices)
                mesh.Vertices.Add(v.X, v.Y, v.Z);

            // add new HeFaces
            foreach (int[] f in faces)
                mesh.Faces.Add(f);

            return mesh;
        }
        */


        /*
        /// <summary>
        /// creates an hemesh from a collection of polylines
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolylines2(IList<Polyline> polylines, double tolerance)
        {
            List<Point3d> faceVerts = new List<Point3d>();
            List<Polyline> facePolys = new List<Polyline>();

            // get all polyline vertices
            foreach (Polyline p in polylines)
            {
                // skip open polylines
                if (!p.IsClosed) continue;

                // closed polyline must have more than 3 vertices
                if (p.Count < 3) continue;

                // collect all points except the last
                for (int i = 0; i < p.Count - 1; i++)
                    faceVerts.Add(p[i]);
     
                // add to list of face polygons
                facePolys.Add(p);
            }

            // remove duplicate points
            int[] faceIndices; 
            Point3d[] vertices = RhinoUtil.RemoveDuplicatePoints(faceVerts, tolerance, out faceIndices).ToArray();
            int[][] faces = new int[facePolys.Count][];
      
            // populate face-vertex arrays
            int marker = 0;
            for (int i = 0; i < facePolys.Count; i++)
            {
                int n = facePolys[i].Count - 1; // the last point was skipped in each polygon
                int[] face = new int[n];

                for (int j = 0; j < n; j++)
                {
                    face[j] = (faceIndices[marker]);
                    marker++;
                }

                faces[i] = face;
            }

            // create from face vertex data
            return CreateFromFaceVertexData(vertices, faces);
        }
        */

        #endregion


        private HeVertexList _vertices;
        private HeEdgeList _edges;
        private HeFaceList _faces;


        /// <summary>
        /// 
        /// </summary>
        public HeMesh() 
        {
            _vertices = new HeVertexList(this);
            _edges = new HeEdgeList(this);
            _faces = new HeFaceList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeVertexList Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeEdgeList Edges
        { 
            get { return _edges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeFaceList Faces
        {
            get { return _faces; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh (V:{0} E:{1} F:{2})", _vertices.Count, _edges.Count, _faces.Count);
        }


        /// <summary>
        /// removes all unused elements in the mesh
        /// </summary>
        public void Compact()
        {
            _vertices.Compact();
            _edges.Compact();
            _faces.Compact();
        }


        /// <summary>
        /// removes all objects coinciding with unused elements in the mesh
        /// useful for maintaining a matching collection of user defined objects
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertexAttributes"></param>
        /// <param name="edgeAttributes"></param>
        /// <param name="faceAttributes"></param>
        public void CompactAttributes<V, E, F>(List<V> vertexAttributes, List<E> edgeAttributes, List<F> faceAttributes)
        {
            _vertices.CompactAttributes(vertexAttributes);
            _edges.CompactAttributes(edgeAttributes);
            _faces.CompactAttributes(faceAttributes);
        }


        /// <summary>
        /// returns a deep copy of the mesh
        /// all mesh elements are copied while maintaining the references between them
        /// </summary>
        /// <returns></returns>
        public HeMesh Duplicate()
        {
            HeMesh result = new HeMesh();

            // add new elements to duplicate mesh
            foreach (HeVertex v in _vertices)
                result._vertices.Add(new HeVertex(v.Position));

            for (int i = 0; i < _edges.Count; i++)
                result._edges.Add(new HeEdge());

            for (int i = 0; i < _faces.Count; i++)
                result._faces.Add(new HeFace());

            // link vertices to edges
            for (int i = 0; i < _vertices.Count; i++)
            {
                HeVertex v0 = _vertices[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = result._vertices[i];
                v1.Outgoing = result._edges[v0.Outgoing.Index];
            }

            // link faces to edges
            for (int i = 0; i < _faces.Count; i++)
            {
                HeFace f0 = _faces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = result._faces[i];
                f1.First = result._edges[f0.First.Index];
            }

            // link edges to vertices, edges, and faces
            for (int i = 0; i < _edges.Count; i++)
            {
                HeEdge e0 = _edges[i];
                HeEdge e1 = result._edges[i];
                e1.Prev = result._edges[e0.Prev.Index];
                e1.Next = result._edges[e0.Next.Index];
                e1.Twin = result._edges[e0.Twin.Index];

                if (!e0.IsUnused) e1.Start = result._vertices[e0.Start.Index];
                if(e0.Face != null) e1.Face = result._faces[e0.Face.Index];
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeMesh other)
        {
            // cache current number of elements in mesh
            int nv = _vertices.Count;
            int ne = _edges.Count;
            int nf = _faces.Count;

            // append elements
            foreach (HeVertex v in other._vertices)
                _vertices.Add(new HeVertex(v.Position));

            for (int i = 0; i < other._edges.Count; i++)
                _edges.Add(new HeEdge());

            for (int i = 0; i < other._faces.Count; i++)
                _faces.Add(new HeFace());

            // link new vertices to new edges
            for (int i = 0; i < other._vertices.Count; i++)
            {
                HeVertex v0 = other._vertices[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = _vertices[i + nv];
                v1.Outgoing = _edges[v0.Outgoing.Index + ne];
            }

            // link new faces to new edges
            for (int i = 0; i < other._faces.Count; i++)
            {
                HeFace f0 = other._faces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = _faces[i + nf];
                f1.First = _edges[f0.First.Index + ne];
            }

            // link new edges to new vertices, edges, and faces
            for (int i = 0; i < other._edges.Count; i++)
            {
                HeEdge e0 = other._edges[i];
                HeEdge e1 = _edges[i + ne];
                e1.Prev = _edges[e0.Prev.Index + ne];
                e1.Next = _edges[e0.Next.Index + ne];
                e1.Twin = _edges[e0.Twin.Index + ne];

                if (!e0.IsUnused) e1.Start = _vertices[e0.Start.Index + nv];
                if (e0.Face != null) e1.Face = _faces[e0.Face.Index + nf];
            }
        }


        /// <summary>
        /// triangulates all non-tri faces in the mesh
        /// quads are triangulated by creating an edge between the first and third vertex (can be controlled by orienting faces beforehand)
        /// ngons are triangulated by adding a new vertex at the face center (stellate)
        /// TODO add support for different triangulation schemes
        /// </summary>
        public void Triangulate()
        {
            int nf = _faces.Count;

            for (int i = 0; i < nf; i++)
            {
                HeFace f = _faces[i];
                if (f.IsUnused) continue;

                int d = f.CountEdges();

                if (d == 4)
                    _faces.SplitFace(f.First, f.First.Next.Next);
                else if (d > 4)
                    _faces.Stellate(f);
            }
        }


        /// <summary>
        /// TODO implement more direct dual creation by spinning edges
        /// </summary>
        public HeMesh GetDual()
        {
            HeMesh result = new HeMesh();

            // add vertices from face centers
            foreach (HeFace f in _faces)
                result._vertices.Add(f.GetCenter());
            
            // add faces by circulating vertices
            foreach (HeVertex v in _vertices)
            {
                if (v.IsBoundary) continue;

                List<int> indices = new List<int>();
                foreach (HeFace f in v.SurroundingFaces)
                    indices.Add(f.Index);

                result.Faces.Add(indices);
            }

            return result;
        }


        /*
        /// <summary>
        /// returns the dual of this mesh
        /// excludes boundary vertices
        /// </summary>
        public HeMesh GetDual2()
        {
            HeMesh dual = new HeMesh();

            // add all objects

            // add a face for every vertex in the original
            // if the vertex is a boundary vertex flag the new face as unused
            for (int i = 0; i < _vertices.Count; i++)
            {
                HeVertex v = _vertices[i];
                HeFace fd = new HeFace();
                fd.First = v.Outgoing;

                if (v.IsBoundary) fd.MakeUnused();
                dual.Faces.Add(fd);
            }

            // add a vertex for every face in the original
            for (int i = 0; i < _faces.Count; i++)
            {
                HeFace f = _faces[i];
                HeVertex vd = new HeVertex(f.GetCenter());

                // set appropriate outgoing edge (should be on boundary)
                //vd.Outgoing

                dual.Vertices.Add(vd);
            }

            // add edges
            // set boundary edges to unused
            for (int i = 0; i < _edges.Count; i+=2)
            {
                HeEdge e0 = _edges[i];
                HeEdge e1 = e0.Twin;

                HeVertex v0 = dual._vertices[e0.Face.Index];
                HeVertex v1 = dual._vertices[e1.Face.Index];

                HeFace f0 = dual._faces[e0.Start.Index];
                HeFace f1 = dual._faces[e1.Start.Index];


                e0 = dual.Edges.AddPair(v0, v1);
                e1 = e0.Twin;

                e0.Face = f1;
                e1.Face = f0;




                ed.Face = f1;
                ed.Twin
            }

            // link elements up
        }
        */

        
        /// <summary>
        /// Returns the entries of the laplacian matrix in column-major order
        /// http://en.wikipedia.org/wiki/Laplacian_matrix
        /// </summary>
        /// <returns></returns>
        public double[] GetLaplacianMatrix()
        {
            int nv = _vertices.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = _vertices[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (HeEdge e in v.OutgoingEdges)
                {
                    int j = e.End.Index;
                    result[i * nv + j] = -1.0;
                    wsum++;
                }

                result[i + i * nv] = wsum;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public double[] GetLaplacianMatrix(IList<double> edgeWeights)
        {
            _edges.SizeCheck(edgeWeights);

            int nv = _vertices.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = _vertices[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (HeEdge e in v.OutgoingEdges)
                {
                    int j = e.End.Index;
                    double w = edgeWeights[e.Index];
                    result[i * nv + j] = -w;
                    wsum += w;
                }

                result[i * nv + i] = wsum;
            }

            return result;
        }
    }
}

