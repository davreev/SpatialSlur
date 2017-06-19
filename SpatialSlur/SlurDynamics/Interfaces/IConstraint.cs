using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="P"></typeparam>
    public interface IConstraint<P>
    {
        /// <summary>
        /// 
        /// </summary>
        double Weight { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        void Calculate(IReadOnlyList<P> particles);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        void Apply(IReadOnlyList<P> particles);


        /// <summary>
        /// 
        /// </summary>
        void SetHandles(IEnumerable<int> indices);
    }
}
