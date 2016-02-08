using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class RhinoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this Field2d field)
        {
            return field.Domain.ToBoundingBox();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this Field3d field)
        {
            return field.Domain.ToBoundingBox();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(this MeshField field)
        {
            return field.DisplayMesh.GetBoundingBox(false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        public static MeshPoint ClosestMeshPoint(this MeshField field, Point3d point)
        {
            return field.DisplayMesh.ClosestMeshPoint(point, 0.0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="point"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        public static MeshPoint ClosestMeshPoint(this MeshField field, Point3d point, double maxDist)
        {
            return field.DisplayMesh.ClosestMeshPoint(point, maxDist);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        public static bool Transform(this MeshField field, Transform xform)
        {
            field.Mesh.Transform(xform);
            return field.DisplayMesh.Transform(xform);
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static PointCloud ToPointCloud<T>(this Field3d<T> field, Func<T, Color> mapper)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="mapper"></param>
        /// <param name="result"></param>
        public static void UpdatePointCloud<T>(this Field3d<T> field, Func<T, Color> mapper, PointCloud result)
        {
            throw new NotImplementedException();
        }
    }
}
