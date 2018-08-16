
/*
 * Notes
 */ 

#if USING_RHINO

using System;
using Rhino.Geometry;
using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Field3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static IField3d<double> CreateDistance(Mesh mesh)
        {
            return Create(p => p.DistanceTo(mesh.ClosestPoint(p)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static IField3d<double> CreateSignedDistance(Mesh mesh)
        {
            return Create(p =>
            {
                var mp = mesh.ClosestMeshPoint(p, 0.0);
                var d = p - (Vector3d)mp.Point;
                return d.Length * Math.Sign(Vector3d.Dot(d, mesh.NormalAt(mp)));
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public static IField3d<double> CreateSignedPerpDistance(Mesh mesh)
        {
            return Create(p =>
            {
                var mp = mesh.ClosestMeshPoint(p, 0.0);
                var n = mesh.NormalAt(mp);
                var m = n.SquareLength;
                return m > 0.0 ? Vector3d.Dot(p - (Vector3d)mp.Point, n) / Math.Sqrt(m) : 0.0;
            });
        }
    }
}

#endif