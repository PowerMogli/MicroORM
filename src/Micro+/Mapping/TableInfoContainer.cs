using System;
using System.Collections.Concurrent;

namespace MicroORM.Mapping
{
    internal static class TableInfoContainer
    {
        private static ConcurrentDictionary<Type, TableInfo> _mappings = new ConcurrentDictionary<Type, TableInfo>();
        private static ConcurrentDictionary<Type, Type> _interfacePersistents = new ConcurrentDictionary<Type, Type>();
        private static TableInfo _lastMapping;

        /// <summary>
        /// Returns the mapping for a given object. If the mapping does not exist it is created by this routine.
        /// </summary>
        /// <param name="obj">The object the mapping is returned.</param>
        public static TableInfo GetTableInfo(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            return GetTableInfo(obj.GetType());
        }

        /// <summary>
        /// Returns the mapping for the given persistent type. If the mapping does not exist it is 
        /// created by this routine.
        /// </summary>
        /// <param name="type">Type of object the mapping is returned.</param>
        public static TableInfo GetTableInfo(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type == typeof(string) || type.IsValueType || type.IsEnum) return null;

            // Get the real persistent type.
            type = GetPersistentType(type);

            TableInfo tableInfo = null;

            if (_lastMapping != null && _lastMapping.PersistentType == type) return _lastMapping;

            if (!_mappings.TryGetValue(type, out tableInfo))
            {
                tableInfo = TableInfoBuilder.CreateTypeMapping(type);
                _mappings.TryAdd(type, tableInfo);
            }

            return _lastMapping = tableInfo;
        }

        /// <summary>
        /// Gets the persistent type from the given type.
        /// </summary>
        /// <param name="type">The type that's persistent type is returned.</param>
        public static Type GetPersistentType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // If not an interface return the given type.
            if (!type.IsInterface)
                return type;

            // Get the persistent type for the interface.
            Type t = null;
            if (_interfacePersistents.TryGetValue(type, out t))
                return t;

            // Throw an exception if the interface is not registered with a persistent.
            throw new TableInfoException(string.Format("There is no persistent type registered for the interface type: {0}.", type.FullName));
        }
    }
}
