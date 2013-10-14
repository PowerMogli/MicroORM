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
        object ResolveNullValue(object value, Type type);
        IDbCommand CreateCommand();
        void SetupParameter(IDbDataParameter parameter, string name, object value);
        object ExecuteInsert(IQuery query);
        void ExecuteCommand(IQuery query);
        ObjectReader<T> ExecuteReader<T>(IQuery query);
        T ExecuteScalar<T>(IQuery query);
    }
}
