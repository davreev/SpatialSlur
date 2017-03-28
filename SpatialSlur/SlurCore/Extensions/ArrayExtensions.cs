using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
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
            other.CopyTo(array, 0);
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
        public static void SetRange<T>(this T[] array, T[] other, int thisIndex, int otherIndex, int count)
        {
            Array.Copy(other, otherIndex, array, thisIndex, count);
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
        public static IEnumerable<T> Range<T>(this T[] source, int index, int count)
        {
            for (int i = 0; i < count; i++)
                yield return source[i + index];
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> EveryNth<T>(this T[] source, int n)
        {
            return EveryNth(source, n, 0, source.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> EveryNth<T>(this T[] source, int n, int index, int count)
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
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n)
            where T : IComparable<T>
        {
            return keys.QuickSelect(items, n, 0, keys.Length - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n, int from, int to)
            where T : IComparable<T>
        {
            return keys.QuickSelect(items, n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// Also sorts an array of corresponding items.
        /// </summary>
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n, IComparer<T> comparer)
        {
            return keys.QuickSelect(items, n, 0, keys.Length - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n, int from, int to, IComparer<T> comparer)
        {
            return keys.QuickSelect(items, n, from, to, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n, Comparison<T> compare)
        {
            return keys.QuickSelect(items, n, 0, keys.Length - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this T[] keys, U[] items, int n, int from, int to, Comparison<T> compare)
        {
            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = keys.Partition(items, from, to, compare);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return keys[i];
            }
            return keys[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<T, U>(this T[] keys, U[] items, int from, int to, Comparison<T> compare)
        {
            T pivot = keys[from]; // get pivot element
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
                items.Swap(i, j);
            }

            // swap with pivot element
            keys.Swap(from, j);
            items.Swap(from, j);
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
            Parallel.ForEach(Partitioner.Create(0, v0.Length), range =>
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

            int i;
            t = SlurMath.Fract(t * last, out i);

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
        public static bool ApproxEquals(this double[] v0, double[] v1, double epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[] v0, double[] v1, double epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[] v0, double[] v1, double[] epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[] v0, double[] v1, double[] epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (Math.Abs(v1[i] - v0[i]) >= epsilon[i]) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this double[] vector)
        {
            return Sum(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Sum(this double[] vector, int count)
        {
            double sum = 0.0;

            for (int i = 0; i < count; i++)
                sum += vector[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this double[] vector)
        {
            return Sum(vector) / vector.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Mean(this double[] vector, int count)
        {
            return Sum(vector, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(this double[] vector, double[] weights)
        {
            return WeightedSum(vector, weights, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedSum(this double[] vector, double[] weights, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += vector[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(this double[] vector, double[] weights)
        {
            return WeightedMean(vector, weights, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double WeightedMean(this double[] vector, double[] weights, int count)
        {
            double sum = 0.0;
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vector[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(this double[] vector)
        {
            return Max(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Max(this double[] vector, int count)
        {
            double result = vector[0];

            for (int i = 1; i < count; i++)
            {
                double t = vector[i];
                if (t > result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this double[] v0, double[] v1, double[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this double[] v0, double[] v1, double[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Max(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(this double[] vector)
        {
            return Min(vector, vector.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Min(this double[] vector, int count)
        {
            double result = vector[0];

            for (int i = 1; i < count; i++)
            {
                double t = vector[i];
                if (t < result) result = t;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this double[] v0, double[] v1, double[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this double[] v0, double[] v1, double[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Min(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this double[] vector, double[] result)
        {
            Abs(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this double[] vector, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Math.Abs(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this double[] vector, double[] result)
        {
            AbsParallel(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this double[] vector, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Math.Abs(vector[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(this double[] v0, double[] v1)
        {
            return Dot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Dot(this double[] v0, double[] v1, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += v0[i] * v1[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AbsDot(this double[] v0, double[] v1)
        {
            return AbsDot(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double AbsDot(this double[] v0, double[] v1, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(v0[i] * v1[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Angle(this double[] v0, double[] v1)
        {
            return Angle(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Angle(this double[] v0, double[] v1, int count)
        {
            double d = L2Norm(v0, count) * L2Norm(v1, count);

            if (d > 0.0)
                return Math.Acos(SlurMath.Clamp(Dot(v0, v1, count) / d, -1.0, 1.0)); // clamp dot product to remove noise

            return double.NaN;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this double[] v0, double[] v1, double[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this double[] v0, double[] v1, double[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v1, Dot(v0, v1, count) / Dot(v1, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this double[] v0, double[] v1, double[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this double[] v0, double[] v1, int count, double[] result)
        {
            Project(v0, v1, count, result);
            Subtract(v0, result, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this double[] v0, double[] v1, double[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            ProjectParallel(v0, v1, count, result);
            SubtractParallel(v0, result, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this double[] v0, double[] v1, double[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v1, Dot(v0, v1, count) / Dot(v1, v1, count) * 2.0, count, result);
            AddScaled(result, v0, -1.0, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this double[] v0, double[] v1, double[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v1, Dot(v0, v1, count) / Dot(v1, v1, count) * 2.0, count, result);
            AddScaledParallel(result, v0, -1.0, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this double[] v0, double[] v1, double[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this double[] v0, double[] v1, int count, double[] result)
        {
            Scale(v0, Dot(v1, v1, count) / Dot(v0, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this double[] v0, double[] v1, double[] v2, double[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this double[] v0, double[] v1, double[] v2, int count, double[] result)
        {
            Scale(v0, Dot(v1, v2, count) / Dot(v0, v2, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this double[] v0, double[] v1, double[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            ScaleParallel(v0, Dot(v1, v1, count) / Dot(v0, v1, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this double[] v0, double[] v1, double[] v2, double[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this double[] v0, double[] v1, double[] v2, int count, double[] result)
        {
            ScaleParallel(v0, Dot(v1, v2, count) / Dot(v0, v2, count), count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(this double[] vector, double[] result)
        {
            return Unitize(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool Unitize(this double[] vector, int count, double[] result)
        {
            double d = Dot(vector, vector, count);

            if (d > 0.0)
            {
                Scale(vector, 1.0 / Math.Sqrt(d), count, result);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool UnitizeParallel(this double[] vector, double[] result)
        {
            return UnitizeParallel(vector, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool UnitizeParallel(this double[] vector, int count, double[] result)
        {
            double d = Dot(vector, vector, count);

            if (d > 0.0)
            {
                ScaleParallel(vector, 1.0 / Math.Sqrt(d), count, result);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static double L1Norm(this double[] vector)
        {
            return L1Norm(vector, vector.Length);
        }


        /// <summary>
        /// Returns Manhattan length
        /// </summary>
        public static double L1Norm(this double[] vector, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(vector[i]);

            return result;
        }


        /// <summary>
        /// Returns Euclidean length
        /// </summary>
        public static double L2Norm(this double[] vector)
        {
            return L2Norm(vector, vector.Length);
        }


        /// <summary>
        /// Returns Euclidean length
        /// </summary>
        public static double L2Norm(this double[] vector, int count)
        {
            return Math.Sqrt(Dot(vector, vector, count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L2DistanceTo(this double[] v0, double[] v1)
        {
            return L2DistanceTo(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L2DistanceTo(this double[] v0, double[] v1, int count)
        {
            return Math.Sqrt(L2DistanceToSqr(v0, v1, count));
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L2DistanceToSqr(this double[] v0, double[] v1)
        {
            return L2DistanceToSqr(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L2DistanceToSqr(this double[] v0, double[] v1, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
            {
                double d = v1[i] - v0[i];
                result += d * d;
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L1DistanceTo(this double[] v0, double[] v1)
        {
            return L1DistanceTo(v0, v1, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static double L1DistanceTo(this double[] v0, double[] v1, int count)
        {
            double result = 0.0;

            for (int i = 0; i < count; i++)
                result += Math.Abs(v1[i] - v0[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this double[] v0, double v1, double[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this double[] v0, double v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this double[] v0, double[] v1, double[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this double[] v0, double v1, double[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this double[] v0, double v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this double[] v0, double[] v1, double[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this double[] v0, double[] v1, double[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this double[] v0, double[] v1, double[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this double[] vector, double t, double[] result)
        {
            Scale(vector, t, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this double[] vector, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vector[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this double[] vector, double t, double[] result)
        {
            ScaleParallel(vector, t, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this double[] vector, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vector[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this double[] v0, double[] v1, double t, double[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this double[] v0, double[] v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this double[] v0, double[] v1, double[] t, double[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this double[] v0, double t0, double[] v1, double t1, double[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this double[] v0, double t0, double[] v1, double t1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this double[] v0, double[] t0, double[] v1, double[] t1, double[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this double[] v0, double[] t0, double[] v1, double[] t1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] v1, double t, double[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] v1, double[] t, double[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double t0, double[] v1, double t1, double[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double t0, double[] v1, double t1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] t0, double[] v1, double[] t1, double[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this double[] v0, double[] t0, double[] v1, double[] t1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double v2, double t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double v2, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double[] v2, double t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double[] v2, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double[] v2, double[] t, double[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this double[] v0, double[] v1, double[] v2, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double v2, double t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double v2, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double[] v2, double t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double[] v2, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double[] v2, double[] t, double[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this double[] v0, double[] v1, double[] v2, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + (v1[i] - v2[i]) * t[i];
            });
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(this double[] v0, double[] v1, double[] result)
        {
            Multiply(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void Multiply(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(this double[] v0, double[] v1, double[] result)
        {
            MultiplyParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise multiplication
        /// </summary>
        public static void MultiplyParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(this double[] v0, double[] v1, double[] result)
        {
            Divide(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void Divide(this double[] v0, double[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] / v1[i];
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void DivideParallel(this double[] v0, double[] v1, double[] result)
        {
            DivideParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Component-wise division
        /// </summary>
        public static void DivideParallel(this double[] v0, double[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] / v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static double Lerp(this double[] vector, double t)
        {
            int last = vector.Length - 1;

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                return vector[0];
            else if (i >= last)
                return vector[last];

            return SlurMath.Lerp(vector[i], vector[i + 1], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[] v0, double v1, double t, double[] result)
        {
            Lerp(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Lerp(this double[] v0, double v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this double[] v0, double[] v1, double t, double[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this double[] v0, double[] v1, double t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this double[] v0, double[] v1, double[] t, double[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double v1, double t, double[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double[] v1, double t, double[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double[] v1, double t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double[] v1, double[] t, double[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this double[] v0, double[] v1, double[] t, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = SlurMath.Lerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this double[] v0, double[] v1, double t, double[] result)
        {
            SlerpTo(v0, v1, t, Angle(v0, v1), v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this double[] v0, double[] v1, double t, double angle, double[] result)
        {
            SlerpTo(v0, v1, t, angle, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this double[] v0, double[] v1, double t, double angle, int count, double[] result)
        {
            double st = 1.0 / Math.Sin(angle);
            AddScaled(v0, Math.Sin((1.0 - t) * angle) * st, v1, Math.Sin(t * angle) * st, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this double[] v0, double[] v1, double t, double[] result)
        {
            SlerpToParallel(v0, v1, t, Angle(v0, v1), v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this double[] v0, double[] v1, double t, double angle, double[] result)
        {
            SlerpToParallel(v0, v1, t, angle, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this double[] v0, double[] v1, double t, double angle, int count, double[] result)
        {
            double st = 1.0 / Math.Sin(angle);
            AddScaledParallel(v0, Math.Sin((1.0 - t) * angle) * st, v1, Math.Sin(t * angle) * st, count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this double[] vector, Domain domain, double[] result)
        {
            Normalize(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this double[] vector, Domain domain, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this double[] vector, Domain domain, double[] result)
        {
            NormalizeParallel(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this double[] vector, Domain domain, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vector[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this double[] vector, Domain domain, double[] result)
        {
            Evaluate(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this double[] vector, Domain domain, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vector[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this double[] vector, Domain domain, double[] result)
        {
            EvaluateParallel(vector, domain, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this double[] vector, Domain domain, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vector[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this double[] vector, Domain from, Domain to, double[] result)
        {
            Remap(vector, from, to, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this double[] vector, Domain from, Domain to, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain.Remap(vector[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this double[] vector, Domain from, Domain to, double[] result)
        {
            RemapParallel(vector, from, to, vector.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this double[] vector, Domain from, Domain to, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, vector.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain.Remap(vector[i], from, to);
            });
        }

        #endregion


        #region Vec2d[]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, double epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, double epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, Vec2d epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, Vec2d epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, Vec2d[] epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec2d[] v0, Vec2d[] v1, Vec2d[] epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon[i])) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(this Vec2d[] vectors)
        {
           return Sum(vectors, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Sum(this Vec2d[] vectors, int count)
        {
            Vec2d sum = new Vec2d();

            for (int i = 0; i < count; i++)
                sum += vectors[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(this Vec2d[] vectors)
        {
            return Sum(vectors) / vectors.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Mean(this Vec2d[] vectors, int count)
        {
            return Sum(vectors, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(this Vec2d[] vectors, double[] weights)
        {
            return WeightedSum(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedSum(this Vec2d[] vectors, double[] weights, int count)
        {
            var result = new Vec2d();

            for (int i = 0; i < count; i++)
                result += vectors[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(this Vec2d[] vectors, double[] weights)
        {
            return WeightedMean(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d WeightedMean(this Vec2d[] vectors, double[] weights, int count)
        {
            var sum = new Vec2d();
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vectors[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Max(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Min(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this Vec2d[] vectors, Vec2d[] result)
        {
            Abs(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this Vec2d[] vectors, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Abs(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this Vec2d[] vectors, Vec2d[] result)
        {
            AbsParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this Vec2d[] vectors, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Abs(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Dot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            DotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AbsDot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.AbsDot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AbsDotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.AbsDot(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Cross(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] ^ v1[i];
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            CrossParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] ^ v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            Angle(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Angle(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            AngleParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Angle(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Project(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Project(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Reject(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Reject(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Reflect(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Reflect(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.MatchProjection(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, Vec2d[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.MatchProjection(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.MatchProjection(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, Vec2d[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.MatchProjection(v0[i], v1[i], v2[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this Vec2d[] vectors, Vec2d[] result)
        {
            Unitize(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this Vec2d[] vectors, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
            {
                var v = vectors[i];
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(this Vec2d[] vectors, Vec2d[] result)
        {
            UnitizeParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(this Vec2d[] vectors, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vectors[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2Norm(this Vec2d[] vectors, double[] result)
        {
            L2Norm(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2Norm(this Vec2d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2NormParallel(this Vec2d[] vectors, double[] result)
        {
            L2NormParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2NormParallel(this Vec2d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].Length;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1Norm(this Vec2d[] vectors, double[] result)
        {
            L1Norm(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1Norm(this Vec2d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].ManhattanLength;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1NormParallel(this Vec2d[] vectors, double[] result)
        {
            L1NormParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1NormParallel(this Vec2d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].ManhattanLength;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceTo(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L2DistanceTo(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceTo(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].DistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L2DistanceToParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].DistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqr(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L2DistanceToSqr(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqr(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].SquareDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqrParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L2DistanceToSqrParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqrParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].SquareDistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceTo(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L1DistanceTo(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceTo(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].ManhattanDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceToParallel(this Vec2d[] v0, Vec2d[] v1, double[] result)
        {
            L1DistanceToParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceToParallel(this Vec2d[] v0, Vec2d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].ManhattanDistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec2d[] v0, Vec2d v1, Vec2d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec2d[] v0, Vec2d v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec2d[] v0, Vec2d v1, Vec2d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec2d[] v0, Vec2d v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this Vec2d[] v0, Vec2d[] v1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec2d[] vectors, double t, Vec2d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec2d[] vectors, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec2d[] vectors, double[] t, Vec2d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec2d[] vectors, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec2d[] vectors, double t, Vec2d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec2d[] vectors, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec2d[] vectors, double[] t, Vec2d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec2d[] vectors, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, double t0, Vec2d[] v1, double t1, Vec2d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, double t0, Vec2d[] v1, double t1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, Vec2d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, double t0, Vec2d[] v1, double t1, Vec2d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, double t0, Vec2d[] v1, double t1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, Vec2d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec2d[] v0, double[] t0, Vec2d[] v1, double[] t1, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, Vec2d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d v2, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, Vec2d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec2d[] v0, Vec2d[] v1, Vec2d[] v2, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec2d Lerp(this Vec2d[] vectors, double t)
        {
            int last = vectors.Length - 1;

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
        public static void LerpTo(this Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < v0.Length; i++)
                result[i] = Vec2d.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Lerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec2d.Slerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d v1, double t, Vec2d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d[] v1, double t, Vec2d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d[] v1, double t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, Vec2d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec2d[] v0, Vec2d[] v1, double[] t, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec2d.Slerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            Normalize(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            NormalizeParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            Evaluate(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this Vec2d[] vectors, Domain2d domain, Vec2d[] result)
        {
            EvaluateParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this Vec2d[] vectors, Domain2d domain, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this Vec2d[] vectors, Domain2d from, Domain2d to, Vec2d[] result)
        {
            Remap(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this Vec2d[] vectors, Domain2d from, Domain2d to, int count, Vec2d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain2d.Remap(vectors[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this Vec2d[] vectors, Domain2d from, Domain2d to, Vec2d[] result)
        {
            RemapParallel(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this Vec2d[] vectors, Domain2d from, Domain2d to, int count, Vec2d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain2d.Remap(vectors[i], from, to);
            });
        }

        #endregion


        #region Vec3d[]


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, double epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, double epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, Vec3d epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, Vec3d epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, Vec3d[] epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this Vec3d[] v0, Vec3d[] v1, Vec3d[] epsilon, int count)
        {
            for (int i = 0; i < count; i++)
                if (!v0[i].ApproxEquals(v1[i], epsilon[i])) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(this Vec3d[] vectors)
        {
            return Sum(vectors, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Sum(this Vec3d[] vectors, int count)
        {
            Vec3d sum = new Vec3d();

            for (int i = 0; i < count; i++)
                sum += vectors[i];

            return sum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(this Vec3d[] vectors)
        {
            return Sum(vectors) / vectors.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Mean(this Vec3d[] vectors, int count)
        {
            return Sum(vectors, count) / count;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(this Vec3d[] vectors, double[] weights)
        {
            return WeightedSum(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedSum(this Vec3d[] vectors, double[] weights, int count)
        {
            var result = new Vec3d();

            for (int i = 0; i < count; i++)
                result += vectors[i] * weights[i];

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(this Vec3d[] vectors, double[] weights)
        {
            return WeightedMean(vectors, weights, vectors.Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d WeightedMean(this Vec3d[] vectors, double[] weights, int count)
        {
            var sum = new Vec3d();
            double wsum = 0.0;

            for (int i = 0; i < count; i++)
            {
                double w = weights[i];
                sum += vectors[i] * w;
                wsum += w;
            }

            return sum / wsum;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Max(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Max(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Max(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MaxParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MaxParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Max(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Min(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Min(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Min(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MinParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MinParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Min(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this Vec3d[] vectors, Vec3d[] result)
        {
            Abs(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Abs(this Vec3d[] vectors, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Abs(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this Vec3d[] vectors, Vec3d[] result)
        {
            AbsParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsParallel(this Vec3d[] vectors, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Abs(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            Dot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Dot(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            DotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void DotParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AbsDot(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDot(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.AbsDot(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AbsDotParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AbsDotParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.AbsDot(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Cross(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void Cross(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] ^ v1[i];
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            CrossParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// Returns the pseudo cross product calculated as the dot product between v1 and the perpendicular of v0.
        /// </summary>
        public static void CrossParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] ^ v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            Angle(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Angle(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Angle(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            AngleParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AngleParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Angle(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Project(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Project(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Project(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            ProjectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ProjectParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Project(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Reject(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reject(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Reject(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            RejectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RejectParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Reject(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Reflect(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Reflect(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Reflect(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            ReflectParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ReflectParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Reflect(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MatchProjection(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.MatchProjection(v0[i], v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, Vec3d[] result)
        {
            MatchProjection(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjection(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.MatchProjection(v0[i], v1[i], v2[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            MatchProjectionParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.MatchProjection(v0[i], v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, Vec3d[] result)
        {
            MatchProjectionParallel(v0, v1, v2, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void MatchProjectionParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.MatchProjection(v0[i], v1[i], v2[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this Vec3d[] vectors, Vec3d[] result)
        {
            Unitize(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Unitize(this Vec3d[] vectors, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
            {
                var v = vectors[i];
                v.Unitize();
                result[i] = v;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(this Vec3d[] vectors, Vec3d[] result)
        {
            UnitizeParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void UnitizeParallel(this Vec3d[] vectors, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = vectors[i];
                    v.Unitize();
                    result[i] = v;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2Norm(this Vec3d[] vectors, double[] result)
        {
            L2Norm(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2Norm(this Vec3d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].Length;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2NormParallel(this Vec3d[] vectors, double[] result)
        {
            L2NormParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2NormParallel(this Vec3d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].Length;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1Norm(this Vec3d[] vectors, double[] result)
        {
            L1Norm(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1Norm(this Vec3d[] vectors, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i].ManhattanLength;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1NormParallel(this Vec3d[] vectors, double[] result)
        {
            L1NormParallel(vectors, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1NormParallel(this Vec3d[] vectors, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i].ManhattanLength;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceTo(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L2DistanceTo(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceTo(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].DistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L2DistanceToParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].DistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqr(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L2DistanceToSqr(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqr(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].SquareDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqrParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L2DistanceToSqrParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L2DistanceToSqrParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].SquareDistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceTo(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L1DistanceTo(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceTo(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i].ManhattanDistanceTo(v1[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceToParallel(this Vec3d[] v0, Vec3d[] v1, double[] result)
        {
            L1DistanceToParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void L1DistanceToParallel(this Vec3d[] v0, Vec3d[] v1, int count, double[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i].ManhattanDistanceTo(v1[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec3d[] v0, Vec3d v1, Vec3d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec3d[] v0, Vec3d v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Add(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Add(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec3d[] v0, Vec3d v1, Vec3d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec3d[] v0, Vec3d v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            AddParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void AddParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            Subtract(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Subtract(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] - v1[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] result)
        {
            SubtractParallel(v0, v1, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SubtractParallel(this Vec3d[] v0, Vec3d[] v1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] - v1[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec3d[] vectors, double t, Vec3d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec3d[] vectors, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec3d[] vectors, double[] t, Vec3d[] result)
        {
            Scale(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Scale(this Vec3d[] vectors, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = vectors[i] * t[i];
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec3d[] vectors, double t, Vec3d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec3d[] vectors, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec3d[] vectors, double[] t, Vec3d[] result)
        {
            ScaleParallel(vectors, t, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void ScaleParallel(this Vec3d[] vectors, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = vectors[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t;
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            AddScaled(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] + v1[i] * t[i];
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, double t0, Vec3d[] v1, double t1, Vec3d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, double t0, Vec3d[] v1, double t1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0 + v1[i] * t1;
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, Vec3d[] result)
        {
            AddScaled(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaled(this Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = v0[i] * t0[i] + v1[i] * t1[i];
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t;
            });
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            AddScaledParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + v1 * t
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] + v1[i] * t[i];
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, double t0, Vec3d[] v1, double t1, Vec3d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, double t0, Vec3d[] v1, double t1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0 + v1[i] * t1;
            });
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, Vec3d[] result)
        {
            AddScaledParallel(v0, t0, v1, t1, v0.Length, result);
        }


        /// <summary>
        /// result = v0 * t0 + v1 * t1
        /// </summary>
        public static void AddScaledParallel(this Vec3d[] v0, double[] t0, Vec3d[] v1, double[] t1, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = v0[i] * t0[i] + v1[i] * t1[i];
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t;
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, Vec3d[] result)
        {
            AddScaledDelta(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDelta(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d v2, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t;
            });
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, Vec3d[] result)
        {
            AddScaledDeltaParallel(v0, v1, v2, t, v0.Length, result);
        }


        /// <summary>
        /// result = v0 + (v1 - v2) * t
        /// </summary>
        public static void AddScaledDeltaParallel(this Vec3d[] v0, Vec3d[] v1, Vec3d[] v2, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] += v0[i] + (v1[i] - v2[i]) * t[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static Vec3d Lerp(this Vec3d[] vectors, double t)
        {
            int last = vectors.Length - 1;

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
        public static void LerpTo(this Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Lerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < v0.Length; i++)
                result[i] = Vec3d.Lerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            LerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpTo(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Lerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            LerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void LerpToParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Lerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1, t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1[i], t);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            SlerpTo(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpTo(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Vec3d.Slerp(v0[i], v1[i], t[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d v1, double t, Vec3d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1, t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d[] v1, double t, Vec3d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d[] v1, double t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1[i], t);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, Vec3d[] result)
        {
            SlerpToParallel(v0, v1, t, v0.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SlerpToParallel(this Vec3d[] v0, Vec3d[] v1, double[] t, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Vec3d.Slerp(v0[i], v1[i], t[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            Normalize(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Normalize(this Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Normalize(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            NormalizeParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeParallel(this Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Normalize(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            Evaluate(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Evaluate(this Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = domain.Evaluate(vectors[i]);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this Vec3d[] vectors, Domain3d domain, Vec3d[] result)
        {
            EvaluateParallel(vectors, domain, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void EvaluateParallel(this Vec3d[] vectors, Domain3d domain, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = domain.Evaluate(vectors[i]);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this Vec3d[] vectors, Domain3d from, Domain3d to, Vec3d[] result)
        {
            Remap(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Remap(this Vec3d[] vectors, Domain3d from, Domain3d to, int count, Vec3d[] result)
        {
            for (int i = 0; i < count; i++)
                result[i] = Domain3d.Remap(vectors[i], from, to);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this Vec3d[] vectors, Domain3d from, Domain3d to, Vec3d[] result)
        {
            RemapParallel(vectors, from, to, vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void RemapParallel(this Vec3d[] vectors, Domain3d from, Domain3d to, int count, Vec3d[] result)
        {
            Parallel.ForEach(Partitioner.Create(0, count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = Domain3d.Remap(vectors[i], from, to);
            });
        }


        #endregion


        #region double[][]

        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double epsilon, int count)
        {
            return ApproxEquals(v0, v1, epsilon, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double epsilon, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], epsilon, size)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[] epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[] epsilon, int count)
        {
            return ApproxEquals(v0, v1, epsilon, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[] epsilon, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], epsilon, size)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[][] epsilon)
        {
            return ApproxEquals(v0, v1, epsilon, v0.Length, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[][] epsilon, int count)
        {
            return ApproxEquals(v0, v1, epsilon, count, v0[0].Length);
        }


        /// <summary>
        /// 
        /// </summary>
        public static bool ApproxEquals(this double[][] v0, double[][] v1, double[][] epsilon, int count, int size)
        {
            for (int i = 0; i < count; i++)
                if (!ApproxEquals(v0[i], v1[i], epsilon[i], size)) return false;

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(this double[][] vectors, double[] result)
        {
            Sum(vectors, vectors.Length, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(this double[][] vectors, int count, double[] result)
        {
            Sum(vectors, count, vectors[0].Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Sum(this double[][] vectors, int count, int size, double[] result)
        {
            Array.Clear(result, 0, size);

            for (int i = 0; i < count; i++)
                Add(result, vectors[i], size, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(this double[][] vectors, double[] result)
        {
            Sum(vectors, result);
            Scale(result, 1.0 / vectors.Length, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(this double[][] vectors, int count, double[] result)
        {
            Sum(vectors, count, result);
            Scale(result, 1.0 / count, result);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Mean(this double[][] vectors, int count, int size, double[] result)
        {
            Sum(vectors, count, size, result);
            Scale(result, 1.0 / count, size, result);
        }


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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpTo(vectors[i], vectors[i + 1], t, size, result);
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
                int i;
                double tj = SlurMath.Fract(t[j] * last, out i);

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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpToParallel(vectors[i], vectors[i + 1], t, size, result);
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
                    int i;
                    double tj = SlurMath.Fract((double)(t[j] * last), out i);

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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpTo(vectors[i], vectors[i + 1], t, size, result);
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
                int i;
                double tj = SlurMath.Fract(t[j] * last, out i);

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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpToParallel(vectors[i], vectors[i + 1], t, size, result);
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
                    int i;
                    double tj = SlurMath.Fract(t[j] * last, out i);

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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpTo(vectors[i], vectors[i + 1], t, size, result);
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
                int i;
                double tj = SlurMath.Fract(t[j] * last, out i);

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

            int i;
            t = SlurMath.Fract(t * last, out i);

            if (i < 0)
                result.Set(vectors[0]);
            else if (i >= last)
                result.Set(vectors[last]);
            else
                LerpToParallel(vectors[i], vectors[i + 1], t, size, result);
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
                    int i;
                    double tj = SlurMath.Fract(t[j] * last, out i);

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
