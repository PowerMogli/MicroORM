using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using MicroORM.Base;

namespace MicroORM.Reflection
{
    internal static class ReflectionExtensions
    {
        private delegate void Setter(object dest, object value);

        private static ConcurrentDictionary<int, Setter> _cache;
        //static object setLock=new object();

        /// <summary>
        /// Fast setter. aprox 8x faster than simple Reflection
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetValueFast(this PropertyInfo propertyInfo, object obj, object value)
        {
            Setter inv = null;

            if (_cache == null)
            {
                _cache = new ConcurrentDictionary<int, Setter>();
            }
            var key = propertyInfo.GetHashCode();

            if (!_cache.TryGetValue(key, out inv))
            {
                var mi = propertyInfo.GetSetMethod();
                DynamicMethod met = new DynamicMethod("set_" + key, typeof(void), new[] { typeof(object), typeof(object) }, typeof(Entity).Module, true);
                var il = met.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);//instance           
                il.Emit(OpCodes.Ldarg_1);//value
                if (propertyInfo.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                }
                il.Emit(OpCodes.Call, mi);
                il.Emit(OpCodes.Ret);
                inv = (Setter)met.CreateDelegate(typeof(Setter));
                _cache.TryAdd(key, inv);

            }

            inv(obj, value);
        }
    }
}
