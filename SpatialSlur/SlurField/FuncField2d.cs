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
    public static class FuncField2d
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueAt"></param>
        /// <returns></returns>
        public static FuncField2d<T> Create<T>(Func<Vec2d, T> valueAt)
        {
            return new FuncField2d<T>(valueAt);
        }
    }


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
        internal FuncField2d(Func<Vec2d, T> valueAt)
        {
            ValueAt = valueAt ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        /// <inheritdoc />
        T IField2d<T>.ValueAt(Vec2d point)
        {
            return ValueAt(point);
        }


        /// <inheritdoc />
        T IField3d<T>.ValueAt(Vec3d point)
        {
            return ValueAt(point);
        }

        #endregion
    }
}

