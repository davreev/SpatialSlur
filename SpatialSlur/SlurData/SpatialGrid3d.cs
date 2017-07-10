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
    public class SpatialGrid3d<T>:SpatialMap3d<T>
    {
        private Domain3d _domain;
        private Vec3d _from;
        private double _dx, _dy, _dz;
        private double _dxInv, _dyInv, _dzInv;
        private int _nx, _ny, _nz, _nxy;


        /// <summary>
        ///
        /// </summary>
        public SpatialGrid3d(Domain3d domain, int binCountX, int binCountY, int binCountZ)
        {
            Init(binCountX * binCountY * binCountZ);

            _nx = binCountX;
            _ny = binCountY;
            _nz = binCountZ;
            _nxy = _nx * _ny;
            Domain = domain;
        }


        /// <summary>
        ///
        /// </summary>
        public SpatialGrid3d(Domain3d domain, double binScale)
            :this(domain, binScale, binScale, binScale)
        {
        }


        /// <summary>
        ///
        /// </summary>
        public SpatialGrid3d(Domain3d domain, double binScaleX, double binScaleY, double binScaleZ)
        {
            _nx = Math.Max((int)Math.Round(domain.X.Span / binScaleX), 1);
            _ny = Math.Max((int)Math.Round(domain.Y.Span / binScaleY), 1);
            _nz = Math.Max((int)Math.Round(domain.Z.Span / binScaleZ), 1);
            _nxy = _nx * _ny;

            Init(_nxy * _nz);
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
        public int BinCountZ
        {
            get { return _nz; }
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
        /// 
        /// </summary>
        public double BinScaleZ
        {
            get { return _dz; }
        }


        /// <summary>
        /// Gets or sets the extents of the grid.
        /// Note that setting the domain clears the grid.
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
        /// This is called after any changes to the grid's domain.
        /// </summary>
        private void OnDomainChange()
        {
            _from = _domain.From;

            _dx = _domain.X.Span / _nx;
            _dy = _domain.Y.Span / _ny;
            _dz = _domain.Z.Span / _nz;

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
            _dzInv = 1.0 / _dz;

            Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected sealed override (int,int,int) Discretize(Vec3d point)
        {
            return (
                SlurMath.Clamp((int)Math.Floor((point.X - _from.X) * _dxInv), _nx - 1),
                SlurMath.Clamp((int)Math.Floor((point.Y - _from.Y) * _dyInv), _ny - 1),
                SlurMath.Clamp((int)Math.Floor((point.Z - _from.Z) * _dzInv), _nz - 1));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected sealed override int ToIndex(int i, int j, int k)
        {
            return i + j * _nx + k * _nxy;
        }
    }
}
