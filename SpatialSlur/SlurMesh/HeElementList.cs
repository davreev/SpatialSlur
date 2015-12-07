using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    public abstract class HeElementList<T>:IEnumerable<T> where T : HeElement
    {
        private readonly HeMesh _mesh;
        private readonly List<T> _list;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        internal HeElementList(HeMesh mesh)
        {
            _mesh = mesh;
            _list = new List<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        public HeMesh Mesh
        {
            get { return _mesh; }
        }

        
        /// <summary>
        /// 
        /// </summary>
        protected List<T> List
        {
            get { return _list; }
        }


        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return _list[index]; }
            internal set { _list[index] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }


        /// <summary>
        /// explicit implementation of non-generic method
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // calls the generic version of the method
            return GetEnumerator();
        }


        /// <summary>
        /// checks that the element is ownded by this element list and that it is in use
        /// </summary>
        /// <param name="element"></param>
        internal void Validate(HeElement element)
        {
            if (!Owns(element) || element.IsUnused)
                throw new ArgumentException("The given element is not valid for this operation");
        }


        /// <summary>
        /// true if the element belongs to this element list
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Owns(HeElement element)
        {
            return element == _list[element.Index];
        }


        /// <summary>
        /// adds an edge to the list
        /// </summary>
        /// <param name="element"></param>
        internal void Add(T element)
        {
            element.Index = _list.Count;
            _list.Add(element);
        }


        /// <summary>
        /// removes all unused elements in the list and re-indexes
        /// if the element list has any associated attributes, be sure to compact those first
        /// </summary>
        public void Compact()
        {
            int marker = 0;

            for (int i = 0; i < _list.Count; i++)
            {
                T e = _list[i];
                if (e.IsUnused) continue; // skip unused elements

                e.Index = marker;
                _list[marker++] = e;
            }

            // trim list to include only used elements
            _list.RemoveRange(marker, _list.Count - marker);
        }


        /// <summary>
        /// removes all objects that coincide with unused elements in the list
        /// useful for maintaining a matching collection of user defined element attributes
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public void CompactAttributes<U>(List<U> attributes)
        {
            SizeCheck(attributes);
            int marker = 0;

            for (int i = 0; i < _list.Count; i++)
            {
                if (!_list[i].IsUnused) // skip unused elements
                    attributes[marker++] = attributes[i];
            }

            // trim list to include only used elements
            attributes.RemoveRange(marker, attributes.Count - marker);
        }


        /// <summary>
        /// returns a filtered array of attributes ignoring those that coincide with unused elements in the list
        /// useful for maintaining a matching collection of user defined element attributes
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="attributes"></param>
        public U[] CompactAttributes<U>(IList<U> attributes)
        {
            SizeCheck(attributes);
            int marker = 0;

            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].IsUnused) continue; // skip unused elements
                attributes[marker++] = attributes[i];
            }

            // return subarray
            return attributes.SubArray(0, marker);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        internal void SizeCheck<U>(IList<U> attributes)
        {
            if (attributes.Count != _list.Count)
                throw new ArgumentException("the number of element attributes provided does not match the number of elements in the mesh");
        }
    }
}
