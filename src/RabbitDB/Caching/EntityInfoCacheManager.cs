using RabbitDB.ChangeTracing;
using RabbitDB.Entity;
using System;

namespace RabbitDB.Caching
{
    internal static class EntityInfoCacheManager
    {
        private static readonly object _lock = new object();
        private static EntityInfoCache<object> _referenceCache = new EntityInfoCache<object>();

        internal static EntityInfo GetEntityInfo<TEntity>(TEntity entity)
        {
            var entityInfo = GetEntityInfoFromCache(entity);

            if (entityInfo == null)
            {
                entityInfo = new EntityInfo(ChangeTracingFactory.Create(entity));
                SetEntityInfo<TEntity>(entity, entityInfo);
            }

            UpdateEntityInfoLastCallTime(entityInfo);

            return entityInfo;
        }

        internal static void RemoveFor<TEntity>(TEntity entity)
        {
            _referenceCache.Remove(entity);
        }

        internal static void Dispose()
        {
            lock (_lock) { _referenceCache.Dispose(); }
        }

        private static void UpdateEntityInfoLastCallTime(EntityInfo entityInfo)
        {
            if (entityInfo != null)
                entityInfo.LastCallTime = DateTime.Now;
        }

        private static EntityInfo GetEntityInfoFromCache<TEntity>(TEntity entity)
        {
            lock (_lock)
            {
                if (_referenceCache == null)
                    return null;

                return _referenceCache.Get(entity);
            }
        }

        private static void SetEntityInfo<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            lock (_lock)
            {
                if (_referenceCache == null)
                    return;

                if (_referenceCache.Get(entity) == null)
                    _referenceCache.Add(entity, entityInfo);
                else
                    _referenceCache.Update(entity, entityInfo);
            }
        }
    }
}