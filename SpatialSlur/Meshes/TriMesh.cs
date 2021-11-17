
/*
 * Notes
 * 
 * TODO make index type generic to handle line segments and point clouds
 */

using System;
using SpatialSlur.Collections;

using static SpatialSlur.Collections.DynamicArray;

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
            get { return _positions.First(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex normals or null.
        /// </summary>
        public ArrayView<TVec3> Normals
        {
            get { return _normals.First(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex tangents or null.
        /// </summary>
        public ArrayView<TVec3> Tangents
        {
            get { return _tangents.First(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of vertex texture coordinates or null.
        /// </summary>
        public ArrayView<TVec2> TextureCoords
        {
            get {  return _textureCoords.First(_vertexCount); }
        }


        /// <summary>
        /// Returns the array of faces.
        /// </summary>
        public ArrayView<Vector3i> Faces
        {
            get { return _faces.First(_faceCount); }
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                SetVertexCount(value);
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
                if (value < 0)
                    throw new ArgumentOutOfRangeException("The value cannot be negative.");

                SetFaceCount(value);
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
        /// <param name="value"></param>
        private void SetVertexCount(int value)
        {
            if (value > _positions.Length)
            {
                ExpandToFit(ref _positions, value);

                if (HasNormals)
                    Array.Resize(ref _normals, _positions.Length);

                if (HasTangents)
                    Array.Resize(ref _tangents, _positions.Length);

                if (HasTextureCoords)
                    Array.Resize(ref _textureCoords, _positions.Length);
            }

            _vertexCount = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetFaceCount(int value)
        {
            if (value > _faces.Length)
                ExpandToFit(ref _faces, value);

            _faceCount = value;
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
            {
                int newCapacity = Math.Max(index << 1, minCapacity);
                Array.Resize(ref _positions, newCapacity);

                if (HasNormals)
                    Array.Resize(ref _normals, newCapacity);

                if (HasTangents)
                    Array.Resize(ref _tangents, newCapacity);

                if (HasTextureCoords)
                    Array.Resize(ref _textureCoords, newCapacity);
            }

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
            {
                int newCapacity = Math.Max(index << 1, minCapacity);
                Array.Resize(ref _faces, newCapacity);
            }

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

            // Resizes arrays if necessary
            SetVertexCount(nv0 + nv1);

            // Assign attributes
            _positions.SetRange(nv0, other._positions, 0, nv1);

            if (HasNormals && other.HasNormals)
                _normals.SetRange(nv0, other._normals, 0, nv1);

            if (HasTangents && other.HasTangents)
                _tangents.SetRange(nv0, other._tangents, 0, nv1);

            if (HasTextureCoords && other.HasTextureCoords)
                _textureCoords.SetRange(nv0, other._textureCoords, 0, nv1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private void AppendFaces(TriMesh<TVec3, TVec2> other)
        {
            int nf0 = _faceCount;
            int nf1 = other._faceCount;

            // Resizes arrays if necessary
            SetFaceCount(nf0 + nf1);
            
            // Assign attributes
            _faces.SetRange(nf0, other._faces, 0, nf1);
        }
    }
}
