using System.Collections.Generic;
using System.Linq;
using RabbitDB.Entity;
using RabbitDB.Mapping;
using RabbitDB.Reflection;

namespace RabbitDB.Materialization
{
    internal class EntityHashSetManager
    {
        internal static Dictionary<string, int> ComputeEntityHashSet<TEntity>(TEntity entity)
        {
            Dictionary<string, int> _entityHashSet = new Dictionary<string, int>();

            foreach (KeyValuePair<string, object> kvp in ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity }))
            {
                _entityHashSet.Add(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
            }
            return _entityHashSet;
        }

        internal static KeyValuePair<string, object>[] ComputeUpdateValues<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            Dictionary<string, int> entityHashSet = EntityHashSetManager.ComputeEntityHashSet(entity);
            KeyValuePair<string, object>[] entityValues = RemoveUnusedPropertyValues<TEntity>(entity);

            Dictionary<string, object> valuesToUpdate = new Dictionary<string, object>();
            foreach (var kvp in entityHashSet)
            {
                var oldHash = entityInfo.ValueSnapshot[kvp.Key];
                if (oldHash.Equals(kvp.Value) == false)
                {
                    valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
                    entityInfo.ChangesSnapshot.Add(kvp.Key, kvp.Value);
                }
            }

            return valuesToUpdate.ToArray();
        }

        private static KeyValuePair<string, object>[] RemoveUnusedPropertyValues<TEntity>(TEntity entity)
        {
            KeyValuePair<string, object>[] entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return entityValues.Where(kvp => tableInfo.DbTable.DbColumns.Any(column => column.Name == kvp.Key)).ToArray();
        }
    }
}