using RabbitDB.Entity.ChangeTracker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity
{
    internal class EntityInfo : IDisposable, IChangeTracer
    {
        private IChangeTracer _changeTracer;

        internal EntityInfo(IChangeTracer changeTracer)
            : this()
        {
            _changeTracer = changeTracer;
        }

        private EntityInfo()
        {
            this.EntityState = EntityState.None;
            this.LastCallTime = DateTime.Now;
        }

        private bool _disposed;

        internal bool CanBeRemoved { get { return DateTime.Now.Subtract(this.LastCallTime) > TimeSpan.FromMinutes(2); } }
        internal EntityState EntityState { get; set; }

        internal DateTime LastCallTime { get; set; }

        public void ClearChanges()
        {
            _changeTracer.ClearChanges();
        }

        public void MergeChanges()
        {
            _changeTracer.MergeChanges();
        }

        public void ComputeSnapshot<TEntity>(TEntity entity)
        {
            _changeTracer.ComputeSnapshot(entity);
        }

        public KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            return _changeTracer.ComputeValuesToUpdate();
        }

        internal bool HasChanges()
        {
            var valuesToUpdate = ComputeValuesToUpdate();
            return valuesToUpdate.Count() > 0
                || this.EntityState == EntityState.Deleted
                || this.EntityState == EntityState.None;
        }

        private void Dispose(bool dispose)
        {
            if (dispose && _disposed == false)
            {
                _changeTracer.Dispose();

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}