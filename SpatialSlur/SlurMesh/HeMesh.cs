using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using Rhino.Geometry;


namespace SpatialSlur.SlurMesh
{
    public class HeMesh
    {
        #region Static

        /// <summary>
        /// Creates a new HeMesh instance from face-vertex information.
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
            foreach(IList<int> f in faces)
                hmf.Add(f);
              
            return hm;
        }


        [Obsolete("Use extension method instead")]
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


        [Obsolete("Use method in RhinoUtil instead")]
        /// <summary>
        /// Creates a HeMesh instance from a collection of Rhino Polylines.
        /// </summary>
        /// <param name="polylines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolylines(IEnumerable<Polyline> polylines, double tolerance)
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

        #endregion


        private HeVertexList _vertices;
        private HalfEdgeList _halfEdges;
        private HeFaceList _faces;


        /// <summary>
        /// 
        /// </summary>
        public HeMesh() 
        {
            _vertices = new HeVertexList(this);
            _halfEdges = new HalfEdgeList(this);
            _faces = new HeFaceList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh(int vertexCapacity, int halfEdgeCapacity, int faceCapacity)
        {
            _vertices = new HeVertexList(this, vertexCapacity);
            _halfEdges = new HalfEdgeList(this, halfEdgeCapacity);
            _faces = new HeFaceList(this, faceCapacity);
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
        public HalfEdgeList HalfEdges
        { 
            get { return _halfEdges; }
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
            return String.Format("HeMesh (V:{0} E:{1} F:{2})", _vertices.Count, _halfEdges.Count, _faces.Count);
        }


        /// <summary>
        /// Removes all unused elements in the mesh.
        /// </summary>
        public void Compact()
        {
            _vertices.Compact();
            _halfEdges.Compact();
            _faces.Compact();
        }


        /// <summary>
        /// Removes all attributes corresponding with unused elements in the mesh.
        /// This is intended for maintaining matching lists of user defined objects.
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
            _halfEdges.CompactAttributes(edgeAttributes);
            _faces.CompactAttributes(faceAttributes);
        }


        /// <summary>
        /// Returns a deep copy of the mesh.
        /// </summary>
        /// <returns></returns>
        public HeMesh Duplicate()
        {
            HeMesh result = new HeMesh();

            // add new elements to duplicate mesh
            foreach (HeVertex v in _vertices)
                result._vertices.Add(new HeVertex(v.Position));

            for (int i = 0; i < _halfEdges.Count; i++)
                result._halfEdges.Add(new HalfEdge());

            for (int i = 0; i < _faces.Count; i++)
                result._faces.Add(new HeFace());

            // link vertices to edges
            for (int i = 0; i < _vertices.Count; i++)
            {
                HeVertex v0 = _vertices[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = result._vertices[i];
                v1.Outgoing = result._halfEdges[v0.Outgoing.Index];
            }

            // link faces to edges
            for (int i = 0; i < _faces.Count; i++)
            {
                HeFace f0 = _faces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = result._faces[i];
                f1.First = result._halfEdges[f0.First.Index];
            }

            // link edges to vertices, edges, and faces
            for (int i = 0; i < _halfEdges.Count; i++)
            {
                HalfEdge e0 = _halfEdges[i];
                HalfEdge e1 = result._halfEdges[i];
                e1.Prev = result._halfEdges[e0.Prev.Index];
                e1.Next = result._halfEdges[e0.Next.Index];
                e1.Twin = result._halfEdges[e0.Twin.Index];

                if (!e0.IsUnused) e1.Start = result._vertices[e0.Start.Index];
                if(e0.Face != null) e1.Face = result._faces[e0.Face.Index];
            }

            return result;
        }


        /// <summary>
        /// Appends the elements of another mesh to this one.
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeMesh other)
        {
            // cache current number of elements in mesh
            int nv = _vertices.Count;
            int ne = _halfEdges.Count;
            int nf = _faces.Count;

            // append elements
            foreach (HeVertex v in other._vertices)
                _vertices.Add(new HeVertex(v.Position));

            for (int i = 0; i < other._halfEdges.Count; i++)
                _halfEdges.Add(new HalfEdge());

            for (int i = 0; i < other._faces.Count; i++)
                _faces.Add(new HeFace());

            // link new vertices to new edges
            for (int i = 0; i < other._vertices.Count; i++)
            {
                HeVertex v0 = other._vertices[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = _vertices[i + nv];
                v1.Outgoing = _halfEdges[v0.Outgoing.Index + ne];
            }

            // link new faces to new edges
            for (int i = 0; i < other._faces.Count; i++)
            {
                HeFace f0 = other._faces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = _faces[i + nf];
                f1.First = _halfEdges[f0.First.Index + ne];
            }

            // link new edges to new vertices, edges, and faces
            for (int i = 0; i < other._halfEdges.Count; i++)
            {
                HalfEdge e0 = other._halfEdges[i];
                HalfEdge e1 = _halfEdges[i + ne];
                e1.Prev = _halfEdges[e0.Prev.Index + ne];
                e1.Next = _halfEdges[e0.Next.Index + ne];
                e1.Twin = _halfEdges[e0.Twin.Index + ne];

                if (!e0.IsUnused) e1.Start = _vertices[e0.Start.Index + nv];
                if (e0.Face != null) e1.Face = _faces[e0.Face.Index + nf];
            }
        }


        /// <summary>
        /// Triangulates all non-triangular faces in the mesh.
        /// Quads are triangulated by creating an edge between the first and third vertex (can be controlled by orienting faces beforehand).
        /// N-gons are triangulated by adding a new vertex at the face center.
        /// 
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
        /// Returns the dual of the mesh.
        /// 
        /// TODO implement more direct dual creation by spinning edges.
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
        /// Returns the entries of the laplacian matrix in column-major order.
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

                foreach (HalfEdge e in v.OutgoingEdges)
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
        /// <param name="halfEdgeWeights"></param>
        /// <returns></returns>
        public double[] GetLaplacianMatrix(IList<double> halfEdgeWeights)
        {
            _halfEdges.SizeCheck(halfEdgeWeights);

            int nv = _vertices.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = _vertices[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (HalfEdge e in v.OutgoingEdges)
                {
                    int j = e.End.Index;
                    double w = halfEdgeWeights[e.Index];
                    result[i * nv + j] = -w;
                    wsum += w;
                }

                result[i * nv + i] = wsum;
            }

            return result;
        }
    }
}

