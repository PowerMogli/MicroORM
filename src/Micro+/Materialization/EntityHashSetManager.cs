using System.Collections.Generic;
using MicroORM.Reflection;
using MicroORM.Entity;
using System.Linq;

namespace MicroORM.Materialization
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
            Dictionary<string, int> entityHashSet = ComputeEntityHashSet(entity);
            KeyValuePair<string, object>[] entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });

            Dictionary<string, object> valuesToUpdate = new Dictionary<string, object>();
            foreach (var kvp in entityHashSet)
            {
                var oldHash = entityInfo.EntityHashSet[kvp.Key];
                if (oldHash.Equals(kvp.Value) == false)
                    valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
            }

            return valuesToUpdate.ToArray();
        }
    }
}
