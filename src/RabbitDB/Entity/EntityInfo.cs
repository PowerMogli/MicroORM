using System;
using System.Collections.Generic;
using RabbitDB.Materialization;

namespace RabbitDB.Entity
{
    internal sealed class EntityInfo
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
        internal Dictionary<string, int> ValueSnapshot { get; set; }
        internal Dictionary<string, int> ChangesSnapshot { get; set; }
        internal DateTime LastCallTime { get; set; }

        internal void ClearChanges()
        {
            this.ChangesSnapshot.Clear();
        }

        internal void MergeChanges()
        {
            foreach (var change in this.ChangesSnapshot)
            {
                this.ValueSnapshot[change.Key] = change.Value;
            }
            ClearChanges();
        }

        internal void ComputeSnapshot<TEntity>(TEntity entity)
        {
            this.ValueSnapshot = EntityHashSetManager.ComputeEntityHashSet(entity);
        }
    }
}