using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurRhino;

/*
 * Notes
 */

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class NormalShader : GH_Component
    {
        /// <summary>
        /// 
        /// </summary>
        public NormalShader()
          : base("Normal Shader", "NormShade",
              "Paints a Mesh based on vertex normal directions.",
              "SpatialSlur", "Display")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh to paint", GH_ParamAccess.item);
            pManager.AddColourParameter("colors", "colors", "", GH_ParamAccess.list);
            pManager.AddVectorParameter("direction", "dir", "", GH_ParamAccess.item, Vector3d.ZAxis);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Painted mesh", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            List<Color> colors = new List<Color>();
            Vector3d dir = new Vector3d();

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetDataList(1, colors)) return;
            if (!DA.GetData(2, ref dir)) return;

            var norms = mesh.Normals;

            if (norms.Count != mesh.Vertices.Count)
                norms.ComputeNormals();
            
            mesh.ColorVertices(i => colors.Lerp(dir * norms[i] * 0.5 + 0.5), true);

            DA.SetData(0, new GH_Mesh(mesh));
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{0A8E67CC-0EC0-46BA-93A0-9019B8804DF1}"); }
        }
    }
}

