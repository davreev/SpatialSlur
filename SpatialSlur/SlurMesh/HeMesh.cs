using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
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
            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add vertices
            foreach (Vec3d v in vertices)
                newVerts.Add(v);

            // add faces
            foreach(IList<int> f in faces)
                newFaces.Add(f);
              
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolygons(IList<IList<Vec3d>> polygons, double tolerance)
        {
            List<Vec3d> points = new List<Vec3d>();
            int[] nsides = new int[polygons.Count];

            // get all polygon points
            for (int i = 0; i < polygons.Count; i++)
            {
                var poly = polygons[i];
                points.AddRange(poly);
                nsides[i] = poly.Count;
            }

            return HeMesh.CreateFromPolygons(points, nsides, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pointsPerPolygon"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static HeMesh CreateFromPolygons(IList<Vec3d> points, IList<int> pointsPerPolygon, double tolerance)
        {
            int[] indexMap;
            List<Vec3d> verts = Vec3d.RemoveDuplicates(points, tolerance, out indexMap);
            IList<int>[] faces = new IList<int>[pointsPerPolygon.Count];

            // get face arrays
            int marker = 0;
            for (int i = 0; i < pointsPerPolygon.Count; i++)
            {
                int n = pointsPerPolygon[i];
                faces[i] = new ArraySegment<int>(indexMap, marker, n);
                marker += n;
            }

            return CreateFromFaceVertexData(verts, faces);
        }

        #endregion


        private HeVertexList _verts;
        private HalfedgeList _hedges;
        private HeFaceList _faces;


        /// <summary>
        /// 
        /// </summary>
        public HeMesh()
        {
            _verts = new HeVertexList(this);
            _hedges = new HalfedgeList(this);
            _faces = new HeFaceList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh(int vertexCapacity, int halfedgeCapacity, int faceCapacity)
        {
            _verts = new HeVertexList(this, vertexCapacity);
            _hedges = new HalfedgeList(this, halfedgeCapacity);
            _faces = new HeFaceList(this, faceCapacity);
        }


        /// <summary>
        /// Creates a deep copy of another HeMesh instance.
        /// </summary>
        public HeMesh(HeMesh other)
        {
            var otherVerts = other._verts;
            var otherHedges = other._hedges;
            var otherFaces = other._faces;
         
            // create element lists
            _verts = new HeVertexList(this, otherVerts.Count);
            _hedges = new HalfedgeList(this, otherHedges.Count);
            _faces = new HeFaceList(this, otherFaces.Count);
            
            // add new elements
            for (int i = 0; i < otherVerts.Count; i++ )
                _verts.Add(new HeVertex(otherVerts[i].Position));

            for (int i = 0; i < otherHedges.Count; i++)
                _hedges.Add(new Halfedge());

            for (int i = 0; i < otherFaces.Count; i++)
                _faces.Add(new HeFace());

            // link vertices to halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                HeVertex v0 = otherVerts[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = _verts[i];
                v1.First = _hedges[v0.First.Index];
            }

            // link faces to halfedges
            for (int i = 0; i < otherFaces.Count; i++)
            {
                HeFace f0 = otherFaces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = _faces[i];
                f1.First = _hedges[f0.First.Index];
            }

            // link halfedges to vertices, faces, and other halfedges
            for (int i = 0; i < otherHedges.Count; i++)
            {
                Halfedge he0 = otherHedges[i];
                Halfedge he1 = _hedges[i];

                he1.Previous = _hedges[he0.Previous.Index];
                he1.Next = _hedges[he0.Next.Index];
                he1.Twin = _hedges[he0.Twin.Index];

                if (he0.Start != null) 
                    he1.Start = _verts[he0.Start.Index];

                if (he0.Face != null) 
                    he1.Face = _faces[he0.Face.Index];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeVertexList Vertices
        {
            get { return _verts; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HalfedgeList Halfedges
        { 
            get { return _hedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeFaceList Faces
        {
            get { return _faces; }
        }


        /// <summary>
        /// Returns the Euler number of the mesh.
        /// </summary>
        public int EulerNumber
        {
            get { return _verts.Count - (_hedges.Count >> 1) + _faces.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("HeMesh (V:{0} HE:{1} F:{2})", _verts.Count, _hedges.Count, _faces.Count);
        }


        /// <summary>
        /// Removes all unused elements in the mesh.
        /// </summary>
        public void Compact()
        {
            _verts.Compact();
            _hedges.Compact();
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
        /// <param name="halfedgeAttributes"></param>
        /// <param name="faceAttributes"></param>
        public void CompactAttributes<V, E, F>(List<V> vertexAttributes, List<E> halfedgeAttributes, List<F> faceAttributes)
        {
            _verts.CompactAttributes(vertexAttributes);
            _hedges.CompactAttributes(halfedgeAttributes);
            _faces.CompactAttributes(faceAttributes);
        }


        /// <summary>
        /// Appends the elements of another mesh to this one.
        /// </summary>
        /// <param name="other"></param>
        public void Append(HeMesh other)
        {
            var otherVerts = other._verts;
            var otherHedges = other._hedges;
            var otherFaces = other._faces;

            // cache current number of elements
            int nv = _verts.Count;
            int ne = _hedges.Count;
            int nf = _faces.Count;

            // append new elements
            for (int i = 0; i < otherVerts.Count; i++)
                _verts.Add(new HeVertex(otherVerts[i].Position));

            for (int i = 0; i < otherHedges.Count; i++)
                _hedges.Add(new Halfedge());

            for (int i = 0; i < otherFaces.Count; i++)
                _faces.Add(new HeFace());

            // link new vertices to new halfedges
            for (int i = 0; i < otherVerts.Count; i++)
            {
                HeVertex v0 = otherVerts[i];
                if (v0.IsUnused) continue;

                HeVertex v1 = _verts[i + nv];
                v1.First = _hedges[v0.First.Index + ne];
            }

            // link new faces to new halfedges
            for (int i = 0; i < otherFaces.Count; i++)
            {
                HeFace f0 = otherFaces[i];
                if (f0.IsUnused) continue;

                HeFace f1 = _faces[i + nf];
                f1.First = _hedges[f0.First.Index + ne];
            }

            // link new edges to new vertices, halfedges, and faces
            for (int i = 0; i < otherHedges.Count; i++)
            {
                Halfedge he0 = other._hedges[i];
                Halfedge he1 = _hedges[i + ne];

                he1.Previous = _hedges[he0.Previous.Index + ne];
                he1.Next = _hedges[he0.Next.Index + ne];
                he1.Twin = _hedges[he0.Twin.Index + ne];

                if (he0.Start != null) 
                    he1.Start = _verts[he0.Start.Index + nv];

                if (he0.Face != null) 
                    he1.Face = _faces[he0.Face.Index + nf];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public List<HeMesh> SplitDisjoint(HeMesh mesh)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Triangulates all non-triangular faces in the mesh.
        /// Quads are triangulated by creating an edge between the first and third vertex.
        /// N-gons are triangulated by adding a new vertex at the face center.
        /// </summary>
        public void Triangulate()
        {
            // TODO add support for different triangulation schemes
            for (int i = 0; i < _faces.Count; i++)
            {
                HeFace f = _faces[i];
                if (f.IsUnused) continue;

                int ne = f.Degree;

                if (ne == 4)
                    _faces.SplitFace(f.First, f.First.Next.Next);
                else if (ne > 4)
                    _faces.Stellate(f);
            }
        }


        /// <summary>
        /// Returns the dual of the mesh.
        /// </summary>
        public HeMesh GetDual()
        {
            // TODO implement more direct dual creation
            // spinning edges for closed meshes
            // or adding edge pairs directly rather than adding face by face
            HeMesh result = new HeMesh();
            var newVerts = result.Vertices;
            var newFaces = result.Faces;

            // add new vertices (1 per face)
            foreach (HeFace f in _faces)
            {
                if (f.IsUnused)
                    newVerts.Add(new Vec3d()); // add dummy vertex for unused elements
                else
                    newVerts.Add(f.GetBarycenter());
            }

            // TODO prientation of dual faces is opposite of original faces
            // circulate in opposite direction?

            // add new faces by circulating old vertices
            var fv = new List<HeVertex>();
            foreach (HeVertex v in _verts)
            {
                //if (v.IsUnused || v.IsBoundary) continue;

                // add dummy face for unused or boundary vertices
                if (v.IsUnused || v.IsBoundary)
                {
                    newFaces.Add(new HeFace()); 
                    continue;
                }

                foreach (HeFace f in v.SurroundingFaces)
                    fv.Add(newVerts[f.Index]);

                newFaces.AddImpl(fv);
                fv.Clear();
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
        /// Returns true if the given vertex belongs to this mesh.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Owns(HeVertex vertex)
        {
            return _verts.Owns(vertex);
        }


        /// <summary>
        /// Returns true if the given halfedge belongs to this mesh.
        /// </summary>
        /// <param name="halfedge"></param>
        /// <returns></returns>
        public bool Owns(Halfedge halfedge)
        {
            return _hedges.Owns(halfedge);
        }


        /// <summary>
        /// Returns true if the given face belongs to this mesh.
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public bool Owns(HeFace face)
        {
            return _faces.Owns(face);
        }


        /// <summary>
        /// Returns the number of holes in the mesh.
        /// </summary>
        /// <returns></returns>
        public int CountHoles(IList<int> edgeIds, int searchId)
        {
            int result = 0;
            int currTag = _hedges.NextTag;

            for (int i = 0; i < _hedges.Count; i++)
            {
                var he = _hedges[i];
                if (he.IsUnused || he.Face != null || he.Tag == currTag) continue;

                result++;

                do
                {
                    he.Tag = currTag;
                    he = he.Next;
                } while (he.Tag != currTag);
            }

            return result;
        }


        /// <summary>
        /// Returns the first halfedge from each hole in the mesh.
        /// </summary>
        /// <returns></returns>
        public List<Halfedge> GetHoles()
        {
            List<Halfedge> result = new List<Halfedge>();
            int currTag = _hedges.NextTag;

            for (int i = 0; i < _hedges.Count; i++)
            {
                var he = _hedges[i];
                if (he.IsUnused || he.Face != null || he.Tag == currTag) continue;

                result.Add(he);

                do
                {
                    he.Tag = currTag;
                    he = he.Next;
                } while (he.Tag != currTag);
            }

            return result;
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static double[] GetIncidenceMatrix()
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public double[] GetLaplacianMatrix()
        {
            int nv = _verts.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = _verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    int j = he.End.Index;
                    result[i * nv + j] = -1.0;
                    wsum++;
                }

                result[i + i * nv] = wsum;
            }

            return result;
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="halfedgeWeights"></param>
        /// <returns></returns>
        public double[] GetLaplacianMatrix(IList<double> halfedgeWeights)
        {
            _hedges.SizeCheck(halfedgeWeights);

            int nv = _verts.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = _verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    int j = he.End.Index;
                    double w = halfedgeWeights[he.Index];
                    result[i * nv + j] = -w;
                    wsum += w;
                }

                result[i * nv + i] = wsum;
            }

            return result;
        }

    }
}

