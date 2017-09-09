using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

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
    public abstract class GridField3d<T> : Grid3d, IField2d<T>, IField3d<T>, IDiscreteField3d<T>
        where T : struct
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="path"></param>
        /// <param name="mapper"></param>
        public static void SaveAsImageStack(GridField3d<T> field, string path, Func<T, Color> mapper)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            Parallel.For(0, field.CountZ, z =>
            {
                using (Bitmap bmp = new Bitmap(field.CountX, field.CountY, PixelFormat.Format32bppArgb))
                {
                    FieldIO.WriteToImage(field, z, bmp, mapper);
                    bmp.Save(String.Format(@"{0}\{1}_{2}{3}", dir, name, z, ext));
                }
            });
        }

        #endregion


        private readonly T[] _values;
        private SampleMode _sampleMode;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, countZ, wrapMode)
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
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        /// <param name="sampleMode"></param>
        public GridField3d(Vec3d origin, Vec3d scale, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
            : base(origin, scale, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ)
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
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        /// <param name="sampleMode"></param>
        public GridField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapMode = WrapMode.Clamp, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, countZ, wrapMode)
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
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        /// <param name="sampleMode"></param>
        public GridField3d(Interval3d interval, int countX, int countY, int countZ, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ, SampleMode sampleMode = SampleMode.Linear)
            : base(interval, countX, countY, countZ, wrapModeX, wrapModeY, wrapModeZ)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sampleMode"></param>
        public GridField3d(Grid3d grid, SampleMode sampleMode = SampleMode.Linear)
            :base(grid)
        {
            _values = new T[Count];
            SampleMode = sampleMode;
        }


        /// <summary>
        /// 
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
        /// <param name="k"></param>
        /// <returns></returns>
        public T this[int i, int j, int k]
        {
            get
            {
                CoreUtil.BoundsCheck(i, CountX);
                CoreUtil.BoundsCheck(j, CountY);
                return _values[IndexAtUnchecked(i, j, k)];
            }
            set
            {
                CoreUtil.BoundsCheck(i, CountX);
                CoreUtil.BoundsCheck(j, CountY);
                _values[IndexAtUnchecked(i, j, k)] = value;
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
        protected abstract GridField3d<T> DuplicateBase(bool copyValues);


        /// <summary>
        /// Returns the value at the given indices.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public T ValueAt(int i, int j, int k)
        {
            return _values[IndexAt(i, j, k)];
        }


        /// <summary>
        /// Returns the value at the given indices.
        /// Assumes the indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public T ValueAtUnchecked(int i, int j, int k)
        {
            return _values[IndexAtUnchecked(i, j, k)];
        }


        /// <inheritdoc/>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vec3d point)
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
        public T ValueAtUnchecked(Vec3d point)
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
        /// <returns></returns>
        private T ValueAtNearest(Vec3d point)
        {
            return _values[IndexAt(point)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private T ValueAtNearestUnchecked(Vec3d point)
        {
            return _values[IndexAtUnchecked(point)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinear(Vec3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinearUnchecked(Vec3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(GridPoint3d point);


        /// <summary>
        /// Sets all values along the boundary of the field to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundary(T value)
        {
            SetBoundaryXY(value);
            SetBoundaryXZ(value);
            SetBoundaryYZ(value);
        }


        /// <summary>
        /// Sets all values along the XY boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXY(T value)
        {
            int offset = Count - CountXY;

            for (int i = 0; i < CountXY; i++)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the XZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXZ(T value)
        {
            int offset = CountXY - CountX;

            for (int k = 0; k < CountZ; k++)
            {
                int i0 = k * CountXY;
                int i1 = i0 + CountX;

                for (int i = i0; i < i1; i++)
                    _values[i] = _values[i + offset] = value;
            }
        }


        /// <summary>
        /// Sets all values along the YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryYZ(T value)
        {
            int offset = CountX - 1;

            for (int i = 0; i < Count; i += CountX)
                _values[i] = _values[i + offset] = value;
        }


        /// <summary>
        /// Sets all values along the lower XY boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXY0(T value)
        {
            for (int i = 0; i < CountXY; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper XY boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXY1(T value)
        {
            for (int i = Count - CountXY; i < Count; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the lower YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXZ0(T value)
        {
            for (int k = 0; k < CountZ; k++)
            {
                int i0 = k * CountXY;
                int i1 = i0 + CountX;

                for (int i = i0; i < i1; i++)
                    _values[i] = value;
            }
        }


        /// <summary>
        /// Sets all values along the upper XZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXZ1(T value)
        {
            int offset = CountXY - CountX;

            for (int k = 0; k < CountZ; k++)
            {
                int i0 = (k + 1) * CountXY - CountX;
                int i1 = i0 + CountX;

                for (int i = i0; i < i1; i++)
                    _values[i + offset] = value;
            }
        }


        /// <summary>
        /// Sets all values along the lower YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryYZ0(T value)
        {
            for (int i = 0; i < Count; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryYZ1(T value)
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
        public void SpatialFunction(Func<Vec3d, T> func, bool normalize = false, bool parallel = false)
        {
            SpatialFunction(func, Values, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="normalize"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec3d, T> func, IDiscreteField<T> result, bool normalize = false, bool parallel = false)
        {
            SpatialFunction(func, result.Values, normalize, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="normalize"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<Vec3d, T> func, T[] result, bool normalize = false, bool parallel = false)
        {
            if (normalize)
                SpatialFunctionUVW(func, result, parallel);
            else
                SpatialFunctionXYZ(func, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionXYZ(Func<Vec3d, T> func, T[] result, bool parallel)
        {
            double x0 = Bounds.X.A;
            double y0 = Bounds.Y.A;
            double z0 = Bounds.Z.A;

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    result[index] = func(CoordinateAt(i,j,k));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionUVW(Func<Vec3d, T> func, T[] result, bool parallel)
        {
            double ti = 1.0 / (CountX - 1);
            double tj = 1.0 / (CountY - 1);
            double tk = 1.0 / (CountZ - 1);

            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    result[index] = func(new Vec3d(i * ti, j * tj, k * tk));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }

        
        /// <summary>
        /// Sets this field to some function of its coordinates in grid space.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, int, T> func, bool parallel = false)
        {
            SpatialFunction(func, Values, parallel);
        }


        /// <summary>
        /// Sets the given field to some function of this field's coordinates in grid space.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, int, T> func, IDiscreteField<T> result, bool parallel = false)
        {
            SpatialFunction(func, result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, int, T> func, T[] result, bool parallel = false)
        {
            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }

                    result[index] = func(i, j, k);
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
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Sample(GridField3d<T> other, bool parallel = false)
        {
            if (ResolutionEquals(other))
            {
                _values.Set(other._values);
                return;
            }

            this.Sample((IField3d<T>)other, parallel);
        }


        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="other"></param>
        /// <param name="converter"></param>
        /// <param name="parallel"></param>
        public void Sample<U>(GridField3d<U> other, Func<U, T> converter, bool parallel = false)
            where U : struct
        {
            if (ResolutionEquals(other))
            {
                other._values.Convert(converter, _values);
                return;
            }

            this.Sample((IField3d<U>)other, converter, parallel);
        }


        /*
        /// <summary>
        /// Sets this field to the values of another.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="parallel"></param>
        public void Sample(IField3d<T> other, bool parallel = false)
        {
            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = other.ValueAt(CoordinateAt(i, j, k));
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
        public void Sample<U>(IField3d<U> other, Func<U, T> converter, bool parallel = false)
        {
            Action<Tuple<int, int>> body = range =>
            {
                (int i, int j, int k) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    _values[index] = converter(other.ValueAt(CoordinateAt(i, j, k)));
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }
        */


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(new Vec3d(point.X, point.Y, 0.0));
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
        IDiscreteField3d<T> IDiscreteField3d<T>.Duplicate(bool copyValues)
        {
            return DuplicateBase(copyValues);
        }

        #endregion
    }
}
