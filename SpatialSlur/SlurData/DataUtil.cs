using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Utility class for related constants and static methods.
    /// </summary>
    public static class DataUtil
    {
        /// <summary></summary>
        public const double DiameterToBinScale = 1.75;
        /// <summary></summary>
        public const double RadiusToBinScale = DiameterToBinScale * 2.0;


        /// <summary>
        /// Consolidates points within a given search radius.
        /// Returns true if the solution converged within the given maximum number of steps.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        /// <param name="tolerance"></param>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public static bool ConsolidatePoints(IList<Vec2d> points, double radius, double tolerance = 1.0e-8, int maxSteps = 10)
        {
            var grid = new HashGrid3d<Vec2d>(points.Count << 1, radius * RadiusToBinScale);

            var radSqr = radius * radius;
            var tolSqr = tolerance * tolerance;
            var converged = false;

            while (!converged && maxSteps-- > 0)
            {
                converged = true;

                // insert into grid
                for (int i = 0; i < points.Count; i++)
                {
                    var p = points[i];
                    grid.Insert(p, p);
                }

                // search grid
                for (int i = 0; i < points.Count; i++)
                {
                    var p0 = points[i];
                    var p1 = grid.Search(new Domain2d(p0, radius))
                        .Where(p => p0.SquareDistanceTo(p) < radSqr)
                        .Mean();

                    points[i] = p1;

                    if (p0.SquareDistanceTo(p1) > tolSqr)
                        converged = false;
                }

                grid.Clear();
            }

            return maxSteps > 0;
        }


        /// <summary>
        /// Consolidates points within a given search radius.
        /// Returns true if the solution converged within the given maximum number of steps.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        /// <param name="tolerance"></param>
        /// <param name="maxSteps"></param>
        /// <returns></returns>
        public static bool ConsolidatePoints(IList<Vec3d> points, double radius, double tolerance = 1.0e-8, int maxSteps = 10)
        {
            var grid = new HashGrid3d<Vec3d>(points.Count << 1, radius * RadiusToBinScale);

            var radSqr = radius * radius;
            var tolSqr = tolerance * tolerance;
            var converged = false;

            while (!converged && maxSteps-- > 0)
            {
                converged = true;

                // insert into grid
                for (int i = 0; i < points.Count; i++)
                {
                    var p = points[i];
                    grid.Insert(p, p);
                }

                // search grid
                for (int i = 0; i < points.Count; i++)
                {
                    var p0 = points[i];
                    var p1 = grid.Search(new Domain3d(p0, radius))
                        .Where(p => p0.SquareDistanceTo(p) < radSqr)
                        .Mean();

                    points[i] = p1;

                    if (p0.SquareDistanceTo(p1) > tolSqr)
                        converged = false;
                }

                grid.Clear();
            }

            return maxSteps > 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(IEnumerable<Vec2d> vectors, double[] result)
        {
            GetCovarianceMatrix(vectors, vectors.Mean(), result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        public static void GetCovarianceMatrix(IEnumerable<Vec2d> vectors, Vec2d mean, double[] result)
        {
            Array.Clear(result, 0, 4);

            // calculate covariance matrix
            foreach (Vec2d v in vectors)
            {
                Vec3d d = v - mean;
                result[0] += d.X * d.X;
                result[1] += d.X * d.Y;
                result[3] += d.Y * d.Y;
            }

            // set symmetric values
            result[2] = result[1];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(IEnumerable<Vec3d> vectors, double[] result)
        {
            GetCovarianceMatrix(vectors, vectors.Mean(), result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        /// <param name="mean"></param>
        public static void GetCovarianceMatrix(IEnumerable<Vec3d> vectors, Vec3d mean, double[] result)
        {
            Array.Clear(result, 0, 9);

            // calculate lower triangular covariance matrix
            foreach (Vec3d v in vectors)
            {
                Vec3d d = v - mean;
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
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(IEnumerable<double[]> vectors, double[] result)
        {
            var mean = new double[vectors.First().Length];
            vectors.Mean(mean);
            GetCovarianceMatrix(vectors, mean, result);
        }


        /// <summary>
        /// Returns the entries of the covariance matrix in column-major order.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="mean"></param>
        /// <param name="result"></param>
        public static void GetCovarianceMatrix(IEnumerable<double[]> vectors, double[] mean, double[] result)
        {
            int n = mean.Length;
            Array.Clear(result, 0, n * n);

            // calculate lower triangular covariance matrix
            foreach (double[] v in vectors)
            {
                for (int j = 0; j < n; j++)
                {
                    double dj = v[j] - mean[j];
                    result[j * n + j] += dj * dj; // diagonal entry

                    for (int k = j + 1; k < n; k++)
                        result[j * n + k] += dj * (v[k] - mean[k]);
                }
            }

            // fill out upper triangular
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * n + i] = result[i * n + j];
            }
        }


        /// <summary>
        /// Returns a new list with no coincident items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec2d> getPosition, double tolerance = 1.0e-8)
        {
            return RemoveCoincident(items, getPosition, out int[] indexMap, tolerance);
        }


        /// <summary>
        /// Returns a new list with no coincident items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec2d> getPosition, out int[] indexMap, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid2d<int>(items.Count << 1);
            return RemoveCoincident(items, getPosition, grid, out indexMap, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec2d> getPosition, HashGrid2d<int> grid, out int[] indexMap, double tolerance = 1.0e-8)
        {
            var result = new List<T>();
            var map = new int[items.Count];
            grid.BinScale = tolerance * RadiusToBinScale;

            // add points to result if no duplicates are found
            for (int i = 0; i < items.Count; i++)
            {
                var p0 = getPosition(items[i]);

                // search for concident in result
                foreach (int j in grid.Search(new Domain2d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[j]), tolerance))
                    {
                        map[i] = j;
                        goto EndFor;
                    }
                }

                // no coincident item found, add i to result
                grid.Insert(p0, result.Count);
                map[i] = result.Count;
                result.Add(items[i]);

                EndFor:;
            }

            indexMap = map;
            return result;
        }


        /// <summary>
        /// Returns a new list with no coincident items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec3d> getPosition, double tolerance = 1.0e-8)
        {
            return RemoveCoincident(items, getPosition, out int[] indexMap, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec3d> getPosition, out int[] indexMap, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid3d<int>(items.Count << 1);
            return RemoveCoincident(items, getPosition, grid, out indexMap, tolerance);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="indexMap"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IReadOnlyList<T> items, Func<T, Vec3d> getPosition, HashGrid3d<int> grid, out int[] indexMap, double tolerance = 1.0e-8)
        {
            var result = new List<T>();
            var map = new int[items.Count];
            grid.BinScale = tolerance * RadiusToBinScale;

            // add points to result if no duplicates are found
            for (int i = 0; i < items.Count; i++)
            {
                var p0 = getPosition(items[i]);

                // search for concident in result
                foreach (int j in grid.Search(new Domain3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[j]), tolerance))
                    {
                        map[i] = j;
                        goto EndFor;
                    }
                }

                // no coincident item found, add i to result
                grid.Insert(p0, result.Count);
                map[i] = result.Count;
                result.Add(items[i]);

                EndFor:;
            }

            indexMap = map;
            return result;
        }


        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> items, Func<T, Vec2d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid2d<int>(items.Count << 2);
            return GetFirstCoincident(items, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="grid"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> items, Func<T, Vec2d> getPosition, HashGrid2d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert
            for (int i = 0; i < items.Count; i++)
                grid.Insert(getPosition(items[i]), i);

            // search
            for (int i = 0; i < items.Count; i++)
            {
                var p0 = getPosition(items[i]);

                foreach (var j in grid.Search(new Domain2d(p0, tolerance)))
                {
                    if (i != j && p0.ApproxEquals(getPosition(items[j]), tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
            }
        }


        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> items, Func<T, Vec3d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid3d<int>(items.Count << 2);
            return GetFirstCoincident(items, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="grid"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> items, Func<T, Vec3d> getPosition, HashGrid3d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert
            for (int i = 0; i < items.Count; i++)
                grid.Insert(getPosition(items[i]), i);

            // search
            for (int i = 0; i < items.Count; i++)
            {
                var p0 = getPosition(items[i]);

                foreach (var j in grid.Search(new Domain3d(p0, tolerance)))
                {
                    if (i != j && p0.ApproxEquals(getPosition(items[j]), tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
            }
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid2d<int>(itemsB.Count << 2);
            return GetFirstCoincident(itemsA, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPosition(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPosition(itemsA[i]);

                foreach (var j in grid.Search(new Domain2d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(itemsB[j]), tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
            }
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid3d<int>(itemsB.Count << 2);
            return GetFirstCoincident(itemsA, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPosition(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPosition(itemsA[i]);

                foreach (var j in grid.Search(new Domain3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(itemsB[j]), tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
            }
        }


        /// <summary>
        /// For each item in A, returns the index of all coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid2d<int>(itemsB.Count << 1);
            return GetAllCoincident(itemsA, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPosition(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPosition(itemsA[i]);

                yield return grid.Search(new Domain2d(p0, tolerance))
                    .Where(j => p0.ApproxEquals(getPosition(itemsB[j]), tolerance));
            }
        }


        /// <summary>
        /// For each item in A, returns the index of all coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, double tolerance = 1.0e-8)
        {
            var grid = new HashGrid3d<int>(itemsB.Count << 1);
            return GetAllCoincident(itemsA, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<int> grid, double tolerance = 1.0e-8)
        {
            grid.BinScale = tolerance * RadiusToBinScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPosition(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPosition(itemsA[i]);

                yield return grid.Search(new Domain3d(p0, tolerance))
                    .Where(j => p0.ApproxEquals(getPosition(itemsB[j]), tolerance));
            }
        }
    }
}
