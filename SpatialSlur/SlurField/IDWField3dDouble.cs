using System;

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
    internal class IDWField3dDouble : IDWField3d<double>
    {
        #region Nested Types

        /// <summary>
        /// 
        /// </summary>
        internal class Factory : IDWFieldFactory<double>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override IDWField3d<double> Create(double power, double epsilon = SlurMath.ZeroTolerance)
            {
                return new IDWField3dDouble(power, epsilon);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public IDWField3dDouble(double power, double epsilon = SlurMath.ZeroTolerance)
            : base(power, epsilon)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public sealed override IDWField3d<double> Duplicate()
        {
            return IDWField3d.Double.CreateCopy(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public sealed override double ValueAt(Vec3d point)
        {
            double sum = DefaultValue * DefaultWeight;
            double wsum = DefaultWeight;

            foreach (var dp in Points)
            {
                double w = dp.Influence / Math.Pow(point.DistanceTo(dp.Point) + Epsilon, Power);
                sum += dp.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : 0.0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gx"></param>
        /// <param name="gy"></param>
        /// <param name="gz"></param>
        public sealed override void GradientAt(Vec3d point, out double gx, out double gy, out double gz)
        {
            throw new NotImplementedException();
        }
    }
}
