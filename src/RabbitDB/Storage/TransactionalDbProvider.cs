// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionalDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The transactional db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System.Data;

    /// <summary>
    /// The transactional db provider.
    /// </summary>
    internal abstract class TransactionalDbProvider : DbProvider, ITransactionalDbProvider
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalDbProvider"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        internal TransactionalDbProvider(string connectionString)
            : base(connectionString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public abstract override string ProviderName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/>.
        /// </returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.DbTransaction != null)
            {
                return this.DbTransaction;
            }

            if (this.DbConnection != null)
            {
                return this.DbTransaction = this.DbConnection.BeginTransaction(isolationLevel);
            }

            CreateConnection();
            return this.DbTransaction = this.DbConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/>.
        /// </returns>
        public IDbTransaction BeginTransaction()
        {
            if (base.DbTransaction != null)
            {
                return base.DbTransaction;
            }

            if (base.DbConnection != null)
            {
                return base.DbTransaction = base.DbConnection.BeginTransaction();
            }

            CreateConnection();

            // ReSharper disable once PossibleNullReferenceException
            return base.DbTransaction = base.DbConnection.BeginTransaction();
        }

        #endregion
    }
}