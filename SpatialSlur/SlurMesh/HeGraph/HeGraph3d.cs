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
    /// Static constructors and extension methods for an HeGraph with commonly used geometric properites.
    /// </summary>
    public static class HeGraph3d
    {
        /// <summary></summary>
        public static readonly HeGraphFactory<V,E> Factory = HeGraphFactory.Create(() => new V(), () => new E());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        public static HeGraph<V,E> Create(int vertexCapacity = 4, int hedgeCapacity = 4)
        {
            return Factory.Create(vertexCapacity, hedgeCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static HeGraph<V, E> Duplicate(this HeGraph<V, E> graph)
        {
            return Factory.CreateFromOther(graph,
                (v0, v1) => v0.Position = v1.Position,
                (he0, he1) => he0.Weight = he1.Weight
                );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoints"></param>
        /// <param name="epsilon"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromLineSegments(IReadOnlyList<Vec3d> endPoints, double epsilon = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return Factory.CreateFromLineSegments(endPoints, (v, p) => v.Position = p, epsilon, allowMultiEdges, allowLoops);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromVertexTopology(HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            return Factory.CreateFromVertexTopology(mesh, (v0, v1) => v0.Position = v1.Position, (he0, he1) => he0.Weight = he1.Weight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeGraph<V, E> CreateFromFaceTopology(HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            Action<V, HeMesh3d.F> setV = (gv, mf) => gv.Position = mf.GetBarycenter(mv => mv.Position);
            return Factory.CreateFromFaceTopology(mesh, setV, (he0, he1) => he0.Weight = he1.Weight);
        }


        /// <summary>
        /// 
        /// </summary>
        public class V : HeVertex<V, E>, IVertex3d
        {
            /// <summary></summary>
            public Vec3d Position { get; set; }

            /// <summary></summary>
            public Vec3d Normal { get; set; }


            #region Explicit interface implementations
            

            /// <summary>
            /// 
            /// </summary>
            Vec2d IVertex3d.TexCoord
            {
                get { return new Vec2d(); }
                set { }
            }


            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        public class E : Halfedge<V, E>
        {
            /// <summary></summary>
            public double Weight;
        }
    }
}
