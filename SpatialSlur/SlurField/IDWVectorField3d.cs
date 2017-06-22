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
    public class IDWVectorField3d : IDWField3d<Vec3d>
    {
        /// <summary>
        /// 
        /// </summary>
        public IDWVectorField3d(double power)
            : base(power)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override Vec3d ValueAt(Vec3d point)
        {
            Vec3d sum = DefaultValue * DefaultWeight;
            double wsum = DefaultWeight;

            foreach (var dp in Points)
            {
                double w = 1.0 / Math.Pow(point.DistanceTo(dp.Point) * dp.Scale + Epsilon, Power);
                sum += dp.Value * w;
                wsum += w;
            }

            return (wsum > 0.0) ? sum / wsum : new Vec3d();
        }
    }
}
