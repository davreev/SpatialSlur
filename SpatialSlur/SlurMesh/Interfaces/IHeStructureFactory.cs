using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    public interface IHeStructureFactory<T, V, E> : IFactory<T>
        where T : IHeStructure<V, E>
        where V : HeElement, IHeVertex<V, E>
        where E : HeElement, IHalfedge<V, E>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <returns></returns>
        T Create(int vertexCapacity, int hedgeCapacity);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public interface IHeStructureFactory<T, V, E, F> : IFactory<T>
        where T : IHeStructure<V, E>
        where E : HeElement, IHalfedge<V, E, F>
        where V : HeElement, IHeVertex<V, E, F>
        where F : HeElement, IHeFace<V, E, F>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertexCapacity"></param>
        /// <param name="hedgeCapacity"></param>
        /// <param name="faceCapacity"></param>
        /// <returns></returns>
        T Create(int vertexCapacity, int hedgeCapacity, int faceCapacity);
    }
}
