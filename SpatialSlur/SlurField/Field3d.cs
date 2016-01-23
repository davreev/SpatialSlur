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
    public abstract class Field3d
    {
        private Domain3d _domain;
        private Vec3d _from; // cached for convenience

        private FieldBoundaryType _boundaryType;
        private double _dx, _dy, _dz;
        private double _dxInv, _dyInv, _dzInv; // cached to avoid uneccesary divs
        private readonly int _nx, _ny, _nz, _nxy, _n;

        // delegates for methods which depend on the field's boundary type
        private Func<Vec3d, int> _indexAt;
        private Func<Vec3d, Vec3i> _index3At;
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
        /// Gets/sets the boundary type for the field.
        /// This property determines how edge cases are handled in many other methods.
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
        /// Iterates through the positions of values in the field. 
        /// Note that these are not explicitly stored in memory.
        /// </summary>
        public IEnumerable<Vec3d> Points
        {
            get
            {
                Vec3d p = _from;

                for (int i = 0; i < _nz; i++)
                {
                    for (int j = 0; j < _ny; j++)
                    {
                        for (int k = 0; k < _nx; k++)
                        {
                            yield return p;
                            p.x += _dx;
                        }
                        p.x = _from.x; // reset x
                        p.y += _dy;
                    }
                    p.y = _from.y; // reset y
                    p.z += _dz;
                }
            }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        protected void OnDomainChange()
        {
            _from = _domain.From;

            _dx = _domain.x.Span / (_nx - 1);
            _dy = _domain.y.Span / (_ny - 1);
            _dz = _domain.z.Span / (_nz - 1);

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
            _dzInv = 1.0 / _dz;
        }


        /// <summary>
        /// This is called after any changes to the field's boundary type
        /// </summary>
        protected virtual void OnBoundaryTypeChange() 
        {
            switch (_boundaryType)
            {
                case FieldBoundaryType.Constant:
                    _indexAt = IndexAtClamped;
                    _index3At = Index3AtClamped;
                    _fieldPointAt = FieldPointAtClamped;
                    break;
                case FieldBoundaryType.Equal:
                    _indexAt = IndexAtClamped;
                    _index3At = Index3AtClamped;
                    _fieldPointAt = FieldPointAtClamped;
                    break;
                case FieldBoundaryType.Periodic:
                    _indexAt = IndexAtWrapped;
                    _index3At = Index3AtWrapped;
                    _fieldPointAt = FieldPointAtWrapped;
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vec3d[] GetPointArray()
        {
            Vec3d[] result = new Vec3d[_n];
            UpdatePointArray(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void UpdatePointArray(IList<Vec3d> points)
        {
            SizeCheck(points);

            Parallel.ForEach(Partitioner.Create(0, _n), range =>
            {
                int i, j, k;
                ExpandIndex(range.Item1, out i, out j, out k);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == _nx) { j++; i = 0; }
                    if (j == _ny) { k++; j = 0; }
                    points[index] = new Vec3d(i * _dx + _from.x, j * _dy + _from.y, k * _dz + _from.z);
                }
            });
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void UpdatePointArray(Vec3d[] points)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Field3d Duplicate();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected void SizeCheck(Field3d other)
        {
            if(!ResolutionEquals(other))
                throw new ArgumentException("The two fields must have the same resolution.");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        protected void SizeCheck<T>(IList<T> list)
        {
            if (list.Count != _n)
                throw new ArgumentException("The size of the given list must match the number of values in the field.");
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
        /// Converts a 3 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int FlattenIndex(Vec3i index3)
        {
            return index3.x + index3.y * _nx + index3.z * _nxy;
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
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public Vec3i ExpandIndex(int index)
        {
            int k = index / _nxy;
            int i = index - k * _nxy; // store remainder in i
            int j = i / _nx;
            return new Vec3i(i - j * _nx, j, k);
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
            k = index / _nxy;
            i = index - k * _nxy; // store remainder in i
            j = i / _nx;
            i -= j * _nx;
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            return _indexAt(point);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            int i = (int)Math.Round((point.x - _from.x) * _dxInv);
            int j = (int)Math.Round((point.y - _from.y) * _dyInv);
            int k = (int)Math.Round((point.z - _from.z) * _dzInv);
            return FlattenIndex(i, j, k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtClamped(Vec3d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _from.x) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _from.y) * _dyInv), _ny - 1);
            int k = SlurMath.Clamp((int)Math.Round((point.z - _from.z) * _dzInv), _nz - 1);
            return FlattenIndex(i, j, k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtWrapped(Vec3d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _from.x) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _from.y) * _dyInv), _ny);
            int k = SlurMath.Mod2((int)Math.Round((point.z - _from.z) * _dzInv), _nz);
            return FlattenIndex(i, j, k);
        }


        /// <summary>
        /// Returns the 3 dimensional index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3i Index3At(Vec3d point)
        {
            return _index3At(point);
        }


        /// <summary>
        /// Returns the 3 dimensional index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3i Index3AtUnchecked(Vec3d point)
        {
            int i = (int)Math.Round((point.x - _from.x) * _dxInv);
            int j = (int)Math.Round((point.y - _from.y) * _dyInv);
            int k = (int)Math.Round((point.z - _from.z) * _dzInv);
            return new Vec3i(i, j, k);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec3i Index3AtClamped(Vec3d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _from.x) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _from.y) * _dyInv), _ny - 1);
            int k = SlurMath.Clamp((int)Math.Round((point.z - _from.z) * _dzInv), _nz - 1);
            return new Vec3i(i, j, k);
        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec3i Index3AtWrapped(Vec3d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _from.x) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _from.y) * _dyInv), _ny);
            int k = SlurMath.Mod2((int)Math.Round((point.z - _from.z) * _dzInv), _nz);
            return new Vec3i(i, j, k);
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
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAt(Vec3d point, FieldPoint3d result)
        {
            _fieldPointAt(point, result);
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
            // convert to grid space and separate fractional and whole components
            int i, j, k;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);
            double w = SlurMath.Fract((point.z - _from.z) * _dzInv, out k);

            // set corner indices
            int index = FlattenIndex(i, j, k);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + 1;
            corners[2] = index + 1 + _nx;
            corners[3] = index + _nx;
            corners[4] = index + _nxy;
            corners[5] = index + 1 + _nxy;
            corners[6] = index + 1 + _nx + _nxy;
            corners[7] = index + _nx + _nxy;

            // compute weights using fractional components
            result.SetWeights(u, v, w);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAtClamped(Vec3d point, FieldPoint3d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j, k;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);
            double w = SlurMath.Fract((point.z - _from.z) * _dzInv, out k);

            // offsets
            int di = 1;
            int dj = _nx;
            int dk = _nxy;

            // clamp whole components and adjust offsets if necessary
            if (i < 0) { i = 0; di = 0; }
            else if (i > _nx - 2) { i = _nx - 1; di = 0; }

            if (j < 0) { j = 0; dj = 0; }
            else if (j > _ny - 2) { j = _ny - 1; dj = 0; }

            if (k < 0) { k = 0; dk = 0; }
            else if (k > _nz - 2) { k = _nz - 1; dk = 0; }

            // set corner indices
            int index = FlattenIndex(i, j, k);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + di + dj;
            corners[3] = index + dj;
            corners[4] = index + dk;
            corners[5] = index + di + dk;
            corners[6] = index + di + dj + dk;
            corners[7] = index + dj + dk;

            // compute weights using fractional components
            result.SetWeights(u, v, w);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtWrapped(Vec3d point, FieldPoint3d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j, k;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);
            double w = SlurMath.Fract((point.z - _from.z) * _dzInv, out k);

            // wrap whole components
            i = SlurMath.Mod2(i, _nx);
            j = SlurMath.Mod2(j, _ny);
            k = SlurMath.Mod2(k, _nz);

            // get offsets
            int di = (i == _nx - 1) ? 1 - _nx : 1;
            int dj = (j == _ny - 1) ? _nx - _nxy : _nx;
            int dk = (k == _nz - 1) ? _nxy - _n : _nxy; 
       
            // set corner indices
            int index = FlattenIndex(i, j, k);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + di + dj;
            corners[3] = index + dj;
            corners[4] = index + dk;
            corners[5] = index + di + dk;
            corners[6] = index + di + dj + dk;
            corners[7] = index + dj + dk;

            // compute weights using fractional components
            result.SetWeights(u, v, w);
        }


        /*
        /// <summary>
        /// Generalizes well for more involved types of interpolation (cubic etc.)
        /// http://paulbourke.net/miscellaneous/interpolation/
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtClamped2(Vec3d point, FieldPoint3d result)
        {
            // convert to grid space and separate fractional and whole components
            int i0, j0, k0;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i0);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j0);
            double w = SlurMath.Fract((point.z - _from.z) * _dzInv, out k0);

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


        /// <summary>
        /// Generalizes well for more involved types of interpolation (cubic etc.)
        /// http://paulbourke.net/miscellaneous/interpolation/
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtWrapped2(Vec3d point, FieldPoint3d result)
        {
            // convert to grid space and separate fractional and whole components
            int i0, j0, k0;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i0);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j0);
            double w = SlurMath.Fract((point.z - _from.z) * _dzInv, out k0);

            int[] corners = result.Corners;
            int index = 0;

            for (int k = 0; k < 2; k++)
            {
                int kk = SlurMath.Mod2(k0 + k, _nz);
                for (int j = 0; j < 2; j++)
                {
                    int jj = SlurMath.Mod2(j0 + j, _ny);
                    for (int i = 0; i < 2; i++)
                    {
                        int ii = SlurMath.Mod2(i0 + i, _nx);
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
