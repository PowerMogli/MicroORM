using System;
using System.Data;
using MicroORM.Base;
using MicroORM.Query;

namespace MicroORM.Storage
{
    interface IDbProvider : IDisposable, IEscapeDbIdentifier
    {
        string ParameterPrefix { get; }
        string ProviderName { get; }
        IDbCommand CreateCommand();
        void SetupParameter(IDbDataParameter parameter, string name, object value);
        void ExecuteCommand(IQuery query);
        ObjectReader<T> ExecuteReader<T>(IQuery query);
    }
}
