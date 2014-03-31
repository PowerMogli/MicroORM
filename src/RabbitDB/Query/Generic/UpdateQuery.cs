using RabbitDB.Entity;
using RabbitDB.Mapping;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace RabbitDB.Query.Generic
{
    internal class UpdateQuery<TEntity> : IQuery
    {
        private TEntity _entity;

        internal UpdateQuery(TEntity entity)
        {
            _entity = entity;
        }

        public IDbCommand Compile(SqlDialect.SqlDialect sqlDialect)
        {
            var valuesToUpdate = new EntityArgumentsReader().GetEntityArguments(_entity, TableInfo<TEntity>.GetTableInfo);
            if (valuesToUpdate == null || valuesToUpdate.Length <= 0)
                throw new InvalidOperationException("Entity had no properties provided!");

            Tuple<string, QueryParameterCollection> result = _entity.PrepareForUpdate(valuesToUpdate[0] as KeyValuePair<string, object>[]);

            SqlQuery sqlQuery = new SqlQuery(result.Item1, result.Item2);
            return sqlQuery.Compile(sqlDialect);
        }
    }
}