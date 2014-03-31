using System.Data;
using System.Data.Common;

namespace RabbitDB.Storage
{
    internal abstract class DbProvider : IDbProvider
    {
        private string _connectionString;
        private readonly System.Data.Common.DbProviderFactory _dbFactory;
        protected IDbConnection _dbConnection;
        protected IDbCommand _dbCommand;
        protected IDbTransaction _dbTransaction;

        public abstract string ProviderName { get; }

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
            if (_dbConnection == null
                || _dbConnection.State != ConnectionState.Open)
            {
                _dbConnection = _dbFactory.CreateConnection();
                _dbConnection.ConnectionString = _connectionString;
                _dbConnection.Open();
            }
        }

        public void CreateConnection()
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

        public IDbCommand SetupCommand(IDbCommand dbCommand)
        {
            _dbCommand = dbCommand;
            dbCommand.Transaction = _dbTransaction;

            return dbCommand;
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