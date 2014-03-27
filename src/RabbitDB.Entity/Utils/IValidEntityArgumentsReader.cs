using System.Collections.Generic;

namespace RabbitDB.Utils
{
    internal interface IValidEntityArgumentsReader
    {
        IEnumerable<KeyValuePair<string, object>> ReadValidEntityArguments();
    }
}