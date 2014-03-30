using RabbitDB.Attributes;
using RabbitDB.Entity;
using System;
using System.Data;
using System.Reflection;

namespace RabbitDB.Mapping
{
    internal class TableInfoBuilder
    {
        private Type _entityType;
        private TableInfo _tableInfo;

        internal TableInfoBuilder(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            if (entityType.IsInterface)
                throw new TableInfoException(string.Format("Cannot create mapping for interface '{0}'!", entityType.FullName));

            _entityType = entityType;
        }

        internal TableInfo Build()
        {
            TableAttribute attribute = GetPersistentAttribute();
            _tableInfo = new TableInfo(_entityType, attribute);
            CreateMemberMappingsFor<ColumnAttribute>(_entityType, AddPropertyMetaInfo);
            //CreateMemberMappingsFor<PrimaryKeyAttribute>(entityType, AddPrimaryKeyInfo);

            return _tableInfo;
        }

        private TableAttribute GetPersistentAttribute()
        {
            TableAttribute attribute = (TableAttribute)Attribute.GetCustomAttribute(_entityType, typeof(TableAttribute), true);
            if (attribute == null)
                return new TableAttribute(_entityType.Name);

            if (attribute.EntityName == null)
                attribute.EntityName = _entityType.Name;

            return attribute;
        }

        private void CreateMemberMappingsFor<TAttribute>(Type entityType, Action<PropertyInfo, Type, ColumnAttribute> action) where TAttribute : ColumnAttribute
        {
            if (IsInvalidEntityType(entityType)) return;

            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                var attribute = GetAttribute<TAttribute>(propertyInfo);

                action(propertyInfo, entityType, attribute);
            }

            CreateMemberMappingsFor<TAttribute>(entityType.BaseType, action);
        }

        private bool IsInvalidEntityType(Type entityType)
        {
            return (entityType == null
                || entityType == typeof(NotifiedEntity)
                || entityType == typeof(Entity.Entity)
                || entityType == typeof(object));
        }

        private void AddPropertyMetaInfo(PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        {
            attribute = CreateAttribute<ColumnAttribute>(attribute, propertyInfo.Name);
            Type propertyType = GetPropertyType(entityType, propertyInfo);
            if (propertyType.IsEnum) attribute.DbType = DbType.Int32;

            _tableInfo.Columns.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute.NullDbType ?? TypeConverter.ToDbType(propertyType), attribute));
        }

        //private static void AddPrimaryKeyInfo(TableInfo tableInfo, PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        //{
        //    if (attribute == null) return;
        //    if (string.IsNullOrWhiteSpace(attribute.ColumnName))
        //        attribute.ColumnName = propertyInfo.Name;

        //    Type propertyType = GetPropertyType(entityType, propertyInfo);

        //    tableInfo.PrimaryKeys.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute));
        //}

        private ColumnAttribute CreateAttribute<TAttribute>(ColumnAttribute attribute, string name)
        {
            if (attribute == null)
                attribute = Activator.CreateInstance(typeof(TAttribute), name) as ColumnAttribute;

            if (string.IsNullOrWhiteSpace(attribute.ColumnName))
                attribute.ColumnName = name;

            return attribute;
        }

        private Type GetPropertyType(Type entityType, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsInterface)
            {
                object instance = Activator.CreateInstance(entityType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);

                object instanceValue = null;
                instanceValue = propertyInfo.GetValue(instance, null);

                if (instanceValue != null) return instanceValue.GetType();
            }

            return propertyInfo.PropertyType;
        }

        private TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(propertyInfo, typeof(TAttribute));
            if (attributes.Length == 0) return null;
            return (TAttribute)attributes[0];
        }
    }
}