
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur;
using SpatialSlur.Meshes.Impl;

using static SpatialSlur.Meshes.Delegates;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// 
    /// </summary>
    public static class VertexExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="start"></param>
        /// <param name="getKey"></param>
        /// <returns></returns>
        public static V NearestMin<V, E, K>(this HeStructure<V, E>.Vertex start, Func<V, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.NearestMin(start.Self, v => v.ConnectedVertices, getKey);
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
        public static V NearestMax<V, E, K>(this HeStructure<V, E>.Vertex start, Func<V, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.NearestMax(start.Self, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMin<V, E, K>(this HeStructure<V, E>.Vertex start, Func<V, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMin(start.Self, v => v.ConnectedVertices, getKey);
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
        public static IEnumerable<V> WalkToMax<V, E, K>(this HeStructure<V, E>.Vertex start, Func<V, K> getKey)
            where V : HeStructure<V, E>.Vertex
            where E : HeStructure<V, E>.Halfedge
            where K : IComparable<K>
        {
            return GraphSearch.WalkToMax(start.Self, v => v.ConnectedVertices, getKey);
        }


        /// <summary>
        /// Returns the unitized sum of area-weighted halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Vertex vertex)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetNormal(vertex, Position3d<V>.Get);
        }


        /// <summary>
        /// Returns the unitized sum of area-weighted halfedge normals around the vertex.
        /// </summary>
        /// <returns></returns>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return vertex.OutgoingHalfedges.Where(he => !he.IsHole).Sum(he => he.GetNormal(getPosition)).Unit;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getPosition"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static Vector3d GetNormal<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getPosition, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return vertex.OutgoingHalfedges.WeightedSum(he => he.GetNormal(getPosition), getWeight).Unit;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static double GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, double> getValue)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = 0.0;
            var n = 0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                sum += getValue(he.End);
                n++;
            }

            return sum - getValue(vertex.Self) * n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static Vector2d GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector2d> getValue)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = Vector2d.Zero;
            var n = 0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                sum += getValue(he.End);
                n++;
            }

            return sum - getValue(vertex.Self) * n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static Vector3d GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getValue)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = Vector3d.Zero;
            var n = 0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                sum += getValue(he.End);
                n++;
            }

            return sum - getValue(vertex.Self) * n;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static double GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, double> getValue, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = 0.0;
            var wsum = 0.0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                var w = getWeight(he);
                sum += getValue(he.End) * w;
                wsum += w;
            }

            return sum - getValue(vertex.Self) * wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static Vector2d GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector2d> getValue, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = Vector2d.Zero;
            var wsum = 0.0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                var w = getWeight(he);
                sum += getValue(he.End) * w;
                wsum += w;
            }

            return sum - getValue(vertex.Self) * wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getValue"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static Vector3d GetLaplacian<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getValue, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = Vector3d.Zero;
            var wsum = 0.0;

            foreach (var he in vertex.OutgoingHalfedges)
            {
                var w = getWeight(he);
                sum += getValue(he.End) * w;
                wsum += w;
            }

            return sum - getValue(vertex.Self) * wsum;
        }


        /// <summary>
        /// Calculates the angle defect at the given vertex.
        /// This is also a measure of discrete Gaussian curvature over the area of the given vertex.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static double GetAngleDefect<V, E, F>(this HeStructure<V, E, F>.Vertex vertex)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetAngleDefect(vertex, Position3d<V>.Get);
        }


        /// <summary>
        /// Calculates the angle defect at the given vertex.
        /// This is also a measure of discrete Gaussian curvature over the area of the given vertex.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static double GetAngleDefect<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // impl ref 
            // http://brickisland.net/DDGFall2017/2017/10/12/assignment-1-coding-investigating-curvature/

            double sum = 0.0;

            foreach (var he in vertex.OutgoingHalfedges)
                sum += he.GetAngle(getPosition);

            return D.TwoPi - sum;
        }


        /// <summary>
        /// Calculates the discrete mean curvature over the area of the given vertex.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static double GetMeanCurvature<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex, IPosition3d
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            return GetMeanCurvature(vertex, Position3d<V>.Get, getWeight);
        }


        /// <summary>
        /// Calculates the discrete mean curvature over the area of the given vertex.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="vertex"></param>
        /// <param name="getPosition"></param>
        /// <param name="getWeight"></param>
        /// <returns></returns>
        public static double GetMeanCurvature<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getPosition, Func<E, double> getWeight)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            // impl ref
            // http://brickisland.net/DDGFall2017/2017/10/12/assignment-1-coding-investigating-curvature/

            return GetLaplacian(vertex, getPosition, getWeight).Length * 0.5;
        }


        /// <summary>
        /// Calculates the circle packing radii for the given vertex.
        /// Assumes the mesh is a circle packing (CP) mesh as defined in http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static double GetCirclePackingRadius<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<V, Vector3d> getPosition)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var p0 = getPosition(vertex.Self);
            var sum = 0.0;
            var n = 0;

            foreach (var he0 in vertex.OutgoingHalfedges)
            {
                if (he0.Face == null) continue;

                var he1 = he0.Next;
                var p1 = getPosition(he1.Start);
                var p2 = getPosition(he1.End);
                
                sum += p0.DistanceTo(p1) - p1.DistanceTo(p2) * 0.5;
                n++;
            }

            return sum / n;
        }


        /// <summary>
        /// Calculates the circle packing radii for the given vertex.
        /// Assumes the mesh is a circle packing (CP) mesh as defined in http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        public static double GetCirclePackingRadius<V, E, F>(this HeStructure<V, E, F>.Vertex vertex, Func<E, double> getLength)
            where V : HeStructure<V, E, F>.Vertex
            where E : HeStructure<V, E, F>.Halfedge
            where F : HeStructure<V, E, F>.Face
        {
            var sum = 0.0;
            var n = 0;

            foreach(var he in vertex.OutgoingHalfedges)
            {
                if (he.Face == null) continue;
                sum += getLength(he) - getLength(he.Next) * 0.5;
                n++;
            }

            return sum / n;
        }
    }
}
