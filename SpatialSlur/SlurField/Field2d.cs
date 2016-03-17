 using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

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

        private FieldBoundaryType _boundaryType;
        private double _dx, _dy;
        private double _dxInv, _dyInv; // cached to avoid uneccesary divs
        private readonly int _nx, _ny, _n;

        // delegates for methods which depend on the field's boundary type
        private Func<Vec2d, int> _indexAt;
        private Func<Vec2d, Vec2i> _index2At;
        private Action<Vec2d, FieldPoint2d> _fieldPointAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        private Field2d(int countX, int countY)
        {
            if (countX < 2 || countY < 2)
                throw new System.ArgumentException("The field must have 2 or more values in each dimension.");

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
        /// <param name="boundaryType"></param>
        protected Field2d(Domain2d domain, int countX, int countY, FieldBoundaryType boundaryType = FieldBoundaryType.Equal)
            : this(countX, countY)
        {
            Domain = domain;
            BoundaryType = boundaryType;
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
            BoundaryType = other._boundaryType;
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
        /// Iterates through the coordinates of each value in the field. 
        /// Note that these are calculated on the fly and not explicitly stored in memory.
        /// If you need to cache them, call GetCoordinates or UpdateCoordinates instead.
        /// </summary>
        public IEnumerable<Vec2d> Coordinates
        {
            get
            {
                Vec2d p = new Vec2d(_x0, _y0);

                for (int i = 0; i < _ny; i++)
                {
                    for (int j = 0; j < _nx; j++)
                    {
                        yield return p;
                        p.x += _dx;
                    }
                    p.x = _x0; // reset x
                    p.y += _dy;
                }
            }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        protected virtual void OnDomainChange()
        {
            _x0 = _domain.x.t0;
            _y0 = _domain.y.t0;

            _dx = _domain.x.Span / (_nx - 1);
            _dy = _domain.y.Span / (_ny - 1);

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
        }


        /// <summary>
        /// This is called after any changes to the field's boundary type.
        /// </summary>
        protected virtual void OnBoundaryTypeChange()
        {
            switch(_boundaryType)
            {
                case FieldBoundaryType.Constant:
                    _indexAt = IndexAtConstant;
                    _index2At = Index2AtConstant;
                    _fieldPointAt = FieldPointAtConstant;
                    break;
                case FieldBoundaryType.Equal:
                    _indexAt = IndexAtEqual;
                    _index2At = Index2AtEqual;
                    _fieldPointAt = FieldPointAtEqual;
                    break;
                case FieldBoundaryType.Periodic:
                    _indexAt = IndexAtPeriodic;
                    _index2At = Index2AtPeriodic;
                    _fieldPointAt = FieldPointAtPeriodic;
                    break;
            }
        }


        /// <summary>
        /// Returns coordinates of all values in the field.
        /// </summary>
        /// <returns></returns>
        public Vec2d[] GetCoordinates()
        {
            Vec2d[] result = new Vec2d[_n];
            UpdateCoordinates(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void UpdateCoordinates(IList<Vec2d> points)
        {
            SizeCheck(points);

            Parallel.ForEach(Partitioner.Create(0, _n), range =>
            {
                int i, j;
                ExpandIndex(range.Item1, out i, out j);

                for (int index = range.Item1; index < range.Item2; index++, i++)
                {
                    if (i == _nx) { j++; i = 0; }
                    points[index] = new Vec2d(i * _dx + _x0, j * _dy + _y0);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        protected void SizeCheck(Field2d other)
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
        public bool ResolutionEquals(Field2d other)
        {
            return (_nx == other._nx && _ny == other._ny);
        }


        /// <summary>
        /// Converts a 2 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="index2"></param>
        /// <returns></returns>
        public int FlattenIndex(Vec2i index2)
        {
            return index2.x + index2.y * _nx;
        }


        /// <summary>
        /// Converts a 2 dimensional index into a 1 dimensional index.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int FlattenIndex(int i, int j)
        {
            return i + j * _nx;
        }


        /// <summary>
        /// Converts a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec2i ExpandIndex(int index)
        {
            int j = index / _nx;
            int i = index - j * _nx;
            return new Vec2i(i, j);
        }


        /// <summary>
        /// Converts a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public void ExpandIndex(int index, out int i, out int j)
        {
            j = index / _nx;
            i = index - j * _nx;
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec2d point)
        { 
            return _indexAt(point);
        }


        /// <summary>
        /// Returns the index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec2d point)
        {
            int i = (int)Math.Round((point.x - _x0) * _dxInv);
            int j = (int)Math.Round((point.y - _y0) * _dyInv);
            return FlattenIndex(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtConstant(Vec2d point)
        {
            int i = (int)Math.Round((point.x - _x0) * _dxInv);
            int j = (int)Math.Round((point.y - _y0) * _dyInv);
      
            // return index of boundary value if out of bounds
            if (i < 0 || j < 0 || i >= _nx || j >= _ny)
                return _n;

            return FlattenIndex(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtEqual(Vec2d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _x0) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _y0) * _dyInv), _ny - 1);
            return FlattenIndex(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtPeriodic(Vec2d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _x0) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _y0) * _dyInv), _ny);
            return FlattenIndex(i, j);
        }


        /// <summary>
        /// Returns the 2 dimensional index of the value nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2i Index2At(Vec2d point)
        {
            return _index2At(point);
        }


        /// <summary>
        /// Returns the 2 dimensional index of the value nearest to the given point.
        /// Assumes the given point is inside the field domain.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2i Index2AtUnchecked(Vec2d point)
        {
            int i = (int)Math.Round((point.x - _x0) * _dxInv);
            int j = (int)Math.Round((point.y - _y0) * _dyInv);
            return new Vec2i(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec2i Index2AtConstant(Vec2d point)
        {
            int i = (int)Math.Round((point.x - _x0) * _dxInv);
            int j = (int)Math.Round((point.y - _y0) * _dyInv);
  
            // return 3d index of boundary value if out of bounds
            if (i < 0 || j < 0 || i >= _nx || j >= _ny)
                return new Vec2i(_nx, _ny);

            return new Vec2i(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec2i Index2AtEqual(Vec2d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _x0) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _y0) * _dyInv), _ny - 1);
            return new Vec2i(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec2i Index2AtPeriodic(Vec2d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _x0) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _y0) * _dyInv), _ny);
            return new Vec2i(i, j);
        }


        /// <summary>
        /// Returns indices and weights of the 4 values nearest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FieldPoint2d FieldPointAt(Vec2d point)
        {
            FieldPoint2d result = new FieldPoint2d();
            _fieldPointAt(point, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void FieldPointAt(Vec2d point, FieldPoint2d result)
        {
            _fieldPointAt(point, result);
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
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j);

            // set corner indices
            int index = FlattenIndex(i, j);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + 1;
            corners[2] = index + 1 + _nx;
            corners[3] = index + _nx;

            // compute weights using fractional components
            result.SetWeights(u, v);
        }


        /// <summary>
        /// TODO test
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtConstant(Vec2d point, FieldPoint2d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j);

            // bit mask (1 = out of bounds, 0 = in bounds)
            int mask = 0;
            if (i < 0 || i >= _nx) mask |= 1;
            if (j < 0 || j >= _ny) mask |= 2;
            if (i < -1 || i >= _nx - 1) mask |= 4;
            if (j < -1 || j >= _ny - 1) mask |= 8;
      
            // set corner indices
            int index = FlattenIndex(i, j);
            int[] corners = result.Corners;

            corners[0] = ((mask & 3) > 0) ? _n : index; // 00 11
            corners[1] = ((mask & 6) > 0) ? _n : index + 1; // 01 10
            corners[2] = ((mask & 12) > 0) ? _n : index + 1 + _nx; // 11 00
            corners[3] = ((mask & 9) > 0) ? _n : index + _nx; // 10 01

            // compute weights using fractional components
            result.SetWeights(u, v);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtEqual(Vec2d point, FieldPoint2d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j);

            // offsets
            int di = 1;
            int dj = _nx;

            // clamp whole components and adjust offsets if necessary
            if (i < 0) { i = 0; di = 0; }
            else if (i >= _nx - 1) { i = _nx - 1; di = 0; }

            if (j < 0) { j = 0; dj = 0; }
            else if (j >= _ny - 1) { j = _ny - 1; dj = 0; }

            // set corner indices
            int index = FlattenIndex(i, j);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + di + dj;
            corners[3] = index + dj;

            // compute weights using fractional components
            result.SetWeights(u, v);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtPeriodic(Vec2d point, FieldPoint2d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j);

            // wrap whole components
            i = SlurMath.Mod2(i, _nx);
            j = SlurMath.Mod2(j, _ny);

            // get offsets
            int di = (i == _nx - 1) ? 1 - _nx : 1;
            int dj = (j == _ny - 1) ? _nx - _n : _nx;

            // set corner indices
            int index = FlattenIndex(i, j);
            int[] corners = result.Corners;

            corners[0] = index;
            corners[1] = index + di;
            corners[2] = index + di + dj;
            corners[3] = index + dj;

            // compute weights using fractional components
            result.SetWeights(u, v);
        }

        
        /*
       /// <summary>
       /// Generalizes well for more involved types of interpolation (cubic etc.)
       /// http://paulbourke.net/miscellaneous/interpolation/
       /// </summary>
       /// <param name="point"></param>
       /// <param name="result"></param>
       private void FieldPointAtClamped2(Vec2d point, FieldPoint2d result)
       {
           // convert to grid space and separate fractional and whole components
           int i0, j0;
           double u = SlurMath.Fract((point.x - _x0) * _dxInv, out i0);
           double v = SlurMath.Fract((point.y - _y0) * _dyInv, out j0);

           int[] corners = result.Corners;
           int index = 0;

           for (int j = 0; j < 2; j++)
           {
               int jj = SlurMath.Clamp(j0 + j, _ny - 1);
               for (int i = 0; i < 2; i++)
               {
                   int ii = SlurMath.Clamp(i0 + i, _nx - 1);
                   corners[index] = FlattenIndex(ii, jj);
                   index++;
               }
           }

           // compute weights using fractional components
           result.SetWeights(u, v);
       }


       /// <summary>
       /// Generalizes well for more involved types of interpolation (cubic etc.)
       /// http://paulbourke.net/miscellaneous/interpolation/
       /// </summary>
       /// <param name="point"></param>
       /// <param name="result"></param>
       private void FieldPointAtWrapped2(Vec2d point, FieldPoint2d result)
       {
           // convert to grid space and separate fractional and whole components
           int i0, j0;
           double u = SlurMath.Fract((point.x - _x0) / _dxInv, out i0);
           double v = SlurMath.Fract((point.y - _y0) / _dyInv, out j0);

           int[] corners = result.Corners;
           int index = 0;

           for (int j = 0; j < 2; j++)
           {
               int jj = SlurMath.Mod2(j0 + j, _ny);
               for (int i = 0; i < 2; i++)
               {
                   int ii = SlurMath.Mod2(i0 + i, _nx);
                   corners[index] = FlattenIndex(ii, jj);
                   index++;
               }
           }

           // compute weights using fractional components
           result.SetWeights(u, v);
       }
       */
    }
}
