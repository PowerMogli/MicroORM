// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionalDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The TransactionalDbProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System.Data;

    /// <summary>
    /// The TransactionalDbProvider interface.
    /// </summary>
    internal interface ITransactionalDbProvider
    {
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
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbTransaction"/>.
        /// </returns>
        IDbTransaction BeginTransaction();

        #endregion
    }
}