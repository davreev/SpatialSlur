
/*
 * Notes
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.Meshes;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Matrix
    {
        /// <summary>
        /// Returns non-zero entries of the vertex incidence matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetVertexIncidence<V, E>(HeStructure<V, E> graph)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                foreach(var he in v.OutgoingHalfedges)
                    yield return (i, he.EdgeIndex, 1.0);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the vertex adjacency matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetVertexAdjacency<V, E>(HeStructure<V, E> graph)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;

                foreach (var v1 in v0.ConnectedVertices)
                    yield return (i, v1.Index, 1.0);
            }
        }
        

        /// <summary>
        /// Returns non-zero entries of the vertex Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetVertexLaplacian<V, E>(HeStructure<V, E> graph)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    yield return (i, he.End, 1.0);
                    wsum++;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the vertex Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetVertexLaplacian<V, E>(
            HeStructure<V, E> graph, 
            Func<E, double> getWeight)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    double w = getWeight(he);
                    yield return (i, he.End, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the vertex Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetVertexLaplacian<V, E>(
            HeStructure<V, E> graph, 
            Func<E, double> getWeight, 
            Func<V, double> getMass,
            bool symmetric)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            if (symmetric)
                return GetVertexLaplacianSymmetric(graph, getWeight, getMass);
            else
                return GetVertexLaplacian(graph, getWeight, getMass);
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<(int Row, int Column, double Value)> GetVertexLaplacian<V, E>(
            HeStructure<V, E> graph, 
            Func<E, double> getWeight, 
            Func<V, double> getMass)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                var aInv = 1.0 / getMass(v);
                double wsum = 0.0;

                foreach (var he in v.OutgoingHalfedges)
                {
                    double w = getWeight(he) * aInv;
                    yield return (i, he.End, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<(int Row, int Column, double Value)> GetVertexLaplacianSymmetric<V, E>(
            HeStructure<V, E> graph, 
            Func<E, double> getWeight, 
            Func<V, double> getMass)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            // Impl ref
            // http://reuter.mit.edu/papers/reuter-smi09.pdf

            var verts = graph.Vertices;

            for (int i = 0; i < verts.Count; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;

                var a0 = getMass(v0);
                double wsum = 0.0;

                foreach (var he in v0.OutgoingHalfedges)
                {
                    var v1 = he.End;
                    double w = getWeight(he) / Math.Sqrt(a0 * getMass(v1));
                    yield return (i, v1, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the face incidence matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetFaceIncidence<V, E, F>(HeStructure<V, E, F> mesh)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                foreach (var he in f.Halfedges)
                    yield return (i, he.EdgeIndex, 1.0);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the face adjacency matrix.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<(int Row, int Column, double Value)> GetFaceAdjacency<V, E, F>(HeStructure<V, E, F> mesh)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f0 = faces[i];
                if (f0.IsUnused) continue;

                foreach (var f1 in f0.AdjacentFaces)
                    yield return (i, f1.Index, 1.0);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the face Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetFaceLaplacian<V, E, F>(HeStructure<V, E, F> mesh)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    yield return (i, he.Twin.Face, 1.0);
                    wsum++;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the face Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetFaceLaplacian<V, E, F>(
            HeStructure<V, E, F> mesh, 
            Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    double w = getWeight(he);
                    yield return (i, he.Twin.Face, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// Returns non-zero entries of the face Laplacian matrix.
        /// </summary>
        public static IEnumerable<(int Row, int Column, double Value)> GetFaceLaplacian<V, E, F>(
            HeStructure<V, E, F> mesh,
            Func<E, double> getWeight,
            Func<F, double> getMass,
            bool symmetric)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            if (symmetric)
                return GetFaceLaplacianSymmetric(mesh, getWeight, getMass);
            else
                return GetFaceLaplacian(mesh, getWeight, getMass);
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<(int Row, int Column, double Value)> GetFaceLaplacian<V, E, F>(
            HeStructure<V, E, F> mesh,
            Func<E, double> getWeight,
            Func<F, double> getMass)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                var aInv = 1.0 / getMass(f);
                double wsum = 0.0;

                foreach (var he in f.Halfedges)
                {
                    double w = getWeight(he) * aInv;
                    yield return (i, he.Twin.Face, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<(int Row, int Column, double Value)> GetFaceLaplacianSymmetric<V, E, F>(
            HeStructure<V, E, F> mesh,
            Func<E, double> getWeight,
            Func<F, double> getMass)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // Impl ref
            // http://reuter.mit.edu/papers/reuter-smi09.pdf

            var faces = mesh.Faces;

            for (int i = 0; i < faces.Count; i++)
            {
                var f0 = faces[i];
                if (f0.IsUnused) continue;

                var a0 = getMass(f0);
                double wsum = 0.0;

                foreach (var he in f0.Halfedges)
                {
                    var f1 = he.Twin.Face;
                    double w = getWeight(he) / Math.Sqrt(a0 * getMass(f1));
                    yield return (i, f1, w);
                    wsum += w;
                }

                yield return (i, i, -wsum);
            }
        }
    }
}
