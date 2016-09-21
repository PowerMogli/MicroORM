// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionalDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The TransactionalDbProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

namespace RabbitDB.Contracts.Storage
{
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