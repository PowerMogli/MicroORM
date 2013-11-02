using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using RabbitDB.Base;
using RabbitDB.Caching;
using RabbitDB.Mapping;
using RabbitDB.Reader;
using RabbitDB.Storage;

namespace RabbitDB.Entity
{
    public class EntityCollection<TEntity> : Collection<TEntity> where TEntity : Entity
    {
        private bool _loaded;

        public EntityCollection()
            : base(new List<TEntity>()) { }

        /// <summary>
        /// Removes all entities from the collection
        /// and all collected information.
        /// </summary>
        public void Flush()
        {
            if (base.Count <= 0) return;

            foreach (TEntity entity in this)
            {
                EntityInfoCacheManager.RemoveFor(entity);
            }
            base.Clear();
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
            if (SavedNoTrackingEntities(entitySet)) return;

            foreach (TEntity entity in entitySet)
            {
                EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
                if (entityInfo.EntityState == EntityState.Loaded) continue;

                base.Add(entity);
                entityInfo.EntityState = EntityState.Loaded;
                entityInfo.ComputeSnapshot(entity);
            }
        }

        private bool SavedNoTrackingEntities(EntitySet<TEntity> entitySet)
        {
            if (DbSession.Configuration.AutoDetectChangesEnabled) return false;

            ((List<TEntity>)base.Items).AddRange(entitySet);

            return true;
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
                    if (DbSession.Configuration.AutoDetectChangesEnabled)
                    {
                        EntityInfo entityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
                        if (entityInfo.EntityState == EntityState.Loaded) continue;
                        entityInfo.EntityState = EntityState.Loaded;
                        entityInfo.ComputeSnapshot(entity);
                    }
                    base.Add(entity);
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
            if (base.Count <= 0) return false;

            bool persistResult = true;
            ((List<TEntity>)base.Items).ForEach(entity => persistResult &= EntityExtensions.PersistChanges(entity));

            return persistResult;
        }

        public bool DeleteAll()
        {
            if (base.Count <= 0) return false;

            bool persistResult = true;
            ((List<TEntity>)base.Items).ForEach(entity =>
            {
                entity.MarkedForDeletion = true;
                persistResult &= EntityExtensions.PersistChanges(entity);
            });
            return persistResult;
        }

        public TEntity FindByKey<TKey>(TKey key)
        {
            if (base.Count <= 0) return default(TEntity);

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            try
            {
                return ((List<TEntity>)base.Items).FirstOrDefault(entity =>
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