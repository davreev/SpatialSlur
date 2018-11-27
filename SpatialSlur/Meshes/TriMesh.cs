
/*
 * Notes
 */

using System;
using SpatialSlur.Collections;

namespace SpatialSlur.Meshes
{
    /// <summary>
    /// Simple face-vertex representation of a triangle mesh.
    /// </summary>
    public abstract class TriMesh<TVec3, TVec2>
        where TVec3 : struct
        where TVec2 : struct
    {
        private TVec3[] _positions = Array.Empty<TVec3>();
        private TVec3[] _normals = null;
        private TVec3[] _tangents = null;
        private TVec2[] _textureCoords = null;
        private int _vertexCount;

        private Vector3i[] _faces = Array.Empty<Vector3i>();
        private int _faceCount;


        /// <summary>
        /// 
        /// </summary>
        public TriMesh()
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        public TriMesh(int vertexCount, int faceCount)
        {
            _positions = new TVec3[_vertexCount = vertexCount];
            _faces = new Vector3i[_faceCount = faceCount];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public TriMesh(TriMesh<TVec3, TVec2> other)
        {
            _positions = other._positions.ShallowCopy();
            _vertexCount = other._vertexCount;

            if (other.HasNormals)
                _normals = other._normals.ShallowCopy();

            if (other.HasTangents)
                _tangents = other._tangents.ShallowCopy();

            if (other.HasTextureCoords)
                _textureCoords = other._textureCoords.ShallowCopy();

            _faces = other._faces.ShallowCopy();
            _faceCount = other._faceCount;
        }


        /// <summary>
        /// Returns the array of vertex positions
        /// </summary>
        public ArrayView<TVec3> Positions
        {
            get { return _positions.AsView(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex normals or null.
        /// </summary>
        public ArrayView<TVec3> Normals
        {
            get { return _normals.AsView(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex tangents or null.
        /// </summary>
        public ArrayView<TVec3> Tangents
        {
            get { return _tangents.AsView(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex texture coordinates or null.
        /// </summary>
        public ArrayView<TVec2> TextureCoords
        {
            get {  return _textureCoords.AsView(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of faces.
        /// </summary>
        public ArrayView<Vector3i> Faces
        {
            get { return _faces.AsView(_faceCount); }
        }


        /// <summary>
        /// Gets or sets whether or not this mesh stores normals.
        /// </summary>
        public bool HasNormals
        {
            get { return _normals != null; }
            set { _normals = value ? InitOptional(_normals) : null; }
        }


        /// <summary>
        /// Gets or sets whether or not this mesh stores tangents.
        /// </summary>
        public bool HasTangents
        {
            get { return _tangents != null; }
            set { _tangents = value ? InitOptional(_tangents) : null; }
        }


        /// <summary>
        /// Gets or sets whether or not this mesh stores texture coordinates.
        /// </summary>
        public bool HasTextureCoords
        {
            get { return _textureCoords != null; }
            set { _textureCoords = value ? InitOptional(_textureCoords) : null; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int VertexCount
        {
            get { return _vertexCount; }
            set
            {
                if(value > VertexCapacity)
                {
                    Array.Resize(ref _positions, value);

                    if (HasNormals)
                        Array.Resize(ref _normals, value);

                    if (HasTangents)
                        Array.Resize(ref _tangents, value);

                    if (HasTextureCoords)
                        Array.Resize(ref _textureCoords, value);
                }

                _vertexCount = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int VertexCapacity
        {
            get { return _positions.Length; }
            set
            {
                if (value < _vertexCount)
                    _vertexCount = value;

                Array.Resize(ref _positions, value);

                if (HasNormals)
                    Array.Resize(ref _normals, value);

                if (HasTangents)
                    Array.Resize(ref _tangents, value);

                if (HasTextureCoords)
                    Array.Resize(ref _textureCoords, value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int FaceCount
        {
            get { return _faceCount; }
            set
            {
                if (value > FaceCapacity)
                    Array.Resize(ref _faces, value);

                _faceCount = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int FaceCapacity
        {
            get { return _faces.Length; }
            set
            {
                if (value < _faceCount)
                    _faceCount = value;

                Array.Resize(ref _faces, value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private T[] InitOptional<T>(T[] buffer)
        {
            return buffer ?? new T[VertexCapacity];
        }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _vertexCount = _faceCount = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void ClearVertices()
        {
            _vertexCount = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void ClearFaces()
        {
            _faceCount = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void AddVertex(TVec3 position)
        {
            const int minCapacity = 4;
            int index = _vertexCount++;

            if (index == VertexCapacity)
                ExpandVertexCapacity(Math.Max(index << 1, minCapacity));

            _positions[index] = position;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="face"></param>
        public void AddFace(Vector3i face)
        {
            const int minCapacity = 4;
            int index = _faceCount++;

            if (index == FaceCapacity)
                ExpandFaceCapacity(Math.Max(index << 1, minCapacity));

            _faces[index] = face;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void Append(TriMesh<TVec3, TVec2> other)
        {
            AppendVertices(other);
            AppendFaces(other);
        }


        /// <summary>
        /// 
        /// </summary>
        private void AppendVertices(TriMesh<TVec3, TVec2> other)
        {
            int nv0 = _vertexCount;
            int nv1 = other._vertexCount;
            int sum = nv0 + nv1;

            if (sum > VertexCapacity)
                ExpandVertexCapacity(nv0 + sum);

            _positions.SetRange(other._positions, nv0, 0, nv1);
            _vertexCount = sum;

            if (HasNormals && other.HasNormals)
                _normals.SetRange(other._normals, nv0, 0, nv1);

            if (HasTangents && other.HasTangents)
                _tangents.SetRange(other._tangents, nv0, 0, nv1);

            if (HasTextureCoords && other.HasTextureCoords)
                _textureCoords.SetRange(other._textureCoords, nv0, 0, nv1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private void AppendFaces(TriMesh<TVec3, TVec2> other)
        {
            int nf0 = _faceCount;
            int nf1 = other._faceCount;
            int sum = nf0 + nf1;

            if (sum > FaceCapacity)
                ExpandFaceCapacity(nf0 + sum);

            _faces.SetRange(other._faces, nf0, 0, nf1);
            _faceCount = sum;
        }


        /// <summary>
        /// Assumes that the new capacity is larger than the current.
        /// </summary>
        private void ExpandVertexCapacity(int newCapacity)
        {
            Array.Resize(ref _positions, newCapacity);

            if (HasNormals)
                Array.Resize(ref _normals, newCapacity);

            if (HasTangents)
                Array.Resize(ref _tangents, newCapacity);

            if (HasTextureCoords)
                Array.Resize(ref _textureCoords, newCapacity);
        }


        /// <summary>
        /// Assumes that the new capacity is larger than the current.
        /// </summary>
        /// <param name="newCapacity"></param>
        private void ExpandFaceCapacity(int newCapacity)
        {
            Array.Resize(ref _faces, newCapacity);
        }
    }
}
