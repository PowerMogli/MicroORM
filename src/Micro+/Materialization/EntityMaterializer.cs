using System;
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
                IPropertyInfo memberInfo = tableInfo.Columns[index];
                int columnIndex = dataReaderSchema.ColumnIndex(index);
                if (columnIndex < 0) continue;

                MaterializeEntity(entity, memberInfo, dataRecord[columnIndex]);
            }

            return entity;
        }

        internal T Materialize<T>(DataReaderSchema dataReaderSchema, IDataRecord dataRecord)
        {
            T entity = Activator.CreateInstance<T>();
            return Materialize<T>(entity, dataReaderSchema, dataRecord);
        }

        private void MaterializeEntity(object entity, IPropertyInfo memberInfo, object value)
        {
            if (!memberInfo.CanWrite) return;

            if (Convert.IsDBNull(value))
                value = memberInfo.IsNullable ? null : _dbProvider.ResolveNullValue(value, memberInfo.PropertyType);

            memberInfo.SetValue(entity, value);
        }
    }
}
