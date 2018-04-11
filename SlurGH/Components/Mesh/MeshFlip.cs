
/*
 * Notes
 */

using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshFlip : GH_Component
    {
        private bool _vertNorms = true;
        private bool _faceNorms = true;
        private bool _faceOrient = true;


        /// <summary>
        /// 
        /// </summary>
        public MeshFlip()
          : base("Mesh Flip", "Flip",
              "Reverses the face windings of a mesh",
              "Mesh", "Util")
        {
        }


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Mesh to flip.", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("mesh", "mesh", "Flipped mesh", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            if (!DA.GetData(0, ref mesh)) return;

            mesh.Flip(_vertNorms, _faceNorms, _faceOrient);

            DA.SetData(0, new GH_Mesh(mesh));
        }


        /// <inheritdoc />
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Vertex Normals", VertexNormalsClicked, true, _vertNorms);
            Menu_AppendItem(menu, "Face Normals", FaceNormalsClicked, true, _faceNorms);
            Menu_AppendItem(menu, "Face Orientation", FaceOrientClicked, true, _faceOrient);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VertexNormalsClicked(object sender, EventArgs e)
        {
            _vertNorms = !_vertNorms;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceNormalsClicked(object sender, EventArgs e)
        {
            _faceNorms = !_faceNorms;
            ExpireSolution(true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceOrientClicked(object sender, EventArgs e)
        {
            _faceOrient = !_faceOrient;
            ExpireSolution(true);
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
            get { return new Guid("{1168132C-A1B4-45BA-9265-D61ACE445A1E}"); }
        }
    }
}
