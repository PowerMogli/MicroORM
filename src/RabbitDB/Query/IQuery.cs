using RabbitDB.Storage;
using System.Data;

namespace RabbitDB.Query
{
    internal interface IQuery
    {
        IDbCommand Compile(SqlDialect.SqlDialect sqlDialect);
    }
}