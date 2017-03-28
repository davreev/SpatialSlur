using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple grid for broad phase collision detection between dynamic objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SpatialGrid2d<T>:Spatial2d<T>
    {
        private Domain2d _domain;
        private Vec2d _from;
        private double _dx, _dy;
        private double _dxInv, _dyInv;
        private int _nx, _ny;


        /// <summary>
        ///
        /// </summary>
        public SpatialGrid2d(Domain2d domain, int binCountX, int binCountY)
            : base(binCountX * binCountY)
        {
            if (binCountX < 1 || binCountY < 1)
                throw new System.ArgumentOutOfRangeException("The data structure must have at least 1 bin in each dimension.");

            _nx = binCountX;
            _ny = binCountY;
            Domain = domain;
        }


        /// <summary>
        ///
        /// </summary>
        public int BinCountX
        {
            get { return _nx; }
        }


        /// <summary>
        ///
        /// </summary>
        public int BinCountY
        {
            get { return _ny; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double BinScaleX
        {
            get { return _dx; }
        }


        /// <summary>
        /// 
        /// </summary>
        public double BinScaleY
        {
            get { return _dy; }
        }


        /// <summary>
        /// Gets or sets the extents of the grid.
        /// Note that setting the domain clears the grid.
        /// </summary>
        public Domain2d Domain
        {
            get { return _domain; }
            set
            {
                if (!value.IsValid)
                    throw new System.ArgumentException("The domain must be valid.");

                _domain = value;
                OnDomainChange();
            }
        }


        /// <summary>
        /// This is called after any changes to the grid's domain.
        /// </summary>
        private void OnDomainChange()
        {
            _from = _domain.From;

            _dx = _domain.x.Span / _nx;
            _dy = _domain.y.Span / _ny;

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;

            Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        internal override void Discretize(Vec2d point, out int i, out int j)
        {
            i = SlurMath.Clamp((int)Math.Floor((point.x - _from.x) * _dxInv), _nx - 1);
            j = SlurMath.Clamp((int)Math.Floor((point.y - _from.y) * _dyInv), _ny - 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        internal override int ToIndex(int i, int j)
        {
            return i + j * _nx;
        }
    }
}
