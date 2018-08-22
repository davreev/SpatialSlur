
/*
 * Notes
 * 
 * IList extension methods are redirected to equivalent array extension methods where possible for better performance.
 */

using System;
using System.Collections.Generic;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class IListExtensions
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
        /// Assumes the length of the array is less than or equal to the number of elements in the given sequence.
        /// </summary>
        public static void Set<T>(this IList<T> list, IEnumerable<T> sequence)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.Set(arr, sequence);
                return;
            }

            var itr = sequence.GetEnumerator();

            for(int i = 0; i < list.Count; i++)
            {
                itr.MoveNext();
                list[i] = itr.Current;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, T value, int count)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.SetRange(arr, value, count);
                return;
            }

            for (int i = 0; i < count; i++)
                list[i] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, T value, int index, int count)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.SetRange(arr, value, index, count);
                return;
            }

            for (int i = 0; i < count; i++)
                list[i + index] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, IReadOnlyList<T> other, int count)
        {
            SetRange(list, other, 0, 0, count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetRange<T>(this IList<T> list, IReadOnlyList<T> other, int thisIndex, int otherIndex, int count)
        {
            if (list is T[] arr0 && other is T[] arr1)
            {
                ArrayExtensions.SetRange(arr0, arr1, thisIndex, otherIndex, count);
                return;
            }

            for (int i = 0; i < count; i++)
                list[thisIndex + i] = other[otherIndex + i];
        }


        /// <summary>
        /// Assumes the specified range is less than or equal to the number of elements in the given sequence.
        /// </summary>
        public static void SetRange<T>(this IList<T> list, IEnumerable<T> sequence, int index, int count)
        {
            if(list is T[] arr)
            {
                ArrayExtensions.SetRange(arr, sequence, index, count);
                return;
            }

            var itr = sequence.GetEnumerator();

            for (int i = 0; i < count; i++)
            {
                itr.MoveNext();
                list[index + i] = itr.Current;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetSelection<T>(this IList<T> list, T value, IEnumerable<int> indices)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.SetSelection(arr, value, indices);
                return;
            }

            foreach (int i in indices)
                list[i] = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void SetSelection<T>(this IList<T> list, IList<T> other, IList<int> indices)
        {
            if (list is T[] arr0 && other is T[] arr1 && indices is int[] arr2)
            {
                ArrayExtensions.SetSelection(arr0, arr1, arr2);
                return;
            }

            for (int i = 0; i < indices.Count; i++)
                list[indices[i]] = other[i];
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
            if (list is T[] arr)
            {
                ArrayExtensions.Shuffle(arr, random);
                return;
            }

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i);
                list.Swap(i, j);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, int index, int count)
        {
            Shuffle(list, new Random(), index, count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, int seed, int index, int count)
        {
            Shuffle(list, new Random(seed), index, count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, Random random, int index, int count)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.Shuffle(arr, random, index, count);
                return;
            }

            for (int i = count - 1; i > 0; i--)
            {
                int j = random.Next(i);
                list.Swap(i + index, j + index);
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
            if (list is T[] arr)
            {
                ArrayExtensions.Shift(arr, offset, from, to);
                return;
            }

            offset = SlurMath.RepeatPos(offset, to - from);
            Reverse(list, from, from + offset);
            Reverse(list, from + offset, to);
            Reverse(list, from, to);
        }


        /// <summary>
        /// Reverses a list of items in place.
        /// </summary>
        public static void Reverse<T>(this IList<T> list)
        {
            Reverse(list, 0, list.Count);
        }


        /// <summary>
        /// Reverses the order of the items within the specified range in place.
        /// </summary>
        public static void Reverse<T>(this IList<T> list, int from, int to)
        {
            if (list is T[] arr)
            {
                ArrayExtensions.Reverse(arr, from, to);
                return;
            }

            while (--to > from)
                list.Swap(from++, to);
        }


        /// <summary>
        /// Equivalent of List.FindIndex for IList
        /// </summary>
        public static int FindIndex<T>(this IList<T> list, Predicate<T> match)
        {
            return list.FindIndex(0, list.Count, match);
        }


        /// <summary>
        /// Equivalent of List.FindIndex for IList
        /// </summary>
        public static int FindIndex<T>(this IList<T> list, int index, int length, Predicate<T> match)
        {
            if (list is T[] arr)
                return Array.FindIndex(arr, index, length, match);

            for (int i = index; i < index + length; i++)
            {
                if (match(list[i]))
                    return i;
            }

            return -1;
        }


        /// <summary>
        /// Moves true elements to the front of the list.
        /// Returns the index after the last true element.
        /// </summary>
        public static int Swim<T>(this IList<T> list, Predicate<T> match)
        {
            if (list is T[] arr)
                return ArrayExtensions.Swim(arr, match);

            int marker = 0;

            for (int i = 0; i < list.Count; i++)
            {
                T t = list[i];

                if (match(t))
                    list[marker++] = t;
            }

            return marker;
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n)
            where T : IComparable<T>
        {
            return items.QuickSelect(n, 0, items.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n, int from, int to)
            where T : IComparable<T>
        {
            if (items is T[] arr)
                return ArrayExtensions.QuickSelect(arr, n, from, to);

            if (n < from || n >= to)
                throw new IndexOutOfRangeException();

            while (to - from > 1)
            {
                int i = items.Partition(from, to);

                if (i > n)
                    to = i;
                else if (i < n)
                    from = i + 1;
                else
                    return items[i];
            }

            return items[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<T>(this IList<T> items, int from, int to)
            where T : IComparable<T>
        {
            T pivot = items[from];
            int i = from;
            int j = to;

            while (true)
            {
                do
                {
                    if (--j == i) goto Break;
                } while (pivot.CompareTo(items[j]) < 0);

                do
                {
                    if (++i == j) goto Break;
                } while (pivot.CompareTo(items[i]) > 0);

                items.Swap(i, j);
            }

            Break:;
            items.Swap(from, j);
            return j;
        }


        /// <summary>
        /// Returns the nth smallest item in linear amortized time.
        /// Partially sorts the array with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n, Comparison<T> compare)
        {
            return items.QuickSelect(n, 0, items.Count, compare);
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n, IComparer<T> comparer)
        {
            return items.QuickSelect(n, 0, items.Count, comparer.Compare);
        }


        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n, int from, int to, IComparer<T> comparer)
        {
            return items.QuickSelect(n, from, to, comparer.Compare);
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static T QuickSelect<T>(this IList<T> items, int n, int from, int to, Comparison<T> compare)
        {
            if (items is T[] arr)
                return ArrayExtensions.QuickSelect(arr, n, from, to, compare);

            if (n < from || n >= to)
                throw new IndexOutOfRangeException();

            while (to - from > 1)
            {
                int i = items.Partition(from, to, compare);

                if (i > n)
                    to = i;
                else if (i < n)
                    from = i + 1;
                else
                    return items[i];
            }

            return items[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<T>(this IList<T> items, int from, int to, Comparison<T> compare)
        {
            T pivot = items[from];
            int i = from;
            int j = to;

            while (true)
            {
                do
                {
                    if (--j == i) goto Break;
                } while (compare(pivot, items[j]) < 0);

                do
                {
                    if (++i == j) goto Break;
                } while (compare(pivot, items[i]) > 0);

                items.Swap(i, j);
            }

            Break:;
            items.Swap(from, j);
            return j;
        }


        /// <summary>
        /// Returns the nth smallest key in linear amortized time.
        /// Partially sorts the keys with respect to the nth item such that items to the left are less than or equal and items to the right are greater than or equal.
        /// Also partially sorts a list of corresponding values.
        /// </summary>
        public static K QuickSelect<K, V>(this IList<K> keys, IList<V> values, int n)
            where K : IComparable<K>
        {
            return keys.QuickSelect(values, n, 0, keys.Count);
        }


        /// <summary>
        /// 
        /// </summary>
        public static K QuickSelect<K, V>(this IList<K> keys, IList<V> values, int n, int from, int to)
            where K : IComparable<K>
        {
            if (keys is K[] arr0 && values is V[] arr1)
                return ArrayExtensions.QuickSelect(arr0, arr1, n, from, to);

            if (n < from || n >= to)
                throw new IndexOutOfRangeException();

            while (to - from > 1)
            {
                int i = keys.Partition(values, from, to);

                if (i > n)
                    to = i;
                else if (i < n)
                    from = i + 1;
                else
                    return keys[i];
            }

            return keys[from];
        }


        /// <summary>
        /// 
        /// </summary>
        private static int Partition<K, V>(this IList<K> keys, IList<V> values, int from, int to)
            where K : IComparable<K>
        {
            K pivot = keys[from];
            int i = from;
            int j = to;

            while (true)
            {
                do
                {
                    if (--j == i) goto Break;
                } while (pivot.CompareTo(keys[j]) < 0);

                do
                {
                    if (++i == j) goto Break;
                } while (pivot.CompareTo(keys[i]) > 0);

                keys.Swap(i, j);
                values.Swap(i, j);
            }

            Break:;
            keys.Swap(from, j);
            values.Swap(from, j);
            return j;
        }

        #endregion
    }
}
