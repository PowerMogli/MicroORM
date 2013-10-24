using System;
using MicroORM.Entity;

namespace MicroORM.Caching
{
    internal static class EntityInfoCacheManager
    {
        private static readonly object _lock = new object();
        private static EntityInfoCache<object> _referenceCache = new EntityInfoCache<object>();

        internal static EntityInfo GetEntityInfo<TEntity>(TEntity entity)
        {
            EntityInfo entityInfo;
            lock (_lock)
            {
                if (_referenceCache == null)
                    return null;

                entityInfo = _referenceCache.Get(entity);
            }

            if (entityInfo == null)
                entityInfo = SetEntityInfo<TEntity>(entity, new EntityInfo());

            if (entityInfo != null)
                entityInfo.LastCallTime = DateTime.Now;

            return entityInfo;
        }

        internal static EntityInfo SetEntityInfo<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            lock (_lock)
            {
                if (_referenceCache == null)
                    return null;

                if (_referenceCache.Get(entity) == null)
                    _referenceCache.Add(entity, entityInfo);
                else
                    _referenceCache.Update(entity, entityInfo);
            }
            return entityInfo;
        }

        internal static void Dispose()
        {
            lock (_lock) { _referenceCache.Dispose(); }
        }
    }
}
