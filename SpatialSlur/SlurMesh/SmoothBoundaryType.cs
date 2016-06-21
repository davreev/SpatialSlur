using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// Enum of boundary types for smoothing.
    /// </summary>
    public enum SmoothBoundaryType
    {
        /// <summary>All boundary vertices are fixed.</summary>
        Fixed,
        /// <summary>Only degree 2 boundary vertices are fixed.</summary>
        CornerFixed,
        /// <summary>No vertices are fixed.</summary>
        Free
    }
}
