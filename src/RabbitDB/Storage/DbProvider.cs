// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System;
    using System.Data;
    using System.Data.Common;

    using RabbitDB.Query;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The db provider.
    /// </summary>
    internal abstract class DbProvider : IDbProvider
    {
        #region Fields

        /// <summary>
        /// The _db command.
        /// </summary>
        protected IDbCommand DbCommand;

        /// <summary>
        /// The _db connection.
        /// </summary>
        protected IDbConnection DbConnection;

        /// <summary>
        /// The _db transaction.
        /// </summary>
        protected IDbTransaction DbTransaction;

        /// <summary>
        /// The _connection string.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// The _db factory.
        /// </summary>
        private readonly DbProviderFactory _dbFactory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        protected DbProvider(string connectionString)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            if (this.ProviderName == null)
            {
                throw new NullReferenceException("Providername was null.");
            }

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            _dbFactory = DbProviderFactories.GetFactory(this.ProviderName);
            _connectionString = connectionString;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public abstract string ProviderName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create command.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            return this.DbConnection.CreateCommand();
        }

        /// <summary>
        /// The create connection.
        /// </summary>
        public void CreateConnection()
        {
            if (this.DbTransaction != null)
            {
                this.DbConnection = this.DbTransaction.Connection;
            }
            else
            {
                CreateNewConnection();
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            DisposeConnection();
            DisposeCommand();
            DisposeTransaction();
        }

        /// <summary>
        /// The prepare command.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        public IDbCommand PrepareCommand(IQuery query, SqlDialect sqlDialect)
        {
            CreateConnection();
            var dbCommand = query.Compile(sqlDialect);
            dbCommand.Transaction = this.DbTransaction;

            return dbCommand;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create new connection.
        /// </summary>
        private void CreateNewConnection()
        {
            if (this.DbConnection != null && this.DbConnection.State == ConnectionState.Open)
            {
                return;
            }

            this.DbConnection = _dbFactory.CreateConnection();

            // ReSharper disable once PossibleNullReferenceException
            this.DbConnection.ConnectionString = _connectionString;
            this.DbConnection.Open();
        }

        /// <summary>
        /// The dispose command.
        /// </summary>
        private void DisposeCommand()
        {
            if (this.DbCommand == null)
            {
                return;
            }

            this.DbCommand.Dispose();
            this.DbCommand = null;
        }

        /// <summary>
        /// The dispose connection.
        /// </summary>
        private void DisposeConnection()
        {
            if (InTransactionMode() || this.DbConnection == null)
            {
                return;
            }

            this.DbConnection.Close();
            this.DbConnection.Dispose();
            this.DbConnection = null;
        }

        /// <summary>
        /// The dispose transaction.
        /// </summary>
        private void DisposeTransaction()
        {
            if (InTransactionMode() || this.DbTransaction == null)
            {
                return;
            }

            this.DbTransaction.Dispose();
            this.DbTransaction = null;
        }

        /// <summary>
        /// The in transaction mode.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InTransactionMode()
        {
            return this.DbTransaction != null && this.DbTransaction.Connection != null;
        }

        #endregion
    }
}