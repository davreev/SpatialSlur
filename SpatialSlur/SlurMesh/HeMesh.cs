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
        public List<HeMesh> SplitDisjoint()
        {
            // TODO alt implementation which add mesh elements directly
            List<HeMesh> result = new List<HeMesh>(); // list of connected components

            Vec2i[] map = new Vec2i[_verts.Count]; // map for each vertex in the parent mesh ( [x, y] => [component index, vertex index])
            Stack<HeVertex> stack = new Stack<HeVertex>();
            int currTag = _verts.NextTag;
            
            // distribute vertices among connected components
            for (int i = 0; i < _verts.Count; i++)
            {
                var v0 = _verts[i];
                if (v0.IsUnused || v0.Tag == currTag) continue; // skip if unused or already visited

                // create new connected component
                HeMesh comp = new HeMesh();
                int compIndex = result.Count;
                var compVerts = comp.Vertices;
          
                // add first vert to stack and flag as visited
                stack.Push(v0);
                v0.Tag = currTag;
               
                while (stack.Count > 0)
                {
                    v0 = stack.Pop();
                    map[v0.Index] = new Vec2i(compIndex, compVerts.Count); // create map for v0
                    compVerts.Add(v0.Position); // add v0 to current component

                    foreach(HeVertex v1 in v0.ConnectedVertices)
                    {
                        if(v1.Tag != currTag)
                        {
                            v1.Tag = currTag;
                            stack.Push(v1);
                        }
                    }
                }

                // add component to result
                result.Add(comp);
            }

            // add faces within each component
            List<int> fv = new List<int>();
            for (int i = 0; i < _faces.Count; i++)
            {
                var f = _faces[i];
                if (f.IsUnused) continue;

                // gather face vertices
                var he = f.First;
                do
                {
                    fv.Add(map[he.Start.Index].y);
                    he = he.Next;
                } while (he != f.First);

                // add face to connected component
                int compIndex = map[he.Start.Index].x;
                result[compIndex].Faces.Add(fv);

                fv.Clear();
            }

            return result;
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

            // TODO orientation of dual faces is opposite of original faces
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


        /// <summary>
        /// Returns the dual of the mesh.
        /// </summary>
        public HeMesh GetDual2()
        {
            HeMesh result = new HeMesh();
            var newVerts = result._verts;
            var newHedges = result._hedges;
            var newFaces = result._faces;

            // add new faces (1 per vertex in original mesh)
            foreach (HeVertex v in _verts)
                newFaces.Add(new HeFace());

            // add new vertices (1 per face in original mesh)
            foreach (HeFace f in _faces)
            {
                if (f.IsUnused)
                    newVerts.Add(new HeVertex()); // add dummy vertex for unused faces
                else
                    newVerts.Add(f.GetBarycenter());
            }
        
            // add new halfedges and link them to faces and verts
            for (int i = 0; i < _hedges.Count; i += 2)
            {
                var he0 = _hedges[i];
                var he1 = _hedges[i + 1];

                var f0 = he0.Face;
                var f1 = he1.Face;

                var v0 = he0.Start;
                var v1 = he1.Start;

                // skip boundary edges and non-manifold dual edges
                if (f0 == null || f1 == null || (v0.IsBoundary && v1.IsBoundary))
                {
                    newHedges.AddPair(null, null); // add placeholder edge
                    continue;
                }

                var he = newHedges.AddPair(newVerts[f1.Index], newVerts[f0.Index]);

                if (!v0.IsBoundary)
                    he.Face = newFaces[v0.Index];

                if (!v1.IsBoundary)
                    he.Twin.Face = newFaces[v1.Index];
            }
            
            // link halfedges to eachother
            for (int i = 0; i < _hedges.Count; i++)
            {
                // TODO
            }

            return result;
        }


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

