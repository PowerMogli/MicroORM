// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;
using System.Data.Common;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Contracts.Storage;
using RabbitDB.Query;

#endregion

namespace RabbitDB.Storage
{
    /// <summary>
    ///     The db provider.
    /// </summary>
    internal abstract class DbProvider : IDbProvider
    {
        #region Fields

        /// <summary>
        ///     The connection string.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        ///     The database provider factory.
        /// </summary>
        private readonly DbProviderFactory _dbFactory;

        /// <summary>
        ///     The database command.
        /// </summary>
        protected IDbCommand DbCommand;

        /// <summary>
        ///     The database connection.
        /// </summary>
        protected IDbConnection DbConnection;

        /// <summary>
        ///     The database transaction.
        /// </summary>
        protected IDbTransaction DbTransaction;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbProvider" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        protected DbProvider(string connectionString)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            if (ProviderName == null)
            {
                throw new NullReferenceException("Providername was null.");
            }

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            _dbFactory = DbProviderFactories.GetFactory(ProviderName);
            _connectionString = connectionString;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the provider name.
        /// </summary>
        public abstract string ProviderName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The create command.
        /// </summary>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            return DbConnection.CreateCommand();
        }

        /// <summary>
        ///     The create connection.
        /// </summary>
        public void CreateConnection()
        {
            if (DbTransaction != null)
            {
                DbConnection = DbTransaction.Connection;
            }
            else
            {
                CreateNewConnection();
            }
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing == false)
            {
                return;
            }

            DisposeConnection();
            DisposeCommand();
            DisposeTransaction();
        }

        /// <summary>
        ///     The prepare command.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        public IDbCommand PrepareCommand(IQuery query, ISqlDialect sqlDialect)
        {
            CreateConnection();

            IDbCommand dbCommand = query.Compile(sqlDialect);

            dbCommand.Transaction = DbTransaction;

            return dbCommand;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The create new connection.
        /// </summary>
        private void CreateNewConnection()
        {
            if (DbConnection != null && DbConnection.State == ConnectionState.Open)
            {
                return;
            }

            DbConnection = _dbFactory.CreateConnection();

            // ReSharper disable once PossibleNullReferenceException
            DbConnection.ConnectionString = _connectionString;
            DbConnection.Open();
        }

        /// <summary>
        ///     The dispose command.
        /// </summary>
        private void DisposeCommand()
        {
            if (DbCommand == null)
            {
                return;
            }

            DbCommand.Dispose();
            DbCommand = null;
        }

        /// <summary>
        ///     The dispose connection.
        /// </summary>
        private void DisposeConnection()
        {
            if (InTransactionMode() || DbConnection == null)
            {
                return;
            }

            DbConnection.Close();
            DbConnection.Dispose();
            DbConnection = null;
        }

        /// <summary>
        ///     The dispose transaction.
        /// </summary>
        private void DisposeTransaction()
        {
            if (InTransactionMode() || DbTransaction == null)
            {
                return;
            }

            DbTransaction.Dispose();
            DbTransaction = null;
        }

        /// <summary>
        ///     The in transaction mode.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool InTransactionMode()
        {
            return DbTransaction?.Connection != null;
        }

        #endregion
    }
}