
/*
 * Notes
 */

#if USING_RHINO

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

using Rhino.Geometry;
using SpatialSlur.SlurField;

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDiscreteField3dExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="getColor"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static PointCloud ToPointCloud<T>(this IDiscreteField3d<T> field, Func<T, Color> getColor, bool parallel = false)
            where T : struct
        {
            var cloud = new PointCloud(field.Coordinates.Select(p => new Point3d(p.X, p.Y, 0.0)));

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var vals = field.Values;

                for (int i = from; i < to; i++)
                    cloud[i].Color = getColor(vals[i]);
            }

            return cloud;
        }
    }
}

#endif