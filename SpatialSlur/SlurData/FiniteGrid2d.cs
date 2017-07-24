using System;
using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple bounded grid for broad phase collision detection between dynamic objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class FiniteGrid2d<T> : Grid2d<T>
    {
        private Domain2d _domain;
        private Vec2d _from;
        private double _dx, _dy;
        private double _dxInv, _dyInv;
        private int _nx, _ny;


        /// <summary>
        ///
        /// </summary>
        public FiniteGrid2d(Domain2d domain, int binCountX, int binCountY)
        {
            _nx = binCountX;
            _ny = binCountY;

            Init(_nx * _ny);
            Domain = domain;
        }


        /// <summary>
        ///
        /// </summary>
        public FiniteGrid2d(Domain2d domain, double binScale)
            : this(domain, binScale, binScale)
        {
        }


        /// <summary>
        ///
        /// </summary>
        public FiniteGrid2d(Domain2d domain, double binScaleX, double binScaleY)
        {
            _nx = Math.Max((int)Math.Round(domain.X.Span / binScaleX), 1);
            _ny = Math.Max((int)Math.Round(domain.Y.Span / binScaleY), 1);

            Init(_nx * _ny);
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
                UpdateBinScale();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="binCountX"></param>
        /// <param name="binCountY"></param>
        /// <param name="binCountZ"></param>
        public void Resize(int binCountX, int binCountY)
        {
            _nx = binCountX;
            _ny = binCountY;

            ResizeImpl(_nx * _ny);
            UpdateBinScale();
        }


        /// <summary>
        ///
        /// </summary>
        public void Resize(double binScale)
        {
            Resize(binScale, binScale);
        }


        /// <summary>
        ///
        /// </summary>
        public void Resize(double binScaleX, double binScaleY)
        {
            _nx = Math.Max((int)Math.Round(_domain.X.Span / binScaleX), 1);
            _ny = Math.Max((int)Math.Round(_domain.Y.Span / binScaleY), 1);

            ResizeImpl(_nx * _ny);
            UpdateBinScale();
        }


        /// <summary>
        /// This is called after any changes to the grid's domain or resolution.
        /// </summary>
        private void UpdateBinScale()
        {
            _from = _domain.From;

            _dx = _domain.X.Span / _nx;
            _dy = _domain.Y.Span / _ny;

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;

            Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        protected sealed override (int, int) Discretize(Vec2d point)
        {
            return (
                SlurMath.Clamp((int)Math.Floor((point.X - _from.X) * _dxInv), _nx - 1),
                SlurMath.Clamp((int)Math.Floor((point.Y - _from.Y) * _dyInv), _ny - 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        protected sealed override int ToIndex(int i, int j)
        {
            return i + j * _nx;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("SpatialGrid3d ({0} x {1})", BinCountX, BinCountY);
        }
    }
}
