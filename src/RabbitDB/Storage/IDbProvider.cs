using RabbitDB.Query;
using System;
using System.Data;

namespace RabbitDB.Storage
{
    internal interface IDbProvider : IDisposable
    {
        string ProviderName { get; }
        IDbCommand CreateCommand();
        IDbCommand SetupCommand(IDbCommand dbCommand);
        void CreateConnection();
    }
}