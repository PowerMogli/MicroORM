using System;
using System.Data;
//using LinFu.DynamicProxy;
using MicroORM.Mapping;
using MicroORM.Storage;
using MicroORM.Entity;
using MicroORM.Caching;

namespace MicroORM.Materialization
{
    class EntityMaterializer
    {
        //private ProxyFactory _proxyFactory = new ProxyFactory();
        private IDbProvider _dbProvider;
        private EntityInfo _entityInfo;
        private CheckSumBuilder _checkSumBuilder;

        internal EntityMaterializer(IDbProvider provider)
        {
            _dbProvider = provider;
            _checkSumBuilder = new CheckSumBuilder();
        }

        internal T Materialize<T>(T entity, DataReaderSchema dataReaderSchema, IDataRecord dataRecord)
        {
            TableInfo tableInfo = TableInfo<T>.GetTableInfo;
            _entityInfo = ReferenceCacheManager.GetEntityInfo<T>(entity);

            for (int index = 0; index < tableInfo.Columns.Count; index++)
            {
                IPropertyInfo memberInfo = tableInfo.Columns[index];
                int columnIndex = dataReaderSchema.ColumnIndex(index);
                if (columnIndex < 0) continue;

                MaterializeEntity(entity, memberInfo, dataRecord[columnIndex]);
            }

            _entityInfo.Checksum = _checkSumBuilder.ToCheckSum();
            _entityInfo.EntityState = EntityState.Loaded;

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
