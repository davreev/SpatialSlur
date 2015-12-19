 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Field2d
    {
        private Domain2d _domain;
        private Vec2d _from; // cached for convenience

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
                throw new System.ArgumentException("the field must have 2 or more values in each dimension");

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
                    throw new System.ArgumentException("The given domain must be valid");

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
        /// Iterates through the positions of values in the field. 
        /// Note that these are not explicitly stored in memory.
        /// </summary>
        public IEnumerable<Vec2d> Points
        {
            get
            {
                Vec2d p = _from;

                for (int i = 0; i < _ny; i++)
                {
                    for (int j = 0; j < _nx; j++)
                    {
                        yield return p;
                        p.x += _dx;
                    }
                    p.x = _from.x; // reset x
                    p.y += _dy;
                }
            }
        }


        /// <summary>
        /// This is called after any changes to the field's domain.
        /// </summary>
        protected virtual void OnDomainChange()
        {
            _from = _domain.From;

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
                    _indexAt = IndexAtClamped;
                    _index2At = ToIndex2AtClamped;
                    _fieldPointAt = FieldPointAtClamped;
                    break;
                case FieldBoundaryType.Equal:
                    _indexAt = IndexAtClamped;
                    _index2At = ToIndex2AtClamped;
                    _fieldPointAt = FieldPointAtClamped;
                    break;
                case FieldBoundaryType.Periodic:
                    _indexAt = IndexAtWrapped;
                    _index2At = Index2AtWrapped;
                    _fieldPointAt = FieldPointAtWrapped;
                    break;
            }
        }
        

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public abstract Field2d Duplicate();


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
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int FlattenIndex(int i, int j)
        {
            return i + j * _nx;
        }


        /// <summary>
        /// Converts a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public void ExpandIndex(int index, out int i, out int j)
        {
            j = index / _nx;
            i = index - j * _nx;
        }


        /// <summary>
        /// Converts a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public Vec2i ExpandIndex(int index)
        {
            int j = index / _nx;
            int i = index - j * _nx;
            return new Vec2i(i, j);
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
            int i = (int)Math.Round((point.x - _from.x) * _dxInv);
            int j = (int)Math.Round((point.y - _from.y) * _dyInv);
            return FlattenIndex(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtClamped(Vec2d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _from.x) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _from.y) * _dyInv), _ny - 1);
            return FlattenIndex(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int IndexAtWrapped(Vec2d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _from.x) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _from.y) * _dyInv), _ny);
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
            int i = (int)Math.Round((point.x - _from.x) * _dxInv);
            int j = (int)Math.Round((point.y - _from.y) * _dyInv);
            return new Vec2i(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec2i ToIndex2AtClamped(Vec2d point)
        {
            int i = SlurMath.Clamp((int)Math.Round((point.x - _from.x) * _dxInv), _nx - 1);
            int j = SlurMath.Clamp((int)Math.Round((point.y - _from.y) * _dyInv), _ny - 1);
            return new Vec2i(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vec2i Index2AtWrapped(Vec2d point)
        {
            int i = SlurMath.Mod2((int)Math.Round((point.x - _from.x) * _dxInv), _nx);
            int j = SlurMath.Mod2((int)Math.Round((point.y - _from.y) * _dyInv), _ny);
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
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);

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
        ///
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        private void FieldPointAtClamped(Vec2d point, FieldPoint2d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);

            // offsets
            int di = 1;
            int dj = _nx;

            // clamp whole components and adjust offsets if necessary
            if (i < 0) { i = 0; di = 0; }
            else if (i > _nx - 2) { i = _nx - 1; di = 0; }

            if (j < 0) { j = 0; dj = 0; }
            else if (j > _ny - 2) { j = _ny - 1; dj = 0; }

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
        private void FieldPointAtWrapped(Vec2d point, FieldPoint2d result)
        {
            // convert to grid space and separate fractional and whole components
            int i, j;
            double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i);
            double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j);

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
           double u = SlurMath.Fract((point.x - _from.x) * _dxInv, out i0);
           double v = SlurMath.Fract((point.y - _from.y) * _dyInv, out j0);

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
           double u = SlurMath.Fract((point.x - _from.x) / _dxInv, out i0);
           double v = SlurMath.Fract((point.y - _from.y) / _dyInv, out j0);

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
