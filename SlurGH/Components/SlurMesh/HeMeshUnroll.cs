using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

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


        /// <inheritdoc />
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "heMesh", "heMesh", "Mesh to unroll", GH_ParamAccess.item);
            pManager.AddIntegerParameter("startFace", "start", "Face to start unrolling from", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("unrollFactor", "factor", "Per-edge unroll factors", GH_ParamAccess.list, 1.0);

            pManager[2].Optional = true;
        }


        /// <inheritdoc />
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new HeMesh3dParam(), "result", "result", "Unrolled mesh", GH_ParamAccess.item);
        }


        /// <inheritdoc />
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_HeMesh3d hemGoo = null;
            if (!DA.GetData(0, ref hemGoo)) return;

            int start = -1;
            if (!DA.GetData(1, ref start)) return;

            List<double> factors = new List<double>();
            DA.GetDataList(2, factors);

            var mesh = new HeMesh3d(hemGoo.Value);
            var f = mesh.Faces[start];

            // ensures mesh is unrollable
            HeMeshUnroller.DetachFaceCycles(mesh, f);

            // perform unroll
            var unrolled = new Vec3d[mesh.Vertices.Count];
            var last = factors.Count - 1;
            HeMeshUnroller.Unroll(mesh, f, (v, p) => unrolled[v] = p, he => factors[Math.Min(he >> 1, last)]);

            // set vertex attributes
            var fn = f.GetNormal(v => v.Position);
            mesh.Vertices.Action(v =>
            {
                v.Position = unrolled[v];
                v.Normal = fn;
            });

            DA.SetData(0, new GH_HeMesh3d(mesh));
        }


        /// <inheritdoc />
        protected override System.Drawing.Bitmap Icon
        {
            get { return null; }
        }


        /// <inheritdoc />
        public override Guid ComponentGuid
        {
            get { return new Guid("fcef3326-58b4-4fd2-99c4-17ebe0dbbbe6"); }
        }
    }
}