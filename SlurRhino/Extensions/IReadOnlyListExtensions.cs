using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    using G = HeGraph<HeGraph3d.V, HeGraph3d.E>;

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
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance)
        {
            return RemoveDuplicatePoints(points, tolerance, out int[] indexMap, out RTree tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance, out int[] indexMap)
        {
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out RTree tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(this IReadOnlyList<Point3d> points, double tolerance, out int[] indexMap, out RTree tree)
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


        #region IReadOnlyList<Line>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="tolerance"></param>
        /// <param name="allowMultiEdges"></param>
        /// <param name="allowLoops"></param>
        /// <returns></returns>
        public static G ToHeGraph(this IReadOnlyList<Line> lines, double tolerance = 1.0e-8, bool allowMultiEdges = false, bool allowLoops = false)
        {
            return HeGraph3d.Factory.CreateFromLineSegments(lines, (v, p) => v.Position = p, tolerance, allowMultiEdges, allowLoops);
        }

        #endregion
    }
}
