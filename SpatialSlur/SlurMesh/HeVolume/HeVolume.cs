using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TV"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <typeparam name="TF"></typeparam>
    /// <typeparam name="TC"></typeparam>
    public class HeVolume<TV, TE, TF, TC> : IHeStructure<TV, TE, TF, TC>
        where TV : HeVertex<TV, TE, TF, TC>
        where TE : Halfedge<TV, TE, TF, TC>
        where TF : HeFace<TV, TE, TF, TC>
        where TC : HeCell<TV, TE, TF, TC>
    {
        private HeElementList<TV> _vertices;
        private HeElementList<TE> _hedges;
        private HeElementList<TF> _faces;
        private HeElementList<TC> _cells;

        private Func<TV> _newTV;
        private Func<TE> _newTE;
        private Func<TF> _newTF;
        private Func<TC> _newTC;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexProvider"></param>
        /// <param name="hedgeProvider"></param>
        /// <param name="faceProvider"></param>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        public HeVolume(Func<TV> vertexProvider, Func<TE> hedgeProvider, Func<TF> faceProvider, Func<TC> cellProvider, int vertexCapacity = 4, int hedgeCapacity = 4, int faceCapacity = 4, int cellCapacity = 4)
        {
            VertexProvider = vertexProvider;
            HalfedgeProvider = hedgeProvider;
            FaceProvider = faceProvider;

            _vertices = new HeElementList<TV>(vertexCapacity);
            _hedges = new HeElementList<TE>(hedgeCapacity);
            _faces = new HeElementList<TF>(faceCapacity);
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TV> Vertices
        {
            get { return _vertices; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TE> Halfedges
        {
            get { return _hedges; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TF> Faces
        {
            get { return _faces; }
        }


        /// <summary>
        /// 
        /// </summary>
        public HeElementList<TC> Cells
        {
            get { return _cells; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<TV> VertexProvider
        {
            get { return _newTV; }
            set { _newTV = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<TE> HalfedgeProvider
        {
            get { return _newTE; }
            set { _newTE = value ?? throw new ArgumentNullException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<TF> FaceProvider
        {
            get { return _newTF; }
            set { _newTF = value ?? throw new ArgumentNullException(); }
        }
    }
}
