using System;
using MicroORM.Entity;
using MicroORM.Materialization;
using System.Threading.Tasks;

namespace MicroORM.Caching
{
    internal class CacheItem<TEntity>
    {
        private Task _hashTask;
        internal EntityInfo EntityInfo { get; set; }
        private WeakReference Reference { get; set; }

        internal CacheItem(TEntity entity, EntityInfo entityInfo)
        {
            this.EntityInfo = entityInfo;
            this.Reference = new WeakReference(entity);
            _hashTask = Task.Factory.StartNew(() =>
                 {
                     this.EntityInfo.EntityHashSet = EntityHashSetManager.ComputeEntityHashSet(entity);
                 });
        }

        internal TEntity Target
        {
            get { return (TEntity)this.Reference.Target; }
        }

        internal bool IsAlive
        {
            get { return this.Reference.IsAlive; }
        }

        internal bool IsHashed
        {
            get
            {
                _hashTask.Wait();
                return true;
            }
        }
    }
}
