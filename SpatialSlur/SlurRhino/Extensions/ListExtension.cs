
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Generic;
using Rhino.Geometry;
using SpatialSlur.SlurField;
using SpatialSlur.SlurRhino;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ListExtension
    {
        #region List<IDWMesh3d<T>>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        public static void Add<T>(this List<IDWObject3d<T>> objects, Mesh mesh, T value, double influence = 1.0)
        {
            objects.Add(IDWMesh3d.Create(mesh, value, influence));
        }

        #endregion
    }
}

#endif
