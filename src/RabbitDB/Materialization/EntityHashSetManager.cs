using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitDB.Entity;
using RabbitDB.Reflection;

namespace RabbitDB.Materialization
{
    internal static class EntityHashSetManager
    {
        internal static Dictionary<string, int> ComputeEntityHashSet<TEntity>(TEntity entity)
        {
            var keyValuePairs = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });
            //if (keyValuePairs.Length >= 30)
            //return ComputeParallelEntityHashSet(keyValuePairs);

            return ComputeEntityHashSet(keyValuePairs);
        }

        private static Dictionary<string, int> ComputeEntityHashSet(KeyValuePair<string, object>[] keyValuePairs)
        {
            var entityHashSet = new Dictionary<string, int>();

            foreach (var kvp in keyValuePairs)
            {
                entityHashSet.Add(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
            }
            return entityHashSet;
        }

        internal static Dictionary<string, int> ComputeParallelEntityHashSet(KeyValuePair<string, object>[] keyValuePairs)
        {
            var processedKeyValuePairs = new KeyValuePair<string, int>[keyValuePairs.Length];

            Parallel.ForEach(keyValuePairs, (kvp, loopState, elementIndex) =>
            {
                processedKeyValuePairs[elementIndex] = new KeyValuePair<string, int>(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
            });
            return processedKeyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static KeyValuePair<string, object>[] ComputeUpdateValues<TEntity>(TEntity entity, EntityInfo entityInfo)
        {
            var entityHashSet = EntityHashSetManager.ComputeEntityHashSet(entity);
            var entityValues = Utils.Utils.RemoveUnusedPropertyValues<TEntity>(entity);

            var valuesToUpdate = new Dictionary<string, object>();
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
    }
}