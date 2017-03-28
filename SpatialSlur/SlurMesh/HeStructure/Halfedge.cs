using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    [Serializable]
    public abstract class Halfedge<E, V> : HeElement
        where E : Halfedge<E, V>
        where V : HeVertex<E, V>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        /// <returns></returns>
        internal static bool AreConsecutive(E he0, E he1)
        {
            return (he0.Next == he1 || he1.Next == he0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal static void MakeConsecutive(E he0, E he1)
        {
            he0.Next = he1;
            he1.Previous = he0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="he0"></param>
        /// <param name="he1"></param>
        internal static void MakeTwins(E he0, E he1)
        {
            he0.Twin = he1;
            he1.Twin = he0;
        }

        #endregion


        private E _prev;
        private E _next;
        private E _twin;
        private V _start;


        /// <summary>
        /// 
        /// </summary>
        internal abstract E Self { get; }


        /// <summary>
        /// 
        /// </summary>
        public E Next
        {
            get { return _next; }
            internal set { _next = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public E Previous
        {
            get { return _prev; }
            internal set { _prev = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public E Twin
        {
            get { return _twin; }
            internal set { _twin = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public V Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public V End
        {
            get { return _twin._start; }
        }


        /// <summary>
        /// 
        /// </summary>
        public sealed override bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// Returns true if the halfedge is the first from its start vertex.
        /// </summary>
        public bool IsFirstAtStart
        {
            get { return ReferenceEquals(this, _start.First); }
        }


        /// <summary>
        /// Circulates the start vertex starting from this halfedge.
        /// </summary>
        public abstract IEnumerable<E> CirculateStart { get; }


        /// <summary>
        /// Returns a halfedge from each pair connected to this one.
        /// </summary>
        internal IEnumerable<E> ConnectedPairs
        {
            get
            {
                yield return _prev;
                yield return _next;
                yield return _twin._prev;
                yield return _twin._next;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal sealed override void MakeUnused()
        {
            _start = null;
            _twin._start = null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract int CountEdgesAtStart();


        #region Attributes

        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <returns></returns>
        public double GetDelta(IReadOnlyList<double> vertexValues)
        {
            return vertexValues[End.Index] - vertexValues[Start.Index];
        }


        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <returns></returns>
        public Vec2d GetDelta(IReadOnlyList<Vec2d> vertexValues)
        {
            return vertexValues[End.Index] - vertexValues[Start.Index];
        }


        /// <summary>
        /// Returns the difference in value between the end vertex and the start vertex.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <returns></returns>
        public Vec3d GetDelta(IReadOnlyList<Vec3d> vertexValues)
        {
            return vertexValues[End.Index] - vertexValues[Start.Index];
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public double Lerp(IReadOnlyList<double> vertexValues, double t)
        {
            return SlurMath.Lerp(vertexValues[Start.Index], vertexValues[End.Index], t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec2d Lerp(IReadOnlyList<Vec2d> vertexValues, double t)
        {
            return vertexValues[Start.Index].LerpTo(vertexValues[End.Index], t);
        }


        /// <summary>
        /// Returns a linearly interpolated value at the given parameter along the halfedge.
        /// </summary>
        /// <param name="vertexValues"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec3d Lerp(IReadOnlyList<Vec3d> vertexValues, double t)
        {
            return vertexValues[Start.Index].LerpTo(vertexValues[End.Index], t);
        }

        #endregion


        #region Geometric Attributes

        /// <summary>
        /// Returns the length of the halfedge.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public double GetLength(IReadOnlyList<Vec2d> vertexPositions)
        {
            return vertexPositions[Start.Index].DistanceTo(vertexPositions[End.Index]);
        }


        /// <summary>
        /// Returns the length of the halfedge.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <returns></returns>
        public double GetLength(IReadOnlyList<Vec3d> vertexPositions)
        {
            return vertexPositions[Start.Index].DistanceTo(vertexPositions[End.Index]);
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public virtual double GetAngle(IReadOnlyList<Vec2d> vertexPositions)
        {
            var p = vertexPositions[Start.Index];

            return Vec2d.Angle(
                vertexPositions[End.Index] - p,
                vertexPositions[Previous.End.Index] - p
                );
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public virtual double GetAngle(IReadOnlyList<Vec3d> vertexPositions)
        {
            var p = vertexPositions[Start.Index];

            return Vec3d.Angle(
                vertexPositions[End.Index] - p,
                vertexPositions[Previous.End.Index] - p
                );
        }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="F"></typeparam>
    [Serializable]
    public abstract class Halfedge<E, V, F> : Halfedge<E, V>
        where E : Halfedge<E, V, F>
        where V : HeVertex<E, V, F>
        where F : HeFace<E, V, F>
    {
        private F _face;


        /// <summary>
        /// 
        /// </summary>
        internal Halfedge() { }


        /// <summary>
        /// 
        /// </summary>
        public F Face
        {
            get { return _face; }
            internal set { _face = value; }
        }


        /// <summary>
        /// Returns true if the halfedge or its twin has no adjacent face.
        /// </summary>
        public bool IsBoundary
        {
            get { return _face == null || Twin._face == null; }
        }


        /// <summary>
        /// Returns true if the halfedge is the first in its face.
        /// </summary>
        public bool IsFirstInFace
        {
            get { return ReferenceEquals(this, _face.First); }
        }


        /// <summary>
        /// Returns true if the halfedge has a null face reference.
        /// </summary>
        public bool IsInHole
        {
            get { return _face == null; }
        }


        /// <summary>
        /// Circulates the face starting from this halfedge.
        /// </summary>
        public IEnumerable<E> CirculateFace
        {
            get
            {
                var he0 = Self;
                var he1 = he0;

                do
                {
                    yield return he1;
                    he1 = he1.Next;
                } while (he1 != he0);
            }
        }


        /// <summary>
        /// Returns a halfedge from each neighbouring quad.
        /// Assumes this halfedge is in a quadrilateral face.
        /// </summary>
        internal IEnumerable<E> AdjacentQuads
        {
            get
            {
                yield return Twin.Next.Next; // down
                yield return Next.Next.Twin; // up
                yield return Previous.Twin.Previous; // left
                yield return Next.Twin.Next; // right
            }
        }
    

        /// <summary>
        /// Returns the first faceless halfedge encountered when circulating around the start vertex.
        /// Returns null if no such halfedge is found.
        /// </summary>
        /// <returns></returns>
        internal E NextBoundaryAtStart
        {
            get
            {
                var he0 = Self;
                var he1 = Twin.Next;

                do
                {
                    if (he1.Face == null) return he1;
                    he1 = he1.Twin.Next;
                } while (he1 != he0);

                return null;
            }
        }


        /// <summary>
        /// Returns the first boundary halfedge encountered when circulating forwards around the face.
        /// Returns null if no such halfedge is found.
        /// </summary>
        /// <returns></returns>
        public E NextBoundaryInFace
        {
            get
            {
                var he0 = Self;
                var he1 = Next;

                do
                {
                    if (he1.Twin.Face == null) return he1;
                    he1 = he1.Next;
                } while (he1 != he0);

                return null;
            }
        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountEdgesInFace()
        {
            var he0 = Self;
            var he1 = he0;
            int count = 0;

            do
            {
                count++;
                he1 = he1.Next;
            } while (he1 != he0);

            return count;
        }


        #region Geometric Attributes

        /// <summary>
        /// Calculates the cotangent of the angle opposite this halfedge.
        /// Assumes the halfedge is in a triangular face.
        /// http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf
        /// </summary>
        /// <returns></returns>
        public double GetCotangent(IReadOnlyList<Vec3d> vertexPositions)
        {
            Vec3d p = vertexPositions[Previous.Start.Index];

            return Vec3d.Cotangent(
                vertexPositions[Start.Index] - p,
                vertexPositions[End.Index] - p
                );
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public override double GetAngle(IReadOnlyList<Vec2d> vertexPositions)
        {
            var p = vertexPositions[Start.Index];

            return Vec2d.Angle(
                p - vertexPositions[Previous.Start.Index],
                vertexPositions[End.Index] - p
                );
        }


        /// <summary>
        /// Returns the angle between this halfedge and its previous.
        /// Result is between 0 and Pi.
        /// </summary>
        /// <returns></returns>
        public override double GetAngle(IReadOnlyList<Vec3d> vertexPositions)
        {
            var p = vertexPositions[Start.Index];

            return Vec3d.Angle(
                p - vertexPositions[Previous.Start.Index],
                vertexPositions[End.Index] - p
                );
        }


        /// <summary>
        /// Calculates the halfedge normal as the cross product of the previous halfedge and this one.
        /// </summary>
        /// <returns></returns>
        public Vec3d GetNormal(IReadOnlyList<Vec3d> vertexPositions)
        {
            Vec3d p = vertexPositions[Start.Index];

            // CCW outward facing normals (for convex faces)
            return Vec3d.Cross(
                p - vertexPositions[Previous.Start.Index],
                vertexPositions[End.Index] - p
                );
        }


        /// <summary>
        /// Returns the area of the quadrilateral formed between this halfedge, its previous, and a given point.
        /// See W in http://www.cs.columbia.edu/~keenan/Projects/Other/TriangleAreasCheatSheet.pdf.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="faceCenter"></param>
        /// <returns></returns>
        public double GetArea(IReadOnlyList<Vec3d> vertexPositions, Vec3d faceCenter)
        {
            Vec3d p0 = vertexPositions[Previous.Start.Index];
            Vec3d p1 = vertexPositions[Start.Index];
            Vec3d p2 = vertexPositions[End.Index];

            Vec3d v0 = (p2 - p1 + p1 - p0) * 0.5;
            return Vec3d.Cross(v0, faceCenter - p1).Length * 0.5; // area of projected planar quad
        }


        /// <summary>
        /// Calcuated as the exterior between adjacent faces.
        /// Result is in range [0 - 2Pi].
        /// Assumes the given face normals are unitized.
        /// </summary>
        /// <param name="vertexPositions"></param>
        /// <param name="faceNormals"></param>
        /// <returns></returns>
        public double GetDihedralAngle(IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceNormals)
        {
            Vec3d tangent = vertexPositions[End.Index] - vertexPositions[Start.Index];
            tangent.Unitize();

            Vec3d x = faceNormals[Face.Index];
            Vec3d y = Vec3d.Cross(x, tangent);

            Vec3d d = faceNormals[Twin.Face.Index];
            double t = Math.Atan2(d * y, d * x);

            t = (t < 0.0) ? t + SlurMath.TwoPI : t; // shift discontinuity to 0
            return SlurMath.Mod(t + Math.PI, SlurMath.TwoPI); // add angle bw normals and faces
        }

        #endregion
    }
}
