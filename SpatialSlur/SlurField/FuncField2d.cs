using System;

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
    public class FuncField2d<T> : IField2d<T>, IField3d<T>
    {
        /// <summary></summary>
        public readonly Func<Vec2d, T> ValueAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueAt"></param>
        public FuncField2d(Func<Vec2d, T> valueAt)
        {
            ValueAt = valueAt ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(point);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        T IField3d<T>.ValueAt(Vec3d point)
        {
            return ValueAt(point);
        }

        #endregion
    }
}

