using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataUtil
    {
        /// <summary></summary>
        public const double RadiusToHashScale = 3.5;

     
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getPosition"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vec2d> getPosition, HashSet<Vec2i> hash = null, int digits = 4)
        {
            if(hash == null)
                hash = new HashSet<Vec2i>();

            var scale = SlurMath.PowersOfTen[digits];

            int i = 0;
            foreach (var item in source)
            {
                var key = (Vec2i)(getPosition(item) * scale);

                if(!hash.Contains(key))
                {
                    hash.Add(key);
                    yield return item;
                }
                    
                i++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getPosition"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vec3d> getPosition, HashSet<Vec3i> hash = null, int digits = 4)
        {
            if (hash == null)
                hash = new HashSet<Vec3i>();

            var scale = SlurMath.PowersOfTen[digits];

            int i = 0;
            foreach (var item in source)
            {
                var key = (Vec3i)(getPosition(item) * scale);

                if (!hash.Contains(key))
                {
                    hash.Add(key);
                    yield return item;
                }

                i++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vec2d> getPosition, HashGrid2d<Vec2d> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<Vec2d>();

            grid.Scale = tolerance * RadiusToHashScale;

            // add points to result if no duplicates are found
            int i = 0;
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                foreach (var p1 in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(p1, tolerance))
                        goto EndFor;
                }

                // no coincident items found
                grid.Insert(p0, p0);
                yield return item;

                EndFor:;
                i++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vec3d> getPosition, HashGrid3d<Vec3d> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<Vec3d>();

            grid.Scale = tolerance * RadiusToHashScale;

            // add points to result if no duplicates are found
            int i = 0;
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                foreach (var p1 in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(p1, tolerance))
                        goto EndFor;
                }

                // no coincident items found
                grid.Insert(p0, p0);
                yield return item;

                EndFor:;
                i++;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="indexMap"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vec2d> getPosition, out List<int> indexMap, HashGrid2d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<int>();

            var result = new List<T>();
            indexMap = new List<int>();
            grid.Scale = tolerance * RadiusToHashScale;

            // add points to result if no duplicates are found
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                // search for concident items in result
                foreach (int j in grid.Search(new Interval2d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[j]), tolerance))
                    {
                        indexMap.Add(j);
                        goto EndFor;
                    }
                }

                // no coincident items found
                grid.Insert(p0, result.Count);
                indexMap.Add(result.Count);
                result.Add(item);

                EndFor:;
            }

            return result;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="indexMap"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vec3d> getPosition, out List<int> indexMap, HashGrid3d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<int>();

            var result = new List<T>();
            indexMap = new List<int>();
            grid.Scale = tolerance * RadiusToHashScale;

            // add points to result if no duplicates are found
            foreach(var item in items)
            {
                var p0 = getPosition(item);

                // search for concident items in result
                foreach (int j in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[j]), tolerance))
                    {
                        indexMap.Add(j);
                        goto EndFor;
                    }
                }

                // no coincident items found
                grid.Insert(p0, result.Count);
                indexMap.Add(result.Count);
                result.Add(item);

                EndFor:;
            }
            
            return result;
        }
        

        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="grid"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IEnumerable<T> items, Func<T, Vec2d> getPosition, HashGrid2d<(Vec2d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<(Vec2d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert
            int i = 0;
            foreach (var item in items)
            {
                var p = getPosition(item);
                grid.Insert(p, (p, i));
                i++;
            }

            // search
            i = 0;
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                foreach (var (p1, j) in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (i != j && p0.ApproxEquals(p1, tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
                i++;
            }
        }
        

        /// <summary>
        /// For each item, returns the index of the first coincident item within the same list. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="grid"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IEnumerable<T> items, Func<T, Vec3d> getPosition, HashGrid3d<(Vec3d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<(Vec3d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert
            int i = 0;
            foreach (var item in items)
            {
                var p = getPosition(item);
                grid.Insert(p, (p, i));
                i++;
            }

            // search
            i = 0;
            foreach (var item in items)
            {
                var p0 = getPosition(item);
                
                foreach (var (p1, j) in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (i != j && p0.ApproxEquals(p1, tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
                i++;
            }
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IEnumerable<T> itemsA, IEnumerable<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<(Vec2d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetFirstCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T, U>(IEnumerable<T> itemsA, Func<T, Vec2d> getPositionA, IEnumerable<U> itemsB, Func<U, Vec2d> getPositionB, HashGrid2d<(Vec2d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<(Vec2d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            int i = 0;
            foreach (var b in itemsB)
            {
                var pB = getPositionB(b);
                grid.Insert(pB, (pB, i));
                i++;
            }

            // search from A
            i = 0;
            foreach (var a in itemsA)
            {
                var pA = getPositionA(a);

                foreach (var (pB, j) in grid.Search(new Interval3d(pA, tolerance)))
                {
                    if (pA.ApproxEquals(pB, tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
                i++;
            }
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IEnumerable<T> itemsA, IEnumerable<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<(Vec3d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetFirstCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T, U>(IEnumerable<T> itemsA, Func<T, Vec3d> getPositionA, IEnumerable<U> itemsB, Func<U, Vec3d> getPositionB, HashGrid3d<(Vec3d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<(Vec3d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            int i = 0;
            foreach (var b in itemsB)
            {
                var pB = getPositionB(b);
                grid.Insert(pB, (pB, i));
                i++;
            }

            // search from A
            i = 0;
            foreach (var a in itemsA)
            {
                var pA = getPositionA(a);

                foreach (var (pB, j) in grid.Search(new Interval3d(pA, tolerance)))
                {
                    if (pA.ApproxEquals(pB, tolerance))
                    {
                        yield return j;
                        goto EndFor;
                    }
                }

                yield return -1;
                EndFor:;
                i++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IEnumerable<T> itemsA, IEnumerable<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<(Vec2d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetAllCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T, U>(IEnumerable<T> itemsA, Func<T, Vec2d> getPositionA, IEnumerable<U> itemsB, Func<U, Vec2d> getPositionB, HashGrid2d<(Vec2d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<(Vec2d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            int i = 0;
            foreach (var b in itemsB)
            {
                var pB = getPositionB(b);
                grid.Insert(pB, (pB, i));
                i++;
            }

            // search from A
            i = 0;
            foreach( var a in itemsA)
            {
                var pA = getPositionA(a);
                yield return Filter(grid.Search(new Interval2d(pA, tolerance)));

                IEnumerable<int> Filter(IEnumerable<(Vec2d, int)> source)
                {
                    foreach (var (pB, j) in source)
                        if (pA.ApproxEquals(pB, tolerance)) yield return j;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IEnumerable<T> itemsA, IEnumerable<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<(Vec3d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetAllCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T, U>(IEnumerable<T> itemsA, Func<T, Vec3d> getPositionA, IEnumerable<U> itemsB, Func<U, Vec3d> getPositionB, HashGrid3d<(Vec3d, int)> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<(Vec3d, int)>();

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            int i = 0;
            foreach (var b in itemsB)
            {
                var pB = getPositionB(b);
                grid.Insert(pB, (pB, i));
                i++;
            }

            // search from A
            i = 0;
            foreach (var a in itemsA)
            {
                var pA = getPositionA(a);
                yield return Filter(grid.Search(new Interval3d(pA, tolerance)));

                IEnumerable<int> Filter(IEnumerable<(Vec3d, int)> source)
                {
                    foreach (var (pB, j) in source)
                        if (pA.ApproxEquals(pB, tolerance)) yield return j;
                }
            }
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
        public static bool ConsolidatePoints(IList<Vec2d> points, double radius, double tolerance = SlurMath.ZeroTolerance, int maxSteps = 4)
        {
            var grid = new HashGrid3d<Vec2d>(radius * RadiusToHashScale, points.Count);

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
                    var p1 = grid.Search(new Interval2d(p0, radius))
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
        public static bool ConsolidatePoints(IList<Vec3d> points, double radius, double tolerance = SlurMath.ZeroTolerance, int maxSteps = 4)
        {
            var grid = new HashGrid3d<Vec3d>(radius * RadiusToHashScale, points.Count);

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
                    var p1 = grid.Search(new Interval3d(p0, radius))
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
            int dim = mean.Length;
            int n = 0;

            Array.Clear(result, 0, dim * dim);

            // calculate lower triangular values
            foreach (double[] v in vectors)
            {
                for (int i = 0; i < dim; i++)
                {
                    double di = v[i] - mean[i];

                    for (int j = i; j < dim; j++)
                        result[i * dim + j] += di * (v[j] - mean[j]);
                }

                n++;
            }

            var t = 1.0 / n;

            // average lower triangular values
            for(int i = 0; i < dim; i++)
            {
                for(int j = i; j < dim; j++)
                    result[i * dim + j] *= t;
            }
            
            // set symmetric values
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < i; j++)
                    result[j * dim + i] = result[i * dim + j];
            }
        }


#if OBSOLETE
        
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
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetFirstCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T, U>(IReadOnlyList<T> itemsA, Func<T, Vec2d> getPositionA, IReadOnlyList<U> itemsB, Func<U, Vec2d> getPositionB, HashGrid2d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid2d<int>(itemsB.Count);

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPositionB(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPositionA(itemsA[i]);

                foreach (var j in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPositionB(itemsB[j]), tolerance))
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
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetFirstCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of the first coincident item in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident<T, U>(IReadOnlyList<T> itemsA, Func<T, Vec3d> getPositionA, IReadOnlyList<U> itemsB, Func<U, Vec3d> getPositionB, HashGrid3d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if (grid == null)
                grid = new HashGrid3d<int>(itemsB.Count);

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPositionB(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPositionA(itemsA[i]);

                foreach (var j in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPositionB(itemsB[j]), tolerance))
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
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPosition"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec2d> getPosition, HashGrid2d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetAllCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T, U>(IReadOnlyList<T> itemsA, Func<T, Vec2d> getPositionA, IReadOnlyList<U> itemsB, Func<U, Vec2d> getPositionB, HashGrid2d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if(grid == null)
                grid = new HashGrid2d<int>(itemsB.Count);

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPositionB(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPositionA(itemsA[i]);

                yield return grid.Search(new Interval2d(p0, tolerance))
                    .Where(j => p0.ApproxEquals(getPositionB(itemsB[j]), tolerance));
            }
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
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T>(IReadOnlyList<T> itemsA, IReadOnlyList<T> itemsB, Func<T, Vec3d> getPosition, HashGrid3d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            return GetAllCoincident(itemsA, getPosition, itemsB, getPosition, grid, tolerance);
        }


        /// <summary>
        /// For each item in A, returns the index of each coincident items in B.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="itemsA"></param>
        /// <param name="getPositionA"></param>
        /// <param name="itemsB"></param>
        /// <param name="getPositionB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident<T, U>(IReadOnlyList<T> itemsA, Func<T, Vec3d> getPositionA, IReadOnlyList<U> itemsB, Func<U, Vec3d> getPositionB, HashGrid3d<int> grid = null, double tolerance = SlurMath.ZeroTolerance)
        {
            if(grid == null)
                grid = new HashGrid3d<int>(itemsB.Count);

            grid.Scale = tolerance * RadiusToHashScale;

            // insert B
            for (int i = 0; i < itemsB.Count; i++)
                grid.Insert(getPositionB(itemsB[i]), i);

            // search from A
            for (int i = 0; i < itemsA.Count; i++)
            {
                var p0 = getPositionA(itemsA[i]);

                yield return grid.Search(new Interval3d(p0, tolerance))
                    .Where(j => p0.ApproxEquals(getPositionB(itemsB[j]), tolerance));
            }
        }

#endif
    }
}
