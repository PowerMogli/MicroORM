using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using RabbitDB.Base;
using RabbitDB.Caching;
using RabbitDB.Mapping;
using RabbitDB.Storage;

namespace RabbitDB.Entity
{
    public class EntityCollection<TEntity> where TEntity : Entity
    {
        List<TEntity> _entityCollection = new List<TEntity>();
        private bool _loaded;
        private bool _trackChanges;

        public EntityCollection(bool trackChanges)
        {
            _trackChanges = trackChanges;
        }

        public EntityCollection()
            : this(true) { }

        /// <summary>
        /// Removes all entities from the collection
        /// and all collected information.
        /// </summary>
        public void Flush()
        {
            if (_entityCollection.Count <= 0) return;

            foreach (TEntity entity in _entityCollection)
            {
                EntityInfoCacheManager.RemoveFor(entity);
            }
            _entityCollection.Clear();
        }

        public void LoadAll()
        {
            if (_loaded) return;

            Tuple<string, DbEngine> result = EntityExtensions.InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(result.Item1, result.Item2))
            {
                EntitySet<TEntity> entitySet = dbSession.GetEntitySet<TEntity>();
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        public void LoadBy(string sql, params object[] arguments)
        {
            if (_loaded) return;

            Tuple<string, DbEngine> result = EntityExtensions.InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(result.Item1, result.Item2))
            {
                EntitySet<TEntity> entitySet = dbSession.GetEntitySet<TEntity>(sql, arguments);
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        public void LoadBy(Expression<Func<TEntity, bool>> criteria)
        {
            if (_loaded) return;

            Tuple<string, DbEngine> result = EntityExtensions.InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(result.Item1, result.Item2))
            {
                EntitySet<TEntity> entitySet = dbSession.GetEntitySet<TEntity>(criteria);
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        private void SetEntity(EntitySet<TEntity> entitySet)
        {
            if (_trackChanges == false)
            {
                _entityCollection.AddRange(entitySet);
                return;
            }

            foreach (TEntity entity in entitySet)
            {
                EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
                if (entityInfo.EntityState == EntityState.Loaded) continue;

                _entityCollection.Add(entity);
                entityInfo.EntityState = EntityState.Loaded;
                entityInfo.ComputeSnapshot(entity);
            }
        }

        public void LoadAll(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            if (_loaded) return;

            Tuple<string, DbEngine> result = EntityExtensions.InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(result.Item1, result.Item2))
            {
                EntityReader<TEntity> entityReader = dbSession.GetEntityReader<TEntity>();
                foreach (TEntity entity in entityReader.Load(materializer))
                {
                    EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
                    if (entityInfo.EntityState == EntityState.Loaded) continue;

                    _entityCollection.Add(entity);
                    entityInfo.EntityState = EntityState.Loaded;
                    entityInfo.ComputeSnapshot(entity);
                }
            }
            _loaded = true;
        }

        public void Reload()
        {
            _loaded = false;
            Flush();
            LoadAll();
        }

        public bool PersistChanges()
        {
            if (_entityCollection.Count <= 0 || _trackChanges == false) return false;

            bool persistResult = true;
            _entityCollection.ForEach(entity => persistResult &= EntityExtensions.PersistChanges(entity));

            return persistResult;
        }

        public void DeleteAll()
        {
            if (_entityCollection.Count <= 0) return;

            _entityCollection.ForEach(entity =>
            {
                entity.MarkedForDeletion = true;
                EntityExtensions.PersistChanges(entity);
            });
        }

        public TEntity FindByKey<TKey>(TKey key)
        {
            if (_entityCollection.Count <= 0) return default(TEntity);

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            try
            {
                return _entityCollection.FirstOrDefault(entity =>
                {
                    object[] primaryKeyValues = tableInfo.GetPrimaryKeyValues<TEntity>(entity);
                    return primaryKeyValues.Any(keyValue => keyValue.Equals(key));
                });
            }
            catch (Exception exception)
            {
                throw new PrimaryKeyException("Mismatch of primary key value type!", exception);
            }
        }
    }
}
