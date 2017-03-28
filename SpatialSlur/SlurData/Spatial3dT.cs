using System;
using System.Collections.Generic;
using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * TODO
 * Line insert/search based on https://en.wikipedia.org/wiki/Bresenham's_line_algorithm
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Abstract base class consolidating common functionality from SpatialHash and SpatialGrid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Spatial3d<T>
    {
        private List<T>[] _bins;
        private int[] _tags; // used to lazily clear bins
        private int _itemCount;
        private int _currTag;

  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binCount"></param>
        protected Spatial3d(int binCount)
        {
            if (binCount < 1)
                throw new System.ArgumentOutOfRangeException("The data structure must have at least 1 bin.");

            _bins = new List<T>[binCount];
            _tags = new int[binCount];
            _currTag = 1; // ensures bins are initially out of sync

            for (int i = 0; i < binCount; i++)
                _bins[i] = new List<T>();
        }


        /// <summary>
        /// Returns the number of items in the map.
        /// </summary>
        public int ItemCount
        {
            get { return _itemCount; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int BinCount
        {
            get { return _bins.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        internal abstract void Discretize(Vec3d point, out int i, out int j, out int k);
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        internal abstract int ToIndex(int i, int j, int k);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param> 
        /// <param name="item"></param>
        private void Insert(int index, T item)
        {
            List<T> bin = _bins[index];

            // synchronize bin if necessary
            if (_tags[index] != _currTag)
            {
                bin.Clear();
                _tags[index] = _currTag;
            }

            bin.Add(item);
            _itemCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="item"></param>
        public void Insert(Vec3d point, T item)
        {
            int i, j, k;
            Discretize(point, out i, out j, out k);
            Insert(ToIndex(i, j, k), item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="item"></param>
        public void Insert(Domain3d domain, T item)
        {
            int i0, j0, k0, i1, j1, k1;
            Discretize(domain.From, out i0, out j0, out k0);
            Discretize(domain.To, out i1, out j1, out k1);

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        Insert(ToIndex(i, j, k), item);
                    }
                }
            }
        }


        /// <summary>
        /// The contents of all found bins are added to the result list.
        /// </summary>
        public void Search(Vec3d point, List<T> result)
        {
            int i, j, k;
            Discretize(point, out i, out j, out k);
            int index = ToIndex(i, j, k);

            // only add if bin is synchronized
            if (_tags[index] == _currTag)
                result.AddRange(_bins[index]);
        }


        /// <summary>
        /// The contents of all found bins are added to the result list.
        /// </summary>
        public void Search(Domain3d domain, List<T> result)
        {
            int i0, j0, k0, i1, j1, k1;
            Discretize(domain.From, out i0, out j0, out k0);
            Discretize(domain.To, out i1, out j1, out k1);

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        int index = ToIndex(i, j, k);

                        // only add if bin is synchronized
                        if (_tags[index] == _currTag)
                            result.AddRange(_bins[index]);
                    }
                }
            }
        }


        /// <summary>
        /// The given function is called on the contents of each found bin.
        /// Empty bins are skipped.
        /// </summary>
        public void Search(Vec3d point, Action<IEnumerable<T>> callback)
        {
            int i, j, k;
            Discretize(point, out i, out j, out k);
            int index = ToIndex(i, j, k);

            // only callback if bin is synched
            if (_tags[index] == _currTag)
                callback(_bins[index]);
        }


        /// <summary>
        /// The given function is called on the contents of each found bin.
        /// Empty bins are skipped.
        /// </summary>
        public void Search(Domain3d domain, Action<IEnumerable<T>> callback)
        {
            int i0, j0, k0, i1, j1, k1;
            Discretize(domain.From, out i0, out j0, out k0);
            Discretize(domain.To, out i1, out j1, out k1);

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        int index = ToIndex(i, j, k);

                        // only callback if bin is synched
                        if (_tags[index] == _currTag)
                            callback(_bins[index]);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _currTag++; // consider reset of tags on overflow
            _itemCount = 0;
        }
    }
}
