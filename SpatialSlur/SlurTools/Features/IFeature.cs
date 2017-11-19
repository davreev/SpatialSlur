using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurTools.Features
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFeature
    {
        /// <summary>
        /// Determines priority during assignment
        /// </summary>
        int Rank { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Vec3d ClosestPoint(Vec3d point);
    }
}
