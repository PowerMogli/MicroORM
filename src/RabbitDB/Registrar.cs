using System;
using System.Collections.Concurrent;

namespace RabbitDB.Base
{
    public static class Registrar<T>
    {
        private static ConcurrentDictionary<string, T> _container = new ConcurrentDictionary<string, T>();

        public static void Flush()
        {
            _container.Clear();
        }

        /// <summary>
        /// Registers a value for the given namespace.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Register(string nameSpace, T value)
        {
            if (_container.ContainsKey(nameSpace)) return false;

            return _container.TryAdd(nameSpace, value);
        }

        internal static T GetFor(Type entityType)
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
