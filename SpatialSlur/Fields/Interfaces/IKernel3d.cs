
/*
 * Notes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur;

namespace SpatialSlur.Fields
{
	/// <summary>
    /// 
    /// </summary>
    public interface IKernel3d : IEnumerable<(Vector3i Offset, double Weight)>
    {
    }
}
