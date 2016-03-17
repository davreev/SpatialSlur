using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// IList interface extension methods.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static U[] ConvertAll<T, U>(this IList<T> source, Func<T, U> converter)
        {
            U[] result = new U[source.Count];

            for (int i = 0; i < source.Count; i++)
                result[i] = converter(source[i]);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void Set<T>(this IList<T> list, T value)
        {
            Set(list, value, 0, list.Count);
        }


        /// <summary>
        /// TODO test performance against lists of various lengths
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public static void Set<T>(this IList<T> list, T value, int index, int length)
        {
            Parallel.ForEach(Partitioner.Create(index, index + length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    list[i] = value;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="other"></param>
        public static void Set<T>(this IList<T> list, IList<T> other)
        {
            Set(list, other, 0, list.Count);
        }


        /// <summary>
        /// TODO test parallel performance against list length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="other"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public static void Set<T>(this IList<T> list, IList<T> other, int index, int length)
        {
            Parallel.ForEach(Partitioner.Create(index, index + length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    list[i] = other[i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="other"></param>
        /// <param name="index"></param>
        /// <param name="otherIndex"></param>
        /// <param name="length"></param>
        public static void Set<T>(this IList<T> list, IList<T> other, int index, int otherIndex, int length)
        {
            Parallel.ForEach(Partitioner.Create(0, length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    list[index + i] = other[otherIndex + i];
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this IList<T> list, int index, int length)
        {
            T[] result = new T[length];

            for (int i = 0, j = index; i < length; i++, j++)
                result[i] = list[j];

            return result;
        }


        /// <summary>
        /// Returns a new IList which includes any elements for which the given delegate returns true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static T[] Compact<T>(this IList<T> list, Func<T, bool> include)
        {
            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (include(t))
                    list[marker++] = t;
            }

            return list.SubArray(0, marker);
        }


        /// <summary>
        /// Swaps a pair of elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }


        /// <summary>
        /// Shuffles a list of items in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            Shuffle(list, new Random());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="seed"></param>
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            Shuffle(list, new Random(seed));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="random"></param>
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i);
                list.Swap(i, j);
            }
        }


        /// <summary>
        /// Shifts a list of items in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        public static void Shift<T>(this IList<T> list, int offset)
        {
            int n = list.Count;
            offset = SlurMath.Mod2(offset, n);

            Reverse(list, 0, offset - 1);
            Reverse(list, offset, n - 1);
            Reverse(list, 0, n - 1);
        }


        /// <summary>
        /// Shifts a subset of a list of items in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="offset"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Shift<T>(this IList<T> list, int offset, int from, int to)
        {
            int n = to - from + 1;
            offset = SlurMath.Mod2(offset, n);

            Reverse(list, from, from + offset - 1);
            Reverse(list, from + offset, to);
            Reverse(list, from, to);
        }


        /// <summary>
        /// Reverses a list of items in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Reverse<T>(this IList<T> list)
        {
            Reverse(list, 0, list.Count - 1);
        }


        /// <summary>
        /// Reverses the order of the items within the specified range in place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void Reverse<T>(this IList<T> list, int from, int to)
        {
            while (to > from)
                list.Swap(from++, to--);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n) where T : IComparable<T>
        {
            return list.QuickSelect(n, 0, list.Count - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to) where T : IComparable<T>
        {
            return list.QuickSelect(n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, IComparer<T> comparer)
        {
            return list.QuickSelect(n, 0, list.Count - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to, IComparer<T> comparer)
        {
            return list.QuickSelect(n, from, to, comparer.Compare);
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the list such that the nth item is in the correct postition. 
        /// All items to the left are less than or equal to the nth and those to the right are greater than or equal to the nth.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T QuickSelect<T>(this IList<T> list, int n, Comparison<T> compare)
        {
            return list.QuickSelect(n, 0, list.Count - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static T QuickSelect<T>(this IList<T> list, int n, int from, int to, Comparison<T> compare)
        {
            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = list.Partition(from, to, compare);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return list[i];
            }
            return list[from];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        private static int Partition<T>(this IList<T> list, int from, int to, Comparison<T> compare)
        {
            T pivot = list[from]; // get pivot element
            int i = from;
            int j = to + 1;

            while (true)
            {
                while (compare(pivot, list[++i]) > 0)
                    if (i == to) break;

                while (compare(pivot, list[--j]) < 0) 
                    if (j == from) break;

                if (i >= j) break; // check if indices have crossed
                list.Swap(i, j);
            }

            // swap with pivot element
            list.Swap(from, j);
            return j;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, IList<U> items) where T : IComparable<T>
        {
            return list.QuickSelect(n, 0, list.Count - 1, (x, y) => x.CompareTo(y), items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to, IList<U> items) where T : IComparable<T>
        {
           return list.QuickSelect(n, from, to, (x, y) => x.CompareTo(y), items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="comparer"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, IComparer<T> comparer, IList<U> items)
        {
            return list.QuickSelect(n, 0, list.Count - 1, comparer.Compare, items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="comparer"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to, IComparer<T> comparer, IList<U> items)
        {
            return list.QuickSelect(n, from, to, comparer.Compare, items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="compare"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, Comparison<T> compare, IList<U> items)
        {
            return list.QuickSelect(n, 0, list.Count - 1, compare, items);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="compare"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to, Comparison<T> compare, IList<U> items)
        {
            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = list.Partition(from, to, compare, items);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return list[i];
            }
            return list[from];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="compare"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private static int Partition<T, U>(this IList<T> list, int from, int to, Comparison<T> compare, IList<U> items)
        {
            T pivot = list[from]; // get pivot element
            int i = from;
            int j = to + 1;

            while (true)
            {
                while (compare(pivot, list[++i]) > 0)
                    if (i == to) break;

                while (compare(pivot, list[--j]) < 0)
                    if (j == from) break;

                if (i >= j) break; // check if indices have crossed
                list.Swap(i, j);
                items.Swap(i, j);
            }

            // swap with pivot element
            list.Swap(from, j);
            items.Swap(from, j);
            return j;
        }
    }
}
