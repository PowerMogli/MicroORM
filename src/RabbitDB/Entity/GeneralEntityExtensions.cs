using System;
using System.Collections.Generic;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Storage;

namespace RabbitDB.Entity
{
    internal static class GeneralEntityExtensions
    {
        internal static Tuple<string, QueryParameterCollection> PrepareForUpdate<TEntity>(this TEntity entity, IDbProvider dbProvider, KeyValuePair<string, object>[] valuesToUpdate)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string updateStatement = tableInfo.CreateUpdateStatement(dbProvider, valuesToUpdate);
            QueryParameterCollection queryParameterCollection = QueryParameterCollection.Create<TEntity>(new object[] { valuesToUpdate });
            queryParameterCollection.AddRange(tableInfo.GetPrimaryKeyValues<TEntity>(entity));

            return new Tuple<string, QueryParameterCollection>(updateStatement, queryParameterCollection);
        }
    }
}