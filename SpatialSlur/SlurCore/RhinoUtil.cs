using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurGraph;
using SpatialSlur.SlurMesh;
using Rhino.Geometry;


namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// General purpose static utility methods for use in Rhino.
    /// </summary>
    public static class RhinoUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(IList<Point3d> points, double tolerance)
        {
            int[] indexMap;
            RTree tree;
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(IList<Point3d> points, double tolerance, out int[] indexMap)
        {
            RTree tree;
            return RemoveDuplicatePoints(points, tolerance, out indexMap, out tree);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <param name="indexMap"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static List<Point3d> RemoveDuplicatePoints(IList<Point3d> points, double tolerance, out int[] indexMap, out RTree tree)
        {
            List<Point3d> result = new List<Point3d>();
            indexMap = new int[points.Count];
            tree = new RTree();

            SearchHelper helper = new SearchHelper();
            Vector3d span = new Vector3d(tolerance, tolerance, tolerance);

            // for each point, search for coincident points in the tree
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                helper.Reset();
                tree.Search(new BoundingBox(pt - span, pt + span), helper.Callback);
                int index = helper.Id;

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


        /// <summary>
        /// Simple helper class for searching an RTree for duplicate points.
        /// </summary>
        private class SearchHelper
        {
            private int _id = -1;

            //
            public int Id
            {
                get { return _id; }
            }

            //
            public void Reset()
            {
                _id = -1;
            }

            //
            public void Callback(Object sender, RTreeEventArgs e)
            {
                _id = e.Id; // cache index of found object
                e.Cancel = true; // abort search
            }
        }


        /// <summary>
        /// Returns the the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vector3d> vectors)
        {
            Vector3d mean;
            return GetCovarianceMatrix(vectors, out mean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Vector3d> vectors, out Vector3d mean)
        {
            // calculate mean
            mean = new Vector3d();
            foreach (Vector3d v in vectors) mean += v;
            mean /= vectors.Count;

            // calculate covariance matrix
            double[] result = new double[9];

            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3d d = vectors[i] - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
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
        /// <param name="points"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Point3d> points)
        {
            Point3d mean;
            return GetCovarianceMatrix(points, out mean);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static double[] GetCovarianceMatrix(IList<Point3d> points, out Point3d mean)
        {
            // calculate mean
            mean = new Point3d();
            foreach (Point3d p in points) mean += p;
            mean /= points.Count;

            // calculate covariance matrix
            double[] result = new double[9];

            for (int i = 0; i < points.Count; i++)
            {
                Vector3d d = points[i] - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[2] += d.X * d.Z;
                result[4] += d.Y * d.Y;
                result[5] += d.Y * d.Z;
                result[8] += d.Z * d.Z;
            }

            // set symmetric values
            result[3] = result[1];
            result[6] = result[2];
            result[7] = result[5];
            return result;
        }


        /// <summary>
        /// Returns the entries of the cotangent weighted laplacian matrix in column-major order.
        /// </summary>
        /// <returns></returns>
        public static double[] GetLaplacianMatrix(Mesh mesh)
        {
            mesh.Faces.ConvertQuadsToTriangles();

            int n = mesh.Vertices.Count;
            double[] result = new double[n * n];
            double[] areas = new double[n];

            Point3d[] points = mesh.Vertices.ToPoint3dArray();
            double t = 0.5 / 3.0;

            // iterate faces to collect weights and vertex areas (lower triangular only)
            foreach (MeshFace mf in mesh.Faces)
            {
                // circulate verts in face
                for (int i = 0; i < 3; i++)
                {
                    int i0 = mf[i];
                    int i1 = mf[(i + 1) % 3];
                    int i2 = mf[(i + 2) % 3];

                    Vector3d v0 = points[i0] - points[i2];
                    Vector3d v1 = points[i1] - points[i2];

                    // add to vertex area
                    double a = Vector3d.CrossProduct(v0, v1).Length;
                    areas[i0] += a * t;

                    // add to edge cotangent weights (assumes consistent face orientation)
                    if (i1 > i0)
                        result[i0 * n + i1] += 0.5 * v0 * v1 / a;
                    else
                        result[i1 * n + i0] += 0.5 * v0 * v1 / a;
                }
            }

            // normalize weights with areas and sum along diagonals
            for (int i = 0; i < n; i++)
            {
                int ii = i * n + i;

                for (int j = i + 1; j < n; j++)
                {
                    double w = result[i * n + j];
                    w /= Math.Sqrt(areas[i] * areas[j]);
                    result[i * n + j] = -w;
                    result[j * n + i] = -w;

                    // sum along diagonal entries
                    result[ii] += w;
                    result[j * n + j] += w;
                }
            }

            return result;
        }

    }
}
