// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfoBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The table info builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;
using System.Reflection;

using RabbitDB.Contracts.Attributes;
using RabbitDB.Entity.Entity;

#endregion

namespace RabbitDB.Mapping
{
    /// <summary>
    ///     The table info builder.
    /// </summary>
    internal class TableInfoBuilder
    {
        #region Fields

        /// <summary>
        ///     The _entity type.
        /// </summary>
        private readonly Type _entityType;

        /// <summary>
        ///     The _table info.
        /// </summary>
        private TableInfo _tableInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableInfoBuilder" /> class.
        /// </summary>
        /// <param name="entityType">
        ///     The entity type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="TableInfoException">
        /// </exception>
        internal TableInfoBuilder(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }

            if (entityType.IsInterface)
            {
                throw new TableInfoException($"Cannot create mapping for interface '{entityType.FullName}'!");
            }

            _entityType = entityType;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The build.
        /// </summary>
        /// <returns>
        ///     The <see cref="TableInfo" />.
        /// </returns>
        internal TableInfo Build()
        {
            TableAttribute attribute = GetPersistentAttribute();

            _tableInfo = new TableInfo(_entityType, attribute);

            CreateMemberMappingsFor<ColumnAttribute>(_entityType, AddPropertyMetaInfo);

            // CreateMemberMappingsFor<PrimaryKeyAttribute>(entityType, AddPrimaryKeyInfo);
            return _tableInfo;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The add property meta info.
        /// </summary>
        /// <param name="propertyInfo">
        ///     The property info.
        /// </param>
        /// <param name="entityType">
        ///     The entity type.
        /// </param>
        /// <param name="attribute">
        ///     The attribute.
        /// </param>
        private void AddPropertyMetaInfo(PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        {
            attribute = CreateAttribute<ColumnAttribute>(attribute, propertyInfo.Name);
            Type propertyType = GetPropertyType(entityType, propertyInfo);

            if (propertyType.IsEnum)
            {
                attribute.DbType = DbType.Int32;
            }

            _tableInfo.Columns.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute.NullDbType ?? TypeConverter.ToDbType(propertyType), attribute));
        }

        // private static void AddPrimaryKeyInfo(TableInfo tableInfo, PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        // {
        // if (attribute == null) return;
        // if (string.IsNullOrWhiteSpace(attribute.ColumnName))
        // attribute.ColumnName = propertyInfo.Name;

        // Type propertyType = GetPropertyType(entityType, propertyInfo);

        // tableInfo.PrimaryKeys.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute));
        // }

        /// <summary>
        ///     The create attribute.
        /// </summary>
        /// <param name="attribute">
        ///     The attribute.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="ColumnAttribute" />.
        /// </returns>
        private static ColumnAttribute CreateAttribute<TAttribute>(ColumnAttribute attribute, string name)
        {
            if (attribute == null)
            {
                attribute = Activator.CreateInstance(typeof(TAttribute), name) as ColumnAttribute;
            }

            if (attribute != null && string.IsNullOrWhiteSpace(attribute.ColumnName))
            {
                attribute.ColumnName = name;
            }

            return attribute;
        }

        /// <summary>
        ///     The create member mappings for.
        /// </summary>
        /// <param name="entityType">
        ///     The entity type.
        /// </param>
        /// <param name="action">
        ///     The action.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        private static void CreateMemberMappingsFor<TAttribute>(Type entityType, Action<PropertyInfo, Type, ColumnAttribute> action) where TAttribute : ColumnAttribute
        {
            if (IsInvalidEntityType(entityType))
            {
                return;
            }

            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                TAttribute attribute = GetAttribute<TAttribute>(propertyInfo);

                action(propertyInfo, entityType, attribute);
            }

            CreateMemberMappingsFor<TAttribute>(entityType.BaseType, action);
        }

        /// <summary>
        ///     The get attribute.
        /// </summary>
        /// <param name="propertyInfo">
        ///     The property info.
        /// </param>
        /// <typeparam name="TAttribute">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TAttribute" />.
        /// </returns>
        private static TAttribute GetAttribute<TAttribute>(MemberInfo propertyInfo) where TAttribute : Attribute
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(propertyInfo, typeof(TAttribute));

            if (attributes.Length == 0)
            {
                return null;
            }

            return (TAttribute)attributes[0];
        }

        /// <summary>
        ///     The get persistent attribute.
        /// </summary>
        /// <returns>
        ///     The <see cref="TableAttribute" />.
        /// </returns>
        private TableAttribute GetPersistentAttribute()
        {
            TableAttribute attribute = (TableAttribute)Attribute.GetCustomAttribute(_entityType, typeof(TableAttribute), true);

            if (attribute == null)
            {
                return new TableAttribute(_entityType.Name);
            }

            if (attribute.EntityName == null)
            {
                attribute.EntityName = _entityType.Name;
            }

            return attribute;
        }

        /// <summary>
        ///     The get property type.
        /// </summary>
        /// <param name="entityType">
        ///     The entity type.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property info.
        /// </param>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        private static Type GetPropertyType(Type entityType, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsInterface)
            {
                return propertyInfo.PropertyType;
            }

            object instance = Activator.CreateInstance(
                entityType,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null,
                null,
                null);

            object instanceValue = propertyInfo.GetValue(instance, null);

            return instanceValue?.GetType() ?? propertyInfo.PropertyType;
        }

        /// <summary>
        ///     The is invalid entity type.
        /// </summary>
        /// <param name="entityType">
        ///     The entity type.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool IsInvalidEntityType(Type entityType)
        {
            return entityType == null
                   || entityType == typeof(NotifiedEntity)
                   || entityType == typeof(Entity.Entity.Entity)
                   || entityType == typeof(object);
        }

        #endregion
    }
}