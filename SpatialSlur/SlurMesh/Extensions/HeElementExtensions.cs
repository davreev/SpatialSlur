using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeElementExtensions
    {
        #region HeVertex

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static V NearestMin<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static V NearestMax<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<V> WalkToMin<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<V> WalkToMax<V, E, K>(this V start, Func<V, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeVertex<V, E, F> vertex)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(vertex, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeVertex<V, E, F> vertex, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return vertex.OutgoingHalfedges.Where(he => !he.IsHole).Sum(he => he.GetNormal(getPosition)).Unit;
        }

        #endregion


        #region Halfedge

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static E NearestMin<V, E, K>(this E start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, he => he.ConnectedEdges, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static E NearestMax<V, E, K>(this E start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, he => he.ConnectedEdges, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<E> WalkToMin<V, E, K>(this E start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, he => he.ConnectedEdges, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<E> WalkToMax<V, E, K>(this E start, Func<E, K> getKey)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, he => he.ConnectedEdges, getKey);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this Halfedge<V, E> hedge)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return GetLength(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        /// <returns></returns>
        public static double GetLength<V, E>(this Halfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this Halfedge<V, E> hedge)
            where V : HeVertex<V, E>, IPosition3d
            where E : Halfedge<V, E>
        {
            return GetAngle(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this Halfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E>
            where E : Halfedge<V, E>
        {
            var p = getPosition(hedge.Start);

            return Vec3d.Angle(
                getPosition(hedge.Previous.Start) - p,
                getPosition(hedge.Next.Start) - p
                );
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetCotangent(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Previous.Start);

            return Vec3d.Cotangent(
                getPosition(hedge.Start) - p,
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Start);

            return Vec3d.Cross(
                p - getPosition(hedge.Previous.Start),
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static Orient3d GetFrame<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return Orient3d.CreateFromPoints(
                getPosition(hedge.Start),
                getPosition(hedge.Next.Start),
                getPosition(hedge.Previous.Start)
                );
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this Halfedge<V, E, F> hedge)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetEdgeNormal(hedge, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.Start);
            Vec3d d1 = (getPosition(hedge.Previous.Start) + getPosition(hedge.Next.End)) * 0.5 - p0;

            hedge = hedge.Twin;

            Vec3d d2 = getPosition(hedge.Start) - p0;
            Vec3d d3 = (getPosition(hedge.Previous.Start) + getPosition(hedge.Next.End)) * 0.5 - p0;

            return (Vec3d.Cross(d1, d2) + Vec3d.Cross(d2, d3)).Unit;
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this Halfedge<V, E, F> hedge, Vec3d faceCenter)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetArea(hedge, IPosition3d<V>.Get, faceCenter);
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See V in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Vec3d faceCenter)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.Start);
            return Vec3d.Cross(getPosition(hedge.End) - p0, faceCenter - p0).Length * 0.5;
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this Halfedge<V, E, F> hedge, Func<F, Vec3d> getNormal)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetDihedralAngle(hedge, IPosition3d<V>.Get, getNormal);
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this Halfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var twin = hedge.Twin;
            Vec3d axis = getPosition(twin.Start) - getPosition(hedge.Start);
            return GeometryUtil.GetDihedralAngle(axis.Unit, getNormal(hedge.Face), getNormal(twin.Face));
        }
        
        #endregion


        #region HeFace

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static F NearestMin<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static F NearestMax<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<F> WalkToMin<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static IEnumerable<F> WalkToMax<V, E, F, K>(this F start, Func<F, K> getKey)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetArea(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the area of this face.
        /// Note that this method assumes the face is planar.
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return face.First.GetNormal(getPosition).Length * 0.5;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Length * 0.5;
        }

        
        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetNormal(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return face.First.GetNormal(getPosition).Unit;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Unit;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this HeFace<V, E, F> face, Func<E, Vec3d> getNormal)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return getNormal(face.First).Unit;
            else
                return face.Halfedges.Sum(getNormal).Unit;
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return face.Vertices.Mean(IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return face.Vertices.Mean(getPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this HeFace<V, E, F> face)
        where V : HeVertex<V, E, F>, IPosition3d
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            return GetCircumcenter(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetCircumcenter(
                getPosition(he.Previous.Start),
                getPosition(he.Start),
                getPosition(he.Next.Start)
                );
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this HeFace<V, E, F> face)
        where V : HeVertex<V, E, F>, IPosition3d
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            return GetIncenter(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetIncenter(
                getPosition(he.Previous.Start),
                getPosition(he.Start),
                getPosition(he.Next.Start)
                );
        }


        /// <summary>
        /// Returns the planar deviation of this face
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetPlanarity<V, E, F>(this HeFace<V, E, F> face)
            where V : HeVertex<V, E, F>, IPosition3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            return GetPlanarity(face, IPosition3d<V>.Get);
        }


        /// <summary>
        /// Returns the planar deviation of this face.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="face"></param>
        /// <param name="getPosition"></param>
        /// <returns></returns>
        public static double GetPlanarity<V, E, F>(this HeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var he0 = face.First;
            var he1 = he0;

            var p0 = getPosition(he1.Start);
            he1 = he1.Next;

            var p1 = getPosition(he1.Start);
            he1 = he1.Next;

            var p2 = getPosition(he1.Start);
            he1 = he1.Next;

            // tri case
            if (ReferenceEquals(he1, he0))
                return 0.0;

            var p3 = getPosition(he1.Start);
            he1 = he1.Next;

            // quad case
            if (ReferenceEquals(he1, he0))
                return GeometryUtil.GetQuadPlanarity(p0, p1, p2, p3);

            // general case
            double sum = 0.0;
            int count = 0;

            foreach (var he in he1.Circulate)
            {
                sum += GeometryUtil.GetQuadPlanarity(p0, p1, p2, p3);
                p0 = p1; p1 = p2; p2 = p3;
                p3 = getPosition(he.Start);
            }

            return sum / count;
        }

        #endregion
    }
}
