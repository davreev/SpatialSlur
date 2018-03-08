using System.Linq;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using SpatialSlur.SlurRhino;

/*
 * Notes 
 */

namespace SpatialSlur.SlurGH.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class GH_HeGraph3d : GH_GeometricGoo<HeGraph3d>
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public static implicit operator HeGraph3d(GH_HeGraph3d graph)
        {
            return graph.Value;
        }

        #endregion


        private BoundingBox _bbox = BoundingBox.Unset;


        /// <summary>
        /// 
        /// </summary>
        public GH_HeGraph3d()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public GH_HeGraph3d(HeGraph3d value)
        {
            Value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public GH_HeGraph3d(GH_HeGraph3d other)
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
            get { return "HeGraph"; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override string TypeDescription
        {
            get { return "HeGraph"; }
        }


        /// <summary>
        /// 
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get
            {
                if (Value != null && !_bbox.IsValid)
                    _bbox = new Interval3d(Value.Vertices.Select(v => v.Position)).ToBoundingBox();

                return _bbox;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {
            return DuplicateGeometry();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return new GH_HeGraph3d(new HeGraph3d(Value));
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
        /// <typeparam name="Q"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q).IsAssignableFrom(typeof(HeGraph3d)))
            {
                object obj = Value;
                target = (Q)obj;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_ObjectWrapper)))
            {
                object obj = new GH_ObjectWrapper(Value);
                target = (Q)obj;
                return true;
            }

            return false;
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override bool CastFrom(object source)
        {
            if (source is HeGraph3d g)
            {
                Value = g;
                return true;
            }

            if (source is HeMesh3d hem)
            {
                Value = HeGraph3d.Factory.CreateFromVertexTopology(hem);
                return true;
            }

            if (source is Mesh m)
            {
                Value = m.ToHeGraph();
                return true;
            }

            if (source is GH_HeMesh3d ghem)
            {
                Value = HeGraph3d.Factory.CreateFromVertexTopology(ghem.Value);
                return true;
            }

            if (source is GH_Mesh gm)
            {
                Value = gm.Value.ToHeGraph();
                return true;
            }
            
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xform"></param>
        /// <returns></returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            var b = Boundingbox;
            b.Transform(xform);
            return b;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xform"></param>
        /// <returns></returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            Value.Transform(xform);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmorph"></param>
        /// <returns></returns>
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            Value.SpaceMorph(xmorph);
            return this;
        }
    }
}