using System;

namespace RabbitDB.Entity
{
    public abstract class Entity
    {
        internal bool MarkedForDeletion { get; set; }

        public Entity() { }

        public event EventHandler EntityDeleted;

        public event EventHandler EntityInserted;

        public event EventHandler EntityUpdated;

        internal void RaiseEntityDeleted()
        {
            EventHandler handler = this.EntityDeleted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal void RaiseEntityInserted()
        {
            EventHandler handler = this.EntityInserted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        internal void RaiseEntityUpdated()
        {
            EventHandler handler = this.EntityUpdated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}