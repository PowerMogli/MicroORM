using RabbitDB.Entity;
using RabbitDB.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        internal static Dictionary<string, int> ComputeEntityHashSetInParallel(KeyValuePair<string, object>[] keyValuePairs)
        {
            var processedKeyValuePairs = new KeyValuePair<string, int>[keyValuePairs.Length];

            Parallel.ForEach(keyValuePairs, (kvp, loopState, elementIndex) =>
            {
                processedKeyValuePairs[elementIndex] = new KeyValuePair<string, int>(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
            });
            return processedKeyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}