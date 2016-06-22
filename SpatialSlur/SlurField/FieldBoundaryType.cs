using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpatialSlur.SlurField
{
    /// <summary>
    /// Enum of field boundary conditions.
    /// </summary>
    public enum FieldBoundaryType
    {
        /// <summary>Assumes a constant value beyond the field domain.</summary>
        Constant,
        /// <summary>Assumes values beyond the field domain are equal to the closest value in the field.</summary>
        Equal,
        /// <summary>Assumes field repeats in all directions.</summary>
        Periodic
    }
}
