using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class HeFace<E, V, F> : HeElement
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        E _first;


        /// <summary>
        /// 
        /// </summary>
        internal abstract F Self { get; }


        /// <summary>
        /// 
        /// </summary>
        public E First
        {
            get { return _first; }
            internal set { _first = value; }
        }


        /// <summary>
        /// Returns true if this node has any incident used edges.
        /// </summary>
        public sealed override bool IsUnused
        {
            get { return _first == null; }
        }


        /// <summary>
        /// Returns true if the face has at least 1 boundary edge.
        /// </summary>
        public bool IsBoundary
        {
            get
            {
                var he0 = _first;
                var he1 = he0;

                do
                {
                    if (he1.Twin.Face == null) return true;
                    he1 = he1.Next;
                } while (he1 != he0);

                return false;
            }
        }


        /// <summary>
        /// Returns the number of edges in this face.
        /// </summary>
        public int Degree
        {
            get { return _first.CountEdgesInFace(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<E> Halfedges
        {
            get
            {
                var he0 = _first;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<V> Vertices
        {
            get
            {
                var he0 = _first;
                var he1 = he0;

                do
                {
                    yield return he1.Start;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Iterates over adjacent faces.
        /// Note that null faces are skipped.
        /// Also if mutliple edges are shared with an adjacent face, that face will be returned multiple times.
        /// </summary>
        public IEnumerable<F> AdjacentFaces
        {
            get
            {
                var he0 = _first;
                var he1 = he0;

                do
                {
                    F f = he1.Twin.Face;
                    if (f != null) yield return f;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal sealed override void MakeUnused()
        {
            _first = null;
        }


        /// <summary>
        /// Sets the first halfedge in the face to the first boundary halfedge encountered during circulation.
        /// Returns true if a boundary halfedge was found.
        /// </summary>
        /// <returns></returns>
        public bool SetFirstToBoundary()
        {
            var he0 = _first;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == null)
                {
                    _first = he1;
                    return true;
                }

                he1 = he1.Next;
            } while (he1 != he0);

            return false;
        }


        /// <summary>
        /// Finds the edge between this face and another.
        /// Returns the halfedge adjacent to this face or null if no edge exists.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public E FindEdgeBetween(F other)
        {
            var he0 = _first;
            var he1 = he0;

            do
            {
                if (he1.Twin.Face == other) return he1;
                he1 = he1.Next;
            } while (he1 != he0);

            return null;
        }


        /// <summary>
        /// Returns the number of boundary edges in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryEdges()
        {
            var he0 = _first;
            var he1 = he0;
            int count = 0;
       
            do
            {
                if (he1.Twin.Face == null) count++;
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
        }


        /// <summary>
        /// Returns the number of boundary vertices in the face.
        /// </summary>
        /// <returns></returns>
        public int CountBoundaryVertices()
        {
            var he0 = _first;
            var he1 = he0;
            int count = 0;

            do
            {
                if (he1.Start.IsBoundary) count++;
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
        }


        #region Geometric Attributes

        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetBarycenter(IReadOnlyList<Vec3d> vertexPositions)
        {
            return Vertices.Mean(vertexPositions);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal(IReadOnlyList<Vec3d> vertexPositions)
        {
            Vec3d result;
            var he0 = First;

            if (he0 == he0.Next.Next.Next)
            {
                // simplified tri case
                result = he0.GetNormal(vertexPositions);
            }
            else
            {
                // general n-gon case
                result = new Vec3d();
                foreach (var he in Halfedges)
                    result += he.GetNormal(vertexPositions);
            }

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal2(IReadOnlyList<Vec3d> halfedgeNormals)
        {
            Vec3d result;
            var he0 = First;

            if (he0 == he0.Next.Next.Next)
                result = halfedgeNormals[he0.Index]; // simplified tri case
            else
                result = Halfedges.Sum(halfedgeNormals); // general n-gon case

            result.Unitize();
            return result;
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetCircumcenter(IReadOnlyList<Vec3d> vertexPositions)
        {
            var he = First;
            Vec3d p0 = vertexPositions[he.Start.Index];

            he = he.Next;
            Vec3d p1 = vertexPositions[he.Start.Index];
            Vec3d p2 = vertexPositions[he.End.Index];

            return p1 + GeometryUtil.GetCurvatureVector(p0 - p1, p2 - p1);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public Vec3d GetIncenter(IReadOnlyList<Vec3d> vertexPositions)
        {
            var he = First;
            Vec3d p0 = vertexPositions[he.Previous.Start.Index];
            Vec3d p1 = vertexPositions[he.Start.Index];
            Vec3d p2 = vertexPositions[he.End.Index];

            double d01 = p0.DistanceTo(p1);
            double d12 = p1.DistanceTo(p2);
            double d20 = p2.DistanceTo(p0);
            double pInv = 1.0 / (d01 + d12 + d20); // inverse perimeter

            return p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public Vec3d GetIncenter(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> edgeLengths)
        {
            var he0 = First;
            var he1 = he0.Next;
            var he2 = he1.Next;

            Vec3d p0 = vertexPositions[he0.Start.Index];
            Vec3d p1 = vertexPositions[he1.Start.Index];
            Vec3d p2 = vertexPositions[he2.Start.Index];

            double d01 = edgeLengths[he0.Index >> 1];
            double d12 = edgeLengths[he1.Index >> 1];
            double d20 = edgeLengths[he2.Index >> 1];
            double pInv = 1.0 / (d01 + d12 + d20); // inverse perimeter

            return p0 * (d12 * pInv) + p1 * (d20 * pInv) + p2 * (d01 * pInv);
        }

        #endregion
    }
}
