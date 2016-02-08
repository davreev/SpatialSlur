using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurData
{
    /// <summary>
    /// Simple voxel grid for broad phase collision detection between dynamic objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpatialGrid3d<T>:Spatial3d<T>
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
            : base(binCountX * binCountY * binCountZ)
        {
            _nx = binCountX;
            _ny = binCountY;
            _nz = binCountZ;
            _nxy = _nx * _ny;
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

            _dx = _domain.x.Span / _nx;
            _dy = _domain.y.Span / _ny;
            _dz = _domain.z.Span / _nz;

            _dxInv = 1.0 / _dx;
            _dyInv = 1.0 / _dy;
            _dzInv = 1.0 / _dz;

            Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        protected override void Discretize(Vec3d point, out int i, out int j, out int k)
        {
            i = SlurMath.Clamp((int)Math.Floor((point.x - _from.x) * _dxInv), _nx - 1);
            j = SlurMath.Clamp((int)Math.Floor((point.y - _from.y) * _dyInv), _ny - 1);
            k = SlurMath.Clamp((int)Math.Floor((point.z - _from.z) * _dzInv), _nz - 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        protected override int ToIndex(int i, int j, int k)
        {
            return i + j * _nx + k * _nxy;
        }
    }
}
