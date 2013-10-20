using MicroORM.Caching;
using MicroORM.Entity;
using MicroORM.Mapping;
using MicroORM.Materialization;
using MicroORM.Query;

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
            EntityInfo entityInfo = ReferenceCacheManager.GetEntityInfo(entity);

            if (entity.Delete)
            {
                if (entity is Entity.Entity && (entityInfo.EntityState == EntityState.Deleted || entityInfo.EntityState == EntityState.None))
                    return;

                Delete(entity, entityInfo);
            }
            else if (entityInfo.EntityState == EntityState.None || entityInfo.EntityState == EntityState.Deleted)
            {
                Insert(entity, entityInfo);
            }
            else
            {
                if (entityInfo.Checksum == CheckSumBuilder.CalculateEntityChecksum(entity))
                    return;

                Update(entity, entityInfo);
            }
        }

        private void Update<TEntity>(TEntity entity, EntityInfo entityInfo)
        {

        }

        internal void Delete<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string deleteStatement = tableInfo.CreateDeleteStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(tableInfo.GetPrimaryKeyValues(entity));

            _dbProvider.ExecuteCommand(new SqlQuery(deleteStatement, arguments));

            entityInfo.EntityState = EntityState.Deleted;
        }

        internal void Insert<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            string insertStatement = tableInfo.CreateInsertStatement(_dbProvider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(Utils.Utils.GetEntityArguments(entity, tableInfo));

            LastInsertId insertId = new LastInsertId(_dbProvider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments)));

            entityInfo.Checksum = CheckSumBuilder.CalculateEntityChecksum(entity);
            entityInfo.EntityState = EntityState.Inserted;
        }
    }
}
