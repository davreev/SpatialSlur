using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SpatialSlur.SlurMesh.HeGraph;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class HeGraphExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static HeGraph<V, E> Duplicate(this HeGraph<V, E> graph)
        {
            return graph.Duplicate(delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="other"></param>
        public static void Append(this HeGraph<V, E> graph, HeGraph<V, E> other)
        {
            graph.Append(other, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="getHandle"></param>
        /// <param name="setHandle"></param>
        /// <returns></returns>
        public static HeGraph<V, E>[] SplitDisjoint(this HeGraph<V, E> graph, Func<E, SplitDisjointHandle> getHandle, Action<E, SplitDisjointHandle> setHandle)
        {
            return graph.SplitDisjoint(delegate { }, delegate { }, getHandle, setHandle);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="mesh"></param>
        public static void AppendVertexTopology(this HeGraph<V, E> graph, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            graph.AppendVertexTopology(mesh, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="mesh"></param>
        public static void AppendFaceTopology(this HeGraph<V, E> graph, HeMesh<HeMesh3d.V, HeMesh3d.E, HeMesh3d.F> mesh)
        {
            graph.AppendFaceTopology(mesh, delegate { }, delegate { });
        }
    }
}
