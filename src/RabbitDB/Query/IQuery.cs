using System.Data;
using RabbitDB.Storage;

namespace RabbitDB.Query
{
    internal interface IQuery
    {
        IDbCommand Compile(IDbProvider provider);
    }
}
