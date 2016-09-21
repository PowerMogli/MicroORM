// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionalDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The transactional db provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using RabbitDB.Contracts.Storage;

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
            if (DbTransaction != null)
            {
                return DbTransaction;
            }

            if (DbConnection != null)
            {
                return DbTransaction = DbConnection.BeginTransaction(isolationLevel);
            }

            CreateConnection();
            return DbTransaction = DbConnection?.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/>.
        /// </returns>
        public IDbTransaction BeginTransaction()
        {
            if (DbTransaction != null)
            {
                return DbTransaction;
            }

            if (DbConnection != null)
            {
                return DbTransaction = DbConnection.BeginTransaction();
            }

            CreateConnection();

            // ReSharper disable once PossibleNullReferenceException
            return DbTransaction = DbConnection.BeginTransaction();
        }

        #endregion
    }
}