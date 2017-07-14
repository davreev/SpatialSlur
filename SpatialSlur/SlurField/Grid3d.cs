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
    [Serializable]
    public abstract class Grid3d
    {
        private Domain3d _domain;
        private double _x0, _y0, _z0; // cached for convenience
        private double _dx, _dy, _dz;
        private double _dxInv, _dyInv, _dzInv; // cached to avoid uneccesary divs
        private readonly int _nx, _ny, _nz, _nxy, _n;

        private SampleMode _sampleMode;
        private WrapMode _wrapModeX, _wrapModeY, _wrapModeZ;
        // Func<int, int> _wrapX, _wrapY, _wrapZ; // compare delegate performance to switch (release & debug)


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        private Grid3d(int countX, int countY, int countZ)
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
        protected Grid3d(Domain3d domain, int countX, int countY, int countZ)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            _sampleMode = SampleMode.Linear;
            WrapModeX = WrapModeY = WrapModeZ = WrapMode.Clamp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapMode"></param>
        protected Grid3d(Domain3d domain, int countX, int countY, int countZ, SampleMode sampleMode, WrapMode wrapMode)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            _sampleMode = sampleMode;
            WrapModeX = WrapModeY = WrapModeZ = wrapMode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="sampleMode"></param>
        /// <param name="wrapModeX"></param>
        /// <param name="wrapModeY"></param>
        /// <param name="wrapModeZ"></param>
        protected Grid3d(Domain3d domain, int countX, int countY, int countZ, SampleMode sampleMode, WrapMode wrapModeX, WrapMode wrapModeY, WrapMode wrapModeZ)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            _sampleMode = sampleMode;
            WrapModeX = wrapModeX;
            WrapModeY = wrapModeY;
            WrapModeZ = wrapModeZ;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected Grid3d(Grid3d other)
        {
            _nx = other._nx;
            _ny = other._ny;
            _nz = other._nz;
            _nxy = other._nxy;
            _n = other._n;

            Domain = other._domain;
            _sampleMode = other._sampleMode;
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
        public SampleMode SampleMode
        {
            get { return _sampleMode; }
            set { _sampleMode = value; }
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
        public WrapMode WrapModeZ
        {
            get { return _wrapModeZ; }
            set { _wrapModeZ = value; }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        protected void OnDomainChange()
        {
            _x0 = _domain.X.T0;
            _y0 = _domain.Y.T0;
            _z0 = _domain.Z.T0;

            _dx = _domain.X.Span / (_nx - 1);
            _dy = _domain.Y.Span / (_ny - 1);
            _dz = _domain.Z.Span / (_nz - 1);

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
        public bool ResolutionEquals(Grid3d other)
        {
            return (_nx == other._nx && _ny == other._ny && _nz == other._nz);
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
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public int WrapZ(int k)
        {
            switch (_wrapModeZ)
            {
                case (WrapMode.Clamp):
                    return Clamp(k, _nz);
                case (WrapMode.Repeat):
                    return Repeat(k, _nz);
                case (WrapMode.MirrorRepeat):
                    return MirrorRepeat(k, _nz);
            }

            throw new NotSupportedException();
        }


        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j, int k)
        {
            return FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
        }


        /// <summary>
        /// Flattens a 3 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j, int k)
        {
            return FlattenIndices(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest value to the given point.
        /// Assumes the point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return FlattenIndices(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int, int, int) IndicesAt(int index)
        {
            return ExpandIndex(index, _nx, _nxy);
        }


        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void IndicesAt(int index, out int i, out int j, out int k)
        {
            (i, j, k) = ExpandIndex(index, _nx, _nxy);
        }


        /// <summary>
        /// Returns the indices of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        public (int, int, int) IndicesAt(Vec3d point)
        {
            return (
                (int)Math.Round((point.X - _x0) * _dxInv),
                (int)Math.Round((point.Y - _y0) * _dyInv),
                (int)Math.Round((point.Z - _z0) * _dzInv));
        }


        /// <summary>
        /// Returns the indices of the nearest value to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void IndicesAt(Vec3d point, out int i, out int j, out int k)
        {
            (i, j, k) = IndicesAt(point);
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAt(Vec3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAt(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vec3d point, GridPoint3d result)
        {
            (double u, double v, double w) = Fract(point, out int i0, out int j0, out int k0);
            result.SetWeights(u, v, w);

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * _nx;
            int k1 = WrapZ(k0 + 1) * _nxy;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * _nx;
            k0 = WrapZ(k0) * _nxy;

            var corners = result.Corners;
            corners[0] = i0 + j0 + k0;
            corners[1] = i1 + j0 + k0;
            corners[2] = i0 + j1 + k0;
            corners[3] = i1 + j1 + k0;
            corners[4] = i0 + j0 + k1;
            corners[5] = i1 + j0 + k1;
            corners[6] = i0 + j1 + k1;
            corners[7] = i1 + j1 + k1;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint3d GridPointAtUnchecked(Vec3d point)
        {
            GridPoint3d result = new GridPoint3d();
            GridPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is inside the field's domain.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnchecked(Vec3d point, GridPoint3d result)
        {
            (double u, double v, double w) = Fract(point, out int i0, out int j0, out int k0);
            result.SetWeights(u, v, w);

            j0 *= _nx;
            k0 *= _nxy;
            int i1 = i0 + 1;
            int j1 = j0 + _nx;
            int k1 = k0 + _nxy;

            var corners = result.Corners;
            corners[0] = i0 + j0 + k0;
            corners[1] = i1 + j0 + k0;
            corners[2] = i0 + j1 + k0;
            corners[3] = i1 + j1 + k0;
            corners[4] = i0 + j0 + k1;
            corners[5] = i1 + j0 + k1;
            corners[6] = i0 + j1 + k1;
            corners[7] = i1 + j1 + k1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected (double, double, double) Fract(Vec3d point, out int i, out int j, out int k)
        {
            return (
            SlurMath.Fract((point.X - _x0) * _dxInv, out i),
            SlurMath.Fract((point.Y - _y0) * _dyInv, out j),
            SlurMath.Fract((point.Z - _z0) * _dzInv, out k)
            );
        }
    }
}
