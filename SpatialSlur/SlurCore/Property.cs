
/*
 * Notes
 */

using System;
using System.Reflection;
using System.Reflection.Emit;

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
            var type = typeof(T);
            var propInfo = type.GetProperty(name);
            return propInfo == null ? Create<T, V>(type.GetField(name)) : Create<T, V>(propInfo);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(PropertyInfo info)
        {
            return Create(CreateGetter<T, V>(info), CreateSetter<T, V>(info));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Func<T, V> CreateGetter<T, V>(PropertyInfo info)
        {
            return (Func<T, V>)Delegate.CreateDelegate(typeof(Func<T, V>), info.GetGetMethod());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Action<T, V> CreateSetter<T, V>(PropertyInfo info)
        {
            return (Action<T, V>)Delegate.CreateDelegate(typeof(Action<T, V>), info.GetSetMethod());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static Property<T, V> Create<T, V>(FieldInfo info)
        {
            return Create(CreateGetter<T, V>(info), CreateSetter<T, V>(info));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        private static Func<T, V> CreateGetter<T, V>(FieldInfo info)
        {
            // impl ref
            // https://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo

            string methodName = $"{info.ReflectedType.FullName}.get_{info.Name}";

            DynamicMethod method = new DynamicMethod(methodName, typeof(V), new Type[1] { typeof(T) }, true);
            ILGenerator gen = method.GetILGenerator();

            if (info.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, info);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, info);
            }

            gen.Emit(OpCodes.Ret);
            return (Func<T, V>)method.CreateDelegate(typeof(Func<T, V>));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        private static Action<T, V> CreateSetter<T, V>(FieldInfo info)
        {
            // impl ref
            // https://stackoverflow.com/questions/16073091/is-there-a-way-to-create-a-delegate-to-get-and-set-values-for-a-fieldinfo

            string methodName = $"{info.ReflectedType.FullName}.set_{info.Name}";

            DynamicMethod method = new DynamicMethod(methodName, null, new Type[2] { typeof(T), typeof(V) }, true);
            ILGenerator gen = method.GetILGenerator();

            if (info.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, info);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, info);
            }

            gen.Emit(OpCodes.Ret);
            return (Action<T, V>)method.CreateDelegate(typeof(Action<T, V>));
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
            Get = get;
            Set = set;
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
