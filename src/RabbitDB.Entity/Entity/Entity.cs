using RabbitDB.Entity.ChangeRecorder;
using System;
using System.Collections.Generic;

namespace RabbitDB.Entity
{
    public abstract class Entity : IDisposable
    {
        public Entity() { }

        public event EventHandler EntityDeleted = delegate { };

        public event EventHandler EntityInserted = delegate { };

        public event EventHandler EntityUpdated = delegate { };

        internal EntityInfo EntityInfo { get; set; }

        internal virtual ChangeRecorderOption ChangeTracerOption { get { return ChangeRecorderOption.Hashed; } }

        internal bool MarkedForDeletion { get; set; }

        internal void RaiseEntityDeleted()
        {
            this.EntityDeleted(this, EventArgs.Empty);
        }

        internal void RaiseEntityInserted()
        {
            this.EntityInserted(this, EventArgs.Empty);
        }

        internal void RaiseEntityUpdated()
        {
            this.EntityUpdated(this, EventArgs.Empty);
        }

        internal bool IsForUpdate()
        {
            return this.EntityInfo.EntityState != EntityState.Deleted;
        }

        internal bool IsForInsert()
        {
            return this.EntityInfo.EntityState == EntityState.Deleted || this.EntityInfo.EntityState == EntityState.None;
        }

        internal bool IsForDeletion()
        {
            return (this.MarkedForDeletion && this.EntityInfo.EntityState != EntityState.Deleted);
        }

        internal bool IsLoaded()
        {
            return (this.EntityInfo != null && this.EntityInfo.EntityState == EntityState.Loaded);
        }

        internal bool HasChanges()
        {
            return MarkedForDeletion || this.EntityInfo.HasChanges();
        }

        internal KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            return this.EntityInfo.ComputeValuesToUpdate();
        }

        public void Dispose()
        {
            this.EntityInfo.Dispose();
        }
    }
}