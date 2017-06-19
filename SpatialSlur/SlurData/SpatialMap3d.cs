using System;
using System.Collections.Generic;
using System.Linq;

using SpatialSlur.SlurCore;

using static SpatialSlur.SlurCore.CoreUtil;

/*
 * Notes
 * 
 * TODO
 * Line insert/search based on https://en.wikipedia.org/wiki/Bresenham's_line_algorithm
 */

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Data structure for handling broad-phase collision checks between 3d objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpatialMap3d<T>
    {
        private Bin[] _bins;
        private int _count;
        private int _currTag;

  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="binCount"></param>
        protected SpatialMap3d(int binCount)
        {
            if (binCount < 1)
                throw new System.ArgumentOutOfRangeException("There must be at least 1 bin in the map.");

            _bins = new Bin[binCount];
            _currTag = int.MinValue;

            // init bins
            for (int i = 0; i < binCount; i++)
                _bins[i] = new Bin();
        }


        /// <summary>
        /// Returns the number of values in the map.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        /// <summary>
        /// Returns the number of bins in the map.
        /// </summary>
        public int BinCount
        {
            get { return _bins.Length; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract (int, int, int) Discretize(Vec3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected abstract int ToIndex(int i, int j, int k);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private Bin GetBin(int i, int j, int k)
        {
            return _bins[ToIndex(i, j, k)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bin"></param>
        /// <param name="value"></param>
        private void Insert(Bin bin, T value)
        {
            if (bin.Tag != _currTag)
            {
                bin.Clear();
                bin.Tag = _currTag;
            }

            bin.Add(value);
            _count++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vec3d point, T value)
        {
            (int i, int j, int k) = Discretize(point);
            Insert(GetBin(i, j, k), value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="value"></param>
        public void Insert(Domain3d domain, T value)
        {
            (int i0, int j0, int k0) = Discretize(domain.From);
            (int i1, int j1, int k1) = Discretize(domain.To);

            if (i1 < i0) Swap(ref i0, ref i1);
            if (j1 < j0) Swap(ref j0, ref j1);
            if (k1 < k0) Swap(ref k0, ref k1);

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        Insert(GetBin(i, j, k), value);
                    }
                }
            }
        }
        

        /// <summary>
        /// Calls the given delegate on each value within the intersecting bin.
        /// Returns false if the search was aborted which occurs if the delegate return false.
        /// Note the delegate may be called on the same value mutliple times.
        /// </summary>
        public bool Search(Vec3d point, Func<T, bool> callback)
        {
            (int i, int j, int k) = Discretize(point);
            var bin = GetBin(i, j, k);

            if (bin.Tag == _currTag)
            {
                foreach (var t in bin)
                    if (!callback(t)) return false;
            }

            return true;
        }


        /// <summary>
        /// Calls the given delegate on each value within each intersecting bin.
        /// Returns false if the search was aborted which occurs if the delegate return false.
        /// Note the delegate may be called on the same value mutliple times.
        /// </summary>
        public bool Search(Domain3d domain, Func<T, bool> callback)
        {
            (int i0, int j0, int k0) = Discretize(domain.From);
            (int i1, int j1, int k1) = Discretize(domain.To);

            if (i1 < i0) Swap(ref i0, ref i1);
            if (j1 < j0) Swap(ref j0, ref j1);
            if (k1 < k0) Swap(ref k0, ref k1);

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        var bin = GetBin(i, j, k);
                        if (bin.Tag != _currTag) continue;

                        foreach (var t in bin)
                            if (!callback(t)) return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Returns the contents of all intersecting bins by appending them to the given list.
        /// Note this may contain duplicates.
        /// </summary>
        public void Search(Vec3d point, List<T> result)
        {
            Search(point, t =>
            {
                result.Add(t);
                return true;
            });
        }


        /// <summary>
        /// Returns the contents of all intersecting bins by appending them to the given list.
        /// Note this may contain duplicates.
        /// </summary>
        public void Search(Domain3d domain, List<T> result)
        {
            Search(domain, t =>
            {
                result.Add(t);
                return true;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _count = 0;

            if (_currTag == int.MaxValue)
                ResetTags();

            _currTag++;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetTags()
        {
            foreach (var bin in _bins) bin.Tag = int.MinValue;
            _currTag = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        private class Bin : List<T>
        {
            public int Tag = int.MinValue;
        }
    }
}
