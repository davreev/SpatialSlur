
/*
 * Notes
 */

#if USING_RHINO


using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Rhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class LineExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Interval3d ToInterval3d(this Line line)
        {
            return new Interval3d(line.From, line.To);
        }
    }
}

#endif
