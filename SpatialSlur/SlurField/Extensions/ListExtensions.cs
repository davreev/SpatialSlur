using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        #region List<IDWPoint3d<T>>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="points"></param>
        /// <param name="point"></param>
        /// <param name="value"></param>
        /// <param name="influence"></param>
        public static void Add<T>(this List<IDWPoint3d<T>> points, Vec3d point, T value, double influence = 1.0)
        {
            points.Add(IDWPoint3d.Create(point, value, influence));
        }

        #endregion
    }
}
