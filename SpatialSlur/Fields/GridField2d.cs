
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.Collections;
using SpatialSlur.Fields;

using static SpatialSlur.Utilities;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField2d
    {
        /// <summary></summary>
        public static readonly GridField2dFactory<double> Double = new GridField2dDouble.Factory();

        /// <summary></summary>
        public static readonly GridField2dFactory<Vector2d> Vector2d = new GridField2dVector2d.Factory();

        /// <summary></summary>
        public static readonly GridField2dFactory<Vector3d> Vector3d = new GridField2dVector3d.Factory();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GridField2d<T> : Grid2d, ISampledField2d<T>, ISampledField3d<T>
        where T : struct
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator T[](GridField2d<T> field)
        {
            return field._values;
        }

        #endregion


        private readonly T[] _values;
        private SampleMode _sampleMode = SampleMode.Linear;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public GridField2d(int countX, int countY)
            : base(countX, countY)
        {
            _values = new T[CountXY];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        public GridField2d(Grid2d grid)
            : base(grid)
        {
            _values = new T[CountXY];
        }


        /// <summary>
        /// Returns a reference to the backing array of grid values.
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
        /// Gets or sets the value at the give point in grid space.
        /// Note that this performs bounds checks for each dimension.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[Vector2i index]
        {
            get
            {
                BoundsCheck(index.X, CountX);
                return _values[ToIndexUnsafe(index)];
            }
            set
            {
                BoundsCheck(index.X, CountX);
                _values[ToIndexUnsafe(index)] = value;
            }
        }


        /// <inheritdoc />
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

        
        /// <summary>
        /// Returns the value at the given point in grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vector2i point)
        {
            return _values[ToIndex(point)];
        }


        /// <summary>
        /// Returns the value at the given point in grid space.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnsafe(Vector2i point)
        {
            return _values[ToIndexUnsafe(point)];
        }


        /// <inheritdoc />
        /// <summary>
        /// Returns the value at the given point in world space using the current sample mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vector2d point)
        {
            // conditional implementation allows for inline optimization
            return SampleMode == SampleMode.Nearest ?
                ValueAtNearest(point) : ValueAtLinear(point);

            /*
            switch (SampleMode)
            {
                case SampleMode.Nearest:
                    return ValueAtNearest(point);
                case SampleMode.Linear:
                    return ValueAtLinear(point);
            }

            throw new NotSupportedException();
            */
        }


        /// <summary>
        /// Returns the value at the given point in world space based on the current sample mode.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnsafe(Vector2d point)
        {
            // conditional implementation allows for inline optimization
            return SampleMode == SampleMode.Nearest ?
                ValueAtNearestUnsafe(point) : ValueAtLinearUnsafe(point);

            /*
            switch (SampleMode)
            {
                case SampleMode.Nearest:
                    return ValueAtNearestUnsafe(point);
                case SampleMode.Linear:
                    return ValueAtLinearUnsafe(point);
            }

            throw new NotSupportedException();
            */
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private T ValueAtNearest(Vector2d point)
        {
            point = Vector2d.Round(ToGridSpace(point));
            return _values[ToIndex(point.As2i)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        private T ValueAtNearestUnsafe(Vector2d point)
        {
            point = Vector2d.Round(ToGridSpace(point));
            return _values[ToIndexUnsafe(point.As2i)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinear(Vector2d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinearUnsafe(Vector2d point);


        /// <summary>
        /// Returns the value at the given grid point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(ref GridPoint2d point);


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAt(Vector2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAt(point, ref result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vector2d point, ref GridPoint2d result)
        {
            result.SetWeights(Vector2d.Fract(ToGridSpace(point), out Vector2i whole));
            GridPointAt(whole, ref result);
        }
        
        
        /// <summary>
        /// Returns a grid point at the given point in grid space
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vector2i point, ref GridPoint2d result)
        {
            var x0 = WrapX(point.X);
            var y0 = WrapY(point.Y) * CountX;

            int x1 = WrapX(point.X + 1);
            int y1 = WrapY(point.Y + 1) * CountX;

            result.Index0 = x0 + y0;
            result.Index1 = x1 + y0;
            result.Index2 = x0 + y1;
            result.Index3 = x1 + y1;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAtUnsafe(Vector2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAtUnsafe(point, ref result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnsafe(Vector2d point, ref GridPoint2d result)
        {
            result.SetWeights(Vector2d.Fract(ToGridSpace(point), out Vector2i whole));
            GridPointAtUnsafe(whole, ref result);
        }


        /// <summary>
        /// Returns a grid point at the given point in grid space.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnsafe(Vector2i point, ref GridPoint2d result)
        {
            var x0 = point.X;
            var y0 = point.Y * CountX;

            int x1 = x0 + 1;
            int y1 = y0 + CountX;

            result.Index0 = x0 + y0;
            result.Index1 = x1 + y0;
            result.Index2 = x0 + y1;
            result.Index3 = x1 + y1;
        }


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
            int offset = CountXY - CountX;

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

            for (int i = 0; i < CountXY; i += CountX)
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
            for (int i = CountXY - CountX; i < CountXY; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the lower Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY0(T value)
        {
            for (int i = 0; i < CountXY; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper Y boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryY1(T value)
        {
            for (int i = CountX - 1; i < CountXY; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets this field to some function of its world space coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SampleInWorldSpace(Func<Vector2d, T> func, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXY);

            void Body(int from, int to)
            {
                var vals = _values;
                var nx = CountX;
                var p = ToGridSpace(from);

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    vals[index] = func(ToWorldSpace(p));
                }
            }
        }


        /// <summary>
        /// Sets this field to some function of its grid space coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SampleInGridSpace(Func<Vector2i, T> func, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXY);

            void Body(int from, int to)
            {
                var vals = _values;
                var nx = CountX;
                var p = ToGridSpace(from);

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    vals[index] = func(p);
                }
            }
        }


        /// <summary>
        /// Sets this field to some function of its unit space coordinates (0-1).
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SampleInUnitSpace(Func<Vector2d, T> func, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXY), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXY);

            void Body(int from, int to)
            {
                var vals = _values;
                (var nx, var ny) = Count;
                var p = ToGridSpace(from);

                var t = new Vector2d(
                    1.0 / (nx - 1),
                    1.0 / (ny - 1));

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    vals[index] = func(p * t);
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
            if (Count == other.Count)
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
            if (Count == other.Count)
            {
                other.Convert(converter, this, parallel);
                return;
            }

            this.Sample((IField2d<U>)other, converter, parallel);
        }


        #region Explicit Interface Implementations

        ArrayView<T> ISampledField<T>.Values
        {
            get { return _values; }
        }
       

        int ISampledField<T>.Count
        {
            get { return CountXY; }
        }


        T IField3d<T>.ValueAt(Vector3d point)
        {
            return ValueAt(new Vector2d(point.X, point.Y));
        }


        ISampledField<T> ISampledField<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        ISampledField2d<T> ISampledField2d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        ISampledField3d<T> ISampledField3d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        IEnumerable<Vector2d> ISampledField2d<T>.Points
        {
            get
            {
                (var nx, var ny) = Count;

                for (int y = 0; y < ny; y++)
                {
                    for (int x = 0; x < nx; x++)
                        yield return ToWorldSpace(new Vector2d(x, y));
                }
            }
        }


        Vector2d ISampledField2d<T>.PointAt(int index)
        {
            return ToWorldSpace(index);
        }


        IEnumerable<Vector3d> ISampledField3d<T>.Points
        {
            get
            {
                (var nx, var ny) = Count;

                for (int y = 0; y < ny; y++)
                {
                    for (int x = 0; x < nx; x++)
                        yield return ToWorldSpace(new Vector2d(x, y)).As3d;
                }
            }
        }


        Vector3d ISampledField3d<T>.PointAt(int index)
        {
            return ToWorldSpace(index).As3d;
        }

#endregion
    }
}
