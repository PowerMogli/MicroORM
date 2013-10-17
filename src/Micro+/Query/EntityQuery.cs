using System.Data;
using MicroORM.Base.Entity;
using MicroORM.Mapping;
using MicroORM.Query.Generic;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class EntityQuery<T> : IQuery where T : Entity
    {
        internal T Entity { get; set; }

        internal EntityQuery(T entity)
        {
            this.Entity = entity;
        }

        public IDbCommand Compile(IDbProvider provider)
        {
            TableInfo tableInfo = TableInfo.GetTableInfo(this.Entity.GetType());
            object[] primaryKey = tableInfo.GetPrimaryKeys(this.Entity);

            SqlQuery<T> sqlQuery = new SqlQuery<T>(primaryKey, null);
            return sqlQuery.Compile(provider);
        }
    }
}
