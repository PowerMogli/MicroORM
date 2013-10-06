using System;
using System.Collections.Concurrent;

namespace MicroORM.Base.Mapping
{
    internal static class TypeMappingContainer
    {
        private static ConcurrentDictionary<Type, TypeMapping> _mappings = new ConcurrentDictionary<Type, TypeMapping>();
        private static ConcurrentDictionary<Type, Type> _interfacePersistents = new ConcurrentDictionary<Type, Type>();
        private static TypeMapping _lastMapping;

        /// <summary>
        /// Returns the mapping for a given object. If the mapping does not exist it is created by this routine.
        /// </summary>
        /// <param name="obj">The object the mapping is returned.</param>
        public static TypeMapping GetTypeMapping(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            return GetTypeMapping(obj.GetType());
        }

        /// <summary>
        /// Returns the mapping for the given persistent type. If the mapping does not exist it is 
        /// created by this routine.
        /// </summary>
        /// <param name="type">Type of object the mapping is returned.</param>
        public static TypeMapping GetTypeMapping(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            // Get the real persistent type.
            type = GetPersistentType(type);

            TypeMapping typeMapping = null;

            if (_lastMapping != null && _lastMapping.PersistentType == type) return _lastMapping;

            if (!_mappings.TryGetValue(type, out typeMapping))
            {
                typeMapping = TypeMappingBuilder.CreateTypeMapping(type);
                _mappings.TryAdd(type, typeMapping);
            }

            return _lastMapping = typeMapping;
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
            throw new TypeMappingException(string.Format("There is no persistent type registered for the interface type: {0}.", type.FullName));
        }
    }
}
