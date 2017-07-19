using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;

using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class HeMeshUnroller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F first)
         where V : HeVertex<V, E, F>, IVertex3d
         where E : Halfedge<V, E, F>
         where F : HeFace<V, E, F>
        {
            Unroll(mesh, first, v => v.Position, (v, p) => v.Position = p, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="setHandle"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F first, Action<E, int> setHandle)
            where V : HeVertex<V, E, F>, IVertex3d
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Unroll(mesh, first, v => v.Position, (v, p) => v.Position = p, setHandle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="getPosition"></param>
        /// <param name="setPosition"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F first, Func<V, Vec3d> getPosition, Action<V, Vec3d> setPosition)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            Unroll(mesh, first, getPosition, setPosition, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="getPosition"></param>
        /// <param name="setPosition"></param>
        public static void Unroll<V, E, F>(HeMesh<V, E, F> mesh, F first, Func<V, Vec3d> getPosition, Action<V, Vec3d> setPosition, Action<E, int> setHandle)
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            var unroller = new HeMeshUnroller<V, E, F>(mesh, first, getPosition, setPosition, setHandle);
            unroller.Unroll();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    internal class HeMeshUnroller<V, E, F>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        private HeMesh<V, E, F> _mesh;
        private F _first;

        private Func<V, Vec3d> _getPosition;
        private Action<V, Vec3d> _setPosition;


        /// <summary>
        /// 
        /// </summary>
        public HeMeshUnroller(HeMesh<V, E, F> mesh, F first, Func<V, Vec3d> getPosition, Action<V, Vec3d> setPosition, Action<E, int> setHandle)
        {
            mesh.Faces.ContainsCheck(first);
            first.RemovedCheck();

            _mesh = mesh;
            _first = first;

            _getPosition = getPosition;
            _setPosition = setPosition;
      
            DetachFaceCycles(setHandle);
        }


        /// <summary>
        /// 
        /// </summary>
        public void DetachFaceCycles(Action<E, int> setHandle)
        {
            var currTag = _mesh.Halfedges.NextTag;

            // tag traversed edges during BFS
            foreach (var he in _mesh.GetFacesBreadthFirst(_first.Yield()))
                he.Older.Tag = currTag;

            var edges = _mesh.Edges;
            var ne = edges.Count;

            // detach all untagged edges
            for (int i = 0; i < ne; i++)
            {
                var he0 = edges[i];

                if (he0.IsRemoved || he0.IsBoundary || he0.Tag == currTag)
                {
                    setHandle(he0, -1); // no child edge
                    continue;
                }

                var he1 = _mesh.DetachEdgeImpl(he0);
                setHandle(he0, he1);

                _setPosition(he1.Start, _getPosition(he0.Start));
                _setPosition(he1.End, _getPosition(he0.End));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unroll()
        {
            var vertTags = new int[_mesh.Vertices.Count];
            int currTag = 0;

            var vertStack = new Stack<V>();
            var hedgeStack = new Stack<E>();

            // push twin of each halfedge in the first face
            foreach (var he in _first.Halfedges)
            {
                if (!he.IsBoundary)
                    hedgeStack.Push(he.Twin);
            }

            // search
            while (hedgeStack.Count > 0)
            {
                var he0 = hedgeStack.Pop();
                var xform = GetHalfedgeTransform(he0);

                foreach (var v in DepthFirstFrom(he0, vertStack, vertTags, ++currTag))
                    _setPosition(v, xform.Apply(_getPosition(v), true));

                // Push other interior edges
                foreach (var he1 in he0.CirculateFace.Skip(1))
                {
                    if (!he1.IsBoundary)
                        hedgeStack.Push(he1.Twin);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="stack"></param>
        /// <param name="tags"></param>
        /// <param name="currTag"></param>
        /// <returns></returns>
        private IEnumerable<V> DepthFirstFrom(E first, Stack<V> stack, int[] tags, int currTag)
        {
            var itr = first.CirculateFace.GetEnumerator();

            // tag first 2
            for(int i = 0; i < 2; i++)
            {
                itr.MoveNext();
                var v = itr.Current.Start;
                tags[v] = currTag;
            }

            // tag and push remaining
            while(itr.MoveNext())
            {
                var v = itr.Current.Start;
                stack.Push(v);
                tags[v] = currTag;
            }

            // dfs from first
            while (stack.Count > 0)
            {
                var v0 = stack.Pop();
                yield return v0;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    if (tags[v1] == currTag) continue; // skip tagged
                    stack.Push(v1);
                    tags[v1] = currTag;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <returns></returns>
        private Transform GetHalfedgeTransform(E hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            Vec3d p0 = _getPosition(he0.Start);
            Vec3d p1 = _getPosition(he1.Start);

            Vec3d p2 = (_getPosition(he0.PrevInFace.Start) + _getPosition(he0.NextInFace.End)) * 0.5;
            Vec3d p3 = (_getPosition(he1.PrevInFace.Start) + _getPosition(he1.NextInFace.End)) * 0.5;

            Vec3d x = p1 - p0;
            Plane b0 = new Plane(p0.ToPoint3d(), x.ToVector3d(), (p2 - p0).ToVector3d());
            Plane b1 = new Plane(p0.ToPoint3d(), x.ToVector3d(), (p1 - p3).ToVector3d());

            return Transform.PlaneToPlane(b0, b1);
        }
    }
}
