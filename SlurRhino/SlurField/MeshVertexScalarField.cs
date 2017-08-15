using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurField;
using SpatialSlur.SlurMesh;

using Rhino.Geometry;

/*
 * Notes
 */

namespace SpatialSlur.SlurRhino
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshVertexScalarField : MeshVertexField<double>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public MeshVertexScalarField(HeMesh3d mesh)
            : base(mesh)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public MeshVertexScalarField(MeshVertexField other)
            : base(other)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MeshVertexScalarField Duplicate()
        {
            var copy = new MeshVertexScalarField(this);
            copy.Set(this);
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override MeshVertexField<double> DuplicateBase()
        {
            return Duplicate();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public override double ValueAt(int i0, int i1, int i2, double w0, double w1, double w2)
        {
            return Values[i0] * w0 + Values[i1] * w1 + Values[i2] * w2;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVertexScalarField GetLaplacian(bool parallel = true)
        {
            var result = new MeshVertexScalarField(this);
            GetLaplacian(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IDiscreteField<double> result, bool parallel = true)
        {
            GetLaplacian(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(double[] result, bool parallel = true)
        {
            var verts = Mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v0 = verts[i];
                    if (v0.IsRemoved) continue;

                    var t = Values[i];
                    var sum = 0.0;
                    int count = 0;

                    foreach (var v1 in v0.ConnectedVertices)
                    {
                        sum += Values[v1] - t;
                        count++;
                    }

                    result[i] = sum / count;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVertexScalarField GetLaplacian(Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            var result = new MeshVertexScalarField(this);
            GetLaplacian(result.Values, getWeight, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(IDiscreteField<double> result, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            GetLaplacian(result.Values, getWeight, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetLaplacian(double[] result, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            var verts = Mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = Values[i];
                    var sum = 0.0;

                    foreach (var he in v.OutgoingHalfedges)
                        sum += (Values[he.End] - t) * getWeight(he);

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public MeshVertexVectorField GetGradient(bool parallel = true)
        {
            var result = new MeshVertexVectorField(this);
            GetGradient(result.Values, parallel);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(IDiscreteField<Vec3d> result, bool parallel = true)
        {
            GetGradient(result.Values, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(Vec3d[] result, bool parallel = true)
        {
            // TODO
            // revise implementation as per
            // http://libigl.github.io/libigl/tutorial/tutorial.html

            // delta / distance * unit direction
            var verts = Mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = Values[i];
                    var sum = new Vec3d();
                    int count = 0;

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var v0 = he.Start;
                        var v1 = he.End;

                        var d = v1.Position - v0.Position;
                        sum += d * ((Values[v1] - t) / d.SquareLength);
                        count++;
                    }

                    result[i] = sum / count;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parallel"></param>
        public void GetGradient(Vec3d[] result, Func<HeMesh3d.Halfedge, double> getWeight, bool parallel = true)
        {
            // TODO
            // revise implementation as per
            // http://libigl.github.io/libigl/tutorial/tutorial.html

            // delta / distance * unit direction
            var verts = Mesh.Vertices;

            Action<Tuple<int, int>> body = range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    var v = verts[i];
                    if (v.IsRemoved) continue;

                    var t = Values[i];
                    var sum = new Vec3d();

                    foreach (var he in v.OutgoingHalfedges)
                    {
                        var v0 = he.Start;
                        var v1 = he.End;

                        var d = v1.Position - v0.Position;
                        var dt = (Values[v1] - t) * getWeight(he);
                        sum += d * (dt / d.SquareLength);
                    }

                    result[i] = sum;
                }
            };

            if (parallel)
                Parallel.ForEach(Partitioner.Create(0, Count), body);
            else
                body(Tuple.Create(0, Count));
        }
    }
}
