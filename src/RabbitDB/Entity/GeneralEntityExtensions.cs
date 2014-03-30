using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.SqlBuilder;
using RabbitDB.Storage;
using System;
using System.Collections.Generic;

namespace RabbitDB.Entity
{
    internal static class GeneralEntityExtensions
    {
        internal static Tuple<string, QueryParameterCollection> PrepareForUpdate<TEntity>(this TEntity entity, IDbProvider dbProvider, KeyValuePair<string, object>[] valuesToUpdate)
        {
            string updateStatement = SqlBuilder<TEntity>.GetUpdateStatement(valuesToUpdate);
            QueryParameterCollection queryParameterCollection = QueryParameterCollection.Create<TEntity>(new object[] { valuesToUpdate });
            
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            queryParameterCollection.AddRange(tableInfo.GetPrimaryKeyValues<TEntity>(entity));

            return new Tuple<string, QueryParameterCollection>(updateStatement, queryParameterCollection);
        }

        internal static Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(this TEntity entity, IDbProvider dbProvider) where TEntity : Entity
        {
            // Any changes made to entity?!
            KeyValuePair<string, object>[] valuesToUpdate = entity.ComputeValuesToUpdate();

            if (valuesToUpdate == null || valuesToUpdate.Length == 0)
                return new Tuple<bool, string, QueryParameterCollection>(false, null, null);

            Tuple<string, QueryParameterCollection> result = entity.PrepareForUpdate(dbProvider, valuesToUpdate);
            return new Tuple<bool, string, QueryParameterCollection>(true, result.Item1, result.Item2);
        }
    }
}