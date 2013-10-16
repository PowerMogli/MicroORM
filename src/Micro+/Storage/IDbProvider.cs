using System;
using System.Data;
using MicroORM.Base;
using MicroORM.Query;

namespace MicroORM.Storage
{
    public interface IDbProvider : IDisposable, IEscapeDbIdentifier
    {
        string ParameterPrefix { get; }
        string ProviderName { get; }
        string ScopeIdentity { get; }
        object ResolveNullValue(object value, Type type);
        IDbCommand CreateCommand();
        void ExecuteCommand(IQuery query);
        ObjectReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}
