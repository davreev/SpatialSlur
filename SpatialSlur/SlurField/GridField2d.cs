using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

using SpatialSlur.SlurCore;

using static SpatialSlur.SlurField.GridUtil;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GridField2d<T> : Grid2d, IField2d<T>, IField3d<T>, IDiscreteField<T>
        where T : struct
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        public static void SaveAsImage(GridField2d<T> field, string path, Func<T, Color> mapper)
        {
            using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
            {
                FieldIO.WriteToImage(field, bmp, mapper);
                bmp.Save(path);
            }
        }

        #endregion


        private readonly T[] _values;

        private SampleMode _sampleMode;
        // private Func<Vec2d, T> _sample;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        protected GridField2d(Domain2d domain, int countX, int countY, SampleMode sampleMode = SampleMode.Linear)
            : base(domain, countX, countY)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="sampleMode"></param>
        protected GridField2d(Grid2d other, SampleMode sampleMode = SampleMode.Linear)
            : base(other)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        protected GridField2d(Domain2d domain, int countX, int countY, WrapMode wrapMode, SampleMode sampleMode = SampleMode.Linear)
            : base(domain, countX, countY, wrapMode)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="sampleMode"></param>
        protected GridField2d(Domain2d domain, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(domain, countX, countY, wrapModeX, wrapModeY)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// Returns the array of values used by this field.
        /// </summary>
        public T[] Values
        {
            get { return _values; }
        }


        /// <summary>
        /// 
        /// </summary>
        public SampleMode SampleMode
        {
            get { return _sampleMode; }
            set { _sampleMode = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return _values[i];
        }


        /// <summary>
        /// Returns a deep copy of this field.
        /// </summary>
        /// <returns></returns>
        protected abstract GridField2d<T> DuplicateBase();


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public T ValueAt(int i, int j)
        {
            return _values[IndexAt(i, j)];
        }


        /// <summary>
        /// Returns the value at the given indices.
        /// Assumes the indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public T ValueAtUnchecked(int i, int j)
        {
            return _values[IndexAtUnchecked(i, j)];
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vec2d point)
        {
            switch (SampleMode)
            {
                case SampleMode.Nearest:
                    return ValueAtNearest(point);
                case SampleMode.Linear:
                    return ValueAtLinear(point);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// Returns the value at the given point.
        /// Assumes the point is inside the field's domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnchecked(Vec2d point)
        {
            switch (SampleMode)
            {
                case SampleMode.Nearest:
                    return ValueAtNearestUnchecked(point);
                case SampleMode.Linear:
                    return ValueAtLinearUnchecked(point);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private T ValueAtNearest(Vec2d point)
        {
            return _values[IndexAt(point)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private T ValueAtNearestUnchecked(Vec2d point)
        {
            return _values[IndexAtUnchecked(point)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinear(Vec2d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinearUnchecked(Vec2d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(GridPoint2d point);


        /// <summary>
        /// Sets all values along the boundary of the field to a given constant
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundary(T value)
        {
            SetBoundaryX(value);
            SetBoundaryY(value);
        }


        /// <summary>
        /// Sets all values along the X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX(T value)
        {
            int offset = Count - CountX;

            for (int i = 0; i < CountX; i++)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY(T value)
        {
            int offset = CountX - 1;

            for (int i = 0; i < Count; i += CountX)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the lower X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX0(T value)
        {
            for (int i = 0; i < CountX; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper X boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryX1(T value)
        {
            for (int i = Count - CountX; i < Count; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the lower Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY0(T value)
        {
            for (int i = 0; i < Count; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY1(T value)
        {
            for (int i = CountX - 1; i < Count; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionXY(Func<Vec2d, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(CoordinateAt(i, j));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionXY(Func<double, double, T> func, bool parallel = false)
        {
            double x0 = Domain.X.T0;
            double y0 = Domain.Y.T0;

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ScaleX + x0, j * ScaleY + y0);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<Vec2d, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(new Vec2d(i * ti, j * tj));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<double, double, T> func, bool parallel = false)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i * ti, j * tj);
                }
            };


            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionIJ(Func<int, int, T> func, bool parallel = false)
        {
            Action<Tuple<int, int>> func2 = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = func(i, j);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func2);
            else
                func2(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Sample(GridField2d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            Sample((IField2d<T>)other, parallel);
        }


        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        public void Sample<U>(GridField2d<U> other, Func<U, T> converter, bool parallel = false)
            where U : struct
        {
            if (ResolutionEquals(other))
            {
                other._values.Convert(converter, _values);
                return;
            }

            Sample((IField2d<U>)other, converter, parallel);
        }


        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Sample(IField2d<T> other, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = other.ValueAt(CoordinateAt(i, j));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        public void Sample<U>(IField2d<U> other, Func<U, T> converter, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = converter(other.ValueAt(CoordinateAt(i, j)));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), func);
            else
                func(Tuple.Create(0, Count));
        }


        #region Explicit interface implementations
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField3d<T>.ValueAt(Vec3d point)
        {
            return ValueAt(new Vec2d(point.X, point.Y));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField<T> IDiscreteField<T>.Duplicate()
        {
            return DuplicateBase();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
                if (item.Equals(_values[i])) return i;

            return -1;
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Contains(T item)
        {
            for (int i = 0; i < Count; i++)
                if (item.Equals(_values[i])) return true;

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection<T>.CopyTo(T[] array, int index)
        {
            array.SetRange(_values, index, 0, Count);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
