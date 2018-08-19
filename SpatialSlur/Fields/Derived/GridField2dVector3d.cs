
/*
 * Notes
 */

using System;
using SpatialSlur.Fields;

namespace SpatialSlur.Fields
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    internal class GridField2dVector3d : GridField2d<Vector3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField2dFactory<Vector3d>
        {
            /// <inheritdoc />
            public override GridField2d<Vector3d> Create(int countX, int countY)
            {
                return new GridField2dVector3d(countX, countY);
            }


            /// <inheritdoc />
            public override GridField2d<Vector3d> Create(Grid2d grid)
            {
                return new GridField2dVector3d(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        public GridField2dVector3d(int countX, int countY)
           : base(countX, countY)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridField2dVector3d(Grid2d other)
            : base(other)
        {
        }


        /// <inheritdoc />
        public sealed override GridField2d<Vector3d> Duplicate(bool setValues)
        {
            var result = GridField2d.Vector3d.Create(this);
            if (setValues) result.Set(this);
            return result;
        }

        
        /// <inheritdoc />
        protected sealed override Vector3d ValueAtLinear(Vector2d point)
        {
            (var u, var v) = Vector2d.Fract(ToGridSpace(point), out Vector2i whole);

            var x0 = WrapX(whole.X);
            var y0 = WrapY(whole.Y) * CountX;

            int x1 = WrapX(whole.X + 1);
            int y1 = WrapY(whole.Y + 1) * CountX;

            var vals = Values;
            return Vector3d.Lerp(
                Vector3d.Lerp(vals[x0 + y0], vals[x1 + y0], u),
                Vector3d.Lerp(vals[x0 + y1], vals[x1 + y1], u),
                v);
        }


        /// <inheritdoc />
        protected sealed override Vector3d ValueAtLinearUnsafe(Vector2d point)
        {
            (var u, var v) = Vector2d.Fract(ToGridSpace(point), out Vector2i whole);

            var x0 = whole.X;
            var y0 = whole.Y * CountX;

            var x1 = x0 + 1;
            var y1 = y0 + CountX;

            var vals = Values;
            return Vector3d.Lerp(
                Vector3d.Lerp(vals[x0 + y0], vals[x1 + y0], u),
                Vector3d.Lerp(vals[x0 + y1], vals[x1 + y1], u),
                v);
        }


        /// <inheritdoc />
        public sealed override Vector3d ValueAt(ref GridPoint2d point)
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
