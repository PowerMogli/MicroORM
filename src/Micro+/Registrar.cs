using System;
using System.Collections.Concurrent;

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

        public static T GetFor(Type entityType)
        {
            T value = default(T);

            string nameSpace = entityType.ToString();

            while (true)
            {
                nameSpace = string.Concat(nameSpace, ".*");

                if (_container.TryGetValue(nameSpace, out value) || nameSpace == ".*") break;

                int lastIndexOf = nameSpace.LastIndexOf('.', nameSpace.Length - 3);
                if (lastIndexOf < 0) nameSpace = ".*";
                else
                    nameSpace = nameSpace.Substring(0, lastIndexOf);
            }
            return value != null ? value : default(T);
        }
    }
}
