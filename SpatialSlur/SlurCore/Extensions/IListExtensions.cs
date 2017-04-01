using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using SpatialSlur.SlurData;

/*
 * Notes
 * All IList extension methods are redirected to equivalent array extension methods where possible for better performance.
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class IListExtensions
    {
        #region IList<T>

        /// <summary>
        /// 
        /// </summary>
        public static void Set<T>(this IList<T> list, T value)
        {
            SetRange(list, value, 0, list.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Set<T>(this IList<T> list, IReadOnlyList<T> other)
        {
            SetRange(list, other, 0, 0, list.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, T value, int index, int count)
        {
            if (list is T[])
            {
                ArrayExtensions.SetRange((T[])list, value, index, count);
                return;
            }

            for (int i = 0; i < count; i++)
                list[i + index] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, IReadOnlyList<T> other, int thisIndex, int otherIndex, int count)
        {
            if (list is T[] && other is T[])
            {
                ArrayExtensions.SetRange((T[])list, (T[])other, thisIndex, otherIndex, count);
                return;
            }

            for (int i = 0; i < count; i++)
                list[thisIndex + i] = other[otherIndex + i];
        }


        /// <summary>
        /// Swaps a pair of elements.
        /// </summary>
        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }


        /// <summary>
        /// Shuffles a list of items in place.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            Shuffle(list, new Random());
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            Shuffle(list, new Random(seed));
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            if (list is T[])
            {
                ArrayExtensions.Shuffle((T[])list, random);
                return;
            }

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i);
                list.Swap(i, j);
            }
        }


        /// <summary>
        /// Shifts a list of items in place.
        /// </summary>
        public static void Shift<T>(this IList<T> list, int offset)
        {
            Shift(list, offset, 0, list.Count - 1);
        }


        /// <summary>
        /// Shifts a subset of a list of items in place.
        /// </summary>
        public static void Shift<T>(this IList<T> list, int offset, int from, int to)
        {
            if (list is T[])
            {
                ArrayExtensions.Shift((T[])list, offset, from, to);
                return;
            }

            offset = SlurMath.Mod2(offset, to - from + 1);
            Reverse(list, from, from + offset - 1);
            Reverse(list, from + offset, to);
            Reverse(list, from, to);
        }


        /// <summary>
        /// Reverses a list of items in place.
        /// </summary>
        public static void Reverse<T>(this IList<T> list)
        {
            Reverse(list, 0, list.Count - 1);
        }


        /// <summary>
        /// Reverses the order of the items within the specified range in place.
        /// </summary>
        public static void Reverse<T>(this IList<T> list, int from, int to)
        {
            if (list is T[])
            {
                ArrayExtensions.Reverse((T[])list, from, to);
                return;
            }

            while (to > from)
                list.Swap(from++, to--);
        }


        /// <summary>
        /// Equivalent of List.FindIndex for IList.
        /// </summary>
        public static int FindIndex<T>(this IList<T> list, Predicate<T> match)
        {
            return list.FindIndex(0, list.Count, match);
        }


        /// <summary>
        /// 
        /// </summary>
        public static int FindIndex<T>(this IList<T> list, int index, int length, Predicate<T> match)
        {
            if (list is T[])
                return ArrayExtensions.FindIndex((T[])list, index, length, match);

            for (int i = index; i < index + length; i++)
            {
                if (match(list[i]))
                    return i;
            }

            return -1;
        }


        /// <summary>
        /// Moves elements for which the given predicate returns true to the front of the list.
        /// Returns the index after the last used element.
        /// </summary>
        public static int Compact<T>(this IList<T> list, Predicate<T> include)
        {
            if (list is T[])
                return ArrayExtensions.Compact((T[])list, include);

            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];
                if (include(t))
                    list[marker++] = t;
            }

            return marker;
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> list, int n)
            where T : IComparable<T>
        {
            return list.QuickSelect(n, 0, list.Count - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to)
            where T : IComparable<T>
        {
            return list.QuickSelect(n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> list, int n, IComparer<T> comparer)
        {
            return list.QuickSelect(n, 0, list.Count - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> list, int n, int from, int to, IComparer<T> comparer)
        {
            return list.QuickSelect(n, from, to, comparer.Compare);
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// </summary>
        public static T QuickSelect<T>(this IList<T> list, int n, Comparison<T> compare)
        {
            return list.QuickSelect(n, 0, list.Count - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this IList<T> list, int n, int from, int to, Comparison<T> compare)
        {
            if (list is T[])
                return ArrayExtensions.QuickSelect((T[])list, n, from, to, compare);

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
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n)
            where T : IComparable<T>
        {
            return keys.QuickSelect(items, n, 0, keys.Count - 1, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n, int from, int to)
            where T : IComparable<T>
        {
            return keys.QuickSelect(items, n, from, to, (x, y) => x.CompareTo(y));
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n, IComparer<T> comparer)
        {
            return keys.QuickSelect(items, n, 0, keys.Count - 1, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n, int from, int to, IComparer<T> comparer)
        {
            return keys.QuickSelect(items, n, from, to, comparer.Compare);
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// Also sorts a list of corresponding items.
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n, Comparison<T> compare)
        {
            return keys.QuickSelect(items, n, 0, keys.Count - 1, compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T, U>(this IList<T> keys, IList<U> items, int n, int from, int to, Comparison<T> compare)
        {
            if (keys is T[] && items is U[])
                return ArrayExtensions.QuickSelect((T[])keys, (U[])items, n, from, to, compare);

            if (n < from || n > to)
                throw new IndexOutOfRangeException();

            while (to > from)
            {
                int i = keys.Partition(from, to, compare, items);
                if (i > n) to = i - 1;
                else if (i < n) from = i + 1;
                else return keys[i];
            }
            return keys[from];
        }


        /// <summary>
        /// 
        /// </summary>
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

        #endregion
    }
}
