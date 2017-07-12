using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SpatialSlur.SlurRhino;
using SpatialSlur.SlurField;
using Rhino.Geometry;

/*
 * Notes 
 */

namespace SpatialSlur.SlurGH.Grasshopper
{
    /// <summary>
    /// 
    /// </summary>
    public class GH_GridScalarField2d : GH_Goo<GridScalarField2d>
    {
        /// <summary>
        /// 
        /// </summary>
        public GH_GridScalarField2d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public GH_GridScalarField2d(GridScalarField2d value)
        {
            Value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GH_GridScalarField2d(GH_GridScalarField2d other)
        {
            Value = other.Value;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsValid
        {
            get { return true; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string TypeName
        {
            get { return "GridScalarField2d"; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string TypeDescription
        {
            get { return "GridScalarField2d"; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {
            return new GH_GridScalarField2d(Value.Duplicate());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object ScriptVariable()
        {
            return Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override bool CastFrom(object source)
        {
            if (source is GH_Goo<GridScalarField2d>)
            {
                Value = ((GH_Goo<GridScalarField2d>)source).Value;
                return true;
            }

            if (source is GridScalarField2d)
            {
                Value = (GridScalarField2d)source;
                return true;
            }

            if (source is GH_Goo<GridField2d>)
            {
                Value = new GridScalarField2d(((GH_Goo<GridField2d>)source).Value);
                return true;
            }

            if (source is GridField2d)
            {
                Value = new GridScalarField2d((GridField2d)source);
                return true;
            }

            return false;
        }
    }
}
