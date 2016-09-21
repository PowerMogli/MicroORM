// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The entity extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;

using RabbitDB.Caching;
using RabbitDB.Contracts.Entity;
using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Session;
using RabbitDB.Query;
using RabbitDB.Session;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Entity
{
    /// <summary>
    ///     The entity extensions.
    /// </summary>
    public static class EntityExtensions
    {
        #region Public Methods

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public static void Load<TEntity>(this TEntity entity) where TEntity : Entity.Entity, new()
        {
            if (entity.IsLoaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                dbSession.Load(entity);

                FinishLoad(entity);
            }
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="customMapper">
        ///     The custom mapper.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public static void Load<TEntity>(this TEntity entity, Action<TEntity, IDataReader> customMapper)
            where TEntity : Entity.Entity, new()
        {
            if (entity.IsLoaded)
            {
                return;
            }

            Tuple<string, DbEngine> sessionConfig = InitializeSession<TEntity>();

            using (IBaseDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                IEntityReader<TEntity> entityReader = dbSession.GetEntityReader<TEntity>(new EntityQuery<TEntity>(entity));

                entityReader.Load(entity, customMapper);

                FinishLoad(entity);
            }
        }

        /// <summary>
        ///     The mark for deletion.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public static void MarkForDeletion<TEntity>(this TEntity entity) where TEntity : Entity.Entity
        {
            entity.MarkedForDeletion = true;
        }

        /// <summary>
        ///     The persist changes.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public static bool PersistChanges<TEntity>(this TEntity entity) where TEntity : Entity.Entity
        {
            if (DbSession.Configuration.AutoDetectChangesEnabled == false)
            {
                throw new InvalidOperationException(
                    "This operation is not allowed because change tracking has been disabled.");
            }

            Tuple<string, DbEngine> sessionConfig = InitializeSession<TEntity>();

            using (IDbSession dbSession = new DbSession(sessionConfig.Item1, sessionConfig.Item2))
            {
                try
                {
                    using (IDbTransaction transaction = dbSession.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        if (dbSession.PersistChanges(entity) == false)
                        {
                            return false;
                        }

                        transaction.Commit();
                        entity.EntityInfo.MergeChanges();
                        return true;
                    }
                }
                catch
                {
                    entity.EntityInfo.ClearChanges();
                    throw;
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The finish load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        internal static void FinishLoad<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            if (entity.EntityInfo == null)
            {
                entity.EntityInfo = EntityInfoCacheManager.GetEntityInfo(entity);
            }

            if (entity.EntityInfo.EntityState == EntityState.Loaded)
            {
                return;
            }

            entity.EntityInfo.EntityState = EntityState.Loaded;
            entity.EntityInfo.ComputeSnapshot(entity);
        }

        /// <summary>
        ///     The initialize session.
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Tuple" />.
        /// </returns>
        internal static Tuple<string, DbEngine> InitializeSession<TEntity>()
        {
            string connectionString = Registrar<string>.GetFor(typeof(TEntity));

            DbEngine dbEngine = Registrar<DbEngine>.GetFor(typeof(TEntity));

            return new Tuple<string, DbEngine>(connectionString, dbEngine);
        }

        #endregion
    }
}