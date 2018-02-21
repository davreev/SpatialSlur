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
    public class Grid2d
    {
        private double _dx;
        private double _dy;

        private double _tx = 1.0;
        private double _ty = 1.0;

        private double _txInv = 1.0; // cached to avoid divs
        private double _tyInv = 1.0;

        private readonly int _nx;
        private readonly int _ny;
        private readonly int _n;

        private WrapMode _wrapX;
        private WrapMode _wrapY;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public Grid2d(int countX, int countY)
        {
            const string errorMessage = "The resolution of the grid must be greater than 1 in each dimension.";

            if (countX < 2 || countY < 2)
                throw new ArgumentException(errorMessage);

            _nx = countX;
            _ny = countY;
            _n = countX * countY;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public Grid2d(Grid2d other)
        {
            _dx = other._dx;
            _dy = other._dy;

            _tx = other._tx;
            _ty = other._ty;

            _txInv = other._txInv;
            _tyInv = other._tyInv;

            _nx = other._nx;
            _ny = other._ny;
            _n = other._n;
            
            _wrapX = other._wrapX;
            _wrapY = other._wrapY;
        }


        /// <summary>
        /// Gets or sets the origin of the grid.
        /// </summary>
        public Vec2d Origin
        {
            get { return new Vec2d(_dx, _dy); }
            set { (_dx, _dy) = value; }
        }


        /// <summary>
        /// Gets or sets the scale of the grid in each dimension.
        /// </summary>
        public Vec2d Scale
        {
            get { return new Vec2d(_tx, _ty); }
            set
            {
                ScaleX = value.X;
                ScaleY = value.Y;
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
        /// Gets or sets the bounds of the grid.
        /// </summary>
        public Interval2d Bounds
        {
            get
            {
                return new Interval2d(
                    _dx, _dx + (_nx + 1) * _tx,
                    _dy, _dy + (_ny + 1) * _ty
                    );
            }
            set
            {
                if (!value.IsValid)
                    throw new System.ArgumentException("The given interval must be valid.");

                Origin = value.A;
                Scale = new Vec2d(
                    value.X.Length / (_nx - 1),
                    value.Y.Length / (_ny - 1)
                    );
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
        /// 
        /// </summary>
        public WrapMode WrapMode
        {
            set { _wrapX = _wrapY = value; }
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
        /// Enumerates over the coordinates of the grid.
        /// </summary>
        public IEnumerable<Vec2d> Coordinates
        {
            get
            {
                for (int j = 0; j < CountY; j++)
                {
                    for (int i = 0; i < CountX; i++)
                        yield return CoordinateAt(i, j);
                }
            }
        }


        /// <summary>
        /// Returns the coordinate at the given indices.
        /// Convention is (i, j) => (x, y)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Vec2d CoordinateAt(int i, int j)
        {
            return new Vec2d(
                i * _tx + _dx,
                j * _ty + _dy);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vec2d CoordinateAt(int index)
        {
            (int i, int j) = IndicesAt(index);
            return CoordinateAt(i, j);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d CoordinateAt(GridPoint2d point)
        {
            var corners = point.Corners;
            var weights = point.Weights;
            var sum = new Vec2d();

            for (int i = 0; i < 4; i++)
                sum += CoordinateAt(corners[i]) * weights[i];

            return sum;
        }


        /// <summary>
        /// Returns true if the grid has the same resolution in each dimension as another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool ResolutionEquals(Grid2d other)
        {
            return (_nx == other._nx && _ny == other._ny);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public (int i, int j) Wrap(int i, int j)
        {
            return (WrapX(i), WrapY(j));
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
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// Applies the current wrap mode to the given indices.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAt(int i, int j)
        {
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), _nx);
        }


        /// <summary>
        /// Flattens a 2 dimensional index into a 1 dimensional index.
        /// Assumes the given indices are within the valid range.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(int i, int j)
        {
            return GridUtil.FlattenIndices(i, j, _nx);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAt(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return GridUtil.FlattenIndices(WrapX(i), WrapY(j), _nx);
        }


        /// <summary>
        /// Returns the index of the nearest point in the grid.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IndexAtUnchecked(Vec2d point)
        {
            (int i, int j) = IndicesAt(point);
            return GridUtil.FlattenIndices(i, j, _nx);
        }

        
        /// <summary>
        /// Expands a 1 dimensional index into a 2 dimensional index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (int i, int j) IndicesAt(int index)
        {
            return GridUtil.ExpandIndex(index, _nx);
        }
        

        /// <summary>
        /// Returns the indices of the nearest point in the grid.
        /// </summary>
        /// <param name="point"></param>
        public (int i, int j) IndicesAt(Vec2d point)
        {
            return (
                (int)Math.Round((point.X - _dx) * _txInv),
                (int)Math.Round((point.Y - _dy) * _tyInv)
                );
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAt(Vec2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAt(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAt(Vec2d point, GridPoint2d result)
        {
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0)
                );

            int i1 = WrapX(i0 + 1);
            int j1 = WrapY(j0 + 1) * _nx;

            i0 = WrapX(i0);
            j0 = WrapY(j0) * _nx;

            var corners = result.Corners;
            corners[0] = i0 + j0;
            corners[1] = i1 + j0;
            corners[2] = i0 + j1;
            corners[3] = i1 + j1;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GridPoint2d GridPointAtUnchecked(Vec2d point)
        {
            GridPoint2d result = new GridPoint2d();
            GridPointAtUnchecked(point, result);
            return result;
        }


        /// <summary>
        /// Returns a grid point at the given point.
        /// Assumes the point is within the bounds of the grid.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        public void GridPointAtUnchecked(Vec2d point, GridPoint2d result)
        {
            point = ToGridSpace(point);

            result.SetWeights(
                SlurMath.Fract(point.X, out int i0),
                SlurMath.Fract(point.Y, out int j0)
                );

            j0 *= _nx;
            int i1 = i0 + 1;
            int j1 = j0 + _nx;

            var corners = result.Corners;
            corners[0] = i0 + j0;
            corners[1] = i1 + j0;
            corners[2] = i0 + j1;
            corners[3] = i1 + j1;
        }


        /// <summary>
        /// Maps a point from world space to grid space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d ToGridSpace(Vec2d point)
        {
            return new Vec2d(
                (point.X - _dx) * _txInv,
                (point.Y - _dy) * _tyInv
            );
        }


        /// <summary>
        /// Maps a point from grid space to world space.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vec2d ToWorldSpace(Vec2d point)
        {
            return new Vec2d(
               point.X * _tx + _dx,
               point.Y * _ty + _dy
           );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal (int i, int j) GetBoundaryOffsets()
        {
            return(
                WrapModeX == WrapMode.Repeat ? _nx - 1 : 0,
                WrapModeY == WrapMode.Repeat ? _n - _nx : 0
                );
        }
    }
}