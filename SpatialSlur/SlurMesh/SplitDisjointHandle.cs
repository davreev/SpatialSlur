using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public class SplitDisjointHandle
    {
        /// <summary>
        /// The index of the component to which the corresponding element belongs.
        /// </summary>
        public int ComponentIndex;


        /// <summary>
        /// The index of the corresponding element within the component.
        /// </summary>
        public int ElementIndex;
    }
}
