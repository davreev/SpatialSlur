using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class IDWScalarField3d : IDWField3d<double>, IDifferentiableField2d<Vec3d>, IDifferentiableField3d<Vec3d>
    {
        /// <summary>
        /// 
        /// </summary>
        public IDWScalarField3d(double power)
            : base(power)
        {
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
        /// <returns></returns>
        public Vec3d GradientAt(Vec3d point)
        {
            // TODO
            throw new NotImplementedException();
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vec3d IDifferentiableField2d<Vec3d>.GradientAt(Vec2d point)
        {
            return GradientAt(point);
        }

        #endregion
    }
}
