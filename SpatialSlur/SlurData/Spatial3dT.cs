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
    public abstract class Spatial3d<T>
    {
        private IList<List<T>> _bins;
        private IList<int> _currId; // used to lazily clear bins

        private int _itemCount;
        private int _id;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        protected Spatial3d(int binCount)
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
        protected abstract void Discretize(Vec3d point, out int i, out int j, out int k);
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected abstract int ToIndex(int i, int j, int k);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param> 
        /// <param name="item"></param>
        private void Insert(int index, T item)
        {
            List<T> bin = _bins[index];

            // synchronize bin if necessary
            if (_currId[index] != _id)
            {
                bin.Clear();
                _currId[index] = _id;
            }

            bin.Add(item);
            _itemCount++;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
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
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
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
        /// 
        /// </summary>
        public void Search(Vec3d point, List<T> result)
        {
            int i, j, k;
            Discretize(point, out i, out j, out k);
            int key = ToIndex(i, j, k);

            // only add if bin is synchronized
            if (_currId[key] == _id)
                result.AddRange(_bins[key]);
        }


        /// <summary>
        /// Note that this method may return duplicates of found items.
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
                        int key = ToIndex(i, j, k);

                        // only add if bin is synchronized
                        if (_currId[key] == _id)
                            result.AddRange(_bins[key]);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Search(Vec3d point, Action<IEnumerable<T>> callback)
        {
            int i, j, k;
            Discretize(point, out i, out j, out k);
            int key = ToIndex(i, j, k);

            // only callback if bin is synched
            if (_currId[key] == _id)
                callback(_bins[key]);
        }


        /// <summary>
        /// Note that the callback may be invoked on the same found item multiple times.
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
                        int key = ToIndex(i, j, k);

                        // only callback if bin is synched
                        if (_currId[key] == _id)
                            callback(_bins[key]);
                    }
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
