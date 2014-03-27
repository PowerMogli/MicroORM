using System;
using RabbitDB.Entity;
using RabbitDB.Materialization;
using RabbitDB.ChangeTracker;
using RabbitDB.Utils;

namespace RabbitDB.Caching
{
    internal static class EntityInfoCacheManager
    {
        private static readonly object _lock = new object();
        private static EntityInfoCache<object> _referenceCache = new EntityInfoCache<object>();

        internal static NotifiedEntityInfo GetNotifiedEntityInfo<TEntity>(TEntity entity)
        {
            NotifiedEntityInfo notifiedEntityInfo = GetEntityInfoFromCache(entity) as NotifiedEntityInfo;

            if (notifiedEntityInfo == null)
            {
                ITracker changeTracker = new Tracker();
                changeTracker.TrackObject(entity);
                notifiedEntityInfo = new NotifiedEntityInfo(changeTracker, new ValidEntityArgumentReader<TEntity>(entity));
                SetEntityInfo<TEntity>(entity, notifiedEntityInfo);
            }
            UpdateEntityInfoLastCallTime(notifiedEntityInfo);

            return notifiedEntityInfo;
        }

        internal static EntityInfo GetEntityInfo<TEntity>(TEntity entity)
        {
            if (entity is NotifiedEntity)
            {
                return GetNotifiedEntityInfo(entity);
            }

            EntityInfo entityInfo = GetEntityInfoFromCache(entity);

            if (entityInfo == null)
                SetEntityInfo<TEntity>(entity, new EntityInfo(new EntityHashSetCreator<TEntity>(entity), new ValidEntityArgumentReader<TEntity>(entity)));

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