
/*
 * Notes
 */

using System;

using SpatialSlur;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncField2d<T> : IField2d<T>, IField3d<T>
    {
        /// <summary></summary>
        public readonly Func<Vector2d, T> ValueAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueAt"></param>
        internal FuncField2d(Func<Vector2d, T> valueAt)
        {
            ValueAt = valueAt ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        T IField2d<T>.ValueAt(Vector2d point)
        {
            return ValueAt(point);
        }


        T IField3d<T>.ValueAt(Vector3d point)
        {
            return ValueAt(point.XY);
        }

        #endregion
    }
}

