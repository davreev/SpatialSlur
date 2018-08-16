using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.Dynamics;

namespace SpatialSlur
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class IEnumerableExtensions
    {
        #region IEnumerable<IConstraint>

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constraints"></param>
        /// <param name="linearTolerance"></param>
        /// <param name="angularTolerance"></param>
        /// <returns></returns>
        public static bool AreSatisfied<T>(this IEnumerable<T> constraints, double linearTolerance, double angularTolerance)
            where T : IConstraint
        {
            foreach(var c in constraints)
            {
                c.GetEnergy(out double lin, out double ang);
                if (lin >= linearTolerance || ang >= angularTolerance) return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraints"></param>
        /// <param name="linear"></param>
        /// <param name="angular"></param>
        public static void GetEnergySum<T>(this IEnumerable<T> constraints, out double linear, out double angular)
            where T : IConstraint
        {
            linear = 0.0;
            angular = 0.0;

            foreach (var c in constraints)
            {
                c.GetEnergy(out double lin, out double ang);
                linear += lin;
                angular += ang;
            }
        }

        #endregion
    }
}
