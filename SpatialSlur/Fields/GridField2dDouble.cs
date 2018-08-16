
/*
 * Notes
 */

using System;
using SpatialSlur;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class GridField2dDouble: GridField2d<double>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField2dFactory<double>
        {
            /// <inheritdoc />
            public override GridField2d<double> Create(int countX, int countY)
            {
                return new GridField2dDouble(countX, countY);
            }


            /// <inheritdoc />
            public override GridField2d<double> Create(Grid2d grid)
            {
                return new GridField2dDouble(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public GridField2dDouble(int countX, int countY)
           : base(countX, countY)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridField2dDouble(Grid2d other)
            : base(other)
        {
        }


        /// <inheritdoc />
        public sealed override GridField2d<double> Duplicate(bool setValues)
        {
            var result = GridField2d.Double.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc />
        protected sealed override double ValueAtLinear(Vector2d point)
        {
            (var u, var v) = Vector2d.Fract(ToGridSpace(point), out Vector2i whole);
            
            var x0 = WrapX(whole.X);
            var y0 = WrapY(whole.Y) * CountX;

            int x1 = WrapX(whole.X + 1);
            int y1 = WrapY(whole.Y + 1) * CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[x0 + y0], vals[x1 + y0], u),
                SlurMath.Lerp(vals[x0 + y1], vals[x1 + y1], u),
                v);
        }


        /// <inheritdoc />
        protected sealed override double ValueAtLinearUnsafe(Vector2d point)
        {
            (var u, var v) = Vector2d.Fract(ToGridSpace(point), out Vector2i whole);

            var x0 = whole.X;
            var y0 = whole.Y * CountX;

            var x1 = x0 + 1;
            var y1 = y0 + CountX;

            var vals = Values;
            return SlurMath.Lerp(
                SlurMath.Lerp(vals[x0 + y0], vals[x1 + y0], u),
                SlurMath.Lerp(vals[x0 + y1], vals[x1 + y1], u),
                v);
        }


        /// <inheritdoc />
        public sealed override double ValueAt(ref GridPoint2d point)
        {
            var vals = Values;

            return
                vals[point.Index0] * point.Weight0 +
                vals[point.Index1] * point.Weight1 +
                vals[point.Index2] * point.Weight2 +
                vals[point.Index3] * point.Weight3;
        }
    }
}
