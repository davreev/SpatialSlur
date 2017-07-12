using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurData;

using static SpatialSlur.SlurCore.ArrayMath;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{ 
    /// <summary>
    /// 
    /// </summary>
    public static class ArrayExtensions
    {
        #region T[]

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] ShallowCopy<T>(this T[] array)
        {
            T[] result = new T[array.Length];
            Array.Copy(array, result, array.Length);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Set<T>(this T[] array, T value)
        {
            SetRange(array, value, 0, array.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Set<T>(this T[] array, T[] other)
        {
            Array.Copy(other, array, array.Length);
        }


        /// <summary>
        /// Assumes the length of the array is less than or equal to the number of elements in the given sequence.
        /// </summary>
        public static void Set<T>(this T[] array, IEnumerable<T> sequence)
        {
            var itr = sequence.GetEnumerator();

            for(int i = 0; i < array.Length; i++)
            {
                array[i] = itr.Current;
                itr.MoveNext();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public static void ClearRange<T>(this T[] array, int index, int count)
        {
            Array.Clear(array, index, count);
        }
        

        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this T[] array, T value, int index, int count)
        {
            for (int i = 0; i < count; i++)
                array[i + index] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this T[] array, T[] other, int count)
        {
            Array.Copy(other, 0, array, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this T[] array, T[] other, int thisIndex, int otherIndex, int count)
        {
            Array.Copy(other, otherIndex, array, thisIndex, count);
        }


        /// <summary>
        /// Assumes the specified range is less than or equal to the number of elements in the given sequence.
        /// </summary>
        public static void SetRange<T>(this T[] array, IEnumerable<T> sequence, int index, int count)
        {
            var itr = sequence.GetEnumerator();

            for (int i = 0; i < count; i++)
            {
                array[index + i] = itr.Current;
                itr.MoveNext();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetSelection<T>(this T[] array, T value, int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
                array[indices[i]] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetSelection<T>(this T[] array, T value, IEnumerable<int> indices)
        {
            foreach(int i in indices)
                array[i] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetSelection<T>(this T[] array, T[] other, int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
                array[indices[i]] = other[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static T[] GetRange<T>(this T[] array, int index, int count)
        {
            T[] result = new T[count];
            Array.Copy(array, index, result, 0, count);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetRange<T>(this T[] array, int index, int count, T[] result)
        {
            Array.Copy(array, index, result, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T[] GetSelection<T>(this T[] array, int[] indices)
        {
            T[] result = new T[indices.Length];
            array.GetSelection(indices, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetSelection<T>(this T[] array, int[] indices, T[] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[i] = array[indices[i]];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void Action<T>(this T[] source, Action<T> action)
        {
            ActionRange(source, 0, source.Length, action);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] Convert<T, U>(this T[] source, Func<T, U> converter)
        {
            return ConvertRange(source, 0, source.Length, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Convert<T, U>(this T[] source, Func<T, U> converter, U[] result)
        {
            ConvertRange(source, 0, source.Length, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] Convert<T, U>(this T[] source, Func<T, int, U> converter)
        {
            return ConvertRange(source, 0, source.Length, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Convert<T, U>(this T[] source, Func<T, int, U> converter, U[] result)
        {
            ConvertRange(source, 0, source.Length, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ActionParallel<T>(this T[] source, Action<T> action)
        {
            ActionRangeParallel(source, 0, source.Length, action);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertParallel<T, U>(this T[] source, Func<T, U> converter)
        {
            return ConvertRangeParallel(source, 0, source.Length, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertParallel<T, U>(this T[] source, Func<T, U> converter, U[] result)
        {
            ConvertRangeParallel(source, 0, source.Length, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertParallel<T, U>(this T[] source, Func<T, int, U> converter)
        {
            return ConvertRangeParallel(source, 0, source.Length, converter);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertParallel<T, U>(this T[] source, Func<T, int, U> converter, U[] result)
        {
            ConvertRangeParallel(source, 0, source.Length, converter, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public static void ActionRange<T>(this T[] source, int index, int count, Action<T> action)
        {
            for (int i = 0; i < count; i++)
                action(source[i + index]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRange<T, U>(this T[] source, int index, int count, Func<T, U> converter)
        {
            U[] result = new U[count];
            source.ConvertRange(index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRange<T, U>(this T[] source, int index, int count, Func<T, U> converter, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = converter(source[i + index]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRange<T, U>(this T[] source, int index, int count, Func<T, int, U> converter)
        {
            U[] result = new U[count];
            source.ConvertRange(index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRange<T, U>(this T[] source, int index, int count, Func<T, int, U> converter, U[] result)
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
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public static void ActionRangeParallel<T>(this T[] source, int index, int count, Action<T> action)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    action(source[i + index]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertRangeParallel<T, U>(this T[] source, int index, int count, Func<T, U> converter)
        {
            U[] result = new U[count];
            source.ConvertRangeParallel(index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRangeParallel<T, U>(this T[] source, int index, int count, Func<T, U> converter, U[] result)
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
        public static U[] ConvertRangeParallel<T, U>(this T[] source, int index, int count, Func<T, int, U> converter)
        {
            U[] result = new U[count];
            source.ConvertRangeParallel(index, count, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertRangeParallel<T, U>(this T[] source, int index, int count, Func<T, int, U> converter, U[] result)
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
        public static void ActionSelection<T>(this T[] source, int[] indices, Action<T> action)
        {
            for (int i = 0; i < indices.Length; i++)
                action(source[indices[i]]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelection<T, U>(this T[] source, int[] indices, Func<T, U> converter)
        {
            U[] result = new U[indices.Length];
            source.ConvertSelection(indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelection<T, U>(this T[] source, int[] indices, Func<T, U> converter, U[] result)
        {
            for (int i = 0; i < indices.Length; i++)
                result[i] = converter(source[indices[i]]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelection<T, U>(this T[] source, int[] indices, Func<T, int, U> converter)
        {
            U[] result = new U[indices.Length];
            source.ConvertSelection(indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelection<T, U>(this T[] source, int[] indices, Func<T, int, U> converter, U[] result)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                int j = indices[i];
                result[i] = converter(source[j], j);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ActionSelectionParallel<T>(this T[] source, int[] indices, Action<T> action)
        {
            Parallel.ForEach(Partitioner.Create(0, indices.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    action(source[indices[i]]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelectionParallel<T, U>(this T[] source, int[] indices, Func<T, U> converter)
        {
            U[] result = new U[indices.Length];
            source.ConvertSelectionParallel(indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelectionParallel<T, U>(this T[] source, int[] indices, Func<T, U> converter, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, indices.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = converter(source[indices[i]]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static U[] ConvertSelectionParallel<T, U>(this T[] source, int[] indices, Func<T, int, U> converter)
        {
            U[] result = new U[indices.Length];
            source.ConvertSelectionParallel(indices, converter, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ConvertSelectionParallel<T, U>(this T[] source, int[] indices, Func<T, int, U> converter, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, indices.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int j = indices[i];
                    result[i] = converter(source[j] , j);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> TakeRange<T>(this T[] source, int index, int count)
        {
            for (int i = 0; i < count; i++)
                yield return source[i + index];
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> TakeSelection<T>(this T[] source, IEnumerable<int> indices)
        {
            foreach (int i in indices)
                yield return source[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> TakeEveryNth<T>(this T[] source, int n)
        {
            return TakeEveryNth(source, n, 0, source.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> TakeEveryNth<T>(this T[] source, int n, int index, int count)
        {
            for (int i = 0; i < count; i += n)
                yield return source[i + index];
        }


        /// <summary>
        /// Allows enumeration over segments of the given list.
        /// </summary>
        public static IEnumerable<ReadOnlySubArray<T>> Segment<T>(this T[] source, IEnumerable<int> sizes)
        {
            int marker = 0;
            foreach (int n in sizes)
            {
                yield return new ReadOnlySubArray<T>(source, marker, n);
                marker += n;
            }
        }


        /// <summary>
        /// Swaps a pair of elements.
        /// </summary>
        public static void Swap<T>(this T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }


        /// <summary>
        /// Shuffles an array of items in place.
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            Shuffle(array, new Random());
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this T[] array, int seed)
        {
            Shuffle(array, new Random(seed));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this T[] array, Random random)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i);
                array.Swap(i, j);
            }
        }


        /// <summary>
        /// Shifts an array of items in place.
        /// </summary>
        public static void Shift<T>(this T[] array, int offset)
        {
            array.Shift(offset, 0, array.Length - 1);
        }


        /// <summary>
        /// Shifts a subset of an array of items in place.
        /// </summary>
        public static void Shift<T>(this T[] array, int offset, int from, int to)
        {
            offset = SlurMath.Mod2(offset, to - from + 1);
            Reverse(array, from, from + offset - 1);
            Reverse(array, from + offset, to);
            Reverse(array, from, to);
        }


        /// <summary>
        /// Reverses an array of items in place.
        /// </summary>
        public static void Reverse<T>(this T[] array)
        {
            Reverse(array, 0, array.Length - 1);
        }


        /// <summary>
        /// Reverses the order of the items within the specified range in place.
        /// </summary>
        public static void Reverse<T>(this T[] array, int from, int to)
        {
            while (to > from)
                array.Swap(from++, to--);
        }


        /// <summary>
        /// Equivalent of List.FindIndex for IList.
        /// </summary>
        public static int FindIndex<T>(this T[] array, Predicate<T> match)
        {
            return array.FindIndex(0, array.Length, match);
        }


        /// <summary>
        /// Equivalent of List.FindIndex for IList.
        /// </summary>
        public static int FindIndex<T>(this T[] array, int index, int length, Predicate<T> match)
        {
            for (int i = index; i < index + length; i++)
            {
                if (match(array[i]))
                    return i;
            }

            return -1;
        }


        /// <summary>
        /// Moves elements for which the given predicate returns true to the front of the list.
        /// Returns the index after the last used element.
        /// </summary>
        public static int Compact<T>(this T[] array, Predicate<T> include)
        {
            int marker = 0;

            for (int i = 0; i < array.Length; i++)
            {
                T t = array[i];
                if (include(t))
                    array[marker++] = t;
            }

            return marker;
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] array, int n)
            where T : IComparable<T>
        {
            return array.QuickSelect(n, 0, array.Length - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] array, int n, int from, int to)
            where T : IComparable<T>
        {
            return array.QuickSelect(n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] array, int n, IComparer<T> comparer)
        {
            return array.QuickSelect(n, 0, array.Length - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] array, int n, int from, int to, IComparer<T> comparer)
        {
            return array.QuickSelect(n, from, to, comparer.Compare);
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// </summary>
        public static T QuickSelect<T>(this T[] array, int n, Comparison<T> compare)
        {
            return array.QuickSelect(n, 0, array.Length - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this T[] array, int n, int from, int to, Comparison<T> compare)
        {
            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = array.Partition(from, to, compare);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return array[i];
            }
            return array[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<T>(this T[] array, int from, int to, Comparison<T> compare)
        {
            T pivot = array[from]; // get pivot element
            int i = from;
            int j = to + 1;

            while (true)
            {
                while (compare(pivot, array[++i]) > 0)
                    if (i == to) break;

                while (compare(pivot, array[--j]) < 0)
                    if (j == from) break;

                if (i >= j) break; // check if indices have crossed
                array.Swap(i, j);
            }

            // swap with pivot element
            array.Swap(from, j);
            return j;
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n)
            where K : IComparable<K>
        {
            return keys.QuickSelect(values, n, 0, keys.Length - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n, int from, int to)
            where K : IComparable<K>
        {
            return keys.QuickSelect(values, n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// Returns the nth smallest key in linear amortized time.
        /// Partially sorts the keys with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// Also partially sorts an array of corresponding values.
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n, IComparer<K> comparer)
        {
            return keys.QuickSelect(values, n, 0, keys.Length - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n, int from, int to, IComparer<K> comparer)
        {
            return keys.QuickSelect(values, n, from, to, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n, Comparison<K> compare)
        {
            return keys.QuickSelect(values, n, 0, keys.Length - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this K[] keys, V[] values, int n, int from, int to, Comparison<K> compare)
        {
            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = keys.Partition(values, from, to, compare);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return keys[i];
            }
            return keys[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<K, V>(this K[] keys, V[] values, int from, int to, Comparison<K> compare)
        {
            K pivot = keys[from]; // get pivot element
            int i = from;
            int j = to + 1;

            while (true)
            {
                while (compare(pivot, keys[++i]) > 0)
                    if (i == to) break;

                while (compare(pivot, keys[--j]) < 0)
                    if (j == from) break;

                if (i >= j) break; // check if indices have crossed
                keys.Swap(i, j);
                values.Swap(i, j);
            }

            // swap with pivot element
            keys.Swap(from, j);
            values.Swap(from, j);
            return j;
        }


        /// <summary>
        /// Sets the result to some function of the given vector
        /// </summary>
        public static void Function<T, U>(this T[] v,  Func<T, U> func, U[] result)
        {
            v.Function(v.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the given vector
        /// </summary>
        public static void Function<T, U>(this T[] v, int count, Func<T, U> func, U[] result)
        {
            v.ConvertRange(0, count, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void Function<T0, T1, U>(this T0[] v0, T1[] v1, Func<T0, T1, U> func, U[] result)
        {
            v0.Function(v1, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void Function<T0, T1, U>(this T0[] v0, T1[] v1, int count, Func<T0, T1, U> func, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = func(v0[i], v1[i]);
        }


        /// <summary>
        /// Sets the result to some function of the 3 given vectors.
        /// </summary>
        public static void Function<T0, T1, T2, U>(this T0[] v0, T1[] v1, T2[] v2, Func<T0, T1, T2, U> func, U[] result)
        {
            v0.Function(v1, v2, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 3 given vectors.
        /// </summary>
        public static void Function<T0, T1, T2, U>(this T0[] v0, T1[] v1, T2[] v2, int count, Func<T0, T1, T2, U> func, U[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = func(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// Sets the result to some function of a given vector.
        /// </summary>
        public static void FunctionParallel<T, U>(this T[] vector, Func<T, U> func, U[] result)
        {
            vector.FunctionParallel(vector.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of a given vector.
        /// </summary>
        public static void FunctionParallel<T, U>(this T[] vector, int count, Func<T, U> func, U[] result)
        {
            vector.ConvertRangeParallel(0, count, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, U>(this T0[] v0, T1[] v1, Func<T0, T1, U> func, U[] result)
        {
            v0.FunctionParallel(v1, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of the 2 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, U>(this T0[] v0, T1[] v1, int count, Func<T0, T1, U> func, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// Sets the result to some function of 3 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, T2, U>(this T0[] v0, T1[] v1, T2[] v2, Func<T0, T1, T2, U> func, U[] result)
        {
            v0.FunctionParallel(v1, v2, v0.Length, func, result);
        }


        /// <summary>
        /// Sets the result to some function of 3 given vectors.
        /// </summary>
        public static void FunctionParallel<T0, T1, T2, U>(this T0[] v0, T1[] v1, T2[] v2, int count, Func<T0, T1, T2, U> func, U[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = func(v0[i], v1[i], v2[i]);
            });
        }

        #endregion


        #region Color[]

        /// <summary>
        /// 
        /// </summary>
        public static Color Lerp(this Color[] colors, double t)
        {
            int last = colors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                return colors[0];
            else if (i >= last)
                return colors[last];

            return colors[i].LerpTo(colors[i + 1], t);
        }

        #endregion


        #region double[]

        /// <summary>
        /// 
        /// </summary>
        public static double Lerp(this double[] vector, double t)
        {
            int last = vector.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                return vector[0];
            else if (i >= last)
                return vector[last];

            return SlurMath.Lerp(vector[i], vector[i + 1], t);
        }

        #endregion


        #region Vec2d[]

        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Lerp(this Vec2d[] vectors, double t)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                return vectors[0];
            else if (i >= last)
                return vectors[last];

            return vectors[i].LerpTo(vectors[i + 1], t);
        }

        #endregion


        #region Vec3d[]

        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Lerp(this Vec3d[] vectors, double t)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                return vectors[0];
            else if (i >= last)
                return vectors[last];

            return vectors[i].LerpTo(vectors[i + 1], t);
        }

        #endregion


        #region double[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double t, double[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double t, int size, double[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double[] t, double[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[][] vectors, double[] t, int size, double[] result)
        {
            int last = vectors.Length - 1;

            for (int j = 0; j < size; j++)
            {
                double tj = SlurMath.Fract(t[j] * last, out int i);

                if (i < 0)
                    result[j] = vectors[0][j];
                else if (i >= last)
                    result[j] = vectors[last][j];
                else
                    result[j] = SlurMath.Lerp(vectors[i][j], vectors[i + 1][j], tj);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this double[][] vectors, double t, double[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this double[][] vectors, double t, int size, double[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.LerpParallel(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this double[][] vectors, double[] t, double[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this double[][] vectors, double[] t, int size, double[] result)
        {
            int last = vectors.Length - 1;

            Parallel.ForEach(Partitioner.Create(0, size), range =>
            {
                for (int j = range.Item1; j < range.Item2; j++)
                {
                    double tj = SlurMath.Fract((double)(t[j] * last), out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = SlurMath.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            });
        }

        #endregion


        #region Vec2d[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double t, Vec2d[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double t, int size, Vec2d[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double[] t, Vec2d[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec2d[][] vectors, double[] t, int size, Vec2d[] result)
        {
            int last = vectors.Length - 1;

            for (int j = 0; j < size; j++)
            {
                double tj = SlurMath.Fract(t[j] * last, out int i);

                if (i < 0)
                    result[j] = vectors[0][j];
                else if (i >= last)
                    result[j] = vectors[last][j];
                else
                    result[j] = Vec2d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec2d[][] vectors, double t, Vec2d[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec2d[][] vectors, double t, int size, Vec2d[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.LerpParallel(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec2d[][] vectors, double[] t, Vec2d[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec2d[][] vectors, double[] t, int size, Vec2d[] result)
        {
            int last = vectors.Length - 1;

            Parallel.ForEach(Partitioner.Create(0, size), range =>
            {
                for (int j = range.Item1; j < range.Item2; j++)
                {
                    double tj = SlurMath.Fract(t[j] * last, out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = Vec2d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            });
        }

        #endregion


        #region Vec3d[][]

        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double t, Vec3d[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double t, int size, Vec3d[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.Lerp(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double[] t, Vec3d[] result)
        {
            Lerp(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this Vec3d[][] vectors, double[] t, int size, Vec3d[] result)
        {
            int last = vectors.Length - 1;

            for (int j = 0; j < size; j++)
            {
                double tj = SlurMath.Fract(t[j] * last, out int i);

                if (i < 0)
                    result[j] = vectors[0][j];
                else if (i >= last)
                    result[j] = vectors[last][j];
                else
                    result[j] = Vec3d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec3d[][] vectors, double t, Vec3d[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec3d[][] vectors, double t, int size, Vec3d[] result)
        {
            int last = vectors.Length - 1;
            t = SlurMath.Fract(t * last, out int i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                ArrayMath.LerpParallel(vectors[i], vectors[i + 1], t, size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec3d[][] vectors, double[] t, Vec3d[] result)
        {
            LerpParallel(vectors, t, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpParallel(this Vec3d[][] vectors, double[] t, int size, Vec3d[] result)
        {
            int last = vectors.Length - 1;

            Parallel.ForEach(Partitioner.Create(0, size), range =>
            {
                for (int j = range.Item1; j < range.Item2; j++)
                {
                    double tj = SlurMath.Fract(t[j] * last, out int i);

                    if (i < 0)
                        result[j] = vectors[0][j];
                    else if (i >= last)
                        result[j] = vectors[last][j];
                    else
                        result[j] = Vec3d.Lerp(vectors[i][j], vectors[i + 1][j], tj);
                }
            });
        }

        #endregion
    }
}
