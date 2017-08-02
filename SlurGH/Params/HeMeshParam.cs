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
    public class HeMeshParam : GH_PersistentParam<GH_HeMesh>
    {
        /// <summary>
        /// 
        /// </summary>
        public HeMeshParam()
            :base("HeMesh", "HeMesh", "Contains a collection of halfedge meshes.", "SpatialSlur", "Parameters")
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
            get { return new Guid("{AEEA983D-0B68-44F0-B908-C5B3FCB09C13}"); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_HeMesh value)
        {
            value = new GH_HeMesh();
            return GH_GetterResult.success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_HeMesh> values)
        {
            values = new List<GH_HeMesh>();
            return GH_GetterResult.success;
        }
    }
}
