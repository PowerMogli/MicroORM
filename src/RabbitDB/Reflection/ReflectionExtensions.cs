// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The reflection extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    using RabbitDB.Session;

    /// <summary>
    /// The reflection extensions.
    /// </summary>
    internal static class ReflectionExtensions
    {
        #region Static Fields

        /// <summary>
        /// The _cache.
        /// </summary>
        private static ConcurrentDictionary<int, Setter> cache;

        /// <summary>
        /// The _cache get.
        /// </summary>
        private static ConcurrentDictionary<int, Func<object, object>> cacheGet;

        #endregion

        #region Delegates

        /// <summary>
        /// The setter.
        /// </summary>
        /// <param name="dest">
        /// The dest.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private delegate void Setter(object dest, object value);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Fast getter. aprox 5x faster than simple Reflection, aprox. 10x slower than manual get
        /// </summary>
        /// <param name="propertyInfo">
        /// The property Info.
        /// </param>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetValueFast(this PropertyInfo propertyInfo, object obj)
        {
            Func<object, object> getter;
            if (cacheGet == null)
            {
                cacheGet = new ConcurrentDictionary<int, Func<object, object>>();
            }

            var hashKey = propertyInfo.GetHashCode();

            if (cacheGet.TryGetValue(hashKey, out getter))
            {
                return getter(obj);
            }

            var mi = propertyInfo.GetGetMethod();
            var met = new DynamicMethod(
                "get_" + hashKey, 
                typeof(object), 
                new[] { typeof(object) }, 
                typeof(DbSession).Module, 
                true);
            var il = met.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); // instance           
            il.Emit(OpCodes.Call, mi); // call getter
            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);
            getter =
                (Func<object, object>)met.CreateDelegate(Expression.GetFuncType(typeof(object), typeof(object)));
            cacheGet.TryAdd(hashKey, getter);

            return getter(obj);
        }

        /// <summary>
        /// The set value fast.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetValueFast(this PropertyInfo propertyInfo, object obj, object value)
        {
            Setter setter = null;

            if (cache == null)
            {
                cache = new ConcurrentDictionary<int, Setter>();
            }

            var key = propertyInfo.GetHashCode();

            if (!cache.TryGetValue(key, out setter))
            {
                var mi = propertyInfo.GetSetMethod();
                var met = new DynamicMethod(
                    "set_" + key, 
                    typeof(void), 
                    new[] { typeof(object), typeof(object) }, 
                    typeof(DbSession).Module, 
                    true);
                var il = met.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // instance           
                il.Emit(OpCodes.Ldarg_1); // value
                if (propertyInfo.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                }

                il.Emit(OpCodes.Call, mi);
                il.Emit(OpCodes.Ret);
                setter = (Setter)met.CreateDelegate(typeof(Setter));
                cache.TryAdd(key, setter);
            }

            setter(obj, value);
        }

        #endregion
    }
}