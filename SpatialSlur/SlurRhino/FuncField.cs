
/*
 * Notes
 */ 

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class FuncField3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static FuncField3d<double> CreateSignedDistance(Mesh mesh)
        {
            return Create(p =>
            {
                var mp = mesh.ClosestMeshPoint(p, 0.0);
                var d = p - (Vec3d)mp.Point;
                return d.Length * Math.Sign(Vec3d.Dot(d, mesh.NormalAt(mp)));
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static FuncField3d<double> CreateSignedDistancePerpendicular(Mesh mesh)
        {
            return Create(p =>
            {
                var mp = mesh.ClosestMeshPoint(p, 0.0);
                var n = mesh.NormalAt(mp);
                var m = n.SquareLength;
                return m > 0.0 ? Vec3d.Dot(p - (Vec3d)mp.Point, n) / Math.Sqrt(m) : 0.0;
            });
        }
    }
}

#endif