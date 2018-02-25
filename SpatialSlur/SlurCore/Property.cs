using System;

/*
 * Notes
 */

namespace SpatialSlur.SlurCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class Property
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="get"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(Func<T, V> get, Action<T, V> set)
        {
            return new Property<T, V>(get, set);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="get"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(string name)
        {
            var propinfo = typeof(T).GetProperty(name);
            var getter = Delegate.CreateDelegate(typeof(Func<T, V>), propinfo.GetGetMethod());
            var setter = Delegate.CreateDelegate(typeof(Action<T, V>), propinfo.GetSetMethod());
            return new Property<T, V>((Func<T, V>)getter, (Action<T, V>)setter);
        }


        /*
        /// <summary>
        /// compiled lambda is notably slower than Delegate.CreateDelegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="get"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(string name)
        {
            var target = Expression.Parameter(typeof(T), "target");
            var value = Expression.Parameter(typeof(V), "value");
            
            var property = Expression.PropertyOrField(target, name);
            var assign = Expression.Assign(property, value);

            return new Property<T, V>(
                Expression.Lambda<Func<T, V>>(property, target).Compile(),
                Expression.Lambda<Action<T, V>>(assign, target, value).Compile()
                );
        }
        */
    }


    /// <summary>
    /// Compound delegate for getting/setting a value V via a target T.
    /// The given getter and setter are assumed to read from and write to the same location respectively.
    /// </summary>
    public struct Property<T, V> : IProperty<T, V>
    {
        /// <summary></summary>
        public readonly Func<T, V> Get;

        /// <summary></summary>
        public readonly Action<T, V> Set;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public static implicit operator Func<T, V>(Property<T,V> property)
        {
            return property.Get;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public static implicit operator Action<T, V>(Property<T, V> property)
        {
            return property.Set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="set"></param>
        public Property(Func<T, V> get, Action<T, V> set)
        {
            Get = get ?? throw new ArgumentNullException();
            Set = set ?? throw new ArgumentNullException();
        }


        #region Explicit interface implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        V IReadOnlyProperty<T, V>.Get(T item)
        {
            return Get(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        void IProperty<T, V>.Set(T item, V value)
        {
            Set(item, value);
        }

        #endregion
    }
}
