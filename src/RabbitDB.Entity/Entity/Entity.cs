using System;
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
    }
}