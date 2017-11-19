#if USING_RHINO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;
using SpatialSlur.SlurMesh;
using Rhino.Geometry;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurRhino
{
    using V = HeMesh3d.Vertex;
    using E = HeMesh3d.Halfedge;
    using F = HeMesh3d.Face;

    /// <summary>
    /// 
    /// </summary>
    public static class RhinoUtil
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static List<NurbsSurface> ExtractNurbsPatches(HeMesh3d mesh, int degreeU, int degreeV)
        {
            if(!IsQuadMesh(mesh))
                throw new ArgumentException("The given mesh must have exclusively quadrilateral faces.");
       
            var verts = mesh.Vertices;
            var corners = verts.Where(v => IsSingular(v)).ToList();

            // if no singular verts, add first boundary vertex (cylinder)
            if (corners.Count == 0)
            {
                var vc = verts.FirstOrDefault(v => !v.IsUnused && v.IsBoundary);

                // if no boundary add first valid vertex (torus)
                if (vc == null)
                    vc = verts.FirstOrDefault(v => !v.IsUnused);

                corners.Add(vc);
            }

            var labels = GetVertexLabels(mesh, corners);
            return GetPatchSurfaces(mesh, corners, labels, degreeU, degreeV).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        private static bool IsQuadMesh(HeMesh3d mesh)
        {
            foreach(var f in mesh.Faces)
            {
                if (f.IsUnused || f.IsDegree(4)) continue;
                return false;
            }

            return true;
        }


        /// <summary>
        ///
        /// </summary>
        private static bool IsSingular(V vertex)
        {
            if (vertex.IsUnused)
                return false;
            
            if (vertex.IsBoundary)
            {
                if (!vertex.IsDegree3)
                    return true;
            }
            else
            {
                if (!vertex.IsDegree(4))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Labels vertices as 0 = interior, 1 = seam, 2 = corner
        /// </summary>
        private static int[] GetVertexLabels(HeMesh3d mesh, List<V> corners)
        {
            var verts = mesh.Vertices;
            var labels = new int[verts.Count];
            int currTag = TagSeams(mesh, corners);

            // label known corners
            foreach (var v in corners)
                labels[v] = 2;

            // set vertex labels based on the number of incident seam edges
            for (int i = 0; i < verts.Count; i++)
            {
                var v = verts[i];
                if (v.IsUnused || labels[i] == 2) continue;

                // count number of tagged edges
                int ne = 0;
                foreach (var he in v.OutgoingHalfedges)
                {
                    if (he.Tag == currTag)
                        if (++ne > 2) break;
                }

                // assign vertex label
                if (v.IsBoundary)
                {
                    if (ne > 0)
                    {
                        labels[i] = 2;
                        corners.Add(v);
                    }
                }
                else
                {
                    if (ne == 2)
                    {
                        labels[i] = 1;
                    }
                    else if (ne > 2)
                    {
                        labels[i] = 2;
                        corners.Add(v);
                    }
                }
            }

            return labels;
        }


        /// <summary>
        /// Tags seam edges by marching outwards from corner vertices
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="corners"></param>
        private static int TagSeams(HeMesh3d mesh, List<V> corners)
        {
            int currTag = mesh.Halfedges.NextTag;
            
            foreach (var v in corners)
            {
                foreach (var he0 in v.OutgoingHalfedges)
                {
                    if (he0.IsBoundary) continue;
                    var he1 = he0;

                    while (he1.Tag != currTag && !he1.IsBoundary)
                    {
                        he1.Tag = he1.Twin.Tag = currTag;
                        he1 = he1.NextInFace.Twin.NextInFace;
                    }
                }
            }

            return currTag;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="corners"></param>
        /// <param name="labels"></param>
        /// <param name="degreeU"></param>
        /// <param name="degreeV"></param>
        /// <returns></returns>
        private static IEnumerable<NurbsSurface> GetPatchSurfaces(HeMesh3d mesh, List<V> corners, int[] labels, int degreeU, int degreeV)
        {
            var currTag = mesh.Faces.NextTag;

            foreach(var v in corners)
            {
                foreach(var he0 in v.OutgoingHalfedges)
                {
                    if (he0.IsHole || he0.Face.Tag == currTag) continue;

                    GetPatchDimensions(he0, labels, out int nu, out int nv);
                    var cps = GetPatchHedges(he0, nu, nv, currTag).Select(he => (Point3d)he.Start.Position);
                    
                    yield return NurbsSurface.CreateFromPoints(cps, nv, nu, degreeU, degreeV); // u and v count flipped
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hedge"></param>
        /// <param name="countU"></param>
        /// <param name="countV"></param>
        private static void GetPatchDimensions(E hedge, int[] labels, out int countU, out int countV)
        {
            var heU = hedge;
            countU = 1;

            do
            {
                countU++;
                heU = heU.NextInFace.Twin.NextInFace;
            } while (labels[heU.Start] != 2);

            var heV = hedge;
            countV = 1;

            do
            {
                countV++;
                heV = heV.NextInFace.NextInFace.Twin;
            } while (labels[heV.Start] != 2);
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromCorner"></param>
        private static IEnumerable<E> GetPatchHedges(E fromCorner, int countU, int countV, int faceTag)
        {
            var heV = fromCorner;

            for (int i = 1; i < countV; i++)
            {
                var heU = heV;

                for (int j = 1; j < countU; j++)
                {
                    yield return heU;
                    heU.Face.Tag = faceTag;
                    heU = heU.NextInFace.Twin.NextInFace; // advance in u direction
                }

                yield return heU; // last in row
                heV = heV.NextInFace.NextInFace.Twin; // advance in v direction
            }

            // last row (don't tag faces)
            {
                if (heV.IsHole)
                {
                    var heU = heV;

                    for (int j = 0; j < countU; j++)
                    {
                        yield return heU;
                        heU = heU.NextInFace; // advance in u direction
                    }
                }
                else
                {
                    var heU = heV;

                    for (int j = 0; j < countU; j++)
                    {
                        yield return heU;
                        heU = heU.NextInFace.Twin.NextInFace; // advance in u direction
                    }
                }
            }
        }
    }
}

#endif