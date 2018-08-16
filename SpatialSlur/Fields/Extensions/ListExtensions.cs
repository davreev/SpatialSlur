
/*
 * Notes
 */
 
using System.Collections.Generic;
using SpatialSlur.Fields;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ListExtensions
    {
        #region List<IDWObject3d<T>>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        public static void Add<T>(this List<IDWObject3d<T>> objects, Vector3d point, T value, double influence = 1.0)
        {
            objects.Add(IDWPoint3d.Create(point, value, influence));
        }

        #endregion
    }
}
