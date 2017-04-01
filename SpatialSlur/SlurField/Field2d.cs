using System;
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
    [Serializable]
    public abstract class Field2d
    {
        private Domain2d _domain;
        private double _x0, _y0; // cached for convenience
        private double _dx, _dy;
        private double _dxInv, _dyInv; // cached to avoid unecessary divs
        private readonly int _nx, _ny, _n;

        private FieldWrapMode _wrapModeX, _wrapModeY;
        private Func<int, int, int> _wrapX, _wrapY;
  

        /// <summary>
        /// 
        /// </summary>
        private Field2d(int countX, int countY)
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
        /// <param name="wrapMode"></param>
        protected Field2d(Domain2d domain, int countX, int countY, FieldWrapMode wrapMode)
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
        protected Field2d(Domain2d domain, int countX, int countY, FieldWrapMode wrapModeX = FieldWrapMode.Clamp, FieldWrapMode wrapModeY = FieldWrapMode.Clamp)
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
        protected Field2d(Field2d other)
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
        public FieldWrapMode WrapModeX
        {
            get { return _wrapModeX; }
            set
            {
                _wrapModeX = value;
                _wrapX = FieldUtil.SelectWrapFunc(value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public FieldWrapMode WrapModeY
        {
            get { return _wrapModeY; }
            set
            {
                _wrapModeY = value;
                _wrapY = FieldUtil.SelectWrapFunc(value);
            }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        private void OnDomainChange()
        {
            _x0 = _domain.x.t0;
            _y0 = _domain.y.t0;

            _dx = _domain.x.Span / (_nx - 1);
            _dy = _domain.y.Span / (_ny - 1);

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
        }


        /// <summary>
        /// Returns coordinates of all values in the field.
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
        public bool ResolutionEquals(Field2d other)
        {
            return (_nx == other._nx && _ny == other._ny);
        }


        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j)
        {
            return FieldUtil.FlattenIndex(i, j, _nx);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return FieldUtil.FlattenIndex(i, j, _nx);
        }


        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// If the given indices are out of range, they are handled according to the current wrap mode.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j)
        {
            return FieldUtil.FlattenIndex(_wrapX(i, _nx), _wrapY(j, _ny), _nx);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// If the given point is outside the field domain, it is handled according to the current wrap mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return FieldUtil.FlattenIndex(_wrapX(i, _nx), _wrapY(j, _ny), _nx);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int) IndicesAt(int index)
        {
            return FieldUtil.ExpandIndex(index, _nx);
        }


        /// <summary>
        /// Expands 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        [Obsolete]
        public void IndicesAt(int index, out int i, out int j)
        {
            (i, j) = FieldUtil.ExpandIndex(index, _nx);
        }


        /// <summary>
        /// Returns the indices of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        public (int, int) IndicesAt(Vec2d point)
        {
            return (
                (int)Math.Round((point.x - _x0) * _dxInv),
                (int)Math.Round((point.y - _y0) * _dyInv));
        }


        /// <summary>
        /// Returns the indices of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void IndicesAt(Vec2d point, out int i, out int j)
        {
            (i, j) = IndicesAt(point);
        }


        /// <summary>
        /// Returns indices and weights of the 4 values nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint2d FieldPointAtUnchecked(Vec2d point)
        {
            FieldPoint2d result = new FieldPoint2d();
            FieldPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAtUnchecked(Vec2d point, FieldPoint2d result)
        {
            int i, j;

            // set weights with fractional components
            result.SetWeights(
             SlurMath.Fract((point.x - _x0) * _dxInv, out i),
             SlurMath.Fract((point.y - _y0) * _dyInv, out j)
             );

            // set corners
            var corners = result.Corners;
            int index = FieldUtil.FlattenIndex(i, j, _nx);
            corners[0] = index;
            corners[1] = index + 1;
            corners[2] = index + _nx;
            corners[3] = index + 1 + _nx;
        }


        /// <summary>
        /// Returns indices and weights of the 4 values nearest to the given point.
        /// If the given point is outside the field domain, it is handled according to the current wrap mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint2d FieldPointAt(Vec2d point)
        {
            FieldPoint2d result = new FieldPoint2d();
            FieldPointAt(point, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAt(Vec2d point, FieldPoint2d result)
        {
            int i0, j0;

            // set weights with fractional components
            result.SetWeights(
             SlurMath.Fract((point.x - _x0) * _dxInv, out i0),
             SlurMath.Fract((point.y - _y0) * _dyInv, out j0)
             );

            // wrap indices
            int i1 = _wrapX(i0 + 1, _nx);
            int j1 = _wrapY(j0 + 1, _ny);

            i0 = _wrapX(i0, _nx);
            j0 = _wrapY(j0, _ny);

            // set corners
            var corners = result.Corners;
            corners[0] = FieldUtil.FlattenIndex(i0, j0, _nx);
            corners[1] = FieldUtil.FlattenIndex(i1, j0, _nx);
            corners[2] = FieldUtil.FlattenIndex(i0, j1, _nx);
            corners[3] = FieldUtil.FlattenIndex(i1, j1, _nx);
        }
    }
}
