using System;
using System.Collections.Generic;
using RabbitDB.Caching;
using RabbitDB.Entity;
using RabbitDB.Mapping;
using RabbitDB.Query;

namespace RabbitDB.Storage
{
    internal class DbEntityPersister
    {
        private IDbProvider _dbProvider;

        internal DbEntityPersister(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        /// <summary>
        /// This method is ONLY for POCOs 
        /// who inherit from Entity.Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        internal bool PersistChanges<TEntity>(TEntity entity, bool isToDelete = false) where TEntity : Entity.Entity
        {
            EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            if (isToDelete)
            {
                // Entity was already deleted
                if (entityInfo.EntityState == EntityState.Deleted)
                    return false;

                Delete(entity);
                entityInfo.EntityState = EntityState.Deleted;
                return true;
            }
            // Entity was already deleted
            // or is not yet loaded!!
            else if (entityInfo.EntityState == EntityState.Deleted || entityInfo.EntityState == EntityState.None)
            {
                Insert(entity);
                entityInfo.EntityState = EntityState.Inserted;
                return true;
            }
            else if (entityInfo.EntityState == EntityState.Loaded
                || entityInfo.EntityState == EntityState.Inserted
                || entityInfo.EntityState == EntityState.Updated)
            {
                Tuple<bool, string, QueryParameterCollection> tuple = PrepareForUpdate<TEntity>(entity, entityInfo);
                if (tuple.Item1 == false) return false;

                Update<TEntity>(new SqlQuery(tuple.Item2, tuple.Item3));
                entityInfo.EntityState = EntityState.Updated;
                return true;
            }

            return false;
        }

        private Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(TEntity entity, EntityInfo entityInfo) where TEntity : Entity.Entity
        {
            // Any changes made to entity?!
            KeyValuePair<string, object>[] valuesToUpdate = entityInfo.ComputeUpdateValues(entity);

            if (valuesToUpdate == null || valuesToUpdate.Length == 0)
                return new Tuple<bool, string, QueryParameterCollection>(false, null, null);

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string updateStatement = tableInfo.CreateUpdateStatement(_dbProvider, valuesToUpdate);
            QueryParameterCollection queryParameterCollection = QueryParameterCollection.Create<TEntity>(new object[] { valuesToUpdate });
            queryParameterCollection.AddRange(tableInfo.GetPrimaryKeyValues<TEntity>(entity));

            return new Tuple<bool, string, QueryParameterCollection>(true, updateStatement, queryParameterCollection);
        }

        internal void Update<TEntity>(IQuery query)
        {
            _dbProvider.ExecuteCommand(query);
        }

        internal void Delete<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string deleteStatement = tableInfo.CreateDeleteStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(tableInfo.GetPrimaryKeyValues(entity));

            _dbProvider.ExecuteCommand(new SqlQuery(deleteStatement, arguments));
        }

        internal void Insert<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string insertStatement = tableInfo.CreateInsertStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(Utils.Utils.GetEntityArguments(entity, tableInfo));

            object insertId = _dbProvider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments));
            tableInfo.SetAutoNumber<TEntity>(entity, insertId);
        }
    }
}
