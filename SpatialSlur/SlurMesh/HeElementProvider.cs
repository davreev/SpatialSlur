using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */

namespace SpatialSlur.SlurMesh
{
    /// <summary>
    /// 
    /// </summary>
    public static class HeElementProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="newVertex"></param>
        /// <param name="newHedge"></param>
        /// <returns></returns>
        public static HeElementProvider<V, E> Create<V, E>(Func<V> newVertex, Func<E> newHedge)
            where V : IHeVertex<V, E>
            where E : IHalfedge<V, E>
        {
            return new HeElementProvider<V, E>(newVertex, newHedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="newVertex"></param>
        /// <param name="newHedge"></param>
        /// <param name="newFace"></param>
        /// <returns></returns>
        public static HeElementProvider<V, E, F> Create<V, E, F>(Func<V> newVertex, Func<E> newHedge, Func<F> newFace)
            where V : IHeVertex<V, E, F>
            where E : IHalfedge<V, E, F>
            where F : IHeFace<V, E, F>
        {
            return new HeElementProvider<V, E, F>(newVertex, newHedge, newFace);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="newVertex"></param>
        /// <param name="newHedge"></param>
        /// <param name="newFace"></param>
        /// <param name="newCell"></param>
        /// <returns></returns>
        public static HeElementProvider<V, E, F, C> Create<V, E, F, C>(Func<V> newVertex, Func<E> newHedge, Func<F> newFace, Func<C> newCell)
            where V : IHeVertex<V, E, F, C>
            where E : IHalfedge<V, E, F, C>
            where F : IHeFace<V, E, F, C>
            where C : IHeCell<V, E, F, C>
        {
            return new HeElementProvider<V, E, F, C>(newVertex, newHedge, newFace, newCell);
        }
    }


   /// <summary>
   /// Compound delegate for the creation of HeElements.
   /// </summary>
   public class HeElementProvider<V, E>
       where V : IHeVertex<V, E>
       where E : IHalfedge<V, E>
   {
       /// <summary></summary>
       public readonly Func<V> NewVertex;
       /// <summary></summary>
       public readonly Func<E> NewHalfedge;


       /// <summary>
       /// 
       /// </summary>
       public HeElementProvider(Func<V> newVertex, Func<E> newHedge)
       {
           NewVertex = newVertex ?? throw new ArgumentNullException();
           NewHalfedge = newHedge ?? throw new ArgumentNullException();
       }
   }


    /// <summary>
    /// Compound delegate for the creation of HeElements.
    /// </summary>
    public class HeElementProvider<V, E, F>:HeElementProvider<V, E>
       where V : IHeVertex<V, E, F>
       where E : IHalfedge<V, E, F>
       where F : IHeFace<V, E, F>
   {
       /// <summary></summary>
       public readonly Func<F> NewFace;


       /// <summary>
       /// 
       /// </summary>
       public HeElementProvider(Func<V> newVertex, Func<E> newHedge, Func<F> newFace)
           :base(newVertex, newHedge)
       {
           NewFace = newFace ?? throw new ArgumentNullException();
        }

   }


    /// <summary>
    /// Compound delegate for the creation of HeElements.
    /// </summary>
    public class HeElementProvider<V, E, F, C> : HeElementProvider<V, E, F>
        where V : IHeVertex<V, E, F, C>
        where E : IHalfedge<V, E, F, C>
        where F : IHeFace<V, E, F, C>
        where C : IHeCell<V, E, F, C>
    {
        /// <summary></summary>
        public readonly Func<C> NewCell;


        /// <summary>
        /// 
        /// </summary>
        public HeElementProvider(Func<V> newVertex, Func<E> newHedge, Func<F> newFace, Func<C> newCell)
            : base(newVertex, newHedge, newFace)
        {
            NewCell = newCell ?? throw new ArgumentNullException();
        }
    }


    /*
    /// <summary>
    /// Wrapper class containing delegates for the creation of HeElements.
    /// </summary>
    public class HeElementProvider<V, E>
        where V : IHeVertex<V, E>
        where E : IHalfedge<V, E>
    {
        private Func<V> _newV;
        private Func<E> _newE;


        /// <summary>
        /// 
        /// </summary>
        public HeElementProvider(Func<V> newVertex, Func<E> newHedge)
        {
            _newV = newVertex ?? throw new ArgumentNullException();
            _newE = newHedge ?? throw new ArgumentNullException();
        }


        /// <summary>
        /// 
        /// </summary>
        public V NewVertex()
        {
            return _newV();
        }


        /// <summary>
        /// 
        /// </summary>
        public E NewHalfedge()
        {
            return _newE();
        }
    }


    /// <summary>
    /// Wrapper class containing factory delegates for HeElements.
    /// </summary>
    public class HeElementProvider<V, E, F>:HeElementProvider<V, E>
        where V : IHeVertex<V, E, F>
        where E : IHalfedge<V, E, F>
        where F : IHeFace<V, E, F>
    {
        private Func<F> _newF;


        /// <summary>
        /// 
        /// </summary>
        public HeElementProvider(Func<V> newVertex, Func<E> newHedge, Func<F> newFace)
            :base(newVertex, newHedge)
        {
            NewFace = newFace;
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<F> NewFace
        {
            get { return _newF; }
            private set { _newF = value ?? throw new ArgumentNullException(); }
        }
    }


    /// <summary>
    /// Wrapper class containing factory delegates for HeElements.
    /// </summary>
    public class HeElementProvider<V, E, F, C> : HeElementProvider<V, E, F>
        where V : IHeVertex<V, E, F, C>
        where E : IHalfedge<V, E, F, C>
        where F : IHeFace<V, E, F, C>
        where C : IHeCell<V, E, F, C>
    {
        private Func<C> _newC;


        /// <summary>
        /// 
        /// </summary>
        public HeElementProvider(Func<V> newVertex, Func<E> newHedge, Func<F> newFace, Func<C> newCell)
            : base(newVertex, newHedge, newFace)
        {
            NewCell = newCell;
        }


        /// <summary>
        /// 
        /// </summary>
        public Func<C> NewCell
        {
            get { return _newC; }
            private set { _newC = value ?? throw new ArgumentNullException(); }
        }
    }
*/
}
