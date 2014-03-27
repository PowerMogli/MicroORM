using RabbitDB.Reflection;
using System.Collections.Generic;

namespace RabbitDB.Materialization
{
    internal class EntityHashSetCreator<TEntity> : IEntityHashSetCreator
    {
        private TEntity _entity;

        internal EntityHashSetCreator(TEntity entity)
        {
            _entity = entity;
        }

        public Dictionary<string, int> ComputeEntityHashSet()
        {
            var keyValuePairs = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { _entity });
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

        //internal static Dictionary<string, int> ComputeEntityHashSetInParallel(KeyValuePair<string, object>[] keyValuePairs)
        //{
        //    var processedKeyValuePairs = new KeyValuePair<string, int>[keyValuePairs.Length];

        //    Parallel.ForEach(keyValuePairs, (kvp, loopState, elementIndex) =>
        //    {
        //        processedKeyValuePairs[elementIndex] = new KeyValuePair<string, int>(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
        //    });
        //    return processedKeyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        //}
    }
}