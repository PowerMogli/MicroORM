using System;
using System.Collections.Concurrent;

namespace RabbitDB.Mapping
{
    internal static class TableInfoContainer
    {
        private static ConcurrentDictionary<Type, TableInfo> _mappings = new ConcurrentDictionary<Type, TableInfo>();
        private static ConcurrentDictionary<Type, Type> _interfacePersistents = new ConcurrentDictionary<Type, Type>();
        private static TableInfo _lastMapping;

        /// <summary>
        /// Returns the mapping for a given object. If the mapping does not exist it is created by this routine.
        /// </summary>
        /// <param name="entity">The object the mapping is returned.</param>
        public static TableInfo GetTableInfo(object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            return GetTableInfo(entity.GetType());
        }

        /// <summary>
        /// Returns the mapping for the given persistent type. If the mapping does not exist it is 
        /// created by this routine.
        /// </summary>
        /// <param name="entityType">Type of object the mapping is returned.</param>
        public static TableInfo GetTableInfo(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException("entityType");

            if (entityType == typeof(string)
                || entityType.IsValueType
                || entityType.IsEnum) { return null; }

            // Get the real persistent type.
            entityType = GetPersistentType(entityType);

            if (_lastMapping != null
                && _lastMapping.EntityType == entityType) { return _lastMapping; }

            TableInfo tableInfo = null;
            if (!_mappings.TryGetValue(entityType, out tableInfo))
            {
                tableInfo = TableInfoBuilder.CreateTypeMapping(entityType);
                _mappings.TryAdd(entityType, tableInfo);
            }

            return _lastMapping = tableInfo;
        }

        /// <summary>
        /// Gets the persistent type from the given type.
        /// </summary>
        /// <param name="entityType">The type that's persistent type is returned.</param>
        public static Type GetPersistentType(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            // If not an interface return the given type.
            if (!entityType.IsInterface) return entityType;

            // Get the persistent type for the interface.
            Type interfaceType = null;
            if (_interfacePersistents.TryGetValue(entityType, out interfaceType)) return interfaceType;

            // Throw an exception if the interface is not registered with a persistent.
            throw new TableInfoException(string.Format("There is no persistent type registered for the interface type: {0}.", entityType.FullName));
        }
    }
}
