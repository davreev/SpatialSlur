
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    public static class Field2d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueAt"></param>
        /// <returns></returns>
        public static IField2d<T> Create<T>(Func<Vector2d, T> valueAt)
        {
            return new FuncField2d<T>(valueAt);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static IField2d<T> CreateTransformed<T>(IField2d<T> other, Transform2d transform)
        {
            transform.Invert();
            return Create(p => other.ValueAt(transform.Apply(p)));
        }
    }
}

