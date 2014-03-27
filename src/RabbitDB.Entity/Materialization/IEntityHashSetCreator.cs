using System.Collections.Generic;

namespace RabbitDB.Materialization
{
    internal interface IEntityHashSetCreator
    {
        Dictionary<string, int> ComputeEntityHashSet();
    }
}
