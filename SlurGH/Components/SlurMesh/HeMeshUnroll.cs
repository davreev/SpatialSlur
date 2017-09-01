using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

using SpatialSlur.SlurRhino;

using SpatialSlur.SlurGH.Types;
using SpatialSlur.SlurGH.Params;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurGH.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class HeMeshUnroll : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MeshUnroll class.
        /// </summary>
        public HeMeshUnroll()
          : base("HeMesh Unroll", "Unroll",
              "Unrolls a halfedge mesh",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "Mesh to unroll", GH_ParamAccess.item);
            pManager.AddIntegerParameter("startFace", "start", "Face to start unrolling from", GH_ParamAccess.item, 0);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("result", "result", "Unrolled mesh", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_HeMesh3d hemGoo = null;
            int start = -1;

            if (!DA.GetData(0, ref hemGoo)) return;
            if (!DA.GetData(1, ref start)) return;

            var mesh = hemGoo.Value.Duplicate();
            var f = mesh.Faces[start];

            // unroll
            HeMeshUnroller.Unroll(mesh, mesh.Faces[start]);

            // set normals
            var fn = f.GetNormal(v => v.Position);
            mesh.Vertices.Action(v => v.Normal = fn);

            DA.SetData(0, new GH_HeMesh3d(mesh));
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }


        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fcef3326-58b4-4fd2-99c4-17ebe0dbbbe6"); }
        }
    }
}