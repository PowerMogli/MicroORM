using System;
using RabbitDB.Entity;

namespace RabbitDB.Caching
{
    internal class CacheItem<TEntity>
    {
        internal EntityInfo EntityInfo { get; set; }
        private WeakReference Reference { get; set; }

        internal CacheItem(TEntity entity, EntityInfo entityInfo)
        {
            this.EntityInfo = entityInfo;
            this.Reference = new WeakReference(entity);
        }

        internal TEntity Target
        {
            get { return (TEntity)this.Reference.Target; }
        }

        internal bool IsAlive
        {
            get { return this.Reference.IsAlive; }
        }
    }
}
