using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Base
{
    public abstract class Registrar<T>
    {
        protected static ConcurrentDictionary<string, T> _container = new ConcurrentDictionary<string, T>();
        public static bool Register(string entityType, T value)
        {
            if (_container.ContainsKey(entityType)) return false;

            return _container.TryAdd(entityType, value);
        }

        public static T GetFor(string entityType)
        {
            T value = default(T);

            if (_container.TryGetValue(entityType, out value)) return value;

            return default(T);
        }
    }
}
