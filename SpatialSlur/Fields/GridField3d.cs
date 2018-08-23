
/*
 * Notes
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using SpatialSlur.Collections;
using SpatialSlur.Fields;

using static SpatialSlur.Utilities;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class GridField3d
    {
        /// <summary></summary>
        public static readonly GridField3dFactory<double> Double = new GridField3dDouble.Factory();

        /// <summary></summary>
        public static readonly GridField3dFactory<Vector3d> Vector3d = new GridField3dVector3d.Factory();

        /// <summary></summary>
        public static readonly GridField3dFactory<Matrix3d> Matrix3d = new GridField3dMatrix3d.Factory();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class GridField3d<T> : Grid3d, ISampledField3d<T>
        where T : struct
    {
        #region Static Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator T[] (GridField3d<T> field)
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
        /// <param name="countZ"></param>
        public GridField3d(int countX, int countY, int countZ)
          : base(countX, countY, countZ)
        {
            _values = new T[CountXYZ];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        public GridField3d(Grid3d grid)
            :base(grid)
        {
            _values = new T[CountXYZ];
        }


        /// <inheritdoc />
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
        /// <param name="point"></param>
        /// <returns></returns>
        public T this[Vector3i point]
        {
            get
            {
                BoundsCheck(point.X, CountX);
                BoundsCheck(point.Y, CountY);
                return _values[ToIndexUnsafe(point)];
            }
            set
            {
                BoundsCheck(point.X, CountX);
                BoundsCheck(point.Y, CountY);
                _values[ToIndexUnsafe(point)] = value;
            }
        }


        /// <inheritdoc />
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
        /// Returns the value at the given point in grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vector3i point)
        {
            return _values[ToIndex(point)];
        }


        /// <summary>
        /// Returns the value at the given point in grid space.
        /// Assumes the given point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAtUnsafe(Vector3i point)
        {
            return _values[ToIndexUnsafe(point)];
        }

            
        /// <summary>
        /// Returns the value at the given point in world space based on the current sample mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public T ValueAt(Vector3d point)
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
        public T ValueAtUnsafe(Vector3d point)
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
        /// <returns></returns>
        private T ValueAtNearest(Vector3d point)
        {
            point = Vector3d.Round(ToGridSpace(point));
            return _values[ToIndex(point.As3i)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private T ValueAtNearestUnsafe(Vector3d point)
        {
            point = Vector3d.Round(ToGridSpace(point));
            return _values[ToIndexUnsafe(point.As3i)];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinear(Vector3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected abstract T ValueAtLinearUnsafe(Vector3d point);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract T ValueAt(ref GridPoint3d point);


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAt(Vector3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAt(point, ref result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vector3d point, ref GridPoint3d result)
        {
            result.SetWeights(Vector3d.Fract(ToGridSpace(point), out Vector3i whole));
            GridPointAt(whole, ref result);
        }


        /// <summary>
        /// Returns a grid point at the given point in grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vector3i point, ref GridPoint3d result)
        {
            var x0 = WrapX(point.X);
            var y0 = WrapY(point.Y) * CountX;
            var z0 = WrapZ(point.Z) * CountXY;

            int x1 = WrapX(point.X + 1);
            int y1 = WrapY(point.Y + 1) * CountX;
            var z1 = WrapZ(point.Z + 1) * CountXY;

            result.Index0 = x0 + y0 + z0;
            result.Index1 = x1 + y0 + z0;
            result.Index2 = x0 + y1 + z0;
            result.Index3 = x1 + y1 + z0;
            result.Index4 = x0 + y0 + z1;
            result.Index5 = x1 + y0 + z1;
            result.Index6 = x0 + y1 + z1;
            result.Index7 = x1 + y1 + z1;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAtUnsafe(Vector3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAtUnsafe(point, ref result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point in world space.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnsafe(Vector3d point, ref GridPoint3d result)
        {
            result.SetWeights(Vector3d.Fract(ToGridSpace(point), out Vector3i whole));
            GridPointAtUnsafe(whole, ref result);
        }


        /// <summary>
        /// Returns a grid point at the given point in grid space.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnsafe(Vector3i point, ref GridPoint3d result)
        {
            var x0 = point.X;
            var y0 = point.Y * CountX;
            var z0 = point.Z * CountXY;

            int x1 = x0 + 1;
            int y1 = y0 + CountX;
            var z1 = z0 + CountXY;

            result.Index0 = x0 + y0 + z0;
            result.Index1 = x1 + y0 + z0;
            result.Index2 = x0 + y1 + z0;
            result.Index3 = x1 + y1 + z0;
            result.Index4 = x0 + y0 + z1;
            result.Index5 = x1 + y0 + z1;
            result.Index6 = x0 + y1 + z1;
            result.Index7 = x1 + y1 + z1;
        }


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
            int offset = CountXYZ - CountXY;

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

            for (int z = 0; z < CountZ; z++)
            {
                int i0 = z * CountXY;
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

            for (int i = 0; i < CountXYZ; i += CountX)
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
            for (int i = CountXYZ - CountXY; i < CountXYZ; i++)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the lower YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryXZ0(T value)
        {
            for (int z = 0; z < CountZ; z++)
            {
                int i0 = z * CountXY;
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

            for (int z = 0; z < CountZ; z++)
            {
                int i0 = (z + 1) * CountXY - CountX;
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
            for (int i = 0; i < CountXYZ; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets all values along the upper YZ boundary to a given constant.
        /// </summary>
        /// <param name="value"></param>
        public void SetBoundaryYZ1(T value)
        {
            for (int i = CountX - 1; i < CountXYZ; i += CountX)
                _values[i] = value;
        }


        /// <summary>
        /// Sets this field to some function of its world space coordinates.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="parallel"></param>
        public void WorldSpaceFunction(Func<Vector3d, T> func, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXYZ);

            void Body(int from, int to)
            {
                var vals = _values;
                var nx = CountX;
                var ny = CountY;
                var p = ToGridSpace(from);

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    if (p.Y == ny)
                    {
                        p.Y = 0;
                        p.Z++;
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
        public void GridSpaceFunction(Func<Vector3i, T> func, bool parallel = false)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXYZ);

            void Body(int from, int to)
            {
                var vals = _values;
                var nx = CountX;
                var ny = CountY;
                var p = ToGridSpace(from);

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    if (p.Y == ny)
                    {
                        p.Y = 0;
                        p.Z++;
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
        public void UnitSpaceFunction(Func<Vector3d, T> func, bool parallel)
        {
            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, CountXYZ), range => Body(range.Item1, range.Item2));
            else
                Body(0, CountXYZ);

            void Body(int from, int to)
            {
                var vals = _values;
                var nx = CountX;
                var ny = CountY;
                var p = ToGridSpace(from);

                var t = new Vector3d(
                    1.0 / (CountX - 1),
                    1.0 / (CountY - 1),
                    1.0 / (CountZ - 1));

                for (int index = from; index < to; index++, p.X++)
                {
                    if (p.X == nx)
                    {
                        p.X = 0;
                        p.Y++;
                    }

                    if (p.Y == ny)
                    {
                        p.Y = 0;
                        p.Z++;
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
        public void Sample(GridField3d<T> other, bool parallel = false)
        {
            if (Count == other.Count)
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
            if (Count == other.Count)
            {
                other.Convert(converter, this, parallel);
                return;
            }

            this.Sample((IField3d<U>)other, converter, parallel);
        }


        #region Explicit Interface Implementations
        
        ArrayView<T> ISampledField<T>.Values
        {
            get { return _values; }
        }
        

        int ISampledField<T>.Count
        {
            get { return CountXYZ; }
        }


        ISampledField<T> ISampledField<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        ISampledField3d<T> ISampledField3d<T>.Duplicate(bool setValues)
        {
            return Duplicate(setValues);
        }


        IEnumerable<Vector3d> ISampledField3d<T>.Points
        {
            get
            {
                (var nx, var ny, var nz) = Count;

                for (int z = 0; z < nz; z++)
                {
                    for (int y = 0; y < ny; y++)
                    {
                        for (int x = 0; x < nx; x++)
                            yield return ToWorldSpace(new Vector3d(x, y, z));
                    }
                }
            }
        }


        Vector3d ISampledField3d<T>.PointAt(int index)
        {
            return ToWorldSpace(index);
        }

#endregion
    }
}
