using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;
using System.Collections.Generic;
using MicroORM.Reflection;
using System;
using LinFu.DynamicProxy;
using MicroORM.Storage;

namespace MicroORM.Materialization
{
    class EntityMaterializer
    {
        private ProxyFactory _proxyFactory = new ProxyFactory();
        private IDbProvider _dbProvider;

        internal EntityMaterializer(IDbProvider provider)
        {
            _dbProvider = provider;
        }

        internal T Materialize<T>(DataReaderSchema dataReaderSchema, IDataRecord dataRecord)
        {
            T entity = Activator.CreateInstance<T>();
            TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));

            for (int index = 0; index < tableInfo.Members.Count; index++)
            {
                IMemberInfo memberInfo = tableInfo.Members[index];
                int columnIndex = dataReaderSchema.ColumnIndex(index);
                if (columnIndex < 0) continue;

                MaterializeEntity(entity, memberInfo, dataRecord[columnIndex]);
            }

            return entity;
        }

        private void MaterializeEntity(object entity, IMemberInfo memberInfo, object value)
        {
            if (!memberInfo.CanWrite) return;

            if (Convert.IsDBNull(value))
                value = memberInfo.IsNullable ? null : _dbProvider.ResolveStorageNullValue(value, memberInfo.MemberType);

            memberInfo.SetValue(entity, value);
        }
    }
}
