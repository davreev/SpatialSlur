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
    [Serializable]
    public abstract class GridField2d
    {
        private Domain2d _domain;
        private double _x0, _y0; // cached for convenience
        private double _dx, _dy;
        private double _dxInv, _dyInv; // cached to avoid unecessary divs
        private readonly int _nx, _ny, _n;

        private WrapMode _wrapModeX, _wrapModeY;
        // Func<int, int> _wrapX, _wrapY; // compare delegate performance to switch (release & debug)


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        private GridField2d(int countX, int countY)
        {
            if (countX < 1 || countY < 1)
                throw new System.ArgumentException("The field must have at least 1 value in each dimension.");

            _nx = countX;
            _ny = countY;
            _n = _nx * _ny;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        protected GridField2d(Domain2d domain, int countX, int countY)
            : this(countX, countY)
        {
            Domain = domain;
            WrapModeX = WrapModeY = WrapMode.Clamp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapMode"></param>
        protected GridField2d(Domain2d domain, int countX, int countY, WrapMode wrapMode)
            : this(countX, countY)
        {
            Domain = domain;
            WrapModeX = WrapModeY = wrapMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        protected GridField2d(Domain2d domain, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY)
            : this(countX, countY)
        {
            Domain = domain;
            WrapModeX = wrapModeX;
            WrapModeY = wrapModeY;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected GridField2d(GridField2d other)
        {
            _nx = other._nx;
            _ny = other._ny;
            _n = other._n;

            Domain = other._domain;
            WrapModeX = other._wrapModeX;
            WrapModeY = other._wrapModeY;
        }


        /// <summary>
        /// Gets/sets the domain of the field.
        /// </summary>
        public Domain2d Domain
        {
            get { return _domain; }
            set
            {
                if (!value.IsValid)
                    throw new System.ArgumentException("The given domain must be valid.");

                _domain = value;
                OnDomainChange();
            }
        }


        /// <summary>
        /// Returns the number of values in the field.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// Returns the number of values in the x direction.
        /// </summary>
        public int CountX
        {
            get { return _nx; }
        }


        /// <summary>
        /// Returns the number of values in the y direction.
        /// </summary>
        public int CountY
        {
            get { return _ny; }
        }


        /// <summary>
        /// Returns the distance between values in the x direction.
        /// </summary>
        public double ScaleX
        {
            get { return _dx; }
        }


        /// <summary>
        /// Returns the distance between values in the y direction.
        /// </summary>
        public double ScaleY
        {
            get { return _dy; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeX
        {
            get { return _wrapModeX; }
            set { _wrapModeX = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeY
        {
            get { return _wrapModeY; }
            set { _wrapModeY = value; }
        }

        
        /// <summary>
        /// Enumerates over the coordinates of the field.
        /// </summary>
        public IEnumerable<Vec2d> Coordinates
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
        private void OnDomainChange()
        {
            _x0 = _domain.X.T0;
            _y0 = _domain.Y.T0;

            _dx = _domain.X.Span / (_nx - 1);
            _dy = _domain.Y.Span / (_ny - 1);

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool ContainsIndices(int i, int j)
        {
            return SlurMath.Contains(i, _nx) && SlurMath.Contains(j, _ny);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Vec2d CoordinateAt(int i, int j)
        {
            return new Vec2d(
                i * _dx + _x0,
                j * _dy + _y0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec2d CoordinateAt(int index)
        {
            (int i, int j) = IndicesAt(index);
            return CoordinateAt(i, j);
        }


        /// <summary>
        /// Returns true if the field has the same number of values in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(GridField2d other)
        {
            return (_nx == other._nx && _ny == other._ny);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WrapX(int i)
        {
            return Wrap(i, _nx, _wrapModeX);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public int WrapY(int j)
        {
            return Wrap(j, _ny, _wrapModeY);
        }
        

        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// Applies the current wrap mode to the given indices.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j)
        {
            return FlattenIndices(WrapX(i), WrapY(j), _nx);
        }


        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j)
        {
            return FlattenIndices(i, j, _nx);
        }


        /// <summary>
        /// Returns the index of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return FlattenIndices(WrapX(i), WrapY(j), _nx);
        }


        /// <summary>
        /// Returns the index of the nearest value to the given point.
        /// Assumes the point is inside the field's domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return FlattenIndices(i, j, _nx);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int) IndicesAt(int index)
        {
            return ExpandIndex(index, _nx);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void IndicesAt(int index, out int i, out int j)
        {
            (i, j) = ExpandIndex(index, _nx);
        }


        /// <summary>
        /// Returns the indices of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        public (int, int) IndicesAt(Vec2d point)
        {
            return (
                (int)Math.Round((point.X - _x0) * _dxInv),
                (int)Math.Round((point.Y - _y0) * _dyInv)
                );
        }


        /// <summary>
        /// Returns the indices of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void IndicesAt(Vec2d point, out int i, out int j)
        {
            (i, j) = IndicesAt(point);
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAt(Vec2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAt(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vec2d point, GridPoint2d result)
        {
            (double u, double v) = Fract(point, out int i0, out int j0);
            result.SetWeights(u, v);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * _nx;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * _nx;

            var corners = result.Corners;
            corners[0] = i0 + j0;
            corners[1] = i1 + j0;
            corners[2] = i0 + j1;
            corners[3] = i1 + j1;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is inside the field's domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAtUnchecked(Vec2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is inside the field's domain.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnchecked(Vec2d point, GridPoint2d result)
        {
            (double u, double v) = Fract(point, out int i0, out int j0);
            result.SetWeights(u, v);

            j0 *= _nx;
            int i1 = i0 + 1;
            int j1 = j0 + _nx;

            var corners = result.Corners;
            corners[0] = i0 + j0;
            corners[1] = i1 + j0;
            corners[2] = i0 + j1;
            corners[3] = i1 + j1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected (double, double) Fract(Vec3d point, out int i, out int j)
        {
            return (
            SlurMath.Fract((point.X - _x0) * _dxInv, out i),
            SlurMath.Fract((point.Y - _y0) * _dyInv, out j)
            );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal (int, int) GetBoundaryOffsets()
        {
            return (
                WrapModeX == WrapMode.Repeat ? CountX - 1 : 0,
                WrapModeY == WrapMode.Repeat ? Count - CountX : 0
                );
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GridField2d<T> : GridField2d, IField2d<T>, IField3d<T>, IDiscreteField2d<T>, IDiscreteField3d<T>
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
        /// <param name="sampleMode"></param>
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
        protected GridField2d(GridField2d other, SampleMode sampleMode = SampleMode.Linear)
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
        public void SpatialFunctionXY(Func<double, double, T> func, bool parallel = false)
        {
            SpatialFunctionXY(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionXY(Func<double, double, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            var values = result.Values;

            double x0 = Domain.X.T0;
            double y0 = Domain.Y.T0;

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    values[index] = func(i * ScaleX + x0, j * ScaleY + y0);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionXY(Func<Vec2d, T> func, bool parallel = false)
        {
            SpatialFunctionXY(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionXY(Func<Vec2d, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            SpatialFunctionXY((x, y) => func(new Vec2d(x, y)), result, parallel);
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<double, double, T> func, bool parallel = false)
        {
            SpatialFunctionUV(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<double, double, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            var values = result.Values;

            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    values[index] = func(i * ti, j * tj);
                }
            };


            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<Vec2d, T> func, bool parallel = false)
        {
            SpatialFunctionUV(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's normalized coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionUV(Func<Vec2d, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            SpatialFunctionUV((u, v) => func(new Vec2d(u, v)), result, parallel);
        }


        /// <summary>
        /// Sets this field to some function of its indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionIJ(Func<int, int, T> func, bool parallel = false)
        {
            SpatialFunctionIJ(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionIJ(Func<int, int, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            var values = result.Values;

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    values[index] = func(i, j);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// Sets this field to some function of its indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionIJ(Func<Vec2i, T> func, bool parallel = false)
        {
            SpatialFunctionIJ(func, this, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's indices.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunctionIJ(Func<Vec2i, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            SpatialFunctionIJ((i, j) => func(new Vec2i(i, j)), result, parallel);
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
            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = other.ValueAt(CoordinateAt(i, j));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
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
            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    _values[index] = converter(other.ValueAt(CoordinateAt(i, j)));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
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
