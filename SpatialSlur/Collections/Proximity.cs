
/*
 * Notes 
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class Proximity
    {
        private static double[] _pow10 = new double[] { 10e0, 10e1, 10e2, 10e3, 10e4, 10e5, 10e6, 10e7, 10e8 };
        private const double _radiusToGridScale = 5.0;


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getPosition"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vector2d> getPosition, int digits = 4)
        {
            return RemoveCoincident(source, getPosition, new HashSet<Vector2i>(), digits);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getPosition"></param>
        /// <param name="hash"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vector2d> getPosition, HashSet<Vector2i> hash, int digits = 4)
        {
            var scale = _pow10[digits];

            foreach (var item in source)
            {
                var key = (getPosition(item) * scale).As2i;

                if (!hash.Contains(key))
                {
                    hash.Add(key);
                    yield return item;
                }
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
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vector3d> getPosition, int digits = 4)
        {
            return RemoveCoincident(source, getPosition, new HashSet<Vector3i>(), digits);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="getPosition"></param>
        /// <param name="hash"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> source, Func<T, Vector3d> getPosition, HashSet<Vector3i> hash, int digits = 4)
        {
            var scale = _pow10[digits];
            
            foreach (var item in source)
            {
                var key = (getPosition(item) * scale).As3i;

                if (!hash.Contains(key))
                {
                    hash.Add(key);
                    yield return item;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector2d> getPosition, double tolerance = D.ZeroTolerance)
        {
            return RemoveCoincident(items, getPosition, new HashGrid2d<Vector2d>(), tolerance);
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
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector2d> getPosition, HashGrid2d<Vector2d> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;
            
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                foreach (var p1 in grid.Search(new Interval2d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(p1, tolerance))
                        goto EndFor;
                }

                // no coincident items found
                grid.Insert(p0, p0);
                yield return item;

                EndFor:;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getPosition"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector3d> getPosition, double tolerance = D.ZeroTolerance)
        {
            return RemoveCoincident(items, getPosition, new HashGrid3d<Vector3d>(), tolerance);
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
        public static IEnumerable<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector3d> getPosition, HashGrid3d<Vector3d> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // add points to result if no duplicates are found
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
            }
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
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector2d> getPosition, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            return RemoveCoincident(items, getPosition, new HashGrid2d<int>(), out indexMap, tolerance);
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
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector2d> getPosition, HashGrid2d<int> grid, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            var result = new List<T>();
            indexMap = new List<int>();
            grid.Scale = tolerance * _radiusToGridScale;

            // add points to result if no duplicates are found
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                // search for concident items in result
                foreach (int i in grid.Search(new Interval2d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[i]), tolerance))
                    {
                        indexMap.Add(i);
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
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector3d> getPosition, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            return RemoveCoincident(items, getPosition, new HashGrid3d<int>(), out indexMap, tolerance);
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
        public static List<T> RemoveCoincident<T>(IEnumerable<T> items, Func<T, Vector3d> getPosition, HashGrid3d<int> grid, out List<int> indexMap, double tolerance = D.ZeroTolerance)
        {
            var result = new List<T>();
            indexMap = new List<int>();
            grid.Scale = tolerance * _radiusToGridScale;

            // add points to result if no duplicates are found
            foreach (var item in items)
            {
                var p0 = getPosition(item);

                // search for concident items in result
                foreach (int i in grid.Search(new Interval3d(p0, tolerance)))
                {
                    if (p0.ApproxEquals(getPosition(result[i]), tolerance))
                    {
                        indexMap.Add(i);
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
        /// For each point, returns the index of the first coincident point within the same collection. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector2d> points, double tolerance = D.ZeroTolerance)
        {
            return GetFirstCoincident(points, new HashGrid2d<(Vector2d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point, returns the index of the first coincident point within the same collection. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector2d> points, HashGrid2d<(Vector2d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert
            {
                int i = 0;
                foreach (var p in points)
                    grid.Insert(p, (p, i++));
            }

            // search
            {
                int j = 0;
                foreach (var p0 in points)
                {
                    foreach (var (p1, i) in grid.Search(new Interval2d(p0, tolerance)))
                    {
                        if (j != i && p0.ApproxEquals(p1, tolerance))
                        {
                            yield return i;
                            goto EndFor;
                        }
                    }

                    yield return -1;
                    EndFor:;
                    j++;
                }
            }
        }


        /// <summary>
        /// For each point, returns the index of the first coincident point within the same collection. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector3d> points, double tolerance = D.ZeroTolerance)
        {
            return GetFirstCoincident(points, new HashGrid3d<(Vector3d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point, returns the index of the first coincident point within the same collection. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector3d> points, HashGrid3d<(Vector3d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert
            {
                int i = 0;
                foreach (var p in points)
                    grid.Insert(p, (p, i++));
            }

            // search
            {
                int j = 0;
                foreach (var p0 in points)
                {
                    foreach (var (p1, i) in grid.Search(new Interval3d(p0, tolerance)))
                    {
                        if (j != i && p0.ApproxEquals(p1, tolerance))
                        {
                            yield return i;
                            goto EndFor;
                        }
                    }

                    yield return -1;
                    EndFor:;
                    j++;
                }
            }
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector2d> pointsA, IEnumerable<Vector2d> pointsB, double tolerance = D.ZeroTolerance)
        {
            return GetFirstCoincident(pointsA, pointsB, new HashGrid2d<(Vector2d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector2d> pointsA, IEnumerable<Vector2d> pointsB, HashGrid2d<(Vector2d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert B
            {
                int i = 0;
                foreach (var pB in pointsB)
                    grid.Insert(pB, (pB, i++));
            }

            // search from A
            {
                foreach (var pA in pointsA)
                {
                    foreach (var (pB, i) in grid.Search(new Interval2d(pA, tolerance)))
                    {
                        if (pA.ApproxEquals(pB, tolerance))
                        {
                            yield return i;
                            goto EndFor;
                        }
                    }

                    yield return -1;
                    EndFor:;
                }
            }
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector3d> pointsA, IEnumerable<Vector3d> pointsB, double tolerance = D.ZeroTolerance)
        {
            return GetFirstCoincident(pointsA, pointsB, new HashGrid3d<(Vector3d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident item is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetFirstCoincident(IEnumerable<Vector3d> pointsA, IEnumerable<Vector3d> pointsB, HashGrid3d<(Vector3d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert B
            {
                int i = 0;
                foreach (var pB in pointsB)
                    grid.Insert(pB, (pB, i++));
            }

            // search from A
            {
                foreach (var pA in pointsA)
                {
                    foreach (var (pB, i) in grid.Search(new Interval3d(pA, tolerance)))
                    {
                        if (pA.ApproxEquals(pB, tolerance))
                        {
                            yield return i;
                            goto EndFor;
                        }
                    }

                    yield return -1;
                    EndFor:;
                }
            }
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident(IEnumerable<Vector2d> pointsA, IEnumerable<Vector2d> pointsB, double tolerance = D.ZeroTolerance)
        {
            return GetAllCoincident(pointsA, pointsB, new HashGrid2d<(Vector2d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident(IEnumerable<Vector2d> pointsA, IEnumerable<Vector2d> pointsB, HashGrid2d<(Vector2d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert B
            {
                int i = 0;
                foreach (var pB in pointsB)
                    grid.Insert(pB, (pB, i++));
            }

            // search from A
            {
                foreach (var pA in pointsA)
                {
                    yield return Filter(grid.Search(new Interval2d(pA, tolerance)));

                    IEnumerable<int> Filter(IEnumerable<(Vector2d, int)> source)
                    {
                        foreach (var (pB, i) in source)
                            if (pA.ApproxEquals(pB, tolerance)) yield return i;
                    }
                }
            }
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident(IEnumerable<Vector3d> pointsA, IEnumerable<Vector3d> pointsB, double tolerance = D.ZeroTolerance)
        {
            return GetAllCoincident(pointsA, pointsB, new HashGrid3d<(Vector3d, int)>(), tolerance);
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="grid"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<int>> GetAllCoincident(IEnumerable<Vector3d> pointsA, IEnumerable<Vector3d> pointsB, HashGrid3d<(Vector3d, int)> grid, double tolerance = D.ZeroTolerance)
        {
            grid.Scale = tolerance * _radiusToGridScale;

            // insert B
            {
                int i = 0;
                foreach (var pB in pointsB)
                    grid.Insert(pB, (pB, i++));
            }

            // search from A
            {
                foreach (var pA in pointsA)
                {
                    yield return Filter(grid.Search(new Interval3d(pA, tolerance)));

                    IEnumerable<int> Filter(IEnumerable<(Vector3d, int)> source)
                    {
                        foreach (var (pB, i) in source)
                            if (pA.ApproxEquals(pB, tolerance)) yield return i;
                    }
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
        public static bool Consolidate(IList<Vector2d> points, double radius, double tolerance = D.ZeroTolerance, int maxSteps = 4)
        {
            var grid = new HashGrid2d<Vector2d>(points.Count);
            grid.Scale = radius * _radiusToGridScale;

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
        public static bool Consolidate(IList<Vector3d> points, double radius, double tolerance = D.ZeroTolerance, int maxSteps = 4)
        {
            var grid = new HashGrid3d<Vector3d>(points.Count);
            grid.Scale = radius * _radiusToGridScale;

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
    }
}
