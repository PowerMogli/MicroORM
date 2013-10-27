using System;
using System.Collections.Generic;
using System.Linq;
using RabbitDB.Mapping;
using RabbitDB.Materialization;
using RabbitDB.Reflection;

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

        internal KeyValuePair<string, object>[] ComputeUpdateValues<TEntity>(TEntity entity)
        {
            Dictionary<string, int> entityHashSet = EntityHashSetManager.ComputeEntityHashSet(entity);
            KeyValuePair<string, object>[] entityValues = RemoveUnusedPropertyValues<TEntity>(entity);

            Dictionary<string, object> valuesToUpdate = new Dictionary<string, object>();
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

        private KeyValuePair<string, object>[] RemoveUnusedPropertyValues<TEntity>(TEntity entity)
        {
            KeyValuePair<string, object>[] entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return entityValues.Where(kvp => tableInfo.DbTable.DbColumns.Any(column => column.Name == kvp.Key)).ToArray();
        }
    }
}
