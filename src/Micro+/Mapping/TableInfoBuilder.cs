using System;
using System.Data;
using System.Reflection;
using MicroORM.Attributes;

namespace MicroORM.Mapping
{
    internal static class TableInfoBuilder
    {
        internal static TableInfo CreateTypeMapping(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");

            if (entityType.IsInterface)
                throw new TableInfoException(string.Format("Cannot create mapping for interface '{0}'! Please use TypeMapping.RegisterPersistentInterface to register the interface with persistent type.", entityType.FullName));

            TableAttribute attribute = GetPersistentAttribute(entityType);

            TableInfo tableInfo = new TableInfo(entityType, attribute.EntityName);
            CreateMemberMappingsFor<ColumnAttribute>(entityType, tableInfo, AddPropertyMetaInfo);
            //CreateMemberMappingsFor<PrimaryKeyAttribute>(entityType, tableInfo, AddPrimaryKeyInfo);

            return tableInfo;
        }

        private static TableAttribute GetPersistentAttribute(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException("entityType");

            TableAttribute attribute = (TableAttribute)Attribute.GetCustomAttribute(entityType, typeof(TableAttribute), true);
            if (attribute == null)
                return new TableAttribute(entityType.Name);

            if (attribute.EntityName == null)
                attribute.EntityName = entityType.Name;

            return attribute;
        }

        private static void CreateMemberMappingsFor<TAttribute>(Type entityType, TableInfo tableInfo, Action<TableInfo, PropertyInfo, Type, ColumnAttribute> action) where TAttribute : ColumnAttribute
        {
            if (entityType == null || entityType == typeof(object)) return;

            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                var attribute = GetAttribute<TAttribute>(propertyInfo);

                action(tableInfo, propertyInfo, entityType, attribute);
            }

            CreateMemberMappingsFor<TAttribute>(entityType.BaseType, tableInfo, action);
        }

        private static void AddPropertyMetaInfo(TableInfo tableInfo, PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        {
            attribute = CreateAttribute<ColumnAttribute>(attribute, propertyInfo.Name);
            Type propertyType = GetPropertyType(entityType, propertyInfo);
            if (propertyType.IsEnum) attribute.DbType = DbType.Int32;

            tableInfo.Columns.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute.NullDbType ?? TypeConverter.ToDbType(propertyType), attribute));
        }

        //private static void AddPrimaryKeyInfo(TableInfo tableInfo, PropertyInfo propertyInfo, Type entityType, ColumnAttribute attribute)
        //{
        //    if (attribute == null) return;
        //    if (string.IsNullOrWhiteSpace(attribute.ColumnName))
        //        attribute.ColumnName = propertyInfo.Name;

        //    Type propertyType = GetPropertyType(entityType, propertyInfo);

        //    tableInfo.PrimaryKeys.Add(new PropertyMetaInfo(propertyInfo, propertyType, attribute));
        //}

        private static ColumnAttribute CreateAttribute<TAttribute>(ColumnAttribute attribute, string name)
        {
            if (attribute == null)
                attribute = Activator.CreateInstance(typeof(TAttribute), name) as ColumnAttribute;

            if (string.IsNullOrWhiteSpace(attribute.ColumnName))
                attribute.ColumnName = name;

            return attribute;
        }

        private static Type GetPropertyType(Type entityType, PropertyInfo propertyInfo)
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

        private static TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(propertyInfo, typeof(TAttribute));
            if (attributes.Length == 0) return null;
            return (TAttribute)attributes[0];
        }
    }
}
