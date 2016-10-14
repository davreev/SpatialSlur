using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SpatialSlur.SlurCore;


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

        private FieldBoundaryType _boundaryType;
        private double _dx, _dy, _dz;
        private readonly int _nx, _ny, _nz, _nxy, _n;

        // inverse values cached to avoid uneccesary divs
        private double _dxInv, _dyInv, _dzInv;
        private readonly double _nxInv, _nxyInv;

        // delegates for methods which depend on the field's boundary type
        private delegate void ToIndex3(Vec3d point, out int i, out int j, out int k);
        private ToIndex3 _index3At;
        private Action<Vec3d, FieldPoint3d> _fieldPointAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        private Field3d(int countX, int countY, int countZ)
        {
            if (countX < 2 || countY < 2 || countZ < 2)
                throw new System.ArgumentException("The field must have 2 or more values in each dimension.");

            _nx = countX;
            _ny = countY;
            _nz = countZ;
            _nxy = _nx * _ny;
            _n = _nxy * _nz;

            _nxInv = 1.0 / _nx;
            _nxyInv = 1.0 / _nxy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        /// <param name="boundaryType"></param>
        protected Field3d(Domain3d domain, int countX, int countY, int countZ, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : this(countX, countY, countZ)
        {
            Domain = domain;
            BoundaryType = boundaryType;
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

            _nxInv = other._nxInv;
            _nxyInv = other._nxyInv;

            Domain = other._domain;
            BoundaryType = other._boundaryType;
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
        /// Gets or sets the boundary type for the field.
        /// This property determines how edge cases are handled in other methods.
        /// </summary>
        public FieldBoundaryType BoundaryType
        {
            get { return _boundaryType; }
            set 
            { 
                _boundaryType = value;
                OnBoundaryTypeChange();
            }
        }


        /// <summary>
        /// Iterates through the coordinates of each value in the field. 
        /// Note that these are calculated on the fly and not explicitly stored in memory.
        /// If you need to cache them, call GetCoordinates instead.
        /// </summary>
        public IEnumerable<Vec3d> Coordinates
        {
            get
            {
                for (int k = 0; k < _nz; k++)
                {
                    for (int j = 0; j < _ny; j++)
                    {
                        for (int i = 0; i < _nx; i++)
                        {
                            yield return CoordinateAt(i, j, k);
                        }
                    }
                }
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
        /// This is called after any changes to the field's boundary type.
        /// </summary>
        protected virtual void OnBoundaryTypeChange() 
        {
            switch (_boundaryType)
            {
                case FieldBoundaryType.Constant:
                    {
                        _index3At = Index3AtConstant;
                        _fieldPointAt = FieldPointAtConstant;
                        break;
                    }
                case FieldBoundaryType.Equal:
                    {
                        _index3At = Index3AtEqual;
                        _fieldPointAt = FieldPointAtEqual;
                        break;
                    }
                case FieldBoundaryType.Periodic:
                    {
                        _index3At = Index3AtPeriodic;
                        _fieldPointAt = FieldPointAtPeriodic;
                        break;
                    }
            }
        }


        /// <summary>
        /// Returns coordinates of all values in the field.
        /// </summary>
        /// <returns></returns>
        public Vec3d[] GetCoordinates()
        {
            Vec3d[] result = new Vec3d[_n];
            GetCoordinates(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="coords"></param>
        public void GetCoordinates(IList<Vec3d> coords)
        {
            Parallel.ForEach(Partitioner.Create(0, _n), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == _nx) { j++; i = 0; }
                    if (j == _ny) { k++; j = 0; }
                    coords[index] = CoordinateAt(i, j, k);
                }
            });
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index3"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(Vec3i index3)
        {
            return CoordinateAt(index3.x, index3.y, index3.z);
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
        /// Returns true if the field has the same number of values in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(Field3d other)
        {
            return (_nx == other._nx && _ny == other._ny && _nz == other._nz);
        }

       
        /// <summary>
        /// Returns true if the field contains the given 3 dimensional index.
        /// </summary>
        /// <param name="index3"></param>
        /// <returns></returns>
        public bool ContainsIndex(Vec3i index3)
        {
            return ContainsIndex(index3.x, index3.y, index3.z);
        }


        /// <summary>
        /// Returns true if the field contains the given 3 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool ContainsIndex(int i, int j, int k)
        {
            return i >= 0 && j >= 0 && k >= 0 && i < _nx && j < _ny && k < _nz;
        }


        /// <summary>
        /// Converts a 3 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="index3"></param>
        /// <returns></returns>
        public int FlattenIndex(Vec3i index3)
        {
            return FlattenIndex(index3.x, index3.y, index3.z);
        }


        /// <summary>
        /// Converts a 3 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int FlattenIndex(int i, int j, int k)
        {
            return i + j * _nx + k * _nxy;
        }


        /// <summary>
        /// Converts a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec3i ExpandIndex(int index)
        {
            int i, j, k;
            ExpandIndex(index, out i, out j, out k);
            return new Vec3i(i, j, k);
        }
       

        /// <summary>
        /// Converts a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void ExpandIndex(int index, out int i, out int j, out int k)
        {
            k = (int)(index * _nxyInv);
            i = index - k * _nxy; // store remainder in i
            j = (int)(i * _nxInv);
            i -= j * _nx;
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            int i, j, k;
            _index3At(point, out i, out j, out k);
            return FlattenIndex(i, j, k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            int i, j, k;
            Index3AtUnchecked(point, out i, out j, out k);
            return FlattenIndex(i, j, k);
        }


        /// <summary>
        /// Returns the 3 dimensional index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3i Index3At(Vec3d point)
        {
            int i, j, k;
            _index3At(point, out i, out j, out k);
            return new Vec3i(i, j, k);
        }


        /// <summary>
        /// Returns the 3 dimensional index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void Index3At(Vec3d point, out int i, out int j, out int k)
        {
            _index3At(point, out i, out j, out k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3i Index3AtUnchecked(Vec3d point)
        {
            int i, j, k;
            Index3AtUnchecked(point, out i, out j, out k);
            return new Vec3i(i, j, k);
        }


        /// <summary>
        /// Returns the 3 dimensional index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void Index3AtUnchecked(Vec3d point, out int i, out int j, out int k)
        {
            i = (int)Math.Round((point.x - _x0) * _dxInv);
            j = (int)Math.Round((point.y - _y0) * _dyInv);
            k = (int)Math.Round((point.z - _z0) * _dzInv);
        }


        /// <summary>
        /// 
        /// </summary>
        private void Index3AtConstant(Vec3d point, out int i, out int j, out int k)
        {
            Index3AtUnchecked(point, out i, out j, out k);

            // set to 3d index of boundary value if out of bounds
            if (!ContainsIndex(i, j, k))
            {
                i = _nx;
                j = _ny;
                k = _nz;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Index3AtEqual(Vec3d point, out int i, out int j, out int k)
        {
            Index3AtUnchecked(point, out i, out j, out k);

            i = SlurMath.Clamp(i, _nx - 1);
            j = SlurMath.Clamp(j, _ny - 1);
            k = SlurMath.Clamp(k, _nz - 1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void Index3AtPeriodic(Vec3d point, out int i, out int j, out int k)
        {
            Index3AtUnchecked(point, out i, out j, out k);

            i = SlurMath.Mod2(i, _nx);
            j = SlurMath.Mod2(j, _ny);
            k = SlurMath.Mod2(k, _nz);
        }


        /// <summary>
        /// Returns indices and weights of the 8 values nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint3d FieldPointAt(Vec3d point)
        {
            FieldPoint3d result = new FieldPoint3d();
            _fieldPointAt(point, result);
            return result;
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
        public void FieldPointAt(Vec3d point, FieldPoint3d result)
        {
            _fieldPointAt(point, result);
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
               SlurMath.Fract((point.z - _z0) * _dzInv, out k));

            // set corner indices
            int index = FlattenIndex(i, j, k);
            var corners = result.Corners;
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
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtConstant(Vec3d point, FieldPoint3d result)
        {
            int i, j, k;

            // set weights with fractional components
            result.SetWeights(
               SlurMath.Fract((point.x - _x0) * _dxInv, out i),
               SlurMath.Fract((point.y - _y0) * _dyInv, out j),
               SlurMath.Fract((point.z - _z0) * _dzInv, out k));

            // create bit mask (1 = out of bounds, 0 = in bounds)
            int mask = 0;
            if (!SlurMath.Contains(i, 0, _nx)) mask |= 1;
            if (!SlurMath.Contains(j, 0, _ny)) mask |= 2;
            if (!SlurMath.Contains(k, 0, _nz)) mask |= 4;
            if (!SlurMath.Contains(i + 1, 0, _nx)) mask |= 8;
            if (!SlurMath.Contains(j + 1, 0, _ny)) mask |= 16;
            if (!SlurMath.Contains(k + 1, 0, _nz)) mask |= 32;

            // set corner indices
            int index = FlattenIndex(i, j, k);
            var corners = result.Corners;
            corners[0] = ((mask & 7) == 0) ? index : _n; // 000 111
            corners[1] = ((mask & 14) == 0) ? index + 1 : _n; // 001 110
            corners[2] = ((mask & 21) == 0) ? index + _nx : _n; // 010 101
            corners[3] = ((mask & 28) == 0) ? index + 1 + _nx : _n; // 011 100
            corners[4] = ((mask & 35) == 0) ? index + _nxy : _n; // 100 011
            corners[5] = ((mask & 42) == 0) ? index + 1 + _nxy : _n; // 101 010
            corners[6] = ((mask & 49) == 0) ? index + _nx + _nxy : _n; // 110 001
            corners[7] = ((mask & 56) == 0) ? index + 1 + _nx + _nxy : _n; // 111 000
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtEqual(Vec3d point, FieldPoint3d result)
        {
            int i, j, k;

            // set weights with fractional components
            result.SetWeights(
               SlurMath.Fract((point.x - _x0) * _dxInv, out i),
               SlurMath.Fract((point.y - _y0) * _dyInv, out j),
               SlurMath.Fract((point.z - _z0) * _dzInv, out k));

            // clamp and get offsets
            int di = 0, dj = 0, dk = 0;

            if (i < 0) i = 0;
            else if (i > _nx - 1) i = _nx - 1;
            else if (i < _nx - 1) di = 1;

            if (j < 0) j = 0;
            else if (j > _ny - 1) j = _ny - 1;
            else if (j < _ny - 1) dj = _nx;

            if (k < 0) k = 0;
            else if (k > _nz - 1) k = _nz - 1;
            else if (k < _nz - 1) dk = _nxy;

            // set corner indices
            int index = FlattenIndex(i, j, k);
            var corners = result.Corners;
            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + dj;
            corners[3] = index + di + dj;
            corners[4] = index + dk;
            corners[5] = index + di + dk;
            corners[6] = index + dj + dk;
            corners[7] = index + di + dj + dk;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtPeriodic(Vec3d point, FieldPoint3d result)
        {
            int i, j, k;
      
            // set weights with fractional components
            result.SetWeights(
               SlurMath.Fract((point.x - _x0) * _dxInv, out i),
               SlurMath.Fract((point.y - _y0) * _dyInv, out j),
               SlurMath.Fract((point.z - _z0) * _dzInv, out k));

            // wrap and get offsets
            i = SlurMath.Mod2(i, _nx);
            j = SlurMath.Mod2(j, _ny);
            k = SlurMath.Mod2(k, _nz);

            int di = (i == _nx - 1) ? 1 - _nx : 1;
            int dj = (j == _ny - 1) ? _nx - _nxy : _nx;
            int dk = (k == _nz - 1) ? _nxy - _n : _nxy;

            // set corner indices
            int index = FlattenIndex(i, j, k);
            var corners = result.Corners;
            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + dj;
            corners[3] = index + di + dj;
            corners[4] = index + dk;
            corners[5] = index + di + dk;
            corners[6] = index + dj + dk;
            corners[7] = index + di + dj + dk;
        }


        /*
        // Alternative implementation better suited to more involved types of interpolation (cubic etc.)
        // http://paulbourke.net/miscellaneous/interpolation/
        private void FieldPointAtEqual2(Vec3d point, FieldPoint3d result)
        {
            // convert to grid space and separate fractional and whole components
            int i0, j0, k0;
            double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i0);
            double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j0);
            double w = SlurMath.Fract((point.z - _z0) * _dzInv, out k0);

            int[] corners = result.Corners;
            int index = 0;

            for (int k = 0; k < 2; k++)
            {
                int kk = SlurMath.Clamp(k0 + k, _nz - 1);
                for (int j = 0; j < 2; j++)
                {
                    int jj = SlurMath.Clamp(j0 + j, _ny - 1);
                    for (int i = 0; i < 2; i++)
                    {
                        int ii = SlurMath.Clamp(i0 + i, _nx - 1);
                        corners[index] = FlattenIndex(ii, jj, kk);
                        index++;
                    }
                }
            }

            // compute weights using fractional components
            result.SetWeights(u, v, w);
        }
        */

    }
}
