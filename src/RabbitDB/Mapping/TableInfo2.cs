// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfo2.cs" company="">
//   
// </copyright>
// <summary>
//   The table info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;

    using RabbitDB.Schema;

    /// <summary>
    /// The table info.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal static class TableInfo<T>
    {
        #region Properties

        /// <summary>
        /// Gets the get table info.
        /// </summary>
        internal static TableInfo GetTableInfo
        {
            get
            {
                if (InternalTableInfo != null)
                {
                    return InternalTableInfo;
                }

                InternalTableInfo = GetInternalTableInfo(typeof(T));
                if (InternalTableInfo == null)
                {
                    return null;
                }

                InternalTableInfo.DbTable = DbSchemaAllocator<T>.DbTable;
                return InternalTableInfo;
            }
        }

        /// <summary>
        /// Gets or sets the internal table info.
        /// </summary>
        private static TableInfo InternalTableInfo { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the persistent type from the given type.
        /// </summary>
        /// <param name="entityType">
        /// The type that's persistent type is returned.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        internal static Type GetEntityType(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            // If not an interface return the given type.
            if (!entityType.IsInterface)
            {
                return entityType;
            }

            // Throw an exception if the interface is not registered with a persistent.
            throw new TableInfoException(
                string.Format("There is no entity type registered for the interface type: {0}.", entityType.FullName));
        }

        /// <summary>
        /// Returns the mapping for a given object. If the mapping does not exist it is created by this routine.
        /// </summary>
        /// <param name="entity">
        /// The object the mapping is returned.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        internal static TableInfo GetInternalTableInfo(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return GetInternalTableInfo(entity.GetType());
        }

        /// <summary>
        /// Returns the mapping for the given entity type. If the mapping does not exist it is 
        /// created by this routine.
        /// </summary>
        /// <param name="entityType">
        /// Type of object the mapping is returned.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        internal static TableInfo GetInternalTableInfo(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            if (entityType == typeof(string)
                || entityType.IsValueType
                || entityType.IsEnum)
            {
                return null;
            }

            // Get the real entity type.
            entityType = GetEntityType(entityType);

            return new TableInfoBuilder(entityType).Build();
        }

        #endregion
    }
}