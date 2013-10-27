using System;
using System.Data;
using RabbitDB.Base;
using RabbitDB.Query;
using RabbitDB.Schema;

namespace RabbitDB.Storage
{
    internal interface IDbProvider : IDisposable, IEscapeDbIdentifier
    {
        string ParameterPrefix { get; }
        string ProviderName { get; }
        string ScopeIdentity { get; }
        object ResolveNullValue(object value, Type type);
        IDbCommand CreateCommand();
        void ExecuteCommand(IQuery query);
        IDataReader ExecuteReader(IQuery query);
        EntityReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}
