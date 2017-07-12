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
            Unroll(mesh, first, v => v.Position, (v, p) => v.Position = p);
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
            var unroller = new HeMeshUnroller<V, E, F>(mesh, first, getPosition, setPosition);
            unroller.DetachFaceCycles();
            unroller.Unroll();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    internal class HeMeshUnroller<V, E, F>
        where V : HeVertex<V, E, F>
        where E : Halfedge<V, E, F>
        where F : HeFace<V, E, F>
    {
        private HeMesh<V, E, F> _mesh;
        private F _first;

        private Stack<V> _vertStack;
        private int[] _vertTags;
        private int _currTag = 0;

        private Func<V, Vec3d> _getPosition;
        private Action<V, Vec3d> _setPosition;


        /// <summary>
        /// 
        /// </summary>
        internal HeMeshUnroller(HeMesh<V, E, F> mesh, F first, Func<V, Vec3d> getPosition, Action<V, Vec3d> setPosition)
        {
            mesh.Faces.ContainsCheck(first);
            first.RemovedCheck();

            _mesh = mesh;
            _first = first;
            _vertTags = new int[_mesh.Vertices.Count];
        }


        /// <summary>
        /// 
        /// </summary>
        public void DetachFaceCycles()
        {
            var detach = new List<E>();
            var currTag = _mesh.Faces.NextTag;
       
            foreach (var he in _mesh.GetFacesBreadthFirst(_first.Yield()))
            {
                var f = he.Face;
                
                if (f.Tag == currTag)
                    detach.Add(he);
                else
                    f.Tag = currTag; 
            }

            foreach (var he0 in detach)
            {
                var he1 = _mesh.DetachEdge(he0);
                _setPosition(he1.Start, _getPosition(he0.Start));
                _setPosition(he1.End, _getPosition(he0.End));
            }
        }


        /*
        /// <summary>
        /// Returns true if the mesh has any number of cycles in the face topology.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        bool HasFaceCycles()
        {
            var faceStack = new Stack<F>();
            var currTag = _mesh.Faces.NextTag;

            faceStack.Push(_first);
            _first.Tag = currTag;

            while (faceStack.Count > 0)
            {
                var f0 = faceStack.Pop();
                int count = 0;

                foreach (var f1 in f0.AdjacentFaces)
                {
                    if (f1.Tag == currTag)
                    {
                        count++;
                        continue;
                    }
                    
                    faceStack.Push(f1);
                    f1.Tag = currTag;
                }

                if (count > 1) return true;
            }

            return false;
        }
        */


        /// <summary>
        /// 
        /// </summary>
        public void Unroll()
        {
            var hedgeStack = new Stack<E>();

            // push twins of halfedges in first face
            foreach (var he in _first.Halfedges)
            {
                if (!he.IsBoundary)
                {
                    hedgeStack.Push(he.Twin);
                    _vertTags[he.Start] = -1;
                }
            }

            // search
            while (hedgeStack.Count > 0)
            {
                var he0 = hedgeStack.Pop();
                var t0 = GetHalfedgeTransform(he0);

                // Transform all verts ahead of stack
                foreach (var v in DepthFirstFrom(he0.Start))
                    _setPosition(v, t0.Apply(_getPosition(v), true));

                // Push other interior edges
                foreach(var he1 in he0.CirculateFace.Skip(1))
                {
                    if (!he1.IsBoundary)
                    {
                        hedgeStack.Push(he1.Twin);
                        _vertTags[he1.Start] = -1;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="vertexPositions"></param>
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
            Vec3d y0 = Vec3d.Cross(p0 - p2, x);
            Vec3d y1 = Vec3d.Cross(p3 - p1, x);

            Plane b0 = new Plane(p0.ToPoint3d(), x.ToVector3d(), y0.ToVector3d());
            Plane b1 = new Plane(p0.ToPoint3d(), x.ToVector3d(), y1.ToVector3d());
            return Transform.PlaneToPlane(b0, b1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        IEnumerable<V> DepthFirstFrom(V first)
        {
            _vertStack.Push(first);
            _vertTags[first] = ++_currTag;

            // dfs from first
            while (_vertStack.Count > 0)
            {
                var v0 = _vertStack.Pop();
                yield return v0;

                foreach (var v1 in v0.ConnectedVertices)
                {
                    int i1 = v1.Index;
                    int t1 = _vertTags[i1];

                    // skip if already visited
                    if (t1 == -1 || t1 == _currTag)
                        continue;

                    _vertStack.Push(v1);
                    _vertTags[i1] = _currTag;
                }
            }
        }
    }
}
