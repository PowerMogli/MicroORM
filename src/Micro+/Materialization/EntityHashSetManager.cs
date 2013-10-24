using System.Collections.Generic;
using MicroORM.Reflection;

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
    }
}