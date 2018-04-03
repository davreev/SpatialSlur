
/*
 * Notes
 */

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using static SpatialSlur.SlurCore.CoreUtil;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3d
    {
        /// <summary></summary>
        public static readonly GridField3dFactory<double> Double = new GridField3dDouble.Factory();

        /// <summary></summary>
        public static readonly GridField3dFactory<Vec3d> Vec3d = new GridField3dVec3d.Factory();
    }


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
        public static implicit operator T[] (GridField3d<T> field)
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
        /// <param name="countZ"></param>
        public GridField3d(int countX, int countY, int countZ)
          : base(countX, countY, countZ)
        {
            _values = new T[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        public GridField3d(Grid3d grid)
            :base(grid)
        {
            _values = new T[Count];
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
                BoundsCheck(i, CountX);
                BoundsCheck(j, CountY);
                return _values[IndexAtUnsafe(i, j, k)];
            }
            set
            {
                BoundsCheck(i, CountX);
                BoundsCheck(j, CountY);
                _values[IndexAtUnsafe(i, j, k)] = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"GridField3d<{typeof(T).Name}> ({CountX}, {CountY}, {CountZ})";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GridField3d<T> Duplicate()
        {
            return Duplicate(true);
        }


        /// <summary>
        /// Returns a deep copy of this field.
        /// </summary>
        /// <returns></returns>
        public abstract GridField3d<T> Duplicate(bool setValues);


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
        public T ValueAtUnsafe(int i, int j, int k)
        {
            return _values[IndexAtUnsafe(i, j, k)];
        }


        /*
        /// <inheritdoc/>
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
        */


        /// <inheritdoc/>
        public T ValueAt(Vec3d point)
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
        public T ValueAtUnsafe(Vec3d point)
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
        public T ValueAtUnsafe(Vec3d point)
        {
            // conditional is used instead of switch for inline optimization
            return SampleMode == SampleMode.Nearest ?
                ValueAtNearestUnsafe(point) : ValueAtLinearUnsafe(point);
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
        private T ValueAtNearestUnsafe(Vec3d point)
        {
            return _values[IndexAtUnsafe(point)];
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
        protected abstract T ValueAtLinearUnsafe(Vec3d point);


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
            SpatialFunction(func, Values, normalize, parallel);
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

        //£
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionXYZ(Func<Vec3d, T> func, T[] result, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (int i, int j, int k) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    result[index] = func(CoordinateAt(i,j,k));
                }
            }
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        private void SpatialFunctionUVW(Func<Vec3d, T> func, T[] result, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                double ti = 1.0 / (CountX - 1);
                double tj = 1.0 / (CountY - 1);
                double tk = 1.0 / (CountZ - 1);

                (int i, int j, int k) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    result[index] = func(new Vec3d(i * ti, j * tj, k * tk));
                }
            }
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
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void SpatialFunction(Func<int, int, int, T> func, T[] result, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), range => Body(range.Item1, range.Item2));
            else
                Body(0, Count);

            void Body(int from, int to)
            {
                (int i, int j, int k) = IndicesAt(from);

                for (int index = from; index < to; index++, i++)
                {
                    if (i == CountX) { j++; i = 0; }
                    if (j == CountY) { k++; j = 0; }
                    result[index] = func(i, j, k);
                }
            }
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
                this.Set(other);
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
                other.Convert(converter, this, parallel);
                return;
            }

            this.Sample((IField3d<U>)other, converter, parallel);
        }
        

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
        IDiscreteField<T> IDiscreteField<T>.Duplicate(bool setValues)
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

        #endregion
    }
}
