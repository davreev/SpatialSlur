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
    /// Infinite uniform grid for broad phase collision detection between dynamic objects.
    /// Implementation is based on http://matthias-mueller-fischer.ch/publications/tetraederCollision.pdf.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HashGrid3d<T>
    {
        private Bin[] _bins;
        private double _scale, _invScale; // scale of implicit grid

        private int _currVersion = int.MinValue;
        private int _currQuery = int.MinValue;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public HashGrid3d(int binCount, double binScale = 1.0)
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
                bin.LastVersion = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        private (int, int, int) IndicesAt(Vec3d point)
        {
            return (Discretize(point.X), Discretize(point.Y), Discretize(point.Z));
        }


        /// <summary>
        /// 
        /// </summary>
        private int Discretize(double t)
        {
            return (int)Math.Floor(t * _invScale);
        }


        /// <summary>
        /// http://cybertron.cg.tu-berlin.de/eitz/pdf/2007_hsh.pdf
        /// </summary>
        private int IndexAt(int i, int j, int k)
        {
            const int p0 = 73856093;
            const int p1 = 19349663;
            const int p2 = 83492791;
            return SlurMath.Mod2(i * p0 ^ j * p1 ^ k * p2, BinCount);
        }


        /// <summary>
        /// 
        /// </summary>
        private Bin GetBin(int i, int j, int k)
        {
            return _bins[IndexAt(i, j, k)];
        }


        /// <summary>
        /// Inserts the given value into the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vec3d point, T value)
        {
            (int i, int j, int k) = IndicesAt(point);
            var bin = GetBin(i, j, k);

            // sync bin if necessary
            if (bin.LastVersion != _currVersion)
            {
                bin.LastVersion = _currVersion;
                bin.Clear();
            }

            bin.Add(value);
            _count++;
        }


        /// <summary>
        /// Inserts the given value into each intersecting bin.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="value"></param>
        public void Insert(Interval3d box, T value)
        {
            box.MakeIncreasing();

            (int i0, int j0, int k0) = IndicesAt(box.A);
            (int i1, int j1, int k1) = IndicesAt(box.B);
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
                        if (bin.LastVersion != _currVersion)
                        {
                            bin.LastVersion = _currVersion;
                            bin.Clear();
                        }

                        bin.Add(value);
                        _count++;
                    }
                }
            }
        }


        /// <summary>
        /// Calls the given delegate on each value within the intersecting bin.
        /// The search can be aborted by returning false from the given callback. If this occurs, this function will also return false.
        /// </summary>
        public bool Search(Vec3d point, Func<T, bool> callback)
        {
            (int i, int j, int k) = IndicesAt(point);
            var bin = GetBin(i, j, k);

            // skip bin if not synced
            if (bin.LastVersion == _currVersion)
            {
                foreach (var t in bin)
                    if (!callback(t)) return false;
            }

            return true;
        }


        /// <summary>
        /// Calls the given delegate on each value within each intersecting bin.
        /// The search can be aborted by returning false from the given callback at any time. If this occurs, this function will also return false.
        /// This method is technically not threadsafe as concurrent calls could result in the same bin being processed multiple times within a single search.
        /// For some applications, this isn't an issue however.
        /// </summary>
        public bool Search(Interval3d box, Func<T, bool> callback)
        {
            foreach(var bin in SearchImpl(box))
            {
                foreach (var t in bin)
                    if (!callback(t)) return false;
            }
            
            return true;
        }


        /// <summary>
        /// Returns the contents of the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<T> Search(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            var bin = GetBin(i, j, k);

            // skip bin if not synced
            if (bin.LastVersion != _currVersion)
                return Enumerable.Empty<T>();

            return bin;
        }


        /// <summary>
        /// Returns the contents of all intersecting bins.
        /// </summary>
        public IEnumerable<T> Search(Interval3d box)
        {
            return SearchImpl(box).SelectMany(x => x);
        }


        /// <summary>
        /// Adds the contents of each intersecting bin to the given stack.
        /// This implementation separates the the collection of bins (not threadsafe) from the processing of their contents (potentially threadsafe) making it better suited to concurrent applications.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="result"></param>
        public void Search(Interval3d box, Stack<IEnumerable<T>> result)
        {
            foreach (var bin in SearchImpl(box))
                result.Push(bin);
        }


        /// <summary>
        /// Returns each intersecting bin.
        /// </summary>
        private IEnumerable<Bin> SearchImpl(Interval3d box)
        {
            box.MakeIncreasing();

            (int i0, int j0, int k0) = IndicesAt(box.A);
            (int i1, int j1, int k1) = IndicesAt(box.B);
            var currQuery = NextQuery;

            for (int k = k0; k <= k1; k++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    for (int i = i0; i <= i1; i++)
                    {
                        var bin = GetBin(i, j, k);

                        // skip bin if not synced or already visited
                        if (bin.LastVersion != _currVersion || bin.LastQuery == currQuery)
                            continue;

                        bin.LastQuery = currQuery;
                        yield return bin;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private class Bin : List<T>
        {
            /// <summary></summary>
            public int LastVersion = int.MinValue;
            public int LastQuery = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="version"></param>
            public Bin(int version)
            {
                LastVersion = version;
            }
        }
    }
}
