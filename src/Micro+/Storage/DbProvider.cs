using System;
using System.Data;
using System.Data.Common;
using MicroORM.Base;
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
        public abstract string ScopeIdentity { get; }

        public DbProvider(string connectionString)
        {
            _dbFactory = DbProviderFactories.GetFactory(this.ProviderName);
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
            CreateConnection();
            _dbCommand = query.Compile(this);
            _dbCommand.Transaction = _dbTransaction;

            return new ObjectReader<T>(_dbCommand.ExecuteReader(), this);
        }

        public virtual T ExecuteScalar<T>(IQuery query)
        {
            try
            {
                CreateConnection();
                _dbCommand = query.Compile(this);
                _dbCommand.Transaction = _dbTransaction;

                return (T)_dbCommand.ExecuteScalar();
            }
            finally
            {
                Dispose();
            }
        }

        public virtual string EscapeName(string value)
        {
            return "\"" + value + "\"";
        }

        public virtual object ResolveNullValue(object value, Type type)
        {
            if (value == null
                || (value is DBNull
                && !type.IsSubclassOf(typeof(ValueType)))) return null;

            Type originalType = type.UnderlyingSystemType;

            if (originalType == typeof(short)) return (short)0;
            else if (originalType == typeof(int)) return (int)0;
            else if (originalType == typeof(long)) return (long)0;
            else if (originalType == typeof(byte)) return (byte)0;
            else if (originalType == typeof(Single)) return (Single)0;
            else if (originalType == typeof(decimal)) return (decimal)0;
            else if (originalType == typeof(double)) return (double)0;
            else if (originalType == typeof(string)) return string.Empty;
            else if (originalType == typeof(bool)) return false;
            else if (originalType == typeof(DateTime)) return new DateTime();
            else if (originalType == typeof(byte[]) || originalType == typeof(object)) return new byte[] { };

            throw new InvalidTypeException("Unsupported type encountered while converting from DBNull.");
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
