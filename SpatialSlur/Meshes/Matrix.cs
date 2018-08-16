
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
        /// Returns the entries of the incidence matrix in row-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexIncidence<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            int nv = verts.Count;
            int ne = graph.Edges.Count;
            Array.Clear(result, 0, nv * ne);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                int offset = i * ne;
                foreach (var he in v.OutgoingHalfedges)
                    result[offset + he.EdgeIndex] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix.
        /// </summary>
        /// <returns></returns>
        public static void GetVertexAdjacency<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;

                int offset = i * nv;
                foreach (var v1 in v0.ConnectedVertices)
                    result[offset + v1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix.
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;
                int offset = i * nv;

                foreach (var he in v.OutgoingHalfedges)
                {
                    result[offset + he.End.Index] = 1.0;
                    wsum++;
                }

                result[offset + i] = -wsum;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix.
        /// </summary>
        public static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<E, double> getWeight, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;
                int offset = i * nv;

                foreach (var he in v.OutgoingHalfedges)
                {
                    double w = getWeight(he);
                    result[offset + he.End.Index] = w;
                    wsum += w;
                }

                result[offset + i] = -wsum;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in row-major order.
        /// </summary>
        private static void GetVertexLaplacian<V, E>(this HeStructure<V, E> graph, Func<E, double> getWeight, Func<V, double> getMass, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            var verts = graph.Vertices;
            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v = verts[i];
                if (v.IsUnused) continue;

                var aInv = 1.0 / getMass(v);
                double wsum = 0.0;
                int offset = i * nv;

                foreach (var he in v.OutgoingHalfedges)
                {
                    double w = getWeight(he) * aInv;
                    result[offset + he.End.Index] = w;
                    wsum += w;
                }

                result[offset + i] = -wsum;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix.
        /// </summary>
        private static void GetVertexLaplacianSymmetric<V, E>(this HeStructure<V, E> graph, Func<E, double> getWeight, Func<V, double> getMass, double[] result)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
        {
            // impl ref
            // http://reuter.mit.edu/papers/reuter-smi09.pdf

            var verts = graph.Vertices;
            int nv = verts.Count;
            Array.Clear(result, 0, nv * nv);

            for (int i = 0; i < nv; i++)
            {
                var v0 = verts[i];
                if (v0.IsUnused) continue;

                var a0 = getMass(v0);
                double wsum = 0.0;
                int offset = i * nv;

                foreach (var he in v0.OutgoingHalfedges)
                {
                    double w = getWeight(he) / Math.Sqrt(a0 * getMass(he.End));
                    result[offset + he.End.Index] = w;
                    wsum += w;
                }

                result[offset + i] = -wsum;
            }
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in row-major order.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceIncidence<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            int nf = faces.Count;
            int ne = mesh.Edges.Count;
            Array.Clear(result, 0, nf * ne);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                int offset = i * ne;
                foreach (var he in f.Halfedges)
                    result[offset + he.EdgeIndex] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the adjacency matrix.
        /// </summary>
        /// <returns></returns>
        public static void GetFaceAdjacency<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f0 = faces[i];
                if (f0.IsUnused) continue;

                var offset = i * nf;

                foreach (var f1 in f0.AdjacentFaces)
                    result[offset + f1.Index] = 1.0;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix.
        /// </summary>
        public static void GetFaceLaplacian<V, E, F>(this HeStructure<V, E, F> mesh, double[] result)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;
                var offset = i * nf;

                foreach (var he in f.Halfedges)
                {
                    result[offset + he.End.Index] = 1.0;
                    wsum++;
                }

                result[offset + i] = -wsum;
            }
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix.
        /// </summary>
        public static void GetFaceLaplacian<V, E, F>(this HeStructure<V, E, F> mesh, Func<E, double> getWeight, double[] result)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var faces = mesh.Faces;
            int nf = faces.Count;
            Array.Clear(result, 0, nf * nf);

            for (int i = 0; i < nf; i++)
            {
                var f = faces[i];
                if (f.IsUnused) continue;

                double wsum = 0.0;
                var offset = i * nf;

                foreach (var he in f.Halfedges)
                {
                    double w = getWeight(he);
                    result[offset + he.End.Index] = w;
                    wsum += w;
                }

                result[offset + i] = -wsum;
            }
        }
    }
}
