using RabbitDB.Materialization;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity
{
    internal class EntityInfo : IDisposable
    {
        internal EntityInfo(IEntityHashSetCreator entityHashSetCreator, IValidEntityArgumentsReader validEntityArgumentsReader)
            : this()
        {
            this.EntityHashSetCreator = entityHashSetCreator;
            this.ValidArgumentReader = validEntityArgumentsReader;
            this.ValueSnapshot = new Dictionary<string, int>();
            this.ChangesSnapshot = new Dictionary<string, int>();
        }

        protected EntityInfo()
        {
            this.EntityState = EntityState.None;
            this.LastCallTime = DateTime.Now;
        }

        protected bool _disposed;

        private IEntityHashSetCreator EntityHashSetCreator { get; set; }
        protected IValidEntityArgumentsReader ValidArgumentReader { get; set; }

        internal bool CanBeRemoved { get { return DateTime.Now.Subtract(this.LastCallTime) > TimeSpan.FromMinutes(2); } }
        internal EntityState EntityState { get; set; }
        private Dictionary<string, int> ValueSnapshot { get; set; }
        private Dictionary<string, int> ChangesSnapshot { get; set; }
        internal DateTime LastCallTime { get; set; }

        internal void ClearChanges()
        {
            this.ChangesSnapshot.Clear();
        }

        internal virtual void MergeChanges()
        {
            foreach (var change in this.ChangesSnapshot)
            {
                this.ValueSnapshot[change.Key] = change.Value;
            }
            ClearChanges();
        }

        internal virtual void ComputeSnapshot<TEntity>(TEntity entity)
        {
            this.ValueSnapshot = EntityHashSetCreator.ComputeEntityHashSet();
        }

        internal virtual KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var entityHashSet = EntityHashSetCreator.ComputeEntityHashSet();
            var entityValues = ValidArgumentReader.ReadValidEntityArguments();

            var valuesToUpdate = new Dictionary<string, object>();
            foreach (var kvp in entityHashSet)
            {
                var oldHash = this.ValueSnapshot[kvp.Key];
                if (oldHash.Equals(kvp.Value) == false)
                {
                    valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
                    this.ChangesSnapshot.Add(kvp.Key, kvp.Value);
                }
            }

            return valuesToUpdate.ToArray();
        }

        internal bool HasChanges()
        {
            var valuesToUpdate = ComputeValuesToUpdate();
            return valuesToUpdate.Count() > 0
                || this.EntityState == EntityState.Deleted
                || this.EntityState == EntityState.None;
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose && _disposed == false)
            {
                this.ValidArgumentReader = null;
                this.EntityHashSetCreator = null;
                this.ValueSnapshot.Clear();
                this.ValueSnapshot = null;
                this.ChangesSnapshot.Clear();
                this.ChangesSnapshot = null;

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}