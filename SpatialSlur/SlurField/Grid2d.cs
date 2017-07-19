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
    public abstract class Grid2d
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
        private Grid2d(int countX, int countY)
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
        protected Grid2d(Domain2d domain, int countX, int countY)
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
        protected Grid2d(Domain2d domain, int countX, int countY, WrapMode wrapMode)
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
        protected Grid2d(Domain2d domain, int countX, int countY, WrapMode wrapModeX, WrapMode wrapModeY)
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
        protected Grid2d(Grid2d other)
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
        /// Returns the coordinate of each value in the field.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec2d[] GetCoordinates(bool parallel = false)
        {
            Vec2d[] result = new Vec2d[_n];
            GetCoordinates(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCoordinates(Vec2d[] result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
            {
                (int i, int j) = IndicesAt(range.Item1);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == _nx) { j++; i = 0; }
                    result[index] = CoordinateAt(i, j);
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, _n), func);
            else
                func(Tuple.Create(0, _n));
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
        public bool ResolutionEquals(Grid2d other)
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
            switch (_wrapModeX)
            {
                case (WrapMode.Clamp):
                    return Clamp(i, _nx);
                case (WrapMode.Repeat):
                    return Repeat(i, _nx);
                case (WrapMode.MirrorRepeat):
                    return MirrorRepeat(i, _nx);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public int WrapY(int j)
        {
            switch (_wrapModeY)
            {
                case (WrapMode.Clamp):
                    return Clamp(j, _ny);
                case (WrapMode.Repeat):
                    return Repeat(j, _ny);
                case (WrapMode.MirrorRepeat):
                    return MirrorRepeat(j, _ny);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
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
                (int)Math.Round((point.Y - _y0) * _dyInv));
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
    }
}
