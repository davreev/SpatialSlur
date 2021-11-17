﻿
/*
 * Notes
 * 
 * impl ref
 * http://matthias-mueller-fischer.ch/publications/tetraederCollision.pdf
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpatialSlur.Collections
{
    /// <summary>
    /// Infinite uniform grid for broad phase collision detection between dynamic objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class HashGrid2d<T>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        private class Bin : List<T>
        {
            // TODO: Derive from DynamicArray instead

            /// <summary></summary>
            public int Version = int.MinValue;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="version"></param>
            public Bin(int version)
            {
                Version = version;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private readonly struct BinKey : IEquatable<BinKey>
        {
            /// <summary></summary>
            public readonly int I;
            /// <summary></summary>
            public readonly int J;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <param name="j"></param>
            public BinKey(int i, int j)
            {
                I = i;
                J = j;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(BinKey other)
            {
                return I == other.I && J == other.J;
            }


            /// <inheritdoc />
            public override int GetHashCode()
            {
                const int p0 = 73856093;
                const int p1 = 19349663;
                return I * p0 ^ J * p1;
            }


            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return obj is BinKey && Equals((BinKey)obj);
            }
        }

        #endregion
        

        private Dictionary<BinKey, Bin> _bins;
        private double _scale = 1.0;
        private double _scaleInv = 1.0;
        private int _version = int.MinValue;
        private int _count;


        /// <summary>
        /// 
        /// </summary>
        public HashGrid2d()
        {
            _bins = new Dictionary<BinKey, Bin>();
        }


        /// <summary>
        /// 
        /// </summary>
        public HashGrid2d(int capacity)
        {
            _bins = new Dictionary<BinKey, Bin>(capacity);
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
                _scaleInv = 1.0 / _scale;

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
                bin.Version = int.MinValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private BinKey ToKey(Vector2d point)
        {
            return new BinKey(
                (int)Math.Floor(point.X * _scaleInv),
                (int)Math.Floor(point.Y * _scaleInv)
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
                return _bins[key] = new Bin(_version);

            return bin;
        }


        /// <summary>
        /// Inserts the given value into the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="value"></param>
        public void Insert(Vector2d point, T value)
        {
            var bin = GetBin(ToKey(point));

            // sync bin if necessary
            if (bin.Version != _version)
            {
                bin.Version = _version;
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
        public void Insert(Interval2d box, T value)
        {
            box.MakeIncreasing();

            var key0 = ToKey(box.A);
            var key1 = ToKey(box.B);

            for (int j = key0.J; j <= key1.J; j++)
            {
                for (int i = key0.I; i <= key1.I; i++)
                {
                    var bin = GetBin(new BinKey(i, j));

                    // sync bin if necessary
                    if (bin.Version != _version)
                    {
                        bin.Version = _version;
                        bin.Clear();
                    }

                    bin.Add(value);
                    _count++;
                }
            }
        }


        /// <summary>
        /// Returns the contents of the intersecting bin.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public IEnumerable<T> Search(Vector2d point)
        {
            if (SearchImpl(point, out Bin bin))
                return bin;

            return Enumerable.Empty<T>();
        }


        /// <summary>
        /// Returns the contents of all intersecting bins.
        /// </summary>
        public IEnumerable<T> Search(Interval2d box)
        {
            return SearchImpl(box).SelectMany(x => x);
        }


        /// <summary>
        /// Returns the intersecting bin if one exists.
        /// </summary>
        private bool SearchImpl(Vector2d point, out Bin bin)
        {
            return (_bins.TryGetValue(ToKey(point), out bin) && bin.Version == _version);
        }


        /// <summary>
        /// Returns each intersecting bin.
        /// </summary>
        private IEnumerable<Bin> SearchImpl(Interval2d box)
        {
            box.MakeIncreasing();

            var key0 = ToKey(box.A);
            var key1 = ToKey(box.B);

            for (int j = key0.J; j <= key1.J; j++)
            {
                for (int i = key0.I; i <= key1.I; i++)
                {
                    if (_bins.TryGetValue(new BinKey(i, j), out Bin bin) && bin.Version == _version)
                        yield return bin;
                }
            }
        }


        /// <summary>
        /// Returns the contents of the intersecting bin.
        /// </summary>
        public void Search(Vector2d point, List<T> result)
        {
            if (_bins.TryGetValue(ToKey(point), out Bin bin) && bin.Version == _version)
                result.AddRange(bin);
        }


        /// <summary>
        /// Returns the contents of all intersecting bin.
        /// </summary>
        public void Search(Interval2d box, List<T> result)
        {
            box.MakeIncreasing();

            var key0 = ToKey(box.A);
            var key1 = ToKey(box.B);

            for (int j = key0.J; j <= key1.J; j++)
            {
                for (int i = key0.I; i <= key1.I; i++)
                {
                    if (_bins.TryGetValue(new BinKey(i, j), out Bin bin) && bin.Version == _version)
                        result.AddRange(bin);
                }
            }
        }

    }
}
