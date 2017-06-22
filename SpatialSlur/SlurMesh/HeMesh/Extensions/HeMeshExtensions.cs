using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SpatialSlur.SlurMesh.HeMesh;

/*
 * Notes 
 */
 
namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class HeMeshExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static HeMesh<V, E, F> Duplicate(this HeMesh<V, E, F> mesh)
        {
            return mesh.Duplicate(delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="other"></param>
        public static void Append(this HeMesh<V, E, F> mesh, HeMesh<V, E, F> other)
        {
            mesh.Append(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HeMesh<V, E, F> GetDual(this HeMesh<V, E, F> mesh)
        {
            return mesh.GetDual(delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="other"></param>
        public static void AppendDual(this HeMesh<V, E, F> mesh, HeMesh<V, E, F> other)
        {
            mesh.AppendDual(other, delegate { }, delegate { }, delegate { });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="getHandle"></param>
        public static HeMesh<V, E, F>[] SplitDisjoint(this HeMesh<V, E, F> mesh, Func<E, ElementHandle> getHandle)
        {
            return mesh.SplitDisjoint(getHandle, delegate { }, delegate { }, delegate { });
        }
    }
}
