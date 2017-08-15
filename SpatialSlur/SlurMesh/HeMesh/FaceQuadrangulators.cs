using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class FaceQuadrangulators
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Fan
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public static Fan<V, E, F> Create<V, E, F>(HeMeshBase<V, E, F> mesh)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return new Fan<V, E, F>(mesh, f => f.First);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getStart"></param>
            /// <returns></returns>
            public static Fan<V, E, F> Create<V, E, F>(HeMeshBase<V, E, F> mesh, Func<F,E> getStart)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return new Fan<V, E, F>(mesh, getStart);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public class Fan<V, E, F> : IFaceQuadrangulator<V, E, F>
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            private HeMeshBase<V, E, F> _mesh;
            private Func<F, E> _getStart;


            /// <summary>
            /// 
            /// </summary>
            internal Fan(HeMeshBase<V, E, F> mesh, Func<F, E> getStart)
            {
                _mesh = mesh ?? throw new ArgumentNullException();
                _getStart = getStart ?? throw new ArgumentNullException();
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="face"></param>
            /// <returns></returns>
            public IEnumerable<(V, V, V, V)> GetQuads(F face)
            {
                face.RemovedCheck();

                var he = _getStart(face);
                var v0 = he.Start;

                he = he.NextInFace;
                var v1 = he.Start;

                do
                {
                    he = he.NextInFace;
                    var v2 = he.Start;

                    if (v2 == v0) break;

                    he = he.NextInFace;
                    var v3 = he.Start;

                    if (v3 == v0)
                    {
                        yield return (v0, v1, v2, null);
                        break;
                    }

                    yield return (v0, v1, v2, v3);
                    v1 = v3;
                } while (true);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="face"></param>
            public void Quadrangulate(F face)
            {
                _mesh.Faces.ContainsCheck(face);
                face.RemovedCheck();

                var he0 = _getStart(face);
                var he1 = he0.NextInFace.NextInFace.NextInFace;

                while (he1 != he0 && he1.NextInFace != he0)
                {
                    he0 = _mesh.SplitFaceImpl(he0, he1);
                    he1 = he1.NextInFace.NextInFace;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static class Strip
        {
            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public static Strip<V, E, F> Create<V, E, F>(HeMeshBase<V, E, F> mesh)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return new Strip<V, E, F>(mesh, f => f.First);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="V"></typeparam>
            /// <typeparam name="E"></typeparam>
            /// <typeparam name="F"></typeparam>
            /// <param name="mesh"></param>
            /// <param name="getStart"></param>
            /// <returns></returns>
            public static Strip<V, E, F> Create<V, E, F>(HeMeshBase<V, E, F> mesh, Func<F, E> getStart)
                where V : HeVertex<V, E, F>
                where E : Halfedge<V, E, F>
                where F : HeFace<V, E, F>
            {
                return new Strip<V, E, F>(mesh, getStart);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        public class Strip<V, E, F> : IFaceQuadrangulator<V, E, F>
            where V : HeVertex<V, E, F>
            where E : Halfedge<V, E, F>
            where F : HeFace<V, E, F>
        {
            private HeMeshBase<V, E, F> _mesh;
            private Func<F, E> _getStart;


            /// <summary>
            /// 
            /// </summary>
            internal Strip(HeMeshBase<V, E, F> mesh, Func<F, E> getStart)
            {
                _mesh = mesh ?? throw new ArgumentNullException();
                _getStart = getStart ?? throw new ArgumentNullException();
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="face"></param>
            /// <returns></returns>
            public IEnumerable<(V, V, V, V)> GetQuads(F face)
            {
                var he0 = _getStart(face);
                var v0 = he0.Start;

                var he1 = he0.NextInFace;
                var v1 = he1.Start;

                do
                {
                    he1 = he1.NextInFace;
                    var v2 = he1.Start;

                    if (v2 == v0) break;

                    he0 = he0.PrevInFace;
                    var v3 = he0.Start;

                    if (v2 == v3)
                    {
                        yield return (v0, v1, v2, null);
                        break;
                    }

                    yield return (v0, v1, v2, v3);
                    v0 = v3;
                    v1 = v2;
                } while (true);
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="face"></param>
            public void Quadrangulate(F face)
            {
                _mesh.Faces.ContainsCheck(face);
                face.RemovedCheck();

                var he0 = _getStart(face);
                var he1 = he0.NextInFace.NextInFace.NextInFace;

                while (he1 != he0 && he1.NextInFace != he0)
                {
                    he0 = _mesh.SplitFaceImpl(he0, he1).PrevInFace;
                    he1 = he1.NextInFace;
                }
            }
        }
    }
}
