using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;
            
            var result = new List<HeQuadStrip<V, E, F>>();
            var stack = new Stack<E>();
            int currTag = faces.NextTag;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsRemoved || f.Tag == currTag) continue; // skip if unused or already visited

                stack.Push((flip) ? f.First.NextInFace : f.First);

                foreach (var strip in GetQuadStrips<V, E, F>(stack, currTag))
                    yield return strip;
            }
        }


        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        public static IEnumerable<HeQuadStrip<V, E, F>> GetQuadStrips<V, E, F>(IHeStructure<V, E, F> mesh, F start, bool flip)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var faces = mesh.Faces;

            faces.ContainsCheck(start);
            start.RemovedCheck();

            var stack = new Stack<E>();
            stack.Push((flip) ? start.First.NextInFace : start.First);

            return GetQuadStrips<V, E, F>(stack, faces.NextTag);
        }


        /// <summary>
        /// 
        /// </summary>
        private static IEnumerable<HeQuadStrip<V, E, F>> GetQuadStrips<V, E, F>(Stack<E> stack, int currTag)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
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
                        if (f != null && f.Tag != currTag) stack.Push(he1);
                    }
                }
            }

            IEnumerable<E> AdjacentQuads(E hedge)
            {
                yield return hedge.PrevAtStart.PrevInFace; // left
                yield return hedge.NextInFace.NextAtStart; // right
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
          where V : HeVertex<V, E, F>
          where E : Halfedge<V, E, F>
          where F : HeFace<V, E, F>
        {
            mesh.Halfedges.ContainsCheck(hedge);
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
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
        {
            var he1 = Advance(hedge);
            if (he1 == hedge) return new HeQuadStrip<V, E, F>(he1, he1); // periodic

            var he0 = Advance(hedge.Twin);
            return new HeQuadStrip<V, E, F>(he0.Twin, he1);

            // advances the halfedge to the next boundary or visited face
            E Advance(E he)
            {
                var f = he.Face;

                while (f != null && f.Tag != currTag)
                {
                    f.Tag = currTag;
                    he = he.NextInFace.NextInFace.Twin;
                    f = he.Face;
                }

                return he;
            }
        }


        /*
        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        public static List<List<TE>> GetQuadStrips<TV, TE, TF>(HeMesh<TV, TE, TF> mesh, bool flip)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var faces = mesh.Faces;

            // TODO return as IEnumerable instead
            var result = new List<List<TE>>();
            var stack = new Stack<TE>();
            int currTag = faces.NextTag;

            for (int i = 0; i < faces.Count; i++)
            {
                var f = faces[i];
                if (f.IsRemoved || f.Tag == currTag) continue; // skip if unused or already visited
                
                stack.Push((flip) ? f.First.NextInFace : f.First);
                GetQuadStrips<TV, TE, TF>(stack, currTag, result);
            }

            return result;
        }


        /// <summary>
        /// Assumes quadrilateral faces.
        /// </summary>
        public static List<List<TE>> GetQuadStrips<TV, TE, TF>(IHeStructure<TV, TE, TF> mesh, TF start, bool flip)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            var faces = mesh.Faces;

            // TODO return as IEnumerable<IEnumerable<Halfedge>> instead
            faces.ContainsCheck(start);
            start.RemovedCheck();

            var result = new List<List<TE>>();
            var stack = new Stack<TE>();

            stack.Push((flip) ? start.First.NextInFace : start.First);
            GetQuadStrips<TV, TE, TF>(stack, faces.NextTag, result);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetQuadStrips<TV, TE, TF>(Stack<TE> stack, int currTag, List<List<TE>> result)
            where TV : HeVertex<TV, TE, TF>
            where TE : Halfedge<TV, TE, TF>
            where TF : HeFace<TV, TE, TF>
        {
            while (stack.Count > 0)
            {
                var he0 = stack.Pop();
                var f0 = he0.Face;

                // don't start from boundary halfedges or those with visited faces
                if (f0 == null || f0.Tag == currTag) continue;

                // backtrack to first encountered visited face or boundary
                var he1 = he0;
                var f1 = he1.Face;
                do
                {
                    he1 = he1.Twin.NextInFace.NextInFace;
                    f1 = he1.Face;
                } while (f1 != null && f1.Tag != currTag && f1 != f0);

                // collect halfedges in strip
                var strip = new List<TE>();
                he1 = he1.PrevInFace.PrevInFace.Twin;
                f1 = he1.Face;
                do
                {
                    // add left and right neighbours to stack
                    stack.Push(he1.PrevInFace.Twin.PrevInFace);
                    stack.Push(he1.NextInFace.Twin.NextInFace);

                    // add current halfedge to strip and flag face as visited
                    strip.Add(he1);
                    f1.Tag = currTag;

                    // advance to next halfedge
                    he1 = he1.NextInFace.NextInFace.Twin;
                    f1 = he1.Face;
                } while (f1 != null && f1.Tag != currTag);

                strip.Add(he1); // add last halfedge
                result.Add(strip);
            }
        }
        */
    }
}
