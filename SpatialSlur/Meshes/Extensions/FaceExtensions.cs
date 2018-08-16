
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class FaceExtensions
    {
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
        public static F NearestMin<V, E, F, K>(this HeStructure<V, E, F>.Face start, Func<F, K> getKey)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            return GraphSearch.NearestMin(start.Self, f => f.AdjacentFaces, getKey);
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
        public static F NearestMax<V, E, F, K>(this HeStructure<V, E, F>.Face start, Func<F, K> getKey)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            return GraphSearch.NearestMax(start.Self, f => f.AdjacentFaces, getKey);
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
        public static IEnumerable<F> WalkToMin<V, E, F, K>(this HeStructure<V, E, F>.Face start, Func<F, K> getKey)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMin(start.Self, f => f.AdjacentFaces, getKey);
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
        public static IEnumerable<F> WalkToMax<V, E, F, K>(this HeStructure<V, E, F>.Face start, Func<F, K> getKey)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMax(start.Self, f => f.AdjacentFaces, getKey);
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetArea(face, Delegates.Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the area of this face.
        /// Note that this method assumes the face is planar.
        /// </summary>
        /// <returns></returns>
        public static double GetArea<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
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
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetNormal(face, Delegates.Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of area-weighted halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            if (face.IsDegree3)
                return face.First.GetNormal(getPosition).Unit;
            else
                return face.Halfedges.Sum(he => he.GetNormal(getPosition)).Unit;
        }


        /// <summary>
        /// Returns the unitized sum of custom-weighted halfedge normals in the face.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Face face, Func<E, Vector3d> getNormal)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            if (face.IsDegree3)
                return getNormal(face.First).Unit;
            else
                return face.Halfedges.Sum(getNormal).Unit;
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetBarycenter<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return face.Vertices.Mean(Delegates.Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the average position of vertices in the face.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetBarycenter<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return face.Vertices.Mean(getPosition);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetCircumcenter<V, E, F>(this HeStructure<V, E, F>.Face face)
        where V : HeStructure<V, E, F>.Vertex, IPosition3d
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
        {
            return GetCircumcenter(face, Delegates.Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetCircumcenter<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
        where V : HeStructure<V, E, F>.Vertex
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
        {
            var he = face.First;
            return Geometry.GetCircumcenter(
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
        public static Vector3d GetIncenter<V, E, F>(this HeStructure<V, E, F>.Face face)
        where V : HeStructure<V, E, F>.Vertex, IPosition3d
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
        {
            return GetIncenter(face, Delegates.Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the circumcenter of the face.
        /// Assumes face is triangular.
        /// http://mathworld.wolfram.com/Incenter.html
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetIncenter<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
        where V : HeStructure<V, E, F>.Vertex
        where E : HeStructure<V, E, F>.Halfedge
        where F : HeStructure<V, E, F>.Face
        {
            var he = face.First;
            return Geometry.GetIncenter(
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
        /// <returns></returns>
        public static double GetPlanarity<V, E, F>(this HeStructure<V, E, F>.Face face)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetPlanarity(face, Delegates.Position3d<V>.Get);
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
        public static double GetPlanarity<V, E, F>(this HeStructure<V, E, F>.Face face, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
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
                return Geometry.GetPlanarity(p0, p1, p2, p3);

            // general case
            double sum = 0.0;
            int count = 0;

            foreach (var he in he1.Circulate)
            {
                sum += Geometry.GetPlanarity(p0, p1, p2, p3);
                p0 = p1; p1 = p2; p2 = p3;
                p3 = getPosition(he.Start);
            }

            return sum / count;
        }
    }
}
