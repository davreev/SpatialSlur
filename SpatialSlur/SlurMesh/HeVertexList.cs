using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public class HeVertexList:HeElementList<HeVertex>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        internal HeVertexList(HeMesh mesh)
            : base(mesh)
        {
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="element"></param>
        public HeVertex Add(double x, double y, double z)
        {
            HeVertex result = new HeVertex(x, y, z);
            Add(result);
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="element"></param>
        public HeVertex Add(Vec3d position)
        {
            HeVertex result = new HeVertex(position);
            Add(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vec3d delta)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    List[i].Position += delta;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public void TranslateEach(IList<Vec3d> deltas)
        {
            SizeCheck(deltas);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    List[i].Position += deltas[i];
            });
        }


        /// <summary>
        /// Counts the number of vertices connected to both given vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonNeighbours(HeVertex v0, HeVertex v1)
        {
            Validate(v0);
            Validate(v1);

            List<int> indices = new List<int>();

            // flag vertices around v0 by setting their index to a unique integer
            foreach (HeVertex v in v0.ConnectedVertices)
            {
                indices.Add(v.Index); // cache vertex indices for reset
                v.Index = -2;
            }

            // count flagged vertices around v1
            int count = 0;
            foreach (HeVertex v in v1.ConnectedVertices)
                if (v.Index == -2) count++;

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Vertices[i].Index = i;

            return count;
        }


        /// <summary>
        /// Returns all vertices connected to both given vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public IList<HeVertex> GetCommonNeighbours(HeVertex v0, HeVertex v1)
        {
            Validate(v0);
            Validate(v1);

            List<HeVertex> result = new List<HeVertex>();
            List<int> indices = new List<int>();

            // flag vertices around v0 by setting their index to a unique integer
            foreach (HeVertex v in v0.ConnectedVertices)
            {
                indices.Add(v.Index); // cache vertex indices for reset
                v.Index = -2;
            }

            // cache flagged vertices around v1
            foreach (HeVertex v in v1.ConnectedVertices)
                if (v.Index == -2) result.Add(v);

            // reset indices of flagged vertices
            foreach (int i in indices)
                Mesh.Vertices[i].Index = i;

            return result;
        }


        /// <summary>
        /// Counts the number of faces that surround both given vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public int CountCommonFaces(HeVertex v0, HeVertex v1)
        {
            Validate(v0);
            Validate(v1);

            List<int> indices = new List<int>();

            // flag faces around v0 by setting their index to a unique integer
            foreach (HeFace f in v0.SurroundingFaces)
            {
                indices.Add(f.Index); // cache vertex indices for reset
                f.Index = -2;
            }

            // count flagged faces around v1
            int count = 0;
            foreach (HeFace f in v1.SurroundingFaces)
                if (f.Index == -2) count++;

            // reset indices of flagged faces
            foreach (int i in indices)
                Mesh.Faces[i].Index = i;

            return count;
        }


        /// <summary>
        /// Returns all faces that surround both given vertices.
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public IList<HeFace> GetCommonFaces(HeVertex v0, HeVertex v1)
        {
            Validate(v0);
            Validate(v1);

            List<HeFace> result = new List<HeFace>();
            List<int> indices = new List<int>();

            // flag faces around v0 by setting their index to a unique integer
            foreach (HeFace f in v0.SurroundingFaces)
            {
                indices.Add(f.Index); // cache vertex indices for reset
                f.Index = -2;
            }

            // cache flagged faces around v1
            foreach (HeFace f in v1.SurroundingFaces)
                if (f.Index == -2) result.Add(f);

            // reset indices of flagged faces
            foreach (int i in indices)
                Mesh.Faces[i].Index = i;

            return result;
        }


        #region Element Attributes
      
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetVertexDegrees()
        {
            int[] result = new int[Count];
            UpdateVertexDegrees(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateVertexDegrees(IList<int> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = List[i].GetDegree();
            });
        }


        /// <summary>
        /// Returns the topological depth of all vertices connected to a set of sources.
        /// </summary>
        /// <returns></returns>
        public int[] GetVertexDepths(IList<int> sources)
        {
            int[] result = new int[Count];
            UpdateVertexDepths(sources, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateVertexDepths(IList<int> sources, IList<int> result)
        {
            SizeCheck(result);

            Queue<int> queue = new Queue<int>();
            result.Set(Int32.MaxValue);

            // enqueue sources and set to zero
            foreach (int i in sources)
            {
                queue.Enqueue(i);
                result[i] = 0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                int t0 = result[i0] + 1;

                foreach (HeVertex v in List[i0].ConnectedVertices)
                {
                    int i1 = v.Index;

                    if (t0 < result[i1])
                    {
                        result[i1] = t0;
                        queue.Enqueue(i1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the topological distance of all vertices connected to a set of sources.
        /// </summary>
        /// <returns></returns>
        public double[] GetVertexDepths(IList<int> sources, IList<double> edgeLengths)
        {
            double[] result = new double[Count];
            UpdateVertexDepths(sources, edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateVertexDepths(IList<int> sources, IList<double> edgeLengths, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Edges.SizeCheck(edgeLengths);

            Queue<int> queue = new Queue<int>();
            result.Set(Double.PositiveInfinity);

            // enqueue sources and set to zero
            foreach (int i in sources)
            {
                queue.Enqueue(i);
                result[i] = 0.0;
            }

            // breadth first search from sources
            while (queue.Count > 0)
            {
                int i0 = queue.Dequeue();
                double t0 = result[i0];

                foreach (HeEdge e in List[i0].IncomingEdges)
                {
                    int i1 = e.Start.Index;
                    double t1 = t0 + edgeLengths[e.Index];

                    if (t1 < result[i1])
                    {
                        result[i1] = t1;
                        queue.Enqueue(i1);
                    }
                }
            }
        }


        /// <summary>
        /// Returns the morse smale classification for each vertex
        /// 0 = normal
        /// 1 = minima
        /// 2 = maxima
        /// 3 = saddle
        /// </summary>
        /// <param name="values"></param>
        public int[] GetMorseSmaleClassification(IList<double> values)
        {
            int[] result = new int[Count];
            UpdateMorseSmaleClassification(values, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public void UpdateMorseSmaleClassification(IList<double> values, IList<int> result)
        {
            SizeCheck(values);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v0 = List[i];
                    double t0 = values[i];

                    // check first neighbour
                    HeEdge first = v0.Outgoing.Twin;
                    double t1 = values[first.Start.Index];

                    bool last = (t1 < t0); // was the last neighbour lower?
                    int count = 0; // number of discontinuities
                    first = first.Next.Twin;

                    // circulate remaining neighbours
                    HeEdge e = first;
                    do
                    {
                        t1 = values[e.Start.Index];

                        if (t1 < t0)
                        {
                            if (!last) count++;
                            last = true;
                        }
                        else
                        {
                            if (last) count++;
                            last = false;
                        }

                        e = e.Next.Twin;
                    } while (e != first);

                    // classify current vertex based on number of discontinuities
                    switch (count)
                    {
                        case 0:
                            result[i] = (last) ? 2 : 1;
                            break;
                        case 2:
                            result[i] = 0;
                            break;
                        default:
                            result[i] = 3;
                            break;
                    }
                }
            });
        }


      

        /// <summary>
        /// Returns the area associated with each vertex.
        /// </summary>
        /// <param name="edgeAreas"></param>
        /// <returns></returns>
        public double[] GetVertexAreas(IList<double> edgeAreas)
        {
            double[] result = new double[Count];
            UpdateVertexAreas(edgeAreas, result);
            return result;
        }

        
        /// <summary>
        ///
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <returns></returns>
        public double[] GetVertexAreas(IList<Vec3d> faceCenters)
        {
            double[] result = new double[Count];
            UpdateVertexAreas(faceCenters, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateVertexAreas(IList<double> edgeAreas, IList<double> result)
        {
            SizeCheck(result);

            HeEdgeList edges = Mesh.Edges;
            edges.SizeCheck(edgeAreas);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    double sum = 0.0;
                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;
                        sum += edgeAreas[e.Index];
                    }

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateVertexAreas(IList<Vec3d> faceCenters, IList<double> result)
        {
            SizeCheck(result);

            HeFaceList faces = Mesh.Faces;
            faces.SizeCheck(faceCenters);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    double sum = 0.0;

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;

                        Vec3d v0 = e.Span * 0.5;
                        Vec3d v1 = faceCenters[e.Face.Index] - e.Start.Position;
                        Vec3d v2 = e.Prev.Twin.Span * 0.5;

                        sum += (Vec3d.Cross(v0, v1).Length + Vec3d.Cross(v1, v2).Length) * 0.5;
                    }

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// Calculates the areas associated with each vertex.
        /// Note that this method assumes all faces are triangular.
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <returns></returns>
        public double[] GetVertexAreasTri()
        {
            double[] result = new double[Count];
            UpdateVertexAreasTri(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="faceCenters"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public void UpdateVertexAreasTri(IList<double> result)
        {
            SizeCheck(result);

            HeFaceList faces = Mesh.Faces;
            double t = 0.5 / 3.0;

            for (int i = 0; i < faces.Count; i++)
            {
                HeFace f = faces[i];
                if (f.IsUnused) continue;

                double a = Vec3d.Cross(f.First.Span, f.First.Next.Span).Length * t;
                foreach (HeVertex v in f.Vertices)
                    result[v.Index] += a;
            }
        }


        /// <summary>
        /// Returns a radii for each vertex.
        /// If the mesh is a circle packing mesh, these will be the radii of tangent spheres centered on each vertex.
        /// http://www.geometrie.tuwien.ac.at/hoebinger/mhoebinger_files/circlepackings.pdf
        /// </summary>
        public double[] GetVertexRadii(IList<double> edgeLengths)
        {
            double[] result = new double[Count];
            UpdateVertexRadii(edgeLengths, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeLengths"></param>
        /// <param name="result"></param>
        public void UpdateVertexRadii(IList<double> edgeLengths, IList<double> result)
        {
            Mesh.Edges.SizeCheck(edgeLengths);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue; // skip unused vertices

                    double sum = 0.0;
                    int n = 0;

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue; // skip boundary edges
                        sum += (edgeLengths[e.Index] + edgeLengths[e.Prev.Index] - edgeLengths[e.Next.Index]) * 0.5;
                        n++;
                    }

                    result[v.Index] = sum / n;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double[] GetGaussianCurvature()
        {
            double[] result = new double[Count];
            UpdateGaussianCurvature(result);
            return result;
        }


        /// <summary>
        /// Returns the gaussian curvature at each vertex.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public double[] GetGaussianCurvature(IList<double> edgeAngles)
        {
            double[] result = new double[Count];
            UpdateGaussianCurvature(edgeAngles, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void UpdateGaussianCurvature(IList<double> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    double sum = 0.0;
                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += Vec3d.Angle(e.Span, e.Prev.Twin.Span);

                    result[i] = Math.Abs(sum - SlurMath.PI2);
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void UpdateGaussianCurvature(IList<double> edgeAngles, IList<double> result)
        {
            Mesh.Edges.SizeCheck(edgeAngles);
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused || v.IsBoundary) continue;

                    double sum = 0.0;
                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += edgeAngles[e.Index];

                    result[i] = Math.Abs(sum - SlurMath.PI2);
                }
            });
        }


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] GetVertexDiagonalLengths()
        {
            double[] result = new double[Mesh.Edges.Count];
            UpdateVertexDiagonalLengths(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void UpdateVertexDiagonalLengths(IList<double> result)
        {
            Mesh.Edges.SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;
                    int d = v.GetDegree();

                    if (d > 4)
                    {
                        // general valence n case
                        HeEdge e0 = v.Outgoing;
                        HeEdge e1 = e0.Twin.Next.Twin.Next;

                        do
                        {
                            result[e0.Index] = e0.End.VectorTo(e1.End).Length;
                            e0 = e0.Twin.Next;
                            e1 = e1.Twin.Next;
                        } while (e0 != v.Outgoing);
                    }
                    else if (d == 4)
                    {
                        // simplified valence 4 case
                        HeEdge e0 = v.Outgoing;
                        HeEdge e1 = e0.Twin.Next.Twin.Next;

                        for (int j = 0; j < 2; j++)
                        {
                            result[e0.Index] = e0.End.VectorTo(e1.End).Length;
                            e0 = e0.Twin.Next;
                            e1 = e1.Twin.Next;
                        }
                    }
                }
            });
        }
        */


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexPositions()
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateVertexPositions(result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void UpdateVertexPositions(IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                    result[i] = List[i].Position;
            });
        }


        /// <summary>
        /// Calculates the vertex normal as the average of edge normals around each vertex.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexNormals(bool unitize)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateVertexNormals(unitize, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexNormals(IList<Vec3d> edgeNormals, bool unitize)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateVertexNormals(edgeNormals, unitize, result); 
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void UpdateVertexNormals(bool unitize, IList<Vec3d> result)
        {
            SizeCheck(result);

            if (unitize)
                UpdateUnitVertexNormals(result);
            else
                UpdateVertexNormals(result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void UpdateVertexNormals(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;
                        sum += Vec3d.Cross(e.Prev.Span, e.Span);
                        n++;
                    }

                    result[i] = sum / n;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void UpdateUnitVertexNormals(IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();
                 
                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;
                        sum += Vec3d.Cross(e.Prev.Span, e.Span);
                    }

                    result[i] = sum / sum.Length;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void UpdateVertexNormals(IList<Vec3d> edgeNormals, bool unitize, IList<Vec3d> result)
        {
            Mesh.Edges.SizeCheck(edgeNormals);
            SizeCheck(result);

            if (unitize)
                UpdateUnitVertexNormals(edgeNormals, result);
            else
                UpdateVertexNormals(edgeNormals, result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void UpdateVertexNormals(IList<Vec3d> edgeNormals, IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;
                        sum += edgeNormals[e.Index];
                        n++;
                    }

                    result[i] = sum / n;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private void UpdateUnitVertexNormals(IList<Vec3d> edgeNormals, IList<Vec3d> result)
        {
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    foreach (HeEdge e in v.OutgoingEdges)
                    {
                        if (e.Face == null) continue;
                        sum += edgeNormals[e.Index];
                    }

                    result[i] = sum / sum.Length;
                }
            });
        }


        /// <summary>
        /// Calculates the Laplacian using a normalized umbrella weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexLaplacians()
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateVertexLaplacians(result);
            return result;
        }


        /// <summary>
        /// Calculates the Laplacian using a custom weighting scheme.
        /// https://www.informatik.hu-berlin.de/forschung/gebiete/viscom/thesis/final/Diplomarbeit_Herholz_201301.pdf
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <returns></returns>
        public Vec3d[] GetVertexLaplacians(IList<double> edgeWeights)
        {
            Vec3d[] result = new Vec3d[Count];
            UpdateVertexLaplacians(edgeWeights, result);
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateVertexLaplacians(IList<Vec3d> result)
        {
            SizeCheck(result);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();
                    int n = 0;

                    foreach (HeEdge e in v.IncomingEdges)
                    {
                        sum += e.Start.Position;
                        n++;
                    }

                    result[i] = sum / n - v.Position;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateVertexLaplacians(IList<double> edgeWeights, IList<Vec3d> result)
        {
            SizeCheck(result);
            Mesh.Edges.SizeCheck(edgeWeights);
         
            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d sum = new Vec3d();

                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += e.Span * edgeWeights[e.Index];

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateVertexLaplacians(IList<double> values, IList<double> edgeWeights, IList<double> result)
        {
            SizeCheck(result);
            Mesh.Edges.SizeCheck(edgeWeights);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    double t = values[i];
                    double sum = 0.0;

                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += (values[e.End.Index] - t) * edgeWeights[e.Index];

                    result[i] = sum;
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeWeights"></param>
        /// <param name="result"></param>
        public void UpdateVertexLaplacians(IList<Vec3d> values, IList<double> edgeWeights, IList<Vec3d> result)
        {
            SizeCheck(result);
            Mesh.Edges.SizeCheck(edgeWeights);

            Parallel.ForEach(Partitioner.Create(0, Count), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    HeVertex v = List[i];
                    if (v.IsUnused) continue;

                    Vec3d t = values[i];
                    Vec3d sum = new Vec3d();

                    foreach (HeEdge e in v.OutgoingEdges)
                        sum += (values[e.End.Index] - t) * edgeWeights[e.Index];

                    result[i] = sum;
                }
            });
        }


        #endregion


        #region Euler Operators

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Remove(HeVertex vertex)
        {
            Validate(vertex);

            // simplified process for interior degree 2 vertices
            if (vertex.IsDeg2 && !vertex.IsBoundary)
                RemoveSimple(vertex);

            HeFaceList faces = Mesh.Faces;
            foreach (HeFace f in vertex.SurroundingFaces)
                faces.Remove(f);

            return true;
        }


        /// <summary>
        /// Simplified removal method for interior degree 2 verts.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private void RemoveSimple(HeVertex vertex)
        {
            HeEdge e0 = vertex.Outgoing;
            HeEdge e1 = e0.Twin;

            HeVertex v0 = vertex; // to be removed
            HeVertex v1 = e1.Start;

            // update vertex-edge refs if necesasry
            e1.Next.Start = v1;
            if (e1.IsOutgoing) v1.Outgoing = e1.Next;

            HeEdge.MakeConsecutive(e0.Prev, e0.Next);
            HeEdge.MakeConsecutive(e1.Prev, e1.Next);

            //flag for removal
            v0.MakeUnused();
            e0.MakeUnused();
            e1.MakeUnused();
        }


        /// <summary>
        /// Merges a pair of boundary vertices.
        /// That the first vertex is retained and the second is flagged as unused.
        /// </summary>
        /// <param name="edge"></param>
        public bool MergeVertices(HeVertex v0, HeVertex v1)
        {
            Validate(v0);
            Validate(v1);

            // both vertices must be on the mesh boundary
            if (!(v0.IsBoundary && v1.IsBoundary)) return false;

            HeEdge e0 = v0.Outgoing;
            HeEdge e1 = v1.Outgoing; // to be removed

            HeEdge e2 = e0.Prev;
            HeEdge e3 = e1.Prev;
    
            // if vertices are consecutive, just collapse the edge between them
            if (e0.Next == e1) 
                return Mesh.Edges.CollapseEdge(e0);
            else if(e1.Next == e0)
                return Mesh.Edges.CollapseEdge(e1);

            // update edge-vertex refs for all edges emanating from v1
            foreach (HeEdge e in v1.OutgoingEdges) 
                e.Start = v0;
       
            // update edge-edge refs
            HeEdge.MakeConsecutive(e3, e0);
            HeEdge.MakeConsecutive(e2, e1);

            // deal with potential collapsed boundary loops on either side of the merge
            if (e0.Next == e3)
            {
                e3 = e3.Twin;
                Mesh.Edges.RemovePair(e3);

                // update edge-face-edge refs
                e0.Face = e3.Face;
                if (e3.IsFirst) e0.MakeFirst();

                // update vertex-edge ref since e0 is no longer a boundary edge
                v0.Outgoing = e1;
            }

            if (e1.Next == e2)
            {
                e2 = e2.Twin;
                Mesh.Edges.RemovePair(e2);

                // update face-edge refs
                e1.Face = e2.Face;
                if (e2.IsFirst) e1.MakeFirst();
            }

            // flag elements for removal
            v1.MakeUnused();
            return true;
        }


        /// <summary>
        /// Splits a vertex in 2 connected by a new edge.
        /// Returns the new edge on success and null on failure.
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <returns></returns>
        public HeEdge SplitVertex(HeEdge e0, HeEdge e1)
        {
            HeEdgeList edges = Mesh.Edges;
            edges.Validate(e0);
            edges.Validate(e1);

            // they must emanate from the same vertex
            // TODO handle degree 2 vertex?
            if (e0.Start != e1.Start) return null;

            // if the same edge then just split the edge
            if (e0 == e1) return Mesh.Edges.SplitEdge(e0);

            HeVertex v0 = e0.Start;
            HeVertex v1 = Add(v0.Position);

            HeEdge e2 = Mesh.Edges.AddPair(v0, v1);
            HeEdge e3 = e2.Twin;

            // update edge-face refs
            e2.Face = e0.Face;
            e3.Face = e1.Face;

            // update start vertex of all outoging edges between from and to
            HeEdge e = e0;
            do
            {
                e.Start = v1;
                e = e.Twin.Next;
            } while (e != e1);

            // update vertex-edge refs if necessary
            if (v0.Outgoing.Start == v1)
            {
                // if v0's outgoing edge now emanates from v1, then can assume v1 is now on the boundary if v0 was originally
                v1.Outgoing = v0.Outgoing;
                v0.Outgoing = e2;
            }
            else
            {
                v1.Outgoing = e3;
            }

            // update edge-edge refs
            HeEdge.MakeConsecutive(e0.Prev, e2);
            HeEdge.MakeConsecutive(e2, e0);
            HeEdge.MakeConsecutive(e1.Prev, e3);
            HeEdge.MakeConsecutive(e3, e1);

            return e2;
        }

    
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="e0"></param>
        /// <param name="e1"></param>
        /// <returns></returns>
        public bool DetatchVertex(HeEdge e0, HeEdge e1)
        {
            throw new NotImplementedException();

            HeEdgeList edges = Mesh.Edges;
            edges.Validate(e0);
            edges.Validate(e1);

            // edges must belong to the same mesh
            // they must emanate from the same vertex
            // and they can't be the same
            if (e0.Start != e1.Start || e0 == e1) return false;

            HeVertex v0 = e0.Start;
            HeVertex v1 = new HeVertex(); // new vertex

            // create new edges if original is not on the boundary
            if(e0.Face != null)
            {
               HeEdge e2 = Mesh.Edges.AddPair(v0, e0.End);

            }

            if(e1.Face != null)
            {
                  
            HeEdge e3 = Mesh.Edges.AddPair(v1, e1.End);
            }
     
        

            // update start vertex of all outgoing edges between e0 and e1
            HeEdge e = e0;
            do
            {
                e.Start = v1;
                e = e.Twin.Next;
            } while (e != e1);

            // update vertex-edge refs
            e0.MakeOutgoing();
            e1.MakeOutgoing();

            /*
            // update vertex-edge refs if necessary
            if (v0.Outgoing.Start == v1)
            {
                // if v0's outgoing edge now emanates from v1, then can assume v1 is now on the boundary if v0 was originally
                v1.Outgoing = v0.Outgoing;
                v0.Outgoing = e1;
            }
            else
            {
                v1.Outgoing = e3;
            }
            */

            return true;
        }
     
        #endregion

    }
}
