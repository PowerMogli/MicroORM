using RabbitDB.Materialization;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity
{
    internal class EntityInfo
    {
        internal EntityInfo()
        {
            this.EntityState = EntityState.None;
            this.ValueSnapshot = new Dictionary<string, int>();
            this.ChangesSnapshot = new Dictionary<string, int>();
            this.LastCallTime = DateTime.Now;
        }

        internal bool CanBeRemoved { get { return DateTime.Now.Subtract(this.LastCallTime) > TimeSpan.FromMinutes(2); } }
        internal EntityState EntityState { get; set; }
        internal Dictionary<string, int> ValueSnapshot { get; private set; }
        internal Dictionary<string, int> ChangesSnapshot { get; private set; }
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
            this.ValueSnapshot = EntityHashSetManager.ComputeEntityHashSet(entity);
        }

        internal virtual KeyValuePair<string, object>[] ComputeValuesToUpdate<TEntity>(TEntity entity, IEnumerable<KeyValuePair<string, object>> entityValues)
        {
            var entityHashSet = EntityHashSetManager.ComputeEntityHashSet(entity);

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
    }
}