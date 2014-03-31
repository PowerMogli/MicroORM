using RabbitDB.Mapping;
using RabbitDB.Query.Generic;
using System.Data;

namespace RabbitDB.Query
{
    internal class EntityQuery<T> : IQuery where T : Entity.Entity
    {
        internal T Entity { get; set; }

        internal EntityQuery(T entity)
        {
            this.Entity = entity;
        }

        public IDbCommand Compile(SqlDialect.SqlDialect sqlDialect)
        {
            TableInfo tableInfo = TableInfo<T>.GetTableInfo;
            object[] primaryKeyValues = tableInfo.GetPrimaryKeyValues(this.Entity);

            SqlQuery<T> sqlQuery = new SqlQuery<T>(primaryKeyValues, null);
            return sqlQuery.Compile(sqlDialect);
        }
    }
}