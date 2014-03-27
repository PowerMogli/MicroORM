using RabbitDB.Caching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RabbitDB.Entity
{
    public abstract class Entity
    {
        internal bool MarkedForDeletion { get; set; }

        public Entity() { }

        public event EventHandler EntityDeleted = delegate { };

        public event EventHandler EntityInserted = delegate { };

        public event EventHandler EntityUpdated = delegate { };

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

        internal virtual bool HasChanges(IEnumerable<KeyValuePair<string, object>> entityValues)
        {
            var entityInfo = EntityInfoCacheManager.GetEntityInfo(this);
            var valuesToUpdate = entityInfo.ComputeValuesToUpdate(this, entityValues);
            return valuesToUpdate.Count() > 0
                || MarkedForDeletion
                || entityInfo.EntityState == EntityState.Deleted
                || entityInfo.EntityState == EntityState.None;
        }
    }
}