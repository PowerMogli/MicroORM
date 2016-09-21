// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The entity collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using RabbitDB.Caching;
using RabbitDB.Contracts;
using RabbitDB.Contracts.Reader;
using RabbitDB.Mapping;
using RabbitDB.Session;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Entity
{
    /// <summary>
    ///     The entity collection.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public class EntityCollection<TEntity> : IEnumerable<TEntity>
        where TEntity : Entity.Entity
    {
        #region Fields

        private readonly List<TEntity> _entityCollection;

        /// <summary>
        ///     The _loaded.
        /// </summary>
        private bool _loaded;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityCollection{TEntity}" /> class.
        /// </summary>
        public EntityCollection()
        {
            _entityCollection = new List<TEntity>();
        }

        #endregion

        #region  Properties

        public TEntity this[int index] => _entityCollection[index];

        #endregion

        #region Public Methods

        /// <summary>
        ///     The delete all.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool DeleteAll()
        {
            if (_entityCollection.Count <= 0)
            {
                return false;
            }

            bool persistResult = true;
            _entityCollection.ForEach(
                entity =>
                {
                    entity.MarkedForDeletion = true;
                    if (entity.HasChanges)
                    {
                        persistResult &= entity.PersistChanges();
                    }
                });

            return persistResult;
        }

        /// <summary>
        ///     The find by key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEntity" />.
        /// </returns>
        public TEntity FindByKey<TKey>(TKey key)
        {
            if (_entityCollection.Count <= 0)
            {
                return default(TEntity);
            }

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            return _entityCollection.FirstOrDefault(
                entity =>
                {
                    object[] primaryKeyValues = tableInfo.GetPrimaryKeyValues(entity);

                    return primaryKeyValues.Any(keyValue => keyValue.Equals(key));
                });
        }

        /// <summary>
        ///     Removes all entities from the collection
        ///     and all collected information.
        /// </summary>
        public void Flush()
        {
            if (_entityCollection.Count <= 0)
            {
                return;
            }

            foreach (TEntity entity in this)
            {
                EntityInfoCacheManager.RemoveFor(entity);
            }

            _entityCollection.Clear();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _entityCollection.GetEnumerator();
        }

        /// <summary>
        ///     The load all.
        /// </summary>
        public void LoadAll()
        {
            if (_loaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (DbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                IEntitySet<TEntity> entitySet = dbSession.GetEntitySet<TEntity>();

                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        ///     The load all.
        /// </summary>
        /// <param name="materializer">
        ///     The materializer.
        /// </param>
        public void LoadAll(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            if (_loaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (DbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                IEntityReader<TEntity> entityReader = dbSession.GetEntityReader<TEntity>();

                EntitySet<TEntity> entitySet = entityReader.Load(materializer) as EntitySet<TEntity>;

                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        ///     The load by.
        /// </summary>
        /// <param name="sql">
        ///     The sql.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        public void LoadBy(string sql, params object[] arguments)
        {
            if (_loaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (DbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                IEntitySet<TEntity> entitySet = dbSession.GetEntitySet<TEntity>(sql, arguments);

                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        ///     The load by.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        public void LoadBy(Expression<Func<TEntity, bool>> criteria)
        {
            if (_loaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = EntityExtensions.InitializeSession<TEntity>();

            using (DbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                IEntitySet<TEntity> entitySet = dbSession.GetEntitySet(criteria);

                FinishLoad(entitySet);
            }

            _loaded = true;
        }

        /// <summary>
        ///     The persist changes.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool PersistChanges()
        {
            if (_entityCollection.Count <= 0)
            {
                return false;
            }

            bool persistResult = true;
            _entityCollection.ForEach(
                entity =>
                {
                    if (entity.HasChanges)
                    {
                        persistResult &= entity.PersistChanges();
                    }
                });

            return persistResult;
        }

        /// <summary>
        ///     The reload.
        /// </summary>
        public void Reload()
        {
            _loaded = false;

            Flush();

            LoadAll();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The finish load.
        /// </summary>
        /// <param name="entitySet">
        ///     The entity set.
        /// </param>
        private void FinishLoad(IEnumerable<TEntity> entitySet)
        {
            foreach (TEntity entity in entitySet)
            {
                if (DbSession.Configuration.AutoDetectChangesEnabled)
                {
                    EntityExtensions.FinishLoad(entity);
                }

                _entityCollection.Add(entity);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}