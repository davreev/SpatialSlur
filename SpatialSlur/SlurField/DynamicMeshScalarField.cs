using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicMeshScalarField : MeshScalarField
    {
        private readonly double[] _deltas;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        public DynamicMeshScalarField(HeMesh mesh)
            : base(mesh)
        {
            _deltas = new double[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        public DynamicMeshScalarField(MeshField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _deltas = new double[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        public DynamicMeshScalarField(MeshScalarField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _deltas = new double[Count];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="duplicateMesh"></param>
        public DynamicMeshScalarField(DynamicMeshScalarField other, bool duplicateMesh = false)
            : base(other, duplicateMesh)
        {
            _deltas = new double[Count];
            _deltas.Set(other._deltas);
        }


        /// <summary>
        /// 
        /// </summary>
        public double[] Deltas
        {
            get { return _deltas; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField Duplicate()
        {
            return new DynamicMeshScalarField(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override MeshField DuplicateDeep()
        {
            return new DynamicMeshScalarField(this, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStep"></param>
        public void Update(double timeStep)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Values[i] += _deltas[i] * timeStep;
                    _deltas[i] = 0.0;
                }
            });
        }


        /// <summary>
        /// Uses a normalized umbrella weighting scheme (Tutte scheme)
        /// http://www.cs.princeton.edu/courses/archive/fall10/cos526/papers/sorkine05.pdf
        /// http://www.igl.ethz.ch/projects/Laplacian-mesh-processing/Laplacian-mesh-optimization/lmo.pdf
        /// </summary>
        /// <param name="rate"></param>
        public void Diffuse(double rate)
        {
            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double sum = 0.0;
                    int n = 0;

                    foreach (HalfEdge e in verts[i].IncomingHalfEdges)
                    {
                        sum += Values[e.Start.Index];
                        n++;
                    }

                    _deltas[i] += (sum / n - Values[i]) * rate;
                }
            });
        }


        /// <summary>
        /// Uses a user defined weighting scheme
        /// http://www.cs.princeton.edu/courses/archive/fall10/cos526/papers/sorkine05.pdf
        /// http://www.igl.ethz.ch/projects/Laplacian-mesh-processing/Laplacian-mesh-optimization/lmo.pdf
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="halfEdgeWeights"></param>
        public void Diffuse(double rate, IList<double> halfEdgeWeights)
        {
            Mesh.HalfEdges.SizeCheck(halfEdgeWeights);

            HeVertexList verts = Mesh.Vertices;
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    double value = Values[i];
                    double sum = 0.0;

                    foreach (HalfEdge e in verts[i].OutgoingHalfEdges)
                        sum += (Values[e.End.Index] - value) * halfEdgeWeights[e.Index];

                    _deltas[i] += sum * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void Deposit(double amount)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] += amount;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void Deposit(double target, double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] += (target - Values[i]) * rate;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rate"></param>
        public void Decay(double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    _deltas[i] -= Values[i] * rate;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thresh"></param>
        /// <param name="rate"></param>
        public void Bifurcate(double thresh, double rate)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (Values[i] > thresh)
                        _deltas[i] += (1.0 - Values[i]) * rate;
                    else if (Values[i] < thresh)
                        _deltas[i] -= Values[i] * rate;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="amount"></param>
        public void DepositAt(int index, double amount)
        {
            _deltas[index] += amount;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="amount"></param>
        public void DepositAt(MeshPoint point, double amount)
        {
            MeshFace face = DisplayMesh.Faces[point.FaceIndex];
            int count = (face.IsQuad) ? 4 : 3;

            for (int i = 0; i < count; i++)
                _deltas[face[i]] += amount * point.T[i];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void DepositAt(int index, double target, double rate)
        {
            DepositAt(index, (target - Values[index]) * rate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <param name="rate"></param>
        public void DepositAt(MeshPoint point, double target, double rate)
        {
            DepositAt(point, (target - Evaluate(point)) * rate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rate"></param>
        public void DecayAt(int index, double rate)
        {
            _deltas[index] -= Values[index] * rate;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rate"></param>
        public void DecayAt(MeshPoint point, double rate)
        {
            DepositAt(point, -Evaluate(point) * rate);
        }
    }
}
