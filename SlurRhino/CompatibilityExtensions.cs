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
    /// <summary>
    /// Extension methods to circumvent type inference errors caused by older dotnet version used within Rhino 5 & Grasshopper.
    /// </summary>
    public static class CompatibilityExtensions
    {
        #region HalfedgeList

        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLengths(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeLengths(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLengths(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeLengths(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeDeltas(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeDeltas(halfedges, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeTangents(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeTangents(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeTangents(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeTangents(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeAngles(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeAngles(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeAngles(halfedges, vertexPositions, edgeLengths, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAngles(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeAngles(halfedges, vertexPositions, edgeLengths, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeAreas(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeAreas(halfedges, vertexPositions, faceCenters, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeCotangents(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeCotangents(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeNormals(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeNormals(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeUnitNormals(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetHalfedgeUnitNormals(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetDihedralAngles(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceNormals, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetDihedralAngles(halfedges, vertexPositions, faceNormals, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLines(this IReadOnlyList<HeMeshHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetHalfedgeLines(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLines(this IReadOnlyList<HeGraphHalfedge> halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetHalfedgeLines(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLines(this HeMeshHalfedgeList halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetHalfedgeLines(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetHalfedgeLines(this HeGraphHalfedgeList halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetHalfedgeLines(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeLines(this HeMeshHalfedgeList halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetEdgeLines(halfedges, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetEdgeLines(this HeGraphHalfedgeList halfedges, IReadOnlyList<Vec3d> vertexPositions, IList<Line> result, bool parallel = false)
        {
            RhinoExtensions.GetHalfedgeLines(halfedges, vertexPositions, result, parallel);
        }

        #endregion


        #region HeVertexList

        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexDegrees(this IReadOnlyList<HeMeshVertex> vertices, IList<int> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexDegrees(vertices, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexDegrees(this IReadOnlyList<HeGraphVertex> vertices, IList<int> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexDegrees(vertices, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeGraphVertex> vertices, IReadOnlyList<double> vertexValues, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeGraphVertex> vertices, IReadOnlyList<Vec2d> vertexValues, IList<Vec2d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetLaplacian(this IReadOnlyList<HeGraphVertex> vertices, IReadOnlyList<Vec3d> vertexValues, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetLaplacian(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeHalfedgeWeights(this IReadOnlyList<HeMeshVertex> vertices, IList<double> halfedgeWeights, bool parallel = false)
        {
            HeAttributeExtensions.NormalizeHalfedgeWeights(vertices, halfedgeWeights, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeHalfedgeWeights(this IReadOnlyList<HeGraphVertex> vertices, IList<double> halfedgeWeights, bool parallel = false)
        {
            HeAttributeExtensions.NormalizeHalfedgeWeights(vertices, halfedgeWeights, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetMorseSmaleClassification(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> vertexValues, IList<int> result, bool parallel = false)
        {
            HeAttributeExtensions.GetMorseSmaleClassification(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetMorseSmaleClassification(this IReadOnlyList<HeGraphVertex> vertices, IReadOnlyList<double> vertexValues, IList<int> result, bool parallel = false)
        {
            HeAttributeExtensions.GetMorseSmaleClassification(vertices, vertexValues, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexAreas(vertices, vertexPositions, faceCenters, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreas(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> halfedgeAreas, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexAreas(vertices, halfedgeAreas, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexAreasBarycentric(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> faceAreas, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexAreasBarycentric(vertices, faceAreas, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetCirclePackingRadii(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> edgeLengths, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetCirclePackingRadii(vertices, edgeLengths, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetMeanCurvature(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> vertexNormals, IReadOnlyList<Vec3d> vertexLaplacian, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetMeanCurvature(vertices, vertexNormals, vertexLaplacian, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetGaussianCurvature(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetGaussianCurvature(vertices, vertexPositions, vertexAreas, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetGaussianCurvature(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<double> halfedgeAngles, IReadOnlyList<double> vertexAreas, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetGaussianCurvature(vertices, halfedgeAngles, vertexAreas, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexNormals(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexNormals(vertices, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetVertexNormals2(this IReadOnlyList<HeMeshVertex> vertices, IReadOnlyList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetVertexNormals(vertices, halfedgeNormals, result, parallel);
        }

        #endregion


        #region HeFaceList


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceDegrees(this IReadOnlyList<HeMeshFace> faces, IList<int> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceDegrees(faces, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void NormalizeHalfedgeWeights(this IReadOnlyList<HeMeshFace> faces, IList<double> halfedgeWeights, bool parallel = false)
        {
            HeAttributeExtensions.NormalizeHalfedgeWeights(faces, halfedgeWeights, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceBarycenters(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceBarycenters(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceCircumcenters(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceCircumcenters(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceIncenters(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceIncenters(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceNormals(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceNormals(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceNormals2(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> halfedgeNormals, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceNormals2(faces, halfedgeNormals, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceNormalsTri(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Vec3d> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceNormalsTri(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceAreas(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceAreas(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceAreas(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IReadOnlyList<Vec3d> faceCenters, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceAreas(faces, vertexPositions, faceCenters, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceAreasTri(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFaceAreasTri(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFacePlanarity(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<double> result, bool parallel = false)
        {
            HeAttributeExtensions.GetFacePlanarity(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFacePolylines(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Polyline> result, bool parallel = false)
        {
            RhinoExtensions.GetFacePolylines(faces, vertexPositions, result, parallel);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void GetFaceCircumcircles(this IReadOnlyList<HeMeshFace> faces, IReadOnlyList<Vec3d> vertexPositions, IList<Circle> result, bool parallel = false)
        {
            RhinoExtensions.GetFaceCircumcircles(faces, vertexPositions, result, parallel);
        }
 
        #endregion

    }
}
