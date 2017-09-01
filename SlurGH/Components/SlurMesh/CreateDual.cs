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
    public class CreateDual : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetDual class.
        /// </summary>
        public CreateDual()
          : base("Create Dual", "Dual",
              "Returns the dual of a given halfedge mesh",
              "SpatialSlur", "Mesh")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "result", "result", "", GH_ParamAccess.item);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_HeMesh3d hemGoo = null;
            if (!DA.GetData(0, ref hemGoo)) return;

            DA.SetData(0, new GH_HeMesh3d(hemGoo.Value.GetDual()));
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        private void AddBoundaryFaces(HeMesh3d mesh)
        {
            var holes = mesh.GetHoles().ToList();
            var verts = mesh.Vertices;

            foreach (var he0 in holes)
            {
                int nv = mesh.Vertices.Count;
                int n = 0;

                // add verts
                foreach (var he1 in he0.CirculateFace)
                {
                    var p = he1.Start.Position;
                    mesh.AddVertex().Position = p;
                    mesh.AddVertex().Position = p;
                    n += 2;
                }

                // add faces
                {
                    var he1 = he0;

                    for (int i = 0; i < n; i += 2)
                    {
                        var he2 = he1.NextInFace;
                        int j = (i + 2) % n;

                        mesh.AddFace(he1.Start, i + 1 + nv, i + nv);
                        mesh.AddFace(he1.Start, he2.Start, j + nv, i + 1 + nv);
                        he1 = he2;
                    }
                }
            }
        }
        */


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
            get { return new Guid("16754bb3-1170-4361-be24-bc6ee85b5268"); }
        }
    }
}