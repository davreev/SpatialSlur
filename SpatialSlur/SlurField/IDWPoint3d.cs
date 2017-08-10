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
    [Serializable]
    public class IDWPoint3d<T>
    {
        /// <summary></summary>
        public Vec3d Point;
        /// <summary></summary>
        public T Value;
        /// <summary></summary>
        public double Weight = 1.0;
    }


    /// <summary>
    /// 
    /// </summary>
    public static class IDWPoint3d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static IDWPoint3d<T> Create<T>(T data, Vec3d point, double weight = 1.0)
        {
            return new IDWPoint3d<T>() { Value = data, Point = point, Weight = weight };
        }
    }
}
