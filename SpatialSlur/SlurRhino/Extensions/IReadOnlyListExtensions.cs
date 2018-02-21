#if USING_RHINO

using System.Collections.Generic;


using SpatialSlur.SlurCore;
using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        #region IReadOnlyList<Point3d>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance = SlurMath.ZeroTolerance)
        {
            return RemoveDuplicatePoints(points, out int[] indexMap, out RTree tree, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, out int[] indexMap, double tolerance = SlurMath.ZeroTolerance)
        {
            return RemoveDuplicatePoints(points, out indexMap, out RTree tree, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="indexMap"></param>
        /// <param name="tree"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, out int[] indexMap, out RTree tree, double tolerance = SlurMath.ZeroTolerance)
        {
            indexMap = new int[points.Count];
            tree = new RTree();

            List<Point3d> result = new List<Point3d>();
            Vector3d span = new Vector3d(tolerance, tolerance, tolerance);

            // for each point, search for coincident points in the tree
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                var index = -1;
                tree.Search(new BoundingBox(pt - span, pt + span), (s, e) =>
                {
                    index = e.Id; // cache index of found object
                    e.Cancel = true; // abort search
                });

                // if no coincident point was found...
                if (index == -1)
                {
                    index = result.Count; // set id of point
                    tree.Insert(pt, index); // insert point in tree
                    result.Add(pt); // add point to results
                }

                indexMap[i] = index;
            }

            return result;
        }

        #endregion
    }
}

#endif