using System;
using System.Data;
using RabbitDB.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Reader;

namespace RabbitDB.Storage
{
    internal interface IDbProvider : IDisposable, IEscapeDbIdentifier
    {
        string ParameterPrefix { get; }
        string ProviderName { get; }
        string ScopeIdentity { get; }
        IDbProviderExpressionBuildHelper BuilderHelper { get; }
        object ResolveNullValue(object value, Type type);
        string ResolveScopeIdentity(TableInfo tableInfo);
        IDbCommand CreateCommand();
        void ExecuteCommand(IQuery query);
        IDataReader ExecuteReader(IQuery query);
        EntityReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}