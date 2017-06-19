using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVertex3d
    {
        /// <summary></summary>
        Vec3d Position { get; set; }

        /// <summary></summary>
        Vec3d Normal { get; set; }

        /// <summary></summary>
        Vec2d TexCoord { get; set; }
    }
}
