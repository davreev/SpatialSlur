using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpatialSlur.SlurCore;

/*
 * Notes
 */ 

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IDWObject3d<T>
    {
        private T _value;
        private double _influence;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract double DistanceTo(Vec3d point);
        

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public double Influence
        {
            get => _influence;
            set => _influence = value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDWObject3d<T> Duplicate();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="converter"></param>
        /// <returns></returns>
        public abstract IDWObject3d<U> Convert<U>(Func<T, U> converter);
    }
}
