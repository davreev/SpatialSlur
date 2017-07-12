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
    public interface IConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        double Weight { get; set; }


        /// <summary>
        /// Returns true if this constraint acts on the orientation of particles.
        /// </summary>
        /// <returns></returns>
        bool AppliesRotation { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        void Calculate(IReadOnlyList<IParticle> particles);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="particles"></param>
        void Apply(IReadOnlyList<IParticle> particles);


        /// <summary>
        /// 
        /// </summary>
        void SetHandles(IEnumerable<int> indices);
    }
}
