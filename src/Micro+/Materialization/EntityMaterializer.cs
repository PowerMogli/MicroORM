using System;
using System.Collections.Generic;
using System.Data;
//using LinFu.DynamicProxy;
using MicroORM.Mapping;
using MicroORM.Storage;

namespace MicroORM.Materialization
{
    class EntityMaterializer
    {
        //private ProxyFactory _proxyFactory = new ProxyFactory();
        private IDbProvider _dbProvider;

        internal EntityMaterializer(IDbProvider provider)
        {
            _dbProvider = provider;
        }

        internal T Materialize<T>(T entity, DataReaderSchema dataReaderSchema, IDataRecord dataRecord)
        {
            TableInfo tableInfo = TableInfo<T>.GetTableInfo;

            for (int index = 0; index < tableInfo.Columns.Count; index++)
            {
                IPropertyInfo propertyInfo = tableInfo.Columns[index];
                int columnIndex = dataReaderSchema.ColumnIndex(index);
                if (columnIndex < 0) continue;

                MaterializeEntity(entity, propertyInfo, dataRecord[columnIndex]);
            }
            return entity;
        }

        internal T Materialize<T>(DataReaderSchema dataReaderSchema, IDataRecord dataRecord)
        {
            T entity = Activator.CreateInstance<T>();
            return Materialize<T>(entity, dataReaderSchema, dataRecord);
        }

        private void MaterializeEntity(object entity, IPropertyInfo propertyInfo, object value)
        {
            if (!propertyInfo.CanWrite) return;

            if (Convert.IsDBNull(value))
                value = propertyInfo.IsNullable ? null : _dbProvider.ResolveNullValue(value, propertyInfo.PropertyType);

            propertyInfo.SetValue(entity, value);
            SaveToEntityCollection(entity, propertyInfo.Name, value);
        }

        private void SaveToEntityCollection(object entity, string name, object value)
        {
            Entity.Entity _entity = entity as Entity.Entity;
            if (_entity == null) return;

            _entity.EntityInfo.EntityValueCollection.Add(new KeyValuePair<string, object>(name, value));
        }
    }
}
