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

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet<TEntity>();
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        public void LoadBy(string sql, params object[] arguments)
        {
            if (_loaded) return;

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet<TEntity>(sql, arguments);
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        public void LoadBy(Expression<Func<TEntity, bool>> criteria)
        {
            if (_loaded) return;

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet<TEntity>(criteria);
                SetEntity(entitySet);
            }
            _loaded = true;
        }

        public void LoadAll(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            if (_loaded) return;

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entityReader = dbSession.GetEntityReader<TEntity>();
                var entitySet = entityReader.Load(materializer) as EntitySet<TEntity>;

                SetEntity(entitySet);
            }
            _loaded = true;
        }

        private void SetEntity(EntitySet<TEntity> entitySet)
        {
            foreach (TEntity entity in entitySet)
            {
                if (DbSession.Configuration.AutoDetectChangesEnabled)
                {
                    EntityExtensions.FinishLoad(entity);
                }
                base.Add(entity);
            }
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
            ((List<TEntity>)base.Items).ForEach(entity =>
            {
                if (entity.HasChanges(Utils.Utils.RemoveUnusedPropertyValues(entity)))
                {
                    persistResult &= EntityExtensions.PersistChanges(entity);
                }
            });

            return persistResult;
        }

        public bool DeleteAll()
        {
            if (base.Count <= 0) return false;

            bool persistResult = true;
            ((List<TEntity>)base.Items).ForEach(entity =>
            {
                entity.MarkedForDeletion = true;
                if (entity.HasChanges(Utils.Utils.RemoveUnusedPropertyValues(entity)))
                {
                    persistResult &= EntityExtensions.PersistChanges(entity);
                }
            });
            return persistResult;
        }

        public TEntity FindByKey<TKey>(TKey key)
        {
            if (base.Count <= 0) return default(TEntity);

            var tableInfo = TableInfo<TEntity>.GetTableInfo;

            return ((List<TEntity>)base.Items).FirstOrDefault(entity =>
            {
                object[] primaryKeyValues = tableInfo.GetPrimaryKeyValues<TEntity>(entity);
                return primaryKeyValues.Any(keyValue => keyValue.Equals(key));
            });
        }
    }
}