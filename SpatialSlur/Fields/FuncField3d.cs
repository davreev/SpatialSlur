
/*
 * Notes
 */

using System;

namespace SpatialSlur.Fields
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncField3d<T>: IField2d<T>, IField3d<T>
    {
        /// <summary></summary>
        public readonly Func<Vector3d, T> ValueAt;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueAt"></param>
        internal FuncField3d(Func<Vector3d, T> valueAt)
        {
            ValueAt = valueAt ?? throw new ArgumentNullException();
        }


        #region Explicit Interface Implementations

        T IField2d<T>.ValueAt(Vector2d point)
        {
            return ValueAt(point.As3d);
        }


        T IField3d<T>.ValueAt(Vector3d point)
        {
            return ValueAt(point);
        }

        #endregion
    }
}