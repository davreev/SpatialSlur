using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 * 
 * TODO
 * Add per-edge unroll factor
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
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
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first)
            where V : HeMeshBase<V, E, F>.Vertex, IVertex3d
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var prop = Property.Create<V, Vec3d>(v => v.Position, (v, p) => v.Position = p);
            Unroll(mesh, first, prop, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="setChildIndex"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Action<E, int> setChildIndex)
            where V : HeMeshBase<V, E, F>.Vertex, IVertex3d
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var prop = Property.Create<V, Vec3d>(v => v.Position, (v, p) => v.Position = p);
            Unroll(mesh, first, prop, setChildIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="position"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Property<V, Vec3d> position)
            where V : HeMeshBase<V, E, F>.Vertex
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            Unroll(mesh, first, position, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="position"></param>
        /// <param name="setChildIndex"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Property<V, Vec3d> position, Action<E, int> setChildIndex)
            where V : HeMeshBase<V, E, F>.Vertex
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var unroller = new HeMeshUnroller<V, E, F>(mesh, first, position);
            unroller.DetachFaceCycles(setChildIndex);
            unroller.Unroll();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Func<E, double> getUnrollFactor)
            where V : HeMeshBase<V, E, F>.Vertex, IVertex3d
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var prop = Property.Create<V, Vec3d>(v => v.Position, (v, p) => v.Position = p);
            Unroll(mesh, first, prop, getUnrollFactor, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="setChildIndex"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Func<E, double> getUnrollFactor, Action<E, int> setChildIndex)
            where V : HeMeshBase<V, E, F>.Vertex, IVertex3d
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var prop = Property.Create<V, Vec3d>(v => v.Position, (v, p) => v.Position = p);
            Unroll(mesh, first, prop, getUnrollFactor, setChildIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="position"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Property<V, Vec3d> position, Func<E, double> getUnrollFactor)
            where V : HeMeshBase<V, E, F>.Vertex
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            Unroll(mesh, first, position, getUnrollFactor, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="mesh"></param>
        /// <param name="first"></param>
        /// <param name="position"></param>
        /// <param name="setChildIndex"></param>
        public static void Unroll<V, E, F>(HeMeshBase<V, E, F> mesh, F first, Property<V, Vec3d> position, Func<E, double> getUnrollFactor, Action<E, int> setChildIndex)
            where V : HeMeshBase<V, E, F>.Vertex
            where E : HeMeshBase<V, E, F>.Halfedge
            where F : HeMeshBase<V, E, F>.Face
        {
            var unroller = new HeMeshUnroller<V, E, F>(mesh, first, position);
            unroller.DetachFaceCycles(setChildIndex);
            unroller.Unroll(getUnrollFactor);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    internal class HeMeshUnroller<V, E, F>
        where V : HeMeshBase<V, E, F>.Vertex
        where E : HeMeshBase<V, E, F>.Halfedge
        where F : HeMeshBase<V, E, F>.Face
    {
        private HeMeshBase<V, E, F> _mesh;
        private F _first;
        
        private Func<V, Vec3d> _getPos;
        private Action<V, Vec3d> _setPos;


        /// <summary>
        /// 
        /// </summary>
        internal HeMeshUnroller(HeMeshBase<V, E, F> mesh, F first, Property<V, Vec3d> position)
        {
            mesh.Faces.ContainsCheck(first);
            first.UnusedCheck();

            _mesh = mesh;
            _first = first;

            _getPos = position.Get;
            _setPos = position.Set;
        }


        /// <summary>
        /// 
        /// </summary>
        public void DetachFaceCycles(Action<E, int> setChildIndex)
        {
            var currTag = _mesh.Halfedges.NextTag;

            // tag traversed edges during BFS
            foreach (var he in _mesh.GetFacesBreadthFirst(_first.Yield()))
                he.Edge.Tag = currTag;

            var edges = _mesh.Edges;
            var ne = edges.Count;

            // detach all untagged edges
            for (int i = 0; i < ne; i++)
            {
                var he0 = edges[i];

                if (he0.IsUnused || he0.IsBoundary || he0.Tag == currTag)
                {
                    setChildIndex(he0, -1); // no child edge
                    continue;
                }

                var he1 = _mesh.DetachEdgeImpl(he0);
                setChildIndex(he0, he1);

                _setPos(he1.Start, _getPos(he0.Start));
                _setPos(he1.End, _getPos(he0.End));
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
                var orient = GetHalfedgeTransform(he0);

                foreach (var v in DepthFirstFrom(he0, vertStack, vertTags, ++currTag))
                    _setPos(v, orient.Apply(_getPos(v)));

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
        /// <param name="hedge"></param>
        /// <returns></returns>
        private Orient3d GetHalfedgeTransform(E hedge)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            Vec3d p0 = _getPos(he0.Start);
            Vec3d p1 = _getPos(he1.Start);

            Vec3d p2 = (_getPos(he0.PreviousInFace.Start) + _getPos(he0.NextInFace.End)) * 0.5;
            Vec3d p3 = (_getPos(he1.PreviousInFace.Start) + _getPos(he1.NextInFace.End)) * 0.5;

            Vec3d x = p1 - p0;

            return Orient3d.CreateFromTo(
                new Orient3d(p0, x, p2 - p0), 
                new Orient3d(p0, x, p1 - p3)
                );
        }


        /// <summary>
        /// 
        /// </summary>
        public void Unroll(Func<E, double> getUnrollFactor)
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
                var t = getUnrollFactor(he0);

                // apply transformation
                if (t > 0.0)
                {
                    var orient = GetHalfedgeTransform(he0, t);

                    foreach (var v in DepthFirstFrom(he0, vertStack, vertTags, ++currTag))
                        _setPos(v, orient.Apply(_getPos(v)));
                }

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
        /// <param name="hedge"></param>
        /// <param name="unrollFactor"></param>
        /// <returns></returns>
        private Orient3d GetHalfedgeTransform(E hedge, double unrollFactor)
        {
            var he0 = hedge;
            var he1 = he0.Twin;

            Vec3d p0 = _getPos(he0.Start);
            Vec3d p1 = _getPos(he1.Start);

            Vec3d p2 = (_getPos(he0.PreviousInFace.Start) + _getPos(he0.NextInFace.End)) * 0.5;
            Vec3d p3 = (_getPos(he1.PreviousInFace.Start) + _getPos(he1.NextInFace.End)) * 0.5;

            Vec3d x = p1 - p0;
            Vec3d y0 = p2 - p0;
            Vec3d y1 = (unrollFactor < 1.0) ? y0.SlerpTo(p1 - p3, unrollFactor) : p1 - p3;

            return Orient3d.CreateFromTo(
                new Orient3d(p0, x, y0),
                new Orient3d(p0, x, y1)
                );
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
            for (int i = 0; i < 2; i++)
            {
                itr.MoveNext();
                var v = itr.Current.Start;
                tags[v] = currTag;
            }

            // tag and push remaining
            while (itr.MoveNext())
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
    }
}
