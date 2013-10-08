using System;
using System.Data;
using System.Data.Common;
using MicroORM.Base;
using MicroORM.Materialization;
using MicroORM.Query;
//using MicroORM.Base.Utils;

namespace MicroORM.Storage
{
    internal abstract class DbProvider : IDbProvider
    {
        private string _connectionString;
        private readonly System.Data.Common.DbProviderFactory _dbFactory;
        protected IDbConnection _dbConnection;
        protected IDbTransaction _dbTransaction;
        protected IDbCommand _dbCommand;

        public abstract string ParameterPrefix { get; }
        public abstract string ProviderName { get; }

        public DbProvider(string connectionString)
        {
            _dbFactory = DbProviderFactories.GetFactory(ProviderName);
            _connectionString = connectionString;
        }

        public IDbCommand CreateCommand()
        {
            return _dbConnection.CreateCommand();
        }

        private void CreateNewConnection()
        {
            _dbConnection = _dbFactory.CreateConnection();
            _dbConnection.ConnectionString = _connectionString;
            _dbConnection.Open();
        }

        protected void CreateConnection()
        {
            if (_dbTransaction != null)
            {
                _dbConnection = _dbTransaction.Connection;
            }
            else
            {
                CreateNewConnection();
            }
        }

        public virtual void ExecuteCommand(IQuery query)
        {
            try
            {
                CreateConnection();
                _dbCommand = query.Compile(this);
                _dbCommand.Transaction = _dbTransaction;

                _dbCommand.ExecuteNonQuery();
            }
            finally
            {
                Dispose();
            }
        }

        public virtual ObjectReader<T> ExecuteReader<T>(IQuery query)
        {
            try
            {
                CreateConnection();
                _dbCommand = query.Compile(this);
                _dbCommand.Transaction = _dbTransaction;

                return new ObjectReader<T>(_dbCommand.ExecuteReader());
            }
            finally
            {
                Dispose();
            }
        }

        public virtual T ExecuteScalar<T>(IQuery query)
        {
            CreateConnection();
            _dbCommand = query.Compile(this);
            _dbCommand.Transaction = _dbTransaction;

            return (T)_dbCommand.ExecuteScalar();
        }

        public virtual string EscapeName(string value)
        {
            return "\"" + value + "\"";
        }

        public virtual void SetupParameter(IDbDataParameter parameter, string name, object value)
        {
            if (name == null) throw new ArgumentNullException("name", "Der Name des Parameters darf niemals NULL sein");

            parameter.ParameterName = string.Concat(this.ParameterPrefix, name);
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

        public void Dispose()
        {
            DisposeConnection();
            DisposeCommand();
            DisposeTransaction();
        }

        private void DisposeCommand()
        {
            if (_dbCommand == null) return;

            _dbCommand.Dispose();
            _dbCommand = null;
        }

        private void DisposeConnection()
        {
            if (InTransactionMode() || _dbConnection == null) return;

            _dbConnection.Close();
            _dbConnection.Dispose();
            _dbConnection = null;
        }

        private void DisposeTransaction()
        {
            if (InTransactionMode() || _dbTransaction == null) return;

            _dbTransaction.Dispose();
            _dbTransaction = null;
        }

        private bool InTransactionMode()
        {
            return (_dbTransaction != null && _dbTransaction.Connection != null);
        }
    }
}
