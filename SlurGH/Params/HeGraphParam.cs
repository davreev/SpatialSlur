using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.Kernel;

using SpatialSlur.SlurGH.Types;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Params
{
    /// <summary>
    /// 
    /// </summary>
    public class HeGraphParam : GH_PersistentParam<GH_HeGraph>
    {
        /// <summary>
        /// 
        /// </summary>
        public HeGraphParam()
            :base("HeGraph", "HeGraph", "Contains a collection of halfedge graphs.", "SpatialSlur", "Parameters")
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override Bitmap Icon
        {
            get { return null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{E173CB1B-FBE2-4F71-89A2-D520C6AF6A7F}"); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_HeGraph value)
        {
            value = new GH_HeGraph();
            return GH_GetterResult.success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_HeGraph> values)
        {
            values = new List<GH_HeGraph>();
            return GH_GetterResult.success;
        }
    }
}
