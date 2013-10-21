using System.Collections.Generic;
using MicroORM.Entity;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Reflection;
using System;

namespace MicroORM.Storage
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
        internal void PersistChanges<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            if (entity.Delete)
            {
                // Entity was already deleted
                // or is not yet loaded!!
                if (entity.EntityInfo.EntityState == EntityState.Deleted || entity.EntityInfo.EntityState == EntityState.None)
                    return;

                Delete(entity);
                entity.EntityInfo.EntityState = EntityState.Deleted;
            }
            // Entity was already deleted
            // or is not yet loaded!!
            else if (entity.EntityInfo.EntityState == EntityState.Deleted || entity.EntityInfo.EntityState == EntityState.None)
            {
                Insert(entity);
                entity.EntityInfo.EntityState = EntityState.Inserted;
            }
            else
            {
                Tuple<bool, string, QueryParameterCollection> tuple = PrepareForUpdate<TEntity>(entity);
                if (tuple.Item1) return;

                Update<TEntity>(new SqlQuery(tuple.Item2, tuple.Item3));
                entity.EntityInfo.EntityState = EntityState.Updated;
            }
        }

        internal Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(TEntity entity)
        {
            // Any changes made to entity?!
            Entity.Entity dbEntity = entity as Entity.Entity;
            KeyValuePair<string, object>[] valuesToUpdate = 
                EntityValueComparer.ComputeChangedValues<TEntity>(
                ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity }),
                dbEntity != null ? dbEntity.EntityInfo.EntityValueCollection : new EntityValueCollection());

            if (valuesToUpdate.Length == 0)
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

            LastInsertId insertId = new LastInsertId(_dbProvider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments)));
        }
    }
}