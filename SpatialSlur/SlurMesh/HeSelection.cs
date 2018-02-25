using System.Collections.Generic;
using System.Linq;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Static methods for selecting groups of elements from halfedge structures.
    /// </summary>
    public static class HeSelection
    {
        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        public static IEnumerable<HeQuadStrip<V, E, F>> GetQuadStrips<V, E, F>(HeMesh<V, E, F> mesh, bool flip)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var faces = mesh.Faces;
            
            var result = new List<HeQuadStrip<V, E, F>>();
            var stack = new Stack<E>();
            int currTag = faces.NextTag;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsUnused || f.Tag == currTag || !f.IsDegree(4)) continue; // skip if unused, visited, or non-quads

                stack.Push((flip) ? f.First.Next : f.First);

                foreach (var strip in GetQuadStrips<V, E, F>(stack, currTag))
                    yield return strip;
            }
        }


        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        public static IEnumerable<HeQuadStrip<V, E, F>> GetQuadStrips<V, E, F>(HeStructure<V, E, F> mesh, F start, bool flip)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            if (!start.IsDegree(4))
                return Enumerable.Empty<HeQuadStrip<V, E, F>>();

            var faces = mesh.Faces;

            start.UnusedCheck();
            faces.OwnsCheck(start);

            var stack = new Stack<E>();
            stack.Push((flip) ? start.First.Next : start.First);

            return GetQuadStrips<V, E, F>(stack, faces.NextTag);
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<HeQuadStrip<V, E, F>> GetQuadStrips<V, E, F>(Stack<E> stack, int currTag)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            while (stack.Count > 0)
            {
                var he = stack.Pop();
                if (he.Face.Tag == currTag) continue; // skip if face is already tagged

                var strip = GetQuadStrip<V, E, F>(he, currTag);
                yield return strip;

                // add hedges of untagged adjacent quads to stack
                foreach(var he0 in strip.SkipLast(1))
                {
                    foreach(var he1 in AdjacentQuads(he0))
                    {
                        var f = he1.Face;
                        if (f != null && f.Tag != currTag && f.IsDegree(4)) stack.Push(he1);
                    }
                }
            }

            IEnumerable<E> AdjacentQuads(E hedge)
            {
                yield return hedge.PreviousAtStart.Previous; // left
                yield return hedge.Next.NextAtStart; // right
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public static HeQuadStrip<V, E, F> GetQuadStrip<V, E, F>(HeMesh<V, E, F> mesh, E hedge)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            mesh.Halfedges.OwnsCheck(hedge);
            return GetQuadStrip<V, E, F>(hedge, mesh.Faces.NextTag);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <returns></returns>
        private static HeQuadStrip<V, E, F> GetQuadStrip<V, E, F>(E hedge, int currTag)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var he1 = Advance(hedge);
            if (he1 == hedge) return new HeQuadStrip<V, E, F>(he1, he1); // periodic

            var he0 = Advance(hedge.Twin);
            return new HeQuadStrip<V, E, F>(he0.Twin, he1);

            // advances the halfedge to the next boundary, visited face, or non-quad
            E Advance(E he)
            {
                var f = he.Face;

                while (f != null && f.Tag != currTag && f.IsDegree(4))
                {
                    f.Tag = currTag;
                    he = he.Next.Next.Twin;
                    f = he.Face;
                }

                return he;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetEdgeLoop<V, E, F>(E hedge)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            if (hedge.Face == null)
                return GetEdgeLoopBoundary<V, E, F>(hedge);

            return GetEdgeLoopInterior<V, E, F>(hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private static IEnumerable<E> GetEdgeLoopBoundary<V, E, F>(E hedge)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var he = hedge;

            // march backwards to corner or start
            do
            {
                if (he.IsAtDegree2) break;
                he = he.Previous;
            } while (he != hedge);

            hedge = he;

            // march forward to corner or start
            do
            {
                yield return he;
                he = he.Next;
            } while (!he.IsAtDegree2 && he != hedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private static IEnumerable<E> GetEdgeLoopInterior<V, E, F>(E hedge)
            where V : HeMesh<V, E, F>.Vertex
            where E : HeMesh<V, E, F>.Halfedge
            where F : HeMesh<V, E, F>.Face
        {
            var he = hedge;

            // march backwards to irregular vertex, boundary, or start
            do
            {
                if (he.Twin.Face == null)
                    if (!he.IsAtDegree3) break;
                else
                    if (he.Start.IsBoundary || !he.IsAtDegree(4)) break;

                he = he.Previous.Twin.Previous;
            } while (he != hedge);

            hedge = he;

            // march forward to irregular vertex, boundary, or start
            do
            {
                yield return he;
                he = he.Next.Twin.Next;

                if (he.Twin.Face == null)
                    if (!he.IsAtDegree3) break;
                else
                    if (he.Start.IsBoundary || !he.IsAtDegree(4)) break;

            } while (he != hedge);
        }
    }
}
