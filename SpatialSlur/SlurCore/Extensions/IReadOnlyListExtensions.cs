using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurData;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        #region IReadOnlyList<T>

        /// <summary>
        /// 
        /// </summary>
        public static T[] GetRange<T>(this IReadOnlyList<T> list, int index, int count)
        {
            if (list is T[])
                return ArrayExtensions.GetRange((T[])list, index, count);

            T[] result = new T[count];
            GetRangeImpl(list, index, count, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetRange<T>(this IReadOnlyList<T> list, int index, int count, IList<T> result)
        {
            if (list is T[] && result is T[])
                ArrayExtensions.GetRange((T[])list, index, count, (T[])result);
            else
                GetRangeImpl(list, index, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetRangeImpl<T>(IReadOnlyList<T> list, int index, int count, IList<T> result)
        {
            for (int i = 0; i < count; i++)
                result[i] = list[i + index];
        }


        /// <summary>
        /// 
        /// </summary>
        public static T[] GetSelection<T>(this IReadOnlyList<T> list, IReadOnlyList<int> indices)
        {
            if (list is T[] && indices is int[])
                return ArrayExtensions.GetSelection((T[])list, (int[])indices);

            T[] result = new T[indices.Count];
            GetSelectionImpl(list, indices, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetSelection<T>(this IReadOnlyList<T> list, IReadOnlyList<int> indices, IList<T> result)
        {
            if (list is T[] && indices is int[] && result is T[])
                ArrayExtensions.GetSelection((T[])list, (int[])indices, (T[])result);
            else
                GetSelectionImpl(list, indices, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void GetSelectionImpl<T>(IReadOnlyList<T> list, IReadOnlyList<int> indices, IList<T> result)
        {
            for (int i = 0; i < indices.Count; i++)
                result[i] = list[indices[i]];
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] Convert<T, U>(this IReadOnlyList<T> source, Func<T, U> converter)
        {
            return ConvertRange(source, 0, source.Count, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Convert<T, U>(this IReadOnlyList<T> source, Func<T, U> converter, IList<U> result)
        {
            ConvertRange(source, 0, source.Count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] Convert<T, U>(this IReadOnlyList<T> source, Func<T, int, U> converter)
        {
            return ConvertRange(source, 0, source.Count, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Convert<T, U>(this IReadOnlyList<T> source, Func<T, int, U> converter, IList<U> result)
        {
            ConvertRange(source, 0, source.Count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertParallel<T, U>(this IReadOnlyList<T> source, Func<T, U> converter)
        {
            return ConvertRangeParallel(source, 0, source.Count, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertParallel<T, U>(this IReadOnlyList<T> source, Func<T, U> converter, IList<U> result)
        {
            ConvertRangeParallel(source, 0, source.Count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertParallel<T, U>(this IReadOnlyList<T> source, Func<T, int, U> converter)
        {
            return ConvertRangeParallel(source, 0, source.Count, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertParallel<T, U>(this IReadOnlyList<T> source, Func<T, int, U> converter, IList<U> result)
        {
            ConvertRangeParallel(source, 0, source.Count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRange<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, U> converter)
        {
            if (source is T[])
                return ArrayExtensions.ConvertRange((T[])source, index, count, converter);

            U[] result = new U[count];
            ConvertRangeImpl(source, index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRange<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, U> converter, IList<U> result)
        {
            if (source is T[] && result is U[])
                ArrayExtensions.ConvertRange((T[])source, index, count, converter, (U[])result);
            else
                ConvertRangeImpl(source, index, count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertRangeImpl<T,U>(IReadOnlyList<T> source, int index, int count, Func<T, U> converter, IList<U> result)
        {
            for (int i = 0; i < count; i++)
                result[i] = converter(source[i + index]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRange<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter)
        {
            if (source is T[])
                return ArrayExtensions.ConvertRange((T[])source, index, count, converter);

            U[] result = new U[count];
            ConvertRangeImpl(source, index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRange<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter, IList<U> result)
        {
            if (source is T[] && result is U[])
                ArrayExtensions.ConvertRange((T[])source, index, count, converter, (U[])result);
            else
                ConvertRangeImpl(source, index, count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertRangeImpl<T, U>(IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter, IList<U> result)
        {
            for (int i = 0; i < count; i++)
            {
                int j = i + index;
                result[i] = converter(source[j], j);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRangeParallel<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, U> converter)
        {
            if (source is T[])
                return ArrayExtensions.ConvertRangeParallel((T[])source, index, count, converter);

            U[] result = new U[count];
            ConvertRangeParallelImpl(source, index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRangeParallel<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, U> converter, IList<U> result)
        {
            if (source is T[] && result is U[])
                ArrayExtensions.ConvertRangeParallel((T[])source, index, count, converter, (U[])result);
            else
                ConvertRangeParallelImpl(source, index, count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertRangeParallelImpl<T, U>(IReadOnlyList<T> source, int index, int count, Func<T, U> converter, IList<U> result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = converter(source[i + index]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRangeParallel<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter)
        {
            if (source is T[])
                return ArrayExtensions.ConvertRangeParallel((T[])source, index, count, converter);

            U[] result = new U[count];
            ConvertRangeParallelImpl(source, index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRangeParallel<T, U>(this IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter, IList<U> result)
        {
            if (source is T[] && result is U[])
                ArrayExtensions.ConvertRangeParallel((T[])source, index, count, converter, (U[])result);
            else
                ConvertRangeParallelImpl(source, index, count, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertRangeParallelImpl<T, U>(IReadOnlyList<T> source, int index, int count, Func<T, int, U> converter, IList<U> result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = i + index;
                    result[i] = converter(source[j], j);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelection<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter)
        {
            if (source is T[] && indices is int[])
                return ArrayExtensions.ConvertSelection((T[])source, (int[])indices, converter);

            U[] result = new U[indices.Count];
            ConvertSelectionImpl(source, indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelection<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter, IList<U> result)
        {
            if (source is T[] && indices is int[] && result is U[])
                ArrayExtensions.ConvertSelection((T[])source, (int[])indices, converter, (U[])result);
            else
                ConvertSelectionImpl(source, indices, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertSelectionImpl<T, U>(IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter, IList<U> result)
        {
            for (int i = 0; i < indices.Count; i++)
                result[i] = converter(source[indices[i]]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelection<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter)
        {
            if (source is T[] && indices is int[])
                return ArrayExtensions.ConvertSelection((T[])source, (int[])indices, converter);

            U[] result = new U[indices.Count];
            ConvertSelectionImpl(source, indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelection<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter, IList<U> result)
        {
            if (source is T[] && indices is int[] && result is U[])
                ArrayExtensions.ConvertSelection((T[])source, (int[])indices, converter, (U[])result);
            else
                ConvertSelectionImpl(source, indices, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertSelectionImpl<T, U>(IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter, IList<U> result)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                int j = indices[i];
                result[i] = converter(source[j], j);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelectionParallel<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter)
        {
            if (source is T[] && indices is int[])
                return ArrayExtensions.ConvertSelectionParallel((T[])source, (int[])indices, converter);

            U[] result = new U[indices.Count];
            ConvertSelectionParallelImpl(source, indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelectionParallel<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter, IList<U> result)
        {
            if (source is T[] && indices is int[] && result is U[])
                ArrayExtensions.ConvertSelectionParallel((T[])source, (int[])indices, converter, (U[])result);
            else
                ConvertSelectionParallelImpl(source, indices, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertSelectionParallelImpl<T, U>(IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, U> converter, IList<U> result)
        {
            Parallel.ForEach(Partitioner.Create(0, indices.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = converter(source[indices[i]]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelectionParallel<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter)
        {
            if (source is T[] && indices is int[])
                return ArrayExtensions.ConvertSelectionParallel((T[])source, (int[])indices, converter);

            U[] result = new U[indices.Count];
            ConvertSelectionParallelImpl(source, indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelectionParallel<T, U>(this IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter, IList<U> result)
        {
            if (source is T[] && indices is int[] && result is U[])
                ArrayExtensions.ConvertSelectionParallel((T[])source, (int[])indices, converter, (U[])result);
            else
                ConvertSelectionParallelImpl(source, indices, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void ConvertSelectionParallelImpl<T, U>(IReadOnlyList<T> source, IReadOnlyList<int> indices, Func<T, int, U> converter, IList<U> result)
        {
            Parallel.ForEach(Partitioner.Create(0, indices.Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = indices[i];
                    result[i] = converter(source[j], j);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> EveryNth<T>(this IReadOnlyList<T> source, int n)
        {
            return EveryNth(source, n, 0, source.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> EveryNth<T>(this IReadOnlyList<T> source, int n, int index, int count)
        {
            for (int i = 0; i < count; i += n)
                yield return source[i + index];
        }


        /// <summary>
        /// Allows enumeration over segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlySubList<T>> Segment<T>(this IReadOnlyList<T> source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return new ReadOnlySubList<T>(source, marker, n);
                marker += n;
            }
        }

        #endregion


        #region IReadOnlyList<Color>

        /// <summary>
        /// 
        /// </summary>
        public static Color Lerp(this IReadOnlyList<Color> colors, double t)
        {
            int last = colors.Count - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                return colors[0];
            else if (i >= last)
                return colors[last];

            return colors[i].LerpTo(colors[i + 1], t);
        }

        #endregion


        #region IReadOnlyList<double>

        /// <summary>
        /// 
        /// </summary>
        public static double Lerp(this IReadOnlyList<double> vector, double t)
        {
            int last = vector.Count - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                return vector[0];
            else if (i >= last)
                return vector[last];

            return SlurMath.Lerp(vector[i], vector[i + 1], t);
        }

        #endregion


        #region IReadOnlyList<Vec2d>

        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Lerp(this IReadOnlyList<Vec2d> vectors, double t)
        {
            int last = vectors.Count - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                return vectors[0];
            else if (i >= last)
                return vectors[last];

            return vectors[i].LerpTo(vectors[i + 1], t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(this IReadOnlyList<Vec2d> points, double epsilon)
        {
            int[] indexMap;
            SpatialHash2d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(this IReadOnlyList<Vec2d> points, double epsilon, out int[] indexMap)
        {
            SpatialHash2d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static List<Vec2d> RemoveDuplicates(this IReadOnlyList<Vec2d> points, double epsilon, out int[] indexMap, out SpatialHash2d<int> hash)
        {
            List<Vec2d> result = new List<Vec2d>();
            indexMap = new int[points.Count];

            hash = new SpatialHash2d<int>(points.Count * 4, epsilon * 2.0);
            List<int> foundIds = new List<int>();
            Vec2d offset = new Vec2d(epsilon, epsilon);

            // add points to result if no duplicates are found in the hash
            for (int i = 0; i < points.Count; i++)
            {
                Vec2d p = points[i];
                hash.Search(new Domain2d(p - offset, p + offset), foundIds);

                // check found ids
                bool isDup = false;
                foreach (int j in foundIds)
                {
                    if (p.ApproxEquals(result[j], epsilon))
                    {
                        indexMap[i] = j;
                        isDup = true;
                        break;
                    }
                }
                foundIds.Clear();

                // if no duplicate, add to result and hash
                if (!isDup)
                {
                    int id = result.Count;
                    indexMap[i] = id;
                    hash.Insert(p, id);
                    result.Add(p);
                }
            }

            return result;
        }


        /// <summary>
        /// For each point, returns the index of the first coincident point within the list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec2d> points, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetFirstCoincident(points, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec2d> points, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int n = points.Count;
            int[] result = new int[n];
            var hash_ = new SpatialHash2d<int>(n * 4, epsilon * 2.0);

            // insert points into spatial hash
            for (int i = 0; i < n; i++)
                hash_.Insert(points[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
             {
                 List<int> foundIds = new List<int>();
                 Vec2d offset = new Vec2d(epsilon, epsilon);

                 for (int i = range.Item1; i < range.Item2; i++)
                 {
                     Vec2d p = points[i];
                     hash_.Search(new Domain2d(p - offset, p + offset), foundIds);

                     int coinId = -1;
                     foreach (int j in foundIds)
                     {
                         if (j == i) continue; // ignore self coincidence

                         if (p.ApproxEquals(points[j], epsilon))
                         {
                             coinId = j;
                             break;
                         }
                     }

                     result[i] = coinId;
                     foundIds.Clear();
                 }
             };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, n), func);
            else
                func(Tuple.Create(0, n));

            hash = hash_;
            return result;
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec2d> pointsA, IReadOnlyList<Vec2d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetFirstCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec2d> pointsA, IReadOnlyList<Vec2d> pointsB, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            int[] result = new int[nA];
            var hash_ = new SpatialHash2d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash_.Insert(pointsB[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
            {
                List<int> foundIds = new List<int>();
                Vec2d offset = new Vec2d(epsilon, epsilon);

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vec2d p = pointsA[i];
                    hash_.Search(new Domain2d(p - offset, p + offset), foundIds);

                    int coinId = -1;
                    foreach (int id in foundIds)
                    {
                        if (p.ApproxEquals(pointsB[id], epsilon))
                        {
                            coinId = id;
                            break;
                        }
                    }

                    result[i] = coinId;
                    foundIds.Clear();
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nA), func);
            else
                func(Tuple.Create(0, nA));

            hash = hash_;
            return result;
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// Note that the resulting list of indices for each point in A may contain duplicate entries.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(this IReadOnlyList<Vec2d> pointsA, IReadOnlyList<Vec2d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash2d<int> hash;
            return GetAllCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(this IReadOnlyList<Vec2d> pointsA, IReadOnlyList<Vec2d> pointsB, double epsilon, out SpatialHash2d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            List<int>[] result = new List<int>[nA];
            var hash_ = new SpatialHash2d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash_.Insert(pointsB[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
            {
                Vec2d offset = new Vec2d(epsilon, epsilon);

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vec2d p = pointsA[i];
                    List<int> coinIds = new List<int>();

                    hash_.Search(new Domain2d(p - offset, p + offset), ids =>
                    {
                        foreach (int id in ids)
                        {
                            if (p.ApproxEquals(pointsB[id], epsilon))
                                coinIds.Add(id);
                        }
                    });

                    result[i] = coinIds;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nA), func);
            else
                func(Tuple.Create(0, nA));

            hash = hash_;
            return result;
        }

        #endregion


        #region IReadOnlyList<Vec3d>

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Lerp(this IReadOnlyList<Vec3d> vectors, double t)
        {
            int last = vectors.Count - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                return vectors[0];
            else if (i >= last)
                return vectors[last];

            return vectors[i].LerpTo(vectors[i + 1], t);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(this IReadOnlyList<Vec3d> points, double epsilon)
        {
            int[] indexMap;
            SpatialHash3d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(this IReadOnlyList<Vec3d> points, double epsilon, out int[] indexMap)
        {
            SpatialHash3d<int> hash;
            return RemoveDuplicates(points, epsilon, out indexMap, out hash);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="hash"></param>
        /// <param name="epsilon"></param>
        /// <param name="indexMap"></param>
        /// <returns></returns>
        public static List<Vec3d> RemoveDuplicates(this IReadOnlyList<Vec3d> points, double epsilon, out int[] indexMap, out SpatialHash3d<int> hash)
        {
            List<Vec3d> result = new List<Vec3d>();
            indexMap = new int[points.Count];

            hash = new SpatialHash3d<int>(points.Count * 4, epsilon * 2.0);
            List<int> foundIds = new List<int>();
            Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

            // add points to result if no duplicates are found in the hash
            for (int i = 0; i < points.Count; i++)
            {
                Vec3d p = points[i];
                hash.Search(new Domain3d(p - offset, p + offset), foundIds);

                bool isDup = false;
                foreach (int j in foundIds)
                {
                    if (p.ApproxEquals(result[j], epsilon))
                    {
                        indexMap[i] = j;
                        isDup = true;
                        break;
                    }
                }
                foundIds.Clear();

                // if no duplicate, add to result and hash
                if (!isDup)
                {
                    int id = result.Count;
                    indexMap[i] = id;
                    hash.Insert(p, id);
                    result.Add(p);
                }
            }

            return result;
        }


        /// <summary>
        /// For each point, returns the index of the first coincident point within the list. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec3d> points, double epsilon, bool parallel = false)
        {
            SpatialHash3d<int> hash;
            return GetFirstCoincident(points, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec3d> points, double epsilon, out SpatialHash3d<int> hash, bool parallel = false)
        {
            int n = points.Count;
            int[] result = new int[n];
            var hash_ = new SpatialHash3d<int>(n * 4, epsilon * 2.0);

            // insert points into spatial hash
            for (int i = 0; i < n; i++)
                hash_.Insert(points[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
             {
                 List<int> foundIds = new List<int>();
                 Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

                 for (int i = range.Item1; i < range.Item2; i++)
                 {
                     Vec3d p = points[i];
                     hash_.Search(new Domain3d(p - offset, p + offset), foundIds);

                     int coinId = -1;
                     foreach (int j in foundIds)
                     {
                         if (j == i) continue; // ignore coincidence with self

                         if (p.ApproxEquals(points[j], epsilon))
                         {
                             coinId = j;
                             break;
                         }
                     }

                     result[i] = coinId;
                     foundIds.Clear();
                 }
             };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, n), func);
            else
                func(Tuple.Create(0, n));

            hash = hash_;
            return result;
        }


        /// <summary>
        /// For each point in A, returns the index of the first coincident point in B. If no coincident point is found, -1 is returned.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec3d> pointsA, IReadOnlyList<Vec3d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash3d<int> hash;
            return GetFirstCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static int[] GetFirstCoincident(this IReadOnlyList<Vec3d> pointsA, IReadOnlyList<Vec3d> pointsB, double epsilon, out SpatialHash3d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            int[] result = new int[nA];
            var hash_ = new SpatialHash3d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash_.Insert(pointsB[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
            {
                List<int> foundIds = new List<int>();
                Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vec3d p = pointsA[i];
                    hash_.Search(new Domain3d(p - offset, p + offset), foundIds);

                    int coinId = -1;
                    foreach (int id in foundIds)
                    {
                        if (p.ApproxEquals(pointsB[id], epsilon))
                        {
                            coinId = id;
                            break;
                        }
                    }

                    result[i] = coinId;
                    foundIds.Clear();
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nA), func);
            else
                func(Tuple.Create(0, nA));

            hash = hash_;
            return result;
        }


        /// <summary>
        /// For each point in A, returns the index of all coincident points in B.
        /// Note that the resulting list of indices for each point in A may contain duplicate entries.
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(this IReadOnlyList<Vec3d> pointsA, IReadOnlyList<Vec3d> pointsB, double epsilon, bool parallel = false)
        {
            SpatialHash3d<int> hash;
            return GetAllCoincident(pointsA, pointsB, epsilon, out hash, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointsA"></param>
        /// <param name="pointsB"></param>
        /// <param name="epsilon"></param>
        /// <param name="hash"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static List<int>[] GetAllCoincident(this IReadOnlyList<Vec3d> pointsA, IReadOnlyList<Vec3d> pointsB, double epsilon, out SpatialHash3d<int> hash, bool parallel = false)
        {
            int nA = pointsA.Count;
            int nB = pointsB.Count;
            List<int>[] result = new List<int>[nA];
            var hash_ = new SpatialHash3d<int>(nB * 4, epsilon * 2.0);

            // insert points
            for (int i = 0; i < nB; i++)
                hash_.Insert(pointsB[i], i);

            // search for collisions
            Action<Tuple<int, int>> func = range =>
             {
                 Vec3d offset = new Vec3d(epsilon, epsilon, epsilon);

                 for (int i = range.Item1; i < range.Item2; i++)
                 {
                     Vec3d p = pointsA[i];
                     List<int> coinIds = new List<int>();

                     hash_.Search(new Domain3d(p - offset, p + offset), ids =>
                     {
                         foreach (int id in ids)
                         {
                             if (p.ApproxEquals(pointsB[id], epsilon))
                                 coinIds.Add(id);
                         }
                     });

                     result[i] = coinIds;
                 }
             };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, nA), func);
            else
                func(Tuple.Create(0, nA));

            hash = hash_;
            return result;
        }

        #endregion


        #region IReadOnlyList<double[]>

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this IReadOnlyList<double[]> vectors, double t, double[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this IReadOnlyList<double[]> vectors, double t, int size, double[] result)
        {
            int last = vectors.Count - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayExtensions.LerpTo(vectors[i], vectors[i + 1], t, size, result);
        }

        #endregion
    }
}
