using System;
using System.Collections.Generic;
using RabbitDB.Caching;
using RabbitDB.Entity;
using RabbitDB.Mapping;
using RabbitDB.Materialization;
using RabbitDB.Query;
using RabbitDB.Utils;

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
        internal bool PersistChanges<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            var entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            if (entity.MarkedForDeletion
                && entityInfo.EntityState != EntityState.Deleted)
            {
                return Delete(entity, entityInfo);
            }
            // Entity was already deleted
            // or is not yet loaded!!
            else if (entityInfo.EntityState == EntityState.Deleted || entityInfo.EntityState == EntityState.None)
            {
                return Insert(entity, entityInfo);
            }
            else if (entityInfo.EntityState != EntityState.Deleted)
            {
                return Update(entity, entityInfo);
            }

            return false;
        }

        private bool Update<TEntity>(TEntity entity, EntityInfo entityInfo) where TEntity : Entity.Entity
        {
            var tuple = PrepareForUpdate<TEntity>(entity, entityInfo);
            if (tuple.Item1)
            {
                return Update<TEntity>(entity, entityInfo, new SqlQuery(tuple.Item2, tuple.Item3));
            }
            return false;
        }

        private bool Update<TEntity>(TEntity entity, EntityInfo entityInfo, SqlQuery sqlQuery) where TEntity : Entity.Entity
        {
            Update<TEntity>(sqlQuery);
            entity.RaiseEntityUpdated();
            entityInfo.EntityState = EntityState.Updated;
            return true;
        }

        private bool Insert<TEntity>(TEntity entity, EntityInfo entityInfo) where TEntity : Entity.Entity
        {
            Insert(entity);
            entity.RaiseEntityInserted();
            entityInfo.EntityState = EntityState.Inserted;
            return true;
        }

        private bool Delete<TEntity>(TEntity entity, EntityInfo entityInfo) where TEntity : Entity.Entity
        {
            Delete(entity);
            entity.RaiseEntityDeleted();
            entityInfo.EntityState = EntityState.Deleted;
            return true;
        }

        private Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            // Any changes made to entity?!
            KeyValuePair<string, object>[] valuesToUpdate = entityInfo.ComputeValuesToUpdate();

            if (valuesToUpdate == null || valuesToUpdate.Length == 0)
                return new Tuple<bool, string, QueryParameterCollection>(false, null, null);

            Tuple<string, QueryParameterCollection> result = entity.PrepareForUpdate(_dbProvider, valuesToUpdate);
            return new Tuple<bool, string, QueryParameterCollection>(true, result.Item1, result.Item2);
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
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(new EntityArgumentsReader().GetEntityArguments(entity, tableInfo));

            object insertId = _dbProvider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments));
            tableInfo.SetAutoNumber<TEntity>(entity, insertId);
        }
    }
}