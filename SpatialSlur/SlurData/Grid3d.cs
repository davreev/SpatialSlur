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
    /// Hash-based uniform grid for broad phase collision detection between dynamic objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Grid3d<T>
    {
        private const int P1 = 73856093; // used in hash function
        private const int P2 = 19349663; // used in hash function
        private const int P3 = 83492791; // used in hash function

        private Bin[] _bins;
        private double _scale, _invScale; // scale of implicit grid

        private int _currVersion = int.MinValue;
        private int _currQuery = int.MinValue;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public Grid3d(int binCount, double binScale = 1.0)
        {
            if (binCount < 1)
                throw new System.ArgumentOutOfRangeException("binCount", "The value must be greater than 0.");

            BinScale = binScale;
            _bins = new Bin[binCount];

            for (int i = 0; i < binCount; i++)
                _bins[i] = new Bin(_currVersion);
        }


        /// <summary>
        /// Returns the number of objects currently in the grid.
        /// </summary>
        public int ObjectCount
        {
            get { return _count; }
        }


        /// <summary>
        /// Returns the number of bins in the grid.
        /// </summary>
        public int BinCount
        {
            get { return _bins.Length; }
        }


        /// <summary>
        /// Gets or sets the scale of the implicit grid used to discretize coordinates.
        /// Note that setting this property also clears the grid.
        /// </summary>
        public double BinScale
        {
            get { return _scale; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException("The value must be larger than 0.");

                _scale = value;
                _invScale = 1.0 / _scale;

                Clear();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private int NextQuery
        {
            get
            {
                if (_currQuery == int.MaxValue)
                    ResetBinQueryTags();

                return ++_currQuery;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetBinQueryTags()
        {
            _currQuery = int.MinValue;

            foreach (var bin in _bins)
                bin.LastQuery = int.MinValue;
        }


        /// <summary>
        /// Resizes the underlying array.
        /// Note that this also clears the grid.
        /// </summary>
        /// <param name="binCount"></param>
        public void Resize(int binCount)
        {
            if (binCount < 1)
                throw new System.ArgumentOutOfRangeException("binCount", "The value must be greater than 0.");

            Clear(); // expire old bins

            int count = _bins.Length;
            Array.Resize(ref _bins, binCount);

            // init any new bins
            for (int i = count; i < binCount; i++)
                _bins[i] = new Bin(_currVersion);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _count = 0;

            if (_currVersion == int.MaxValue)
                ResetBinVersionTags();

            _currVersion++;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetBinVersionTags()
        {
            _currVersion = int.MinValue;

            foreach (var bin in _bins)
                bin.Version = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private (int, int, int) Discretize(Vec3d point)
        {
            return (
                (int)Math.Floor(point.X * _invScale),
                (int)Math.Floor(point.Y * _invScale),
                (int)Math.Floor(point.Z * _invScale));
        }


        /// <summary>
        /// http://cybertron.cg.tu-berlin.de/eitz/pdf/2007_hsh.pdf
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private int ToIndex(int i, int j, int k)
        {
            return SlurMath.Mod2(i * P1 ^ j * P2 ^ k * P3, BinCount);
        }


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
        /// Inserts the given value into the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vec3d point, T value)
        {
            (int i, int j, int k) = Discretize(point);
            var bin = GetBin(i, j, k);

            // sync bin if necessary
            if (bin.Version != _currVersion)
            {
                bin.Version = _currVersion;
                bin.Clear();
            }

            bin.Add(value);
            _count++;
        }


        /// <summary>
        /// Inserts the given value into each intersecting bin.
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="value"></param>
        public void Insert(Domain3d domain, T value)
        {
            domain.MakeIncreasing();

            (int i0, int j0, int k0) = Discretize(domain.From);
            (int i1, int j1, int k1) = Discretize(domain.To);
            var currQuery = NextQuery;

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        var bin = GetBin(i, j, k);

                        // skip bin if already visited
                        if (bin.LastQuery == currQuery) continue;
                        bin.LastQuery = currQuery;

                        // sync bin if necessary
                        if (bin.Version != _currVersion)
                        {
                            bin.Version = _currVersion;
                            bin.Clear();
                        }

                        bin.Add(value);
                        _count++;
                    }
                }
            }
        }


        /// <summary>
        /// Returns the contents of the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<T> Search(Vec3d point)
        {
            (int i, int j, int k) = Discretize(point);
            var bin = GetBin(i, j, k);

            // skip bin if not synced
            if (bin.Version != _currVersion)
                return Enumerable.Empty<T>();

            return bin.Skip(0);
        }


        /// <summary>
        /// Calls the given delegate on each value within the intersecting bin.
        /// The search can be aborted by returning false from the given callback. If this occurs, this function will also return false.
        /// </summary>
        public bool Search(Vec3d point, Func<T, bool> callback)
        {
            (int i, int j, int k) = Discretize(point);
            var bin = GetBin(i, j, k);

            // skip bin if not synced
            if (bin.Version == _currVersion)
            {
                foreach (var t in bin)
                    if (!callback(t)) return false;
            }

            return true;
        }


        /// <summary>
        /// Calls the given delegate on each value within each intersecting bin.
        /// The search can be aborted by returning false from the given callback. If this occurs, this function will also return false.
        /// This method is technically not threadsafe as concurrent calls could result in the same bin being processed multiple times within a single search.
        /// For some applications, this isn't an issue however.
        /// </summary>
        public bool Search(Domain3d domain, Func<T, bool> callback)
        {
            domain.MakeIncreasing();

            (int i0, int j0, int k0) = Discretize(domain.From);
            (int i1, int j1, int k1) = Discretize(domain.To);
            var currQuery = NextQuery;

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        var bin = GetBin(i, j, k);

                        // skip bin if not synced or already visited
                        if (bin.Version != _currVersion || bin.LastQuery == currQuery) continue;
                        bin.LastQuery = currQuery;

                        foreach (var t in bin)
                            if (!callback(t)) return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Adds the contents of each intersecting bin to the given stack.
        /// This separates the task of collecting bins (not threadsafe) from processing their contents (potentially threadsafe) making it more suitable for concurrent applications.
        /// </summary>
        public void Search(Domain2d domain, Stack<IEnumerable<T>> result)
        {
            domain.MakeIncreasing();

            (int i0, int j0, int k0) = Discretize(domain.From);
            (int i1, int j1, int k1) = Discretize(domain.To);
            var currQuery = NextQuery;

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        var bin = GetBin(i, j, k);

                        // skip bin if not synced or already visited
                        if (bin.Version != _currVersion || bin.LastQuery == currQuery)
                            continue;

                        bin.LastQuery = currQuery;
                        result.Push(bin.Skip(0));
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class Bin : List<T>
        {
            /// <summary></summary>
            public int Version = int.MinValue;
            public int LastQuery = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="version"></param>
            public Bin(int version)
            {
                Version = version;
            }
        }
    }
}
