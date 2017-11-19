using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class IHeElementExtensions
    {
        #region IHeVertex

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
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
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
            where V : HeElement<V, E>, IHeVertex<V, E>
            where E : Halfedge<V, E>, IHalfedge<V, E>
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
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
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
            where V : HeElement<V, E>, IHeVertex<V, E>
            where E : Halfedge<V, E>, IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeVertex<V, E, F> vertex)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetNormal(vertex, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeVertex<V, E, F> vertex, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return vertex.OutgoingHalfedges.Where(he => !he.IsHole).Sum(he => he.GetNormal(getPosition)).Direction;
        }

        #endregion


        #region IHalfedge

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
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMin(start, he => he.ConnectedPairs, getKey);
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
            where V : HeElement<V, E>, IHeVertex<V, E>
            where E : Halfedge<V, E>, IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.NearestMax(start, he => he.ConnectedPairs, getKey);
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
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMin(start, he => he.ConnectedPairs, getKey);
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
            where V : HeElement<V, E>, IHeVertex<V, E>
            where E : Halfedge<V, E>, IHalfedge<V, E>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, he => he.ConnectedPairs, getKey);
        }


        /// <summary>
        /// Returns the difference between the end vertex and the start vertex.
        /// </summary>
        public static double GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, double> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns the difference between the end vertex and the start vertex.
        /// </summary>
        public static Vec2d GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns the difference between the end vertex and the start vertex.
        /// </summary>
        public static Vec3d GetDelta<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getValue)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.End) - getValue(hedge.Start);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static double Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, double> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return SlurMath.Lerp(getValue(hedge.Start), getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static Vec2d Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec2d> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.Start).LerpTo(getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        public static Vec3d Lerp<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getValue, double t)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getValue(hedge.Start).LerpTo(getValue(hedge.End), t);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        public static double GetLength<V, E>(this IHalfedge<V, E> hedge)
            where V : IHeVertex<V, E>, IVertex3d
            where E : IHalfedge<V, E>
        {
            return GetLength(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the Euclidean length of the halfedge.
        /// </summary>
        /// <returns></returns>
        public static double GetLength<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return getPosition(hedge.Start).DistanceTo(getPosition(hedge.End));
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this IHalfedge<V, E> hedge)
            where V : IHeVertex<V, E>, IVertex3d
            where E : IHalfedge<V, E>
        {
            return GetAngle(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E>(this IHalfedge<V, E> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            var p = getPosition(hedge.Start);

            return Vec3d.Angle(
                getPosition(hedge.PreviousAtStart.End) - p,
                getPosition(hedge.End) - p);
        }


        /// <summary>
        /// Returns the angle between this halfedge and the previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E, F>(this IHalfedge<V, E, F> hedge)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetAngle(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous at its start vertex.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public static double GetAngle<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var p = getPosition(hedge.Start);

            return Vec3d.Angle(
                getPosition(hedge.PreviousInFace.Start) - p,
                getPosition(hedge.NextInFace.Start) - p
                );
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this IHalfedge<V, E, F> hedge)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetCotangent(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        public static double GetCotangent<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.PreviousInFace.Start);

            return Vec3d.Cotangent(
                getPosition(hedge.Start) - p,
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this IHalfedge<V, E, F> hedge)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetNormal(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        public static Vec3d GetNormal<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p = getPosition(hedge.Start);

            return Vec3d.Cross(
                p - getPosition(hedge.PreviousInFace.Start),
                getPosition(hedge.End) - p
                );
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this IHalfedge<V, E, F> hedge)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetEdgeNormal(hedge, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Calculates the unit length edge normal.
        /// </summary>
        public static Vec3d GetEdgeNormal<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.Start);
            Vec3d d1 = (getPosition(hedge.PreviousInFace.Start) + getPosition(hedge.NextInFace.End)) * 0.5 - p0;

            hedge = hedge.Twin;

            Vec3d d2 = getPosition(hedge.Start) - p0;
            Vec3d d3 = (getPosition(hedge.PreviousInFace.Start) + getPosition(hedge.NextInFace.End)) * 0.5 - p0;

            return (Vec3d.Cross(d1, d2) + Vec3d.Cross(d2, d3)).Direction;
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this IHalfedge<V, E, F> hedge, Vec3d faceCenter)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetArea(hedge, IVertex3dStatic<V>.GetPosition, faceCenter);
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and the given face center.
        /// See W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        public static double GetArea<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Vec3d faceCenter)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            Vec3d p0 = getPosition(hedge.PreviousInFace.Start);
            Vec3d p1 = getPosition(hedge.Start);
            Vec3d p2 = getPosition(hedge.End);

            Vec3d v0 = (p2 - p1 + p1 - p0) * 0.5;
            return Vec3d.Cross(v0, faceCenter - p1).Length * 0.5; // area of projected planar quad
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this IHalfedge<V, E, F> hedge, Func<F, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetDihedralAngle(hedge, IVertex3dStatic<V>.GetPosition, getNormal);
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>>
        public static double GetDihedralAngle<V, E, F>(this IHalfedge<V, E, F> hedge, Func<V, Vec3d> getPosition, Func<F, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            // TODO 
            // refactor as per http://brickisland.net/DDGFall2017/2017/10/12/assignment-1-coding-investigating-curvature/

            Vec3d tangent = getPosition(hedge.End) - getPosition(hedge.Start);
            tangent.Unitize();

            Vec3d x = getNormal(hedge.Face);
            Vec3d y = Vec3d.Cross(x, tangent);

            Vec3d d = getNormal(hedge.Twin.Face);
            double t = Math.Atan2(Vec3d.Dot(d, y), Vec3d.Dot(d, x));

            t = (t < 0.0) ? t + SlurMath.TwoPI : t; // shift discontinuity to 0
            return SlurMath.Mod(t + Math.PI, SlurMath.TwoPI); // add angle bw normals and faces
        }

        #endregion


        #region IHeFace

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
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
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
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
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
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
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
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
            where K : IComparable<K>
        {
            return MeshUtil.WalkToMax(start, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeFace<V, E, F> face)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetNormal(face, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return face.First.GetNormal(getPosition).Direction;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Direction;
        }


        /// <summary>
        /// Returns the unitized sum of halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetNormal<V, E, F>(this IHeFace<V, E, F> face, Func<E, Vec3d> getNormal)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            if (face.IsDegree(3))
                return getNormal(face.First).Direction;
            else
                return face.Halfedges.Sum(getNormal).Direction;
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this IHeFace<V, E, F> face)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return face.Vertices.Mean(IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetBarycenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return face.Vertices.Mean(getPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this IHeFace<V, E, F> face)
        where V : IHeVertex<V, E, F>, IVertex3d
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
        {
            return GetCircumcenter(face, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetCircumcenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : IHeVertex<V, E, F>
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetCircumcenter(
                getPosition(he.PreviousInFace.Start),
                getPosition(he.Start),
                getPosition(he.NextInFace.Start)
                );
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this IHeFace<V, E, F> face)
        where V : IHeVertex<V, E, F>, IVertex3d
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
        {
            return GetIncenter(face, IVertex3dStatic<V>.GetPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vec3d GetIncenter<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
        where V : IHeVertex<V, E, F>
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
        {
            var he = face.First;
            return GeometryUtil.GetIncenter(
                getPosition(he.PreviousInFace.Start),
                getPosition(he.Start),
                getPosition(he.NextInFace.Start)
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
        public static double GetPlanarity<V, E, F>(this IHeFace<V, E, F> face)
            where V : IHeVertex<V, E, F>, IVertex3d
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return GetPlanarity(face, IVertex3dStatic<V>.GetPosition);
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
        public static double GetPlanarity<V, E, F>(this IHeFace<V, E, F> face, Func<V, Vec3d> getPosition)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            var he0 = face.First;
            var he1 = he0;

            var p0 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            var p1 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            var p2 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            // tri case
            if (ReferenceEquals(he1, he0))
                return 0.0;

            var p3 = getPosition(he1.Start);
            he1 = he1.NextInFace;

            // quad case
            if (ReferenceEquals(he1, he0))
                return GeometryUtil.GetQuadPlanarity(p0, p1, p2, p3);

            // general case
            double sum = 0.0;
            int count = 0;

            foreach (var he in he1.CirculateFace)
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
