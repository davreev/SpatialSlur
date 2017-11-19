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
        #region Static

        private const int DefaultCapacity = 4;

        #endregion


        private Dictionary<BinKey, Bin> _bins;
        private double _scale = 1.0;
        private double _invScale = 1.0;
        private int _version = int.MinValue;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public HashGrid3d(int capacity = DefaultCapacity)
        {
            _bins = new Dictionary<BinKey, Bin>(capacity, new BinKeyComparer());
        }


        /// <summary>
        /// 
        /// </summary>
        public HashGrid3d(double scale, int capacity = DefaultCapacity)
            :this(capacity)
        {
            Scale = scale;
        }


        /// <summary>
        /// Returns the number of objects currently in the grid.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        /// <summary>
        /// Gets or sets the scale of the implicit grid used to discretize coordinates.
        /// Note that setting this property also clears the grid.
        /// </summary>
        public double Scale
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
        public void Clear()
        {
            _count = 0;

            if (_version == int.MaxValue)
                ResetVersion();

            _version++;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ResetVersion()
        {
            _version = int.MinValue;

            foreach (var bin in _bins.Values)
                bin.LastVersion = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        private BinKey ToKey(Vec3d point)
        {
            return new BinKey(
                (int)Math.Floor(point.X * _invScale),
                (int)Math.Floor(point.Y * _invScale),
                (int)Math.Floor(point.Z * _invScale)
                );
        }


        /// <summary>
        /// Returns the bin associated with the given key if one exists.
        /// If not, creates a new bin, assigns it to the given key, and returns it.
        /// Used for insertion.
        /// </summary>
        private Bin GetBin(BinKey key)
        {
            if (!_bins.TryGetValue(key, out Bin bin))
                _bins[key] = bin = new Bin(_version);

            return bin;
        }


        /// <summary>
        /// Inserts the given value into the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vec3d point, T value)
        {
            var bin = GetBin(ToKey(point));

            // sync bin if necessary
            if (bin.LastVersion != _version)
            {
                bin.LastVersion = _version;
                bin.Clear();
            }

            bin.Push(value);
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

            var key0 = ToKey(box.A);
            var key1 = ToKey(box.B);

            for (int k = key0.K; k <= key1.K; k++)
            {
                for (int j = key0.J; j <= key1.J; j++)
                {
                    for (int i = key0.I; i <= key1.I; i++)
                    {
                        var bin = GetBin(new BinKey(i, j, k));

                        // sync bin if necessary
                        if (bin.LastVersion != _version)
                        {
                            bin.LastVersion = _version;
                            bin.Clear();
                        }

                        bin.Push(value);
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
            if (SearchImpl(point, out Bin bin))
            {
                foreach (var value in bin)
                    if (!callback(value)) return false;
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
            foreach (var bin in SearchImpl(box))
            {
                foreach (var value in bin)
                    if (!callback(value)) return false;
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
            if (SearchImpl(point, out Bin bin))
                return bin;

            return Enumerable.Empty<T>();
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
        /// Returns the intersecting bin if one exists.
        /// </summary>
        private bool SearchImpl(Vec3d point, out Bin bin)
        {
            // return empty if no bin at key
            if (!_bins.TryGetValue(ToKey(point), out bin))
                return false;

            // skip bin if not synced
            if (bin.LastVersion != _version)
                return false;

            return true;
        }


        /// <summary>
        /// Returns each intersecting bin.
        /// </summary>
        private IEnumerable<Bin> SearchImpl(Interval3d box)
        {
            box.MakeIncreasing();

            var key0 = ToKey(box.A);
            var key1 = ToKey(box.B);

            for (int k = key0.K; k <= key1.K; k++)
            {
                for (int j = key0.J; j <= key1.J; j++)
                {
                    for (int i = key0.I; i <= key1.I; i++)
                    {
                        // skip if no bin at key
                        if (!_bins.TryGetValue(new BinKey(i, j, k), out Bin bin))
                            continue;

                        // skip bin if not synced
                        if (bin.LastVersion != _version)
                            continue;

                        yield return bin;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        private class Bin : Stack<T>
        {
            /// <summary></summary>
            public int LastVersion = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="version"></param>
            public Bin(int version)
            {
                LastVersion = version;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private struct BinKey
        {
            /// <summary></summary>
            public readonly int I;
            /// <summary></summary>
            public readonly int J;
            /// <summary></summary>
            public readonly int K;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <param name="j"></param>
            public BinKey(int i, int j, int k)
            {
                I = i;
                J = j;
                K = k;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private class BinKeyComparer : EqualityComparer<BinKey>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public override bool Equals(BinKey a, BinKey b)
            {
                return a.I == b.I && a.J == b.J && a.K == b.K;
            }


            /// <summary>
            /// http://cybertron.cg.tu-berlin.de/eitz/pdf/2007_hsh.pdf
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override int GetHashCode(BinKey obj)
            {
                const int p0 = 73856093;
                const int p1 = 19349663;
                const int p2 = 83492791;
                return obj.I * p0 ^ obj.J * p1 ^ obj.K * p2;
            }
        }
    }
}
