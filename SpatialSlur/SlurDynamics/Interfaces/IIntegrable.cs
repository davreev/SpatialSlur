using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurDynamics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIntegrable
    {
        /// <summary>
        /// Updates position and returns speed.
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        double UpdatePosition(double timeStep, double damping);


        /// <summary>
        /// Updates rotation and returns angular speed.
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="damping"></param>
        /// <returns></returns>
        double UpdateRotation(double timeStep, double damping);
    }
}
