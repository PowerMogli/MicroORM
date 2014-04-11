// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The entity collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;

    using RabbitDB.Caching;
    using RabbitDB.Mapping;
    using RabbitDB.Session;

    /// <summary>
    /// The entity collection.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public class EntityCollection<TEntity> : Collection<TEntity>
        where TEntity : Entity
    {
        #region Fields

        /// <summary>
        /// The _loaded.
        /// </summary>
        private bool _loaded;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection{TEntity}"/> class.
        /// </summary>
        public EntityCollection()
            : base(new List<TEntity>())
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The delete all.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeleteAll()
        {
            if (base.Count <= 0)
            {
                return false;
            }

            bool persistResult = true;
            ((List<TEntity>)base.Items).ForEach(
                entity =>
                {
                    entity.MarkedForDeletion = true;
                    if (entity.HasChanges())
                    {
                        persistResult &= entity.PersistChanges();
                    }
                });

            return persistResult;
        }

        /// <summary>
        /// The find by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        public TEntity FindByKey<TKey>(TKey key)
        {
            if (base.Count <= 0)
            {
                return default(TEntity);
            }

            var tableInfo = TableInfo<TEntity>.GetTableInfo;

            return base.Items.FirstOrDefault(
                entity =>
                {
                    var primaryKeyValues = tableInfo.GetPrimaryKeyValues(entity);
                    return primaryKeyValues.Any(keyValue => keyValue.Equals(key));
                });
        }

        /// <summary>
        /// Removes all entities from the collection
        /// and all collected information.
        /// </summary>
        public void Flush()
        {
            if (base.Count <= 0)
            {
                return;
            }

            foreach (var entity in this)
            {
                EntityInfoCacheManager.RemoveFor(entity);
            }

            Clear();
        }

        /// <summary>
        /// The load all.
        /// </summary>
        public void LoadAll()
        {
            if (_loaded)
            {
                return;
            }

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet<TEntity>();
                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        /// The load all.
        /// </summary>
        /// <param name="materializer">
        /// The materializer.
        /// </param>
        public void LoadAll(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            if (_loaded)
            {
                return;
            }

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entityReader = dbSession.GetEntityReader<TEntity>();
                var entitySet = entityReader.Load(materializer) as EntitySet<TEntity>;

                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        /// The load by.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void LoadBy(string sql, params object[] arguments)
        {
            if (_loaded)
            {
                return;
            }

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet<TEntity>(sql, arguments);
                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        /// The load by.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        public void LoadBy(Expression<Func<TEntity, bool>> criteria)
        {
            if (_loaded)
            {
                return;
            }

            var sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (var dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                var entitySet = dbSession.GetEntitySet(criteria);
                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        /// The persist changes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool PersistChanges()
        {
            if (base.Count <= 0)
            {
                return false;
            }

            bool persistResult = true;
            ((List<TEntity>)base.Items).ForEach(
                entity =>
                {
                    if (entity.HasChanges())
                    {
                        persistResult &= entity.PersistChanges();
                    }
                });

            return persistResult;
        }

        /// <summary>
        /// The reload.
        /// </summary>
        public void Reload()
        {
            this._loaded = false;
            this.Flush();
            this.LoadAll();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The finish load.
        /// </summary>
        /// <param name="entitySet">
        /// The entity set.
        /// </param>
        private void FinishLoad(IEnumerable<TEntity> entitySet)
        {
            foreach (var entity in entitySet)
            {
                if (DbSession.Configuration.AutoDetectChangesEnabled)
                {
                    EntityExtensions.FinishLoad(entity);
                }

                Add(entity);
            }
        }

        #endregion
    }
}