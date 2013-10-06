using System;
using System.Data;
using System.Data.Common;
using MicroORM.Base.Query;
//using MicroORM.Base.Utils;

namespace MicroORM.Base.Storage
{
    internal abstract class DbProvider : IDbProvider
    {
        private string _connectionString;
        private readonly DbProviderFactory _factory;
        private IDbConnection _connection = null;

        public abstract string ParameterPrefix { get; }
        public abstract string ProviderName { get; }

        public DbProvider(string connectionString)
        {
            _factory = DbProviderFactories.GetFactory(ProviderName);
            _connectionString = connectionString;
        }

        public IDbCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        private void CreateNewConnection()
        {
            _connection = _factory.CreateConnection();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
        }

        protected void CreateConnection()
        {
            if (DatabaseTransaction.IsActive)
            {
                if (DatabaseTransaction.Transaction == null)
                {
                    CreateNewConnection();
                    DatabaseTransaction.Create(_connection);
                }
                else
                {
                    _connection = DatabaseTransaction.Transaction.Connection;
                }
                return;
            }
            CreateNewConnection();
        }

        public abstract void ExecuteCommand(IQuery query);

        public abstract ObjectReader<T> ExecuteReader<T>(IQuery query);

        public void Dispose()
        {
            if (!DatabaseTransaction.IsActive)
                _connection.Dispose();
        }

        public virtual string EscapeName(string value)
        {
            return "\"" + value + "\"";
        }

        public virtual void SetupParameter(IDbDataParameter parameter, string name, object value)
        {
            if (name == null) name = "";
            parameter.ParameterName = string.Concat(ParameterPrefix, name);
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    parameter.Value = value.ConvertTo(Enum.GetUnderlyingType(value.GetType()));
                }
                else
                {
                    parameter.Value = value;
                }
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}
