#if USING_UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVertex3f
    {
        /// <summary></summary>
        Vector3 Position { get; set; }

        /// <summary></summary>
        Vector3 Normal { get; set; }

        /// <summary></summary>
        Vector2 Texture { get; set; }
    }
}

#endif