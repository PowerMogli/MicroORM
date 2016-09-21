// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfoCacheManager.cs" company="">
//   
// </copyright>
// <summary>
//   The entity info cache manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using RabbitDB.Entity.Entity;

namespace RabbitDB.Caching
{
    using System;

    using RabbitDB.ChangeTracing;
    using RabbitDB.Entity;

    /// <summary>
    /// The entity info cache manager.
    /// </summary>
    internal static class EntityInfoCacheManager
    {
        #region Static Fields

        /// <summary>
        /// The _lock.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The _reference cache.
        /// </summary>
        private static readonly EntityInfoCache<object> ReferenceCache = new EntityInfoCache<object>();

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        internal static void Dispose()
        {
            lock (Lock)
            {
                ReferenceCache.Dispose();
            }
        }

        /// <summary>
        /// The get entity info.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityInfo"/>.
        /// </returns>
        internal static EntityInfo GetEntityInfo<TEntity>(TEntity entity)
        {
            var entityInfo = GetEntityInfoFromCache(entity);

            if (entityInfo == null)
            {
                entityInfo = new EntityInfo(ChangeTracingFactory.Create(entity));
                SetEntityInfo(entity, entityInfo);
            }

            entityInfo.UpdateLastCallTime();

            return entityInfo;
        }

        /// <summary>
        /// The remove for.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        internal static void RemoveFor<TEntity>(TEntity entity)
        {
            ReferenceCache.Remove(entity);
        }

        /// <summary>
        /// The get entity info from cache.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityInfo"/>.
        /// </returns>
        private static EntityInfo GetEntityInfoFromCache<TEntity>(TEntity entity)
        {
            lock (Lock)
            {
                return ReferenceCache == null ? null : ReferenceCache.Get(entity);
            }
        }

        /// <summary>
        /// The set entity info.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entityInfo">
        /// The entity info.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        private static void SetEntityInfo<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            lock (Lock)
            {
                if (ReferenceCache == null)
                {
                    return;
                }

                if (ReferenceCache.Get(entity) == null)
                {
                    ReferenceCache.Add(entity, entityInfo);
                }
                else
                {
                    ReferenceCache.Update(entity, entityInfo);
                }
            }
        }
        
        #endregion
    }
}