
/*
 * Notes
 */

using System;

using SpatialSlur;
using SpatialSlur.Fields;

using D = SpatialSlur.SlurMath.Constantsd;

namespace SpatialSlur.Fields
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
            /// <inheritdoc />
            public override IDWField3d<double> Create(double power, double epsilon = D.ZeroTolerance)
            {
                return new IDWField3dDouble(power, epsilon);
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public IDWField3dDouble(double power, double epsilon = D.ZeroTolerance)
            : base(power, epsilon)
        {
        }


        /// <inheritdoc />
        public sealed override IDWField3d<double> Duplicate()
        {
            return IDWField3d.Double.CreateCopy(this);
        }


        /// <inheritdoc />
        public sealed override double ValueAt(Vector3d point)
        {
            double sum = 0.0;
            double wsum = 0.0;

            foreach (var obj in Objects)
            {
                double w = obj.Influence / Math.Pow(obj.DistanceTo(point) + Epsilon, Power);
                sum += obj.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : 0.0;
        }


        /// <inheritdoc />
        public sealed override void GradientAt(Vector3d point, out double gx, out double gy, out double gz)
        {
            throw new NotImplementedException();
        }
    }
}
