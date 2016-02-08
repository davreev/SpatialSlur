using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Consolidates common functionality from SpatialHash and SpatialGrid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Spatial2d<T>
    {
        private IList<List<T>> _bins;
        private IList<int> _currId; // used to lazily clear bins

        private int _itemCount;
        private int _id;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="binCount"></param>
        protected Spatial2d(int binCount)
        {
            if (binCount < 1)
                throw new System.ArgumentOutOfRangeException("The data structure must have at least 1 bin.");

            _bins = new List<T>[binCount];
            _currId = new int[binCount];

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
            get { return _bins.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        protected abstract void Discretize(Vec2d point, out int i, out int j);
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected abstract int ToIndex(int i, int j);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void Insert(int index, T value)
        {
            List<T> bin = _bins[index];

            // synchronize bin if necessary
            if (_currId[index] != _id)
            {
                bin.Clear();
                _currId[index] = _id;
            }

            bin.Add(value);
            _itemCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="item"></param>
        public void Insert(Vec2d point, T item)
        {
            int i, j;
            Discretize(point, out i, out j);
            Insert(ToIndex(i, j), item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="item"></param>
        public void Insert(Domain2d domain, T item)
        {
            int i0, j0, i1, j1;
            Discretize(domain.From, out i0, out j0);
            Discretize(domain.To, out i1, out j1);

            for (int j = j0; j <= j1; j++)
            {
                for (int i = i0; i <= i1; i++)
                {
                    Insert(ToIndex(i, j), item);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Search(Vec2d point, List<T> result)
        {
            int i, j;
            Discretize(point, out i, out j);
            int index = ToIndex(i, j);

            // only add to result if bin is synchronized
            if (_currId[index] == _id)
                result.AddRange(_bins[index]);
        }


        /// <summary>
        /// 
        /// </summary>
        public void Search(Domain2d domain, List<T> result)
        {
            int i0, j0, i1, j1;
            Discretize(domain.From, out i0, out j0);
            Discretize(domain.To, out i1, out j1);

            for (int j = j0; j <= j1; j++)
            {
                for (int i = i0; i <= i1; i++)
                {
                    int index = ToIndex(i, j);

                    // only add to result if bin is synchronized
                    if (_currId[index] == _id)
                        result.AddRange(_bins[index]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Search(Vec2d point, Action<IEnumerable<T>> callback)
        {
            int i, j;
            Discretize(point, out i, out j);
            int index = ToIndex(i, j);

            // only callback if bin is synchronized
            if (_currId[index] == _id)
                callback(_bins[index]);
        }


        /// <summary>
        ///
        /// </summary>
        public void Search(Domain2d domain, Action<IEnumerable<T>> callback)
        {
            int i0, j0, i1, j1;
            Discretize(domain.From, out i0, out j0);
            Discretize(domain.To, out i1, out j1);

            for (int j = j0; j <= j1; j++)
            {
                for (int i = i0; i <= i1; i++)
                {
                    int index = ToIndex(i, j);

                    // only callback if bin is synchronized
                    if (_currId[index] == _id)
                        callback(_bins[index]);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _id++;
            _itemCount = 0;
        }
    }
}
