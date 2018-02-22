using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField2d
    {
        /// <summary></summary>
        public static readonly GridField2dFactory<double> Double = new GridField2dDouble.Factory();

        /// <summary></summary>
        public static readonly GridField2dFactory<Vec2d> Vec2d = new GridField2dVec2d.Factory();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GridField2d<T> : Grid2d, IField2d<T>, IField3d<T>, IDiscreteField2d<T>, IDiscreteField3d<T>
        where T : struct
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator T[](GridField2d<T> field)
        {
            return field.Values;
        }

        #endregion


        private readonly T[] _values;
        private SampleMode _sampleMode = SampleMode.Linear;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public GridField2d(int countX, int countY)
            : base(countX, countY)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        public GridField2d(Grid2d grid)
            : base(grid)
        {
            _values = new T[Count];
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
                return _values[IndexAtUnsafe(i, j)];
            }
            set
            {
                CoreUtil.BoundsCheck(i, CountX);
                _values[IndexAtUnsafe(i, j)] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"GridField2d<{typeof(T).Name}> ({CountX}, {CountY})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridField2d<T> Duplicate()
        {
            return Duplicate(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract GridField2d<T> Duplicate(bool setValues);


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
        public T ValueAtUnsafe(int i, int j)
        {
            return _values[IndexAtUnsafe(i, j)];
        }


        /*
        /// <inheritdoc/>
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
        */


        /// <inheritdoc/>
        public T ValueAt(Vec2d point)
        {
            // conditional is used instead of switch for inline optimization
            return SampleMode == SampleMode.Nearest ?
                ValueAtNearest(point) : ValueAtLinear(point);
        }


        /*
        /// <summary>
        /// Returns the value at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnsafe(Vec2d point)
        {
            switch (SampleMode)
            {
                case SampleMode.Nearest:
                    return ValueAtNearestUnsafe(point);
                case SampleMode.Linear:
                    return ValueAtLinearUnsafe(point);
            }

            throw new NotSupportedException();
        }
        */


        /// <summary>
        /// Returns the value at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnsafe(Vec2d point)
        {
            // conditional is used instead of switch for inline optimization
            return SampleMode == SampleMode.Nearest ?
                ValueAtNearestUnsafe(point) : ValueAtLinearUnsafe(point);
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
        private T ValueAtNearestUnsafe(Vec2d point)
        {
            return _values[IndexAtUnsafe(point)];
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
        protected abstract T ValueAtLinearUnsafe(Vec2d point);


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
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
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
                this.Set(other);
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
                other.Convert(converter, this, parallel);
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
        IDiscreteField<T> IDiscreteField<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField2d<T> IDiscreteField2d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDiscreteField3d<T> IDiscreteField3d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
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
