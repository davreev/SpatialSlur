using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurData;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurGraph;


namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// General purpose static geometry methods.
    /// </summary>
    public static class GeometryUtil
    {
        /// <summary>
        /// Returns parameters for the closest pair of points between lines a and b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="ta"></param>
        /// <param name="tb"></param>
        public static void LineLineClosestPoints(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1, out double ta, out double tb)
        {
            LineLineClosestPoints(a1 - a0, b1 - b0, a0 - b0, out ta, out tb);
        }


        /// <summary>
        /// Returns the shortest vector from line a to line b.
        /// http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <returns></returns>
        public static Vec3d LineLineShortestVector(Vec3d a0, Vec3d a1, Vec3d b0, Vec3d b1)
        {
            Vec3d u = a1 - a0;
            Vec3d v = b1 - b0;
            Vec3d w = a0 - b0;

            double tu, tv;
            LineLineClosestPoints(u, v, w, out tu, out tv);

            return v * tv - u * tu - w;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void LineLineClosestPoints(Vec3d u, Vec3d v, Vec3d w, out double tu, out double tv)
        {
            double uu = u * u;
            double uv = u * v;
            double vv = v * v;
            double uw = u * w;
            double vw = v * w;

            double denom = 1.0 / (uu * vv - uv * uv);
            tu = (uv * vw - vv * uw) * denom;
            tv = (uu * vw - uv * uw) * denom;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ReflectInPlane(Vec3d point, Vec3d origin, Vec3d normal)
        {
            return point + Vec3d.Project(origin - point, normal) * 2.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d origin, Vec3d normal)
        {
            return point + Vec3d.Project(origin - point, normal);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <param name="origin"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vec3d ProjectToPlane(Vec3d point, Vec3d origin, Vec3d normal, Vec3d direction)
        {
            return point + direction * (((origin - point) * normal) / (direction * normal));
        }


        /// <summary>
        /// Returns the center of the circle that passes through the 3 given points.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vec3d GetCurvatureCenter(Vec3d p0, Vec3d p1, Vec3d p2)
        {
            return p1 + GetCurvatureVector(p0 - p1, p2 - p1);
        }


        /// <summary>
        /// http://www.block.arch.ethz.ch/brg/files/2013-ijss-vanmele-shaping-tension-structures-with-actively-bent-linear-elements_1386929572.pdf
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vec3d GetCurvatureVector(Vec3d v0, Vec3d v1)
        {
            Vec3d v2 = Vec3d.Cross(v0, v1);
            return Vec3d.Cross((v0.SquareLength * v1 - v1.SquareLength * v0), v2) / (2.0 * v2.SquareLength);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Vec3d GetBarycentric(Vec3d point, Vec3d a, Vec3d b, Vec3d c)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsInTriangle(Vec3d point, Vec3d a, Vec3d b, Vec3d c)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns entries of a rotation matrix in column-major order.
        /// Assumes the given axis is unit length.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double[] GetRotationMatrix(Vec3d axis, double angle)
        {
            double[] m = new double[9];

            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            double t = 1.0 - c;

            m[0] = c + axis.x * axis.x * t; // m00
            m[4] = c + axis.y * axis.y * t; // m11
            m[8] = c + axis.z * axis.z * t; // m22

            double tmp1 = axis.x * axis.y * t;
            double tmp2 = axis.z * s;
            m[1] = tmp1 + tmp2; // m01
            m[3] = tmp1 - tmp2; // m10

            tmp1 = axis.x * axis.z * t;
            tmp2 = axis.y * s;
            m[2] = tmp1 - tmp2; // m02
            m[6] = tmp1 + tmp2; tmp1 = axis.y * axis.z * t; // m20

            tmp2 = axis.x * s;
            m[5] = tmp1 + tmp2; // m21
            m[7] = tmp1 - tmp2; // m12

            return m;
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec2d GetGradient(Func<Vec2d, double> func, Vec2d vector, double delta)
        {
            double x = vector.x;
            double y = vector.y;
       
            double gx = func(new Vec2d(x + delta, y)) - func(new Vec2d(x - delta, y));
            double gy = func(new Vec2d(x, y + delta)) - func(new Vec2d(x, y - delta));
            return new Vec2d(gx, gy) / (delta * 2.0);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d GetGradient(Func<Vec3d, double> func, Vec3d vector, double delta)
        {
            double x = vector.x;
            double y = vector.y;
            double z = vector.z;

            double gx = func(new Vec3d(x + delta, y, z)) - func(new Vec3d(x - delta, y, z));
            double gy = func(new Vec3d(x, y + delta, z)) - func(new Vec3d(x, y - delta, z));
            double gz = func(new Vec3d(x, y, z + delta)) - func(new Vec3d(x, y, z - delta));
            return new Vec3d(gx, gy, gz) / (delta * 2.0);
        }


        /// <summary>
        /// Returns a numerical approximation of the gradient of the given function with respect to the given vector.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="vector"></param>
        /// <param name="delta"></param>
        /// <param name="result"></param>
        public static void GetGradient(Func<VecKd, double> func, VecKd vector, double delta, VecKd result)
        {
            double d2 = 1.0 / (delta * 2.0);

            for (int i = 0; i < vector.K; i++)
            {
                double t = vector[i];

                vector[i] = t + delta;
                double g0 = func(vector);

                vector[i] = t - delta;
                double g1 = func(vector);

                result[i] = (g0 - g1) * d2;
                vector[i] = t;
            }
        }


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec2d> vectors)
        {
            Vec2d mean;
            return GetCovarianceMatrix(vectors, out mean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec2d> vectors, out Vec2d mean)
        {
            // calculate mean
            mean = new Vec2d();
            foreach (Vec2d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[4];
            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[3] += d.y * d.y;
            }

            // set symmetric values
            result[2] = result[1];
            return result;
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec3d> vectors)
        {
            Vec3d mean;
            return GetCovarianceMatrix(vectors, out mean);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vec3d> vectors, out Vec3d mean)
        {
            // calculate mean
            mean = new Vec3d();
            foreach (Vec3d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[9];

            for (int i = 0; i < vectors.Count; i++)
            {
                Vec3d d = vectors[i] - mean;
                result[0] += d.x * d.x;
                result[1] += d.x * d.y;
                result[2] += d.x * d.z;
                result[4] += d.y * d.y;
                result[5] += d.y * d.z;
                result[8] += d.z * d.z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
            return result;
        }


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<VecKd> vectors)
        {
            int n = vectors[0].K;

            // calculate mean
            VecKd mean = new VecKd(n);
            foreach (VecKd v in vectors) mean.Add(v);
            mean.Scale(1 / vectors.Count);

            // calculate lower triangular covariance matrix
            double[] result = new double[n * n];
            VecKd d = new VecKd(n);

            for (int k = 0; k < vectors.Count; k++)
            {
                vectors[k].Subtract(mean, d);

                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                        result[i * n + j] += d[i] * d[j];
                }
            }

            // fill out upper triangular
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * n + i] = result[i * n + j];
            }

            return result;
        }


        /// <summary>
        /// Returns the entries of the incidence matrix in column-major order.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double[] GetIncidenceMatrix(HeMesh mesh)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(HeMesh mesh)
        {
            var verts = mesh.Vertices;
            int nv = verts.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    int j = he.End.Index;
                    result[i * nv + j] = -1.0;
                    wsum++;
                }

                result[i + i * nv] = wsum;
            }

            return result;
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="halfedgeWeights"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(HeMesh mesh, IList<double> halfedgeWeights)
        {
            mesh.Halfedges.SizeCheck(halfedgeWeights);

            var verts = mesh.Vertices;
            int nv = verts.Count;
            double[] result = new double[nv * nv];

            for (int i = 0; i < nv; i++)
            {
                HeVertex v = verts[i];
                if (v.IsUnused) continue;

                double wsum = 0.0;

                foreach (Halfedge he in v.OutgoingHalfedges)
                {
                    int j = he.End.Index;
                    double w = halfedgeWeights[he.Index];
                    result[i * nv + j] = -w;
                    wsum += w;
                }

                result[i * nv + i] = wsum;
            }

            return result;
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(Graph graph)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(Graph graph, IList<double> edgeWeights)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(DiGraph graph)
        {
            // TODO
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the entries of the Laplacian matrix in column-major order.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(DiGraph graph, IList<double> edgeWeights)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
