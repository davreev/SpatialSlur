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
    public class IDWScalarField3d : IDWField3d<double>
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
        public override double ValueAt(Vec3d point)
        {
            double sum = DefaultValue * DefaultWeight;
            double wsum = DefaultWeight;

            foreach (var dp in Points)
            {
                double w = 1.0 / Math.Pow(point.DistanceTo(dp.Point) * dp.Scale + Epsilon, Power);
                sum += dp.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : 0.0;
        }
    }
}
