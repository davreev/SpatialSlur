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
    public abstract class GridField2d<T> : Grid2d, IField2d<T>, IField3d<T>, IDiscreteField2d<T>, IDiscreteField3d<T>
        where T : struct
    {
        private readonly T[] _values;
        private SampleMode _sampleMode;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, wrapMode)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="sampleMode"></param>
        public GridField2d(Vec2d origin, Vec2d scale, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, wrapModeX, wrapModeY)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridField2d(Interval2d interval, int countX, int countY, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, wrapMode)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="sampleMode"></param>
        public GridField2d(Interval2d interval, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, wrapModeX, wrapModeY)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sampleMode"></param>
        public GridField2d(Grid2d grid, SampleMode sampleMode = SampleMode.Linear)
            :base(grid)
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
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T this[int i, int j]
        {
            get
            {
                CoreUtil.BoundsCheck(i, CountX);
                return _values[IndexAtUnchecked(i, j)];
            }
            set
            {
                CoreUtil.BoundsCheck(i, CountX);
                _values[IndexAtUnchecked(i, j)] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract GridField2d<T> DuplicateBase(bool copyValues);


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
        /// Assumes the point is within the bounds of the grid.
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
        /// <param name="normalize"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec2d, T> func, bool normalize = false, bool parallel = false)
        {
            SpatialFunction(func, Values, normalize, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="normalize"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec2d, T> func, IDiscreteField<T> result, bool normalize = false, bool parallel = false)
        {
            SpatialFunction(func, result.Values, normalize, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="normalize"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec2d, T> func, T[] result, bool normalize = false, bool parallel = false)
        {
            if (normalize)
                SpatialFunctionUV(func, result, parallel);
            else
                SpatialFunctionXY(func, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionXY(Func<Vec2d, T> func, T[] result, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1,range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    result[index] = func(CoordinateAt(i, j));
                }
            }
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionUV(Func<Vec2d, T> func, T[] result, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                double ti = 1.0 / (CountX - 1);
                double tj = 1.0 / (CountY - 1);

                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    result[index] = func(new Vec2d(i * ti, j * tj));
                }
            }
        }


        /// <summary>
        /// Sets this field to some function of its coordinates in grid space.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, T> func, bool parallel = false)
        {
            SpatialFunction(func, Values, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this fields coordinates in grid space.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            SpatialFunction(func, result.Values, parallel);
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, T> func, T[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (int i, int j) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    result[index] = func(i, j);
                }
            }
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
            
            this.Sample((IField2d<T>)other, parallel);
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

            this.Sample((IField2d<U>)other, converter, parallel);
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
        IDiscreteField<T> IDiscreteField<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField2d<T> IDiscreteField2d<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField3d<T> IDiscreteField3d<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }


        /// <summary>
        /// 
        /// </summary>
        IEnumerable<Vec3d> IDiscreteField3d<T>.Coordinates
        {
            get
            {
                for (int j = 0; j < CountY; j++)
                {
                    for (int i = 0; i < CountX; i++)
                        yield return CoordinateAt(i, j);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Vec3d IDiscreteField3d<T>.CoordinateAt(int index)
        {
            return CoordinateAt(index);
        }

        #endregion
    }
}
