using System;
using System.Collections.Generic;

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
    public class Grid3d
    {
        private double _dx;
        private double _dy;
        private double _dz;

        private double _tx = 1.0;
        private double _ty = 1.0;
        private double _tz = 1.0;
        
        private double _txInv = 1.0; // cached to avoid divs
        private double _tyInv = 1.0;
        private double _tzInv = 1.0;

        private readonly int _nx;
        private readonly int _ny;
        private readonly int _nz;
        private readonly int _nxy;
        private readonly int _n;

        private WrapMode _wrapX;
        private WrapMode _wrapY;
        private WrapMode _wrapZ;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public Grid3d(int countX, int countY, int countZ)
        {
            const string errorMessage = "The resolution of the grid must be greater than 1 in each dimension.";

            if (countX < 2 || countY < 2 || countZ < 2)
                throw new ArgumentException(errorMessage);

            _nx = countX;
            _ny = countY;
            _nz = countZ;
            _nxy = countX * countY;
            _n = _nxy * countZ;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Grid3d(Grid3d other)
        {
            _dx = other._dx;
            _dy = other._dy;
            _dz = other._dz;

            _tx = other._tx;
            _ty = other._ty;
            _tz = other._tz;

            _txInv = other._txInv;
            _tyInv = other._tyInv;
            _tzInv = other._tzInv;

            _nx = other._nx;
            _ny = other._ny;
            _nz = other._nz;
            _nxy = other._nxy;
            _n = other._n;

            _wrapX = other._wrapX;
            _wrapY = other._wrapY;
            _wrapZ = other._wrapZ;
        }


        /// <summary>
        /// Gets or sets the origin of the grid.
        /// </summary>
        public Vec3d Origin
        {
            get { return new Vec3d(_dx, _dy, _dz); }
            set { (_dx, _dy, _dz) = value; }
        }


        /// <summary>
        /// Gets or sets the scale of the grid in each dimension.
        /// </summary>
        public Vec3d Scale
        {
            get { return new Vec3d(_tx, _ty, _tz); }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
                ScaleZ = value.Z;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleX
        {
            get { return _tx; }
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _tx = value;
                _txInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleY
        {
            get { return _ty; }
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _ty = value;
                _tyInv = 1.0 / value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ScaleZ
        {
            get { return _tz; }
            set
            {
                if (value == 0.0)
                    throw new DivideByZeroException();

                _tz = value;
                _tzInv = 1.0 / value;
            }
        }

        
        /// <summary>
        /// Gets or sets the bounds of the grid.
        /// </summary>
        public Interval3d Bounds
        {
            get
            {
                return new Interval3d(
                   _dx, _dx + (_nx + 1) * _tx,
                   _dy, _dy + (_ny + 1) * _ty,
                   _dz, _dz + (_nz + 1) * _tz
                   );
            }
            set
            {
                Origin = value.A;
                ScaleX = value.X.Length / (_nx - 1);
                ScaleY = value.Y.Length / (_ny - 1);
                ScaleZ = value.Z.Length / (_nz - 1);
            }
        }


        /// <summary>
        /// Returns the size of the grid.
        /// </summary>
        public int Count
        {
            get { return _n; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the x direction.
        /// </summary>
        public int CountX
        {
            get { return _nx; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the y direction.
        /// </summary>
        public int CountY
        {
            get { return _ny; }
        }


        /// <summary>
        /// Returns the resolution of the grid in the y direction.
        /// </summary>
        public int CountZ
        {
            get { return _nz; }
        }


        /// <summary>
        /// Returns the size of a single xy layer.
        /// </summary>
        public int CountXY
        {
            get { return _nxy; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapMode
        {
            set { _wrapX = _wrapY = _wrapZ = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeX
        {
            get { return _wrapX; }
            set { _wrapX = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeY
        {
            get { return _wrapY; }
            set { _wrapY = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public WrapMode WrapModeZ
        {
            get { return _wrapZ; }
            set { _wrapZ = value; }
        }


        /// <summary>
        /// Enumerates over coordinates of the grid.
        /// </summary>
        public IEnumerable<Vec3d> Coordinates
        {
            get
            {
                for (int k = 0; k < CountZ; k++)
                {
                    for (int j = 0; j < CountY; j++)
                    {
                        for (int i = 0; i < CountX; i++)
                            yield return CoordinateAt(i, j, k);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the coordinate at the given indices.
        /// Convention is (i, j, k) => (x, y, z)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(int i, int j, int k)
        {
            return new Vec3d(
                i * _tx + _dx,
                j * _ty + _dy,
                k * _tz + _dz);
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
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d CoordinateAt(GridPoint3d point)
        {
            var corners = point.Corners;
            var weights = point.Weights;
            var sum = new Vec3d();

            for (int i = 0; i < 8; i++)
                sum += CoordinateAt(corners[i]) * weights[i];

            return sum;
        }


        /// <summary>
        /// Returns true if the grid has the same resolution in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(Grid3d other)
        {
            return (_nx == other._nx && _ny == other._ny && _nz == other._nz);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public (int i, int j, int k) Wrap(int i, int j, int k)
        {
            return (WrapX(i), WrapY(j), WrapZ(k));
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int WrapX(int i)
        {
            return GridUtil.Wrap(i, _nx, _wrapX);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public int WrapY(int j)
        {
            return GridUtil.Wrap(j, _ny, _wrapY);
        }


        /// <summary>
        /// Applies a wrap function to the given index based on the current wrap mode.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public int WrapZ(int k)
        {
            return GridUtil.Wrap(k, _nz, _wrapZ);
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
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
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
            return GridUtil.FlattenIndices(i, j, k, _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), WrapZ(k), _nx, _nxy);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec3d point)
        {
            (int i, int j, int k) = IndicesAt(point);
            return GridUtil.FlattenIndices(i, j, k, _nx, _nxy);
        }

        
        /// <summary>
        /// Expands a 1 dimensional index into a 3 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int i, int j, int k) IndicesAt(int index)
        {
            return GridUtil.ExpandIndex(index, _nx, _nxy);
        }

        
        /// <summary>
        /// Returns the indices of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        public (int i, int j, int k) IndicesAt(Vec3d point)
        {
            return (
                (int)Math.Round((point.X - _dx) * _txInv),
                (int)Math.Round((point.Y - _dy) * _tyInv),
                (int)Math.Round((point.Z - _dz) * _tzInv)
                );
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
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0),
                SlurMath.Fract(point.Z, out int k0)
                );

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
        /// Assumes the point is within the bounds of the grid.
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
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnchecked(Vec3d point, GridPoint3d result)
        {
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0),
                SlurMath.Fract(point.Z, out int k0)
                );

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
        /// Maps a point from world space to grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ToGridSpace(Vec3d point)
        {
            return new Vec3d(
                (point.X - _dx) * _txInv,
                (point.Y - _dy) * _tyInv,
                (point.Z - _dz) * _tzInv
            );
        }


        /// <summary>
        /// Maps a point from grid space to world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec3d ToWorldSpace(Vec3d point)
        {
            return new Vec3d(
               point.X * _tx + _dx,
               point.Y * _ty + _dy,
               point.Z * _tz + _dz
           );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal (int i, int j, int k) GetBoundaryOffsets()
        {
            return (
                WrapModeX == WrapMode.Repeat ? _nx - 1 : 0,
                WrapModeY == WrapMode.Repeat ? _nxy - _nx : 0,
                WrapModeZ == WrapMode.Repeat ? _n - _nxy : 0
                );
        }
    }
}
