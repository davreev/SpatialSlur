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
    public abstract class Field3d
    {
        private Domain3d _domain;
        private double _x0, _y0, _z0; // cached for convenience
        private double _dx, _dy, _dz;
        private double _dxInv, _dyInv, _dzInv; // cached to avoid uneccesary divs
        private readonly int _nx, _ny, _nz, _nxy, _n;

        private FieldWrapMode _wrapModeX, _wrapModeY, _wrapModeZ;
        private Func<int, int, int> _wrapX, _wrapY, _wrapZ;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        private Field3d(int countX, int countY, int countZ)
        {
            if (countX < 1 || countY < 1 || countZ < 1)
                throw new System.ArgumentException("The field must have at least 1 value in each dimension.");

            _nx = countX;
            _ny = countY;
            _nz = countZ;
            _nxy = _nx * _ny;
            _n = _nxy * _nz;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapMode"></param>
        /// <param name=""></param>
        protected Field3d(Domain3d domain, int countX, int countY, int countZ, FieldWrapMode wrapMode)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            WrapModeX = WrapModeY = WrapModeZ = wrapMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        protected Field3d(Domain3d domain, int countX, int countY, int countZ,
            FieldWrapMode wrapModeX = FieldWrapMode.Clamp,
            FieldWrapMode wrapModeY = FieldWrapMode.Clamp,
            FieldWrapMode wrapModeZ = FieldWrapMode.Clamp)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            WrapModeX = wrapModeX;
            WrapModeY = wrapModeY;
            WrapModeZ = wrapModeZ;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Field3d(Field3d other)
        {
            _nx = other._nx;
            _ny = other._ny;
            _nz = other._nz;
            _nxy = other._nxy;
            _n = other._n;

            Domain = other._domain;
            WrapModeX = other._wrapModeX;
            WrapModeY = other._wrapModeY;
            WrapModeZ = other._wrapModeZ;
        }


        /// <summary>
        /// Gets/sets the domain of the field.
        /// </summary>
        public Domain3d Domain
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
        /// Returns the number of values in the z direction.
        /// </summary>
        public int CountZ
        {
            get { return _nz; }
        }


        /// <summary>
        /// Returns the number of values in a single xy layer.
        /// </summary>
        public int CountXY
        {
            get { return _nxy; }
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
        /// Returns the distance between values in the z direction.
        /// </summary>
        public double ScaleZ
        {
            get { return _dz; }
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
        /// 
        /// </summary>
        public FieldWrapMode WrapModeZ
        {
            get { return _wrapModeZ; }
            set
            {
                _wrapModeZ = value;
                _wrapZ = FieldUtil.SelectWrapFunc(value);
            }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        protected void OnDomainChange()
        {
            _x0 = _domain.x.t0;
            _y0 = _domain.y.t0;
            _z0 = _domain.z.t0;

            _dx = _domain.x.Span / (_nx - 1);
            _dy = _domain.y.Span / (_ny - 1);
            _dz = _domain.z.Span / (_nz - 1);

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
            _dzInv = 1.0 / _dz;
        }


        /// <summary>
        /// Returns coordinates of all values in the field.
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public Vec3d[] GetCoordinates(bool parallel = false)
        {
            Vec3d[] result = new Vec3d[_n];
            GetCoordinates(result, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetCoordinates(Vec3d[] result, bool parallel = false)
        {
            Action<Tuple<int, int>> func = range =>
             {
                 (int i, int j, int k) = IndicesAt(range.Item1);

                 for (int index = range.Item1; index < range.Item2; index++, i++)
                 {
                     if (i == _nx) { j++; i = 0; }
                     if (j == _ny) { k++; j = 0; }
                     result[index] = CoordinateAt(i, j, k);
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
        /// <param name="k"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(int i, int j, int k)
        {
            return new Vec3d(
                i * _dx + _x0,
                j * _dy + _y0,
                k * _dz + _z0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(int index)
        {
            (int i, int j, int k) = IndicesAt(index);
            return CoordinateAt(i, j, k);
        }


        /// <summary>
        /// Returns true if the field has the same number of values in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(Field3d other)
        {
            return (_nx == other._nx && _ny == other._ny && _nz == other._nz);
        }


        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j, int k)
        {
            return FieldUtil.FlattenIndex(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return FieldUtil.FlattenIndex(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// If the given indices are out of range, they are handled according to the current wrap mode.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j, int k)
        {
            return FieldUtil.FlattenIndex(_wrapX(i, _nx), _wrapY(j, _ny), _wrapZ(k, _nz), _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// If the given point is outside the field domain, it is handled according to the current wrap mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return FieldUtil.FlattenIndex(_wrapX(i, _nx), _wrapY(j, _ny), _wrapZ(k, _nz), _nx, _nxy);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int, int) IndicesAt(int index)
        {
            return FieldUtil.ExpandIndex(index, _nx, _nxy);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        [Obsolete]
        public void IndicesAt(int index, out int i, out int j, out int k)
        {
            (i, j, k) = FieldUtil.ExpandIndex(index, _nx, _nxy);
        }


        /// <summary>
        /// Returns the indices of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        public (int, int, int) IndicesAt(Vec3d point)
        {
            return (
                (int)Math.Round((point.x - _x0) * _dxInv),
                (int)Math.Round((point.y - _y0) * _dyInv),
                (int)Math.Round((point.z - _z0) * _dzInv));
        }


        /// <summary>
        /// Returns the indices of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        [Obsolete]
        public void IndicesAt(Vec3d point, out int i, out int j, out int k)
        {
            (i, j, k) = IndicesAt(point);
        }


        /// <summary>
        /// Returns the indices and weights of the 8 values nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint3d FieldPointAtUnchecked(Vec3d point)
        {
            FieldPoint3d result = new FieldPoint3d();
            FieldPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAtUnchecked(Vec3d point, FieldPoint3d result)
        {
            int i, j, k;

            // set weights with fractional components
            result.SetWeights(
               SlurMath.Fract((point.x - _x0) * _dxInv, out i),
               SlurMath.Fract((point.y - _y0) * _dyInv, out j),
               SlurMath.Fract((point.z - _z0) * _dzInv, out k)
               );

            // set corner indices
            var corners = result.Corners;
            int index = FieldUtil.FlattenIndex(i, j, k, _nx, _nxy);
            corners[0] = index;
            corners[1] = index + 1;
            corners[2] = index + _nx;
            corners[3] = index + 1 + _nx;
            corners[4] = index + _nxy;
            corners[5] = index + 1 + _nxy;
            corners[6] = index + _nx + _nxy;
            corners[7] = index + 1 + _nx + _nxy;
        }


        /// <summary>
        /// Returns indices and weights of the 8 values nearest to the given point.
        /// If the given point is outside the field domain, it is handled according to the current wrap mode.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint3d FieldPointAt(Vec3d point)
        {
            FieldPoint3d result = new FieldPoint3d();
            FieldPointAt(point, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAt(Vec3d point, FieldPoint3d result)
        {
            int i0, j0, k0;

            // set weights with fractional components
            result.SetWeights(
               SlurMath.Fract((point.x - _x0) * _dxInv, out i0),
               SlurMath.Fract((point.y - _y0) * _dyInv, out j0),
               SlurMath.Fract((point.z - _z0) * _dzInv, out k0)
               );

            // wrap indices
            int i1 = _wrapX(i0 + 1, _nx);
            int j1 = _wrapY(j0 + 1, _ny);
            int k1 = _wrapZ(k0 + 1, _nz);
            i0 = _wrapX(i0, _nx);
            j0 = _wrapY(j0, _ny);
            k0 = _wrapZ(k0, _nz);

            // set corner indices
            var corners = result.Corners;
            corners[0] = FieldUtil.FlattenIndex(i0, j0, k0, _nx, _nxy);
            corners[1] = FieldUtil.FlattenIndex(i1, j0, k0, _nx, _nxy);
            corners[2] = FieldUtil.FlattenIndex(i0, j1, k0, _nx, _nxy);
            corners[3] = FieldUtil.FlattenIndex(i1, j1, k0, _nx, _nxy);
            corners[4] = FieldUtil.FlattenIndex(i0, j0, k1, _nx, _nxy);
            corners[5] = FieldUtil.FlattenIndex(i1, j0, k1, _nx, _nxy);
            corners[6] = FieldUtil.FlattenIndex(i0, j1, k1, _nx, _nxy);
            corners[7] = FieldUtil.FlattenIndex(i1, j1, k1, _nx, _nxy);
        }
    }
}
