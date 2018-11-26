
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    internal class GridField3dMatrix3d: GridField3d<Matrix3d>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : GridField3dFactory<Matrix3d>
        {
            /// <inheritdoc />
            public override GridField3d<Matrix3d> Create(int countX, int countY, int countZ)
            {
                return new GridField3dMatrix3d(countX, countY, countZ);
            }


            /// <inheritdoc />
            public override GridField3d<Matrix3d> Create(Grid3d grid)
            {
                return new GridField3dMatrix3d(grid);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="countX"></param>
        /// <param name="countY"></param>
        /// <param name="countZ"></param>
        public GridField3dMatrix3d(int countX, int countY, int countZ)
           : base(countX, countY, countZ)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GridField3dMatrix3d(Grid3d other)
            : base(other)
        {
        }


        /// <inheritdoc />
        public sealed override GridField3d<Matrix3d> Duplicate(bool setValues)
        {
            var result = GridField3d.Matrix3d.Create(this);
            if (setValues) result.Set(this);
            return result;
        }


        /// <inheritdoc />
        protected sealed override Matrix3d ValueAtLinear(Vector3d point)
        {
            (var u, var v, var w) = Vector3d.Fract(ToGridSpace(point), out Vector3i whole);

            var x0 = WrapX(whole.X);
            var y0 = WrapY(whole.Y) * CountX;
            var z0 = WrapZ(whole.Z) * CountXY;

            int x1 = WrapX(whole.X + 1);
            int y1 = WrapY(whole.Y + 1) * CountX;
            int z1 = WrapZ(whole.Z + 1) * CountXY;
            
            var vals = Values;
            return Matrix3d.Lerp(
               Matrix3d.Lerp(
                   Matrix3d.Lerp(vals[x0 + y0 + z0], vals[x1 + y0 + z0], u),
                   Matrix3d.Lerp(vals[x0 + y1 + z0], vals[x1 + y1 + z0], u),
                   v),
               Matrix3d.Lerp(
                   Matrix3d.Lerp(vals[x0 + y0 + z1], vals[x1 + y0 + z1], u),
                   Matrix3d.Lerp(vals[x0 + y1 + z1], vals[x1 + y1 + z1], u),
                   v),
               w);
        }


        /// <inheritdoc />
        protected sealed override Matrix3d ValueAtLinearUnsafe(Vector3d point)
        {
            (var u, var v, var w) = Vector3d.Fract(ToGridSpace(point), out Vector3i whole);

            var x0 = whole.X;
            var y0 = whole.Y * CountX;
            var z0 = whole.Z * CountXY;

            int x1 = x0 + 1;
            int y1 = y0 + CountX;
            int z1 = z0 + CountXY;

            var vals = Values;
            return Matrix3d.Lerp(
               Matrix3d.Lerp(
                   Matrix3d.Lerp(vals[x0 + y0 + z0], vals[x1 + y0 + z0], u),
                   Matrix3d.Lerp(vals[x0 + y1 + z0], vals[x1 + y1 + z0], u),
                   v),
               Matrix3d.Lerp(
                   Matrix3d.Lerp(vals[x0 + y0 + z1], vals[x1 + y0 + z1], u),
                   Matrix3d.Lerp(vals[x0 + y1 + z1], vals[x1 + y1 + z1], u),
                   v),
               w);
        }


        /// <inheritdoc />
        public sealed override Matrix3d ValueAt(ref GridPoint3d point)
        {
            var vals = Values;

            return
                vals[point.Index0] * point.Weight0 +
                vals[point.Index1] * point.Weight1 +
                vals[point.Index2] * point.Weight2 +
                vals[point.Index3] * point.Weight3 +
                vals[point.Index4] * point.Weight4 +
                vals[point.Index5] * point.Weight5 +
                vals[point.Index6] * point.Weight6 +
                vals[point.Index7] * point.Weight7;
        }
    }
}
