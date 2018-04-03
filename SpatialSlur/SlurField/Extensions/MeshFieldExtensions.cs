#if USING_RHINO

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public static class MeshFieldExtensions
    {
        #region Double

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<double> GetLaplacian(this MeshField3d<double> field, bool parallel = true)
        {
            var result = MeshField3d.Double.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this MeshField3d<double> field, double[] result, bool parallel = true)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var verts = field.Mesh.Vertices;
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsUnused) continue;

                    var t = vals[i];
                    var sum = 0.0;
                    int count = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += vals[v1] - t;
                        count++;
                    }

                    result[i] = sum / count;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="getWeight"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<double> GetLaplacian(this MeshField3d<double> field, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            var result = MeshField3d.Double.Create(field);
            GetLaplacian(result, getWeight, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="getWeight"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this MeshField3d<double> field, Func<HeMesh3d.Halfedge, double> getWeight, double[] result, bool parallel = true)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var verts = field.Mesh.Vertices;
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = vals[i];
                    var sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (vals[he.End] - t) * getWeight(he);

                    result[i] = sum;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<Vec3d> GetGradient(this MeshField3d<double> field, bool parallel = true)
        {
            var result = MeshField3d.Vec3d.Create(field);
            GetGradient(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this MeshField3d<double> field, Vec3d[] result, bool parallel = true)
        {
            // TODO revise implementation as per
            // http://libigl.github.io/libigl/tutorial/tutorial.html

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var verts = field.Mesh.Vertices;
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = vals[i];

                    var sum = new Vec3d();
                    int count = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var v0 = he.Start;
                        var v1 = he.End;

                        var d = v1.Position - v0.Position;
                        sum += d * ((vals[v1] - t) / d.SquareLength);
                        count++;
                    }

                    result[i] = sum / count;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<Vec3d> GetGradient(this MeshField3d<double> field, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            var result = MeshField3d.Vec3d.Create(field);
            GetGradient(field, getWeight, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="getWeight"></param>
        /// <param name="parallel"></param>
        public static void GetGradient(this MeshField3d<double> field, Func<HeMesh3d.Halfedge, double> getWeight, Vec3d[] result, bool parallel = true)
        {
            // TODO revise implementation as per
            // http://libigl.github.io/libigl/tutorial/tutorial.html

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                for (int i = from; i < to; i++)
                {
                    var verts = field.Mesh.Vertices;
                    var vals = field.Values;

                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = vals[i];
                    var sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var v0 = he.Start;
                        var v1 = he.End;

                        var d = v1.Position - v0.Position;
                        var dt = (vals[v1] - t) * getWeight(he);
                        sum += d * (dt / d.SquareLength);
                    }

                    result[i] = sum;
                }
            }
        }

        #endregion


        #region Vec3d

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<Vec3d> GetLaplacian(this MeshField3d<Vec3d> field, bool parallel = true)
        {
            var result = MeshField3d.Vec3d.Create(field);
            GetLaplacian(field, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this MeshField3d<Vec3d> field, Vec3d[] result, bool parallel = true)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var verts = field.Mesh.Vertices;
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsUnused) continue;

                    var t = vals[i];
                    var sum = Vec3d.Zero;
                    int count = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += vals[v1] - t;
                        count++;
                    }

                    result[i] = sum / count;
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static MeshField3d<Vec3d> GetLaplacian(this MeshField3d<Vec3d> field, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            var result = MeshField3d.Vec3d.Create(field);
            GetLaplacian(field, getWeight, result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public static void GetLaplacian(this MeshField3d<Vec3d> field, Func<HeMesh3d.Halfedge, double> getWeight, Vec3d[] result, bool parallel = true)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, field.Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, field.Count);

            void Body(int from, int to)
            {
                var verts = field.Mesh.Vertices;
                var vals = field.Values;

                for (int i = from; i < to; i++)
                {
                    var v = verts[i];
                    if (v.IsUnused) continue;

                    var t = vals[i];
                    var sum = Vec3d.Zero;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (vals[he.End] - t) * getWeight(he);

                    result[i] = sum;
                }
            }
        }

        #endregion
    }
}

#endif