using RabbitDB.Entity;
using RabbitDB.Query;
using System;
using System.Collections.Generic;

namespace RabbitDB.Storage
{
    internal class DbEntityPersister
    {
        private IDbProvider _dbProvider;
        private IDbPersister _dbPersister;

        internal DbEntityPersister(IDbProvider dbProvider, IDbPersister dbPersister)
        {
            _dbProvider = dbProvider;
            _dbPersister = dbPersister;
        }

        /// <summary>
        /// This method is ONLY for POCOs 
        /// who inherit from Entity.Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        internal bool PersistChanges<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            if (entity.IsForDeletion())
            {
                return Delete(entity);
            }
            else if (entity.IsForInsert())
            {
                return Insert(entity);
            }
            else if (entity.IsForUpdate())
            {
                return Update(entity);
            }

            return false;
        }

        private bool Update<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            var tuple = entity.PrepareForUpdate<TEntity>(_dbProvider);
            if (tuple.Item1)
            {
                return Update<TEntity>(entity, new SqlQuery(tuple.Item2, tuple.Item3));
            }
            return false;
        }

        private bool Update<TEntity>(TEntity entity, SqlQuery sqlQuery) where TEntity : Entity.Entity
        {
            _dbPersister.Update<TEntity>(sqlQuery);
            entity.RaiseEntityUpdated();
            entity.EntityInfo.EntityState = EntityState.Updated;
            return true;
        }

        private bool Insert<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            _dbPersister.Insert(entity);
            entity.RaiseEntityInserted();
            entity.EntityInfo.EntityState = EntityState.Inserted;
            return true;
        }

        private bool Delete<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            _dbPersister.Delete(entity);
            entity.RaiseEntityDeleted();
            entity.EntityInfo.EntityState = EntityState.Deleted;
            return true;
        }
    }
}