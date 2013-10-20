using MicroORM.Entity;

namespace MicroORM.Caching
{
    internal static class ReferenceCacheManager
    {
        private static readonly object _lock = new object();
        private static EntityInfoReferenceCache<object> _referenceCache = new EntityInfoReferenceCache<object>();

        internal static EntityInfo GetEntityInfo<TEntity>(TEntity entity)
        {
            EntityInfo entityInfo;
            lock (_lock)
            {
                entityInfo = _referenceCache.Get(entity);
            }

            if (entityInfo != null)
                return entityInfo;

            return SetObjectInfo<TEntity>(entity, new EntityInfo());
        }

        private static EntityInfo SetObjectInfo<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            lock (_lock)
            {
                if (_referenceCache.Get(entity) == null)
                    _referenceCache.Add(entity, entityInfo);
                else
                    _referenceCache.Update(entity, entityInfo);
            }
            return entityInfo;
        }
    }
}
