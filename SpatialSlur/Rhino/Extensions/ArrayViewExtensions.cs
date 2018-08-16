
/*
 * Notes
 */

#if USING_RHINO

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.Collections;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArrayViewExtensions
    {
        /// <summary>
        /// Workaround for lack of support for ref returns in Grasshopper script components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T Get<T>(this ArrayView<T> view, int index)
        {
            return view[index];
        }


        /// <summary>
        /// Workaround for lack of support for ref returns in Grasshopper script components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public static void Set<T>(this ArrayView<T> view, int index, T item)
        {
            view[index] = item;
        }
    }
}

#endif