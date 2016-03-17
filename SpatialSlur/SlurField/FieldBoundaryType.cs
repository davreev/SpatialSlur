using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Enum of field boundary conditions.
    /// Constant - assumes constant value at the boundary
    /// Equal - assumes equal value at boundary
    /// Periodic - no boundary
    /// </summary>
    public enum FieldBoundaryType
    {
        Constant,
        Equal,
        Periodic
    }
}
