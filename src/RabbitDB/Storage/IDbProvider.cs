using RabbitDB.Query;
using System;
using System.Data;

namespace RabbitDB.Storage
{
    internal interface IDbProvider : IDisposable
    {
        string ProviderName { get; }
        IDbCommand CreateCommand();
        IDbCommand PrepareCommand(IQuery query, SqlDialect.SqlDialect sqlDialect);
        void CreateConnection();
    }
}