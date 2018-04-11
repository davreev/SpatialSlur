using System;

using Grasshopper.Kernel;

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


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "result", "result", "", GH_ParamAccess.item);
        }


        /// <inheritdoc />
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
                foreach (var he1 in he0.Circulate)
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
                        var he2 = he1.Next;
                        int j = (i + 2) % n;

                        mesh.AddFace(he1.Start, i + 1 + nv, i + nv);
                        mesh.AddFace(he1.Start, he2.Start, j + nv, i + 1 + nv);
                        he1 = he2;
                    }
                }
            }
        }
        */


        /// <inheritdoc />
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        /// <inheritdoc />
        public override Guid ComponentGuid
        {
            get { return new Guid("16754bb3-1170-4361-be24-bc6ee85b5268"); }
        }
    }
}