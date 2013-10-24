using System;
using System.Data;
using MicroORM.Base;
using MicroORM.Query;
using MicroORM.Schema;

namespace MicroORM.Storage
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
        ObjectReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}
