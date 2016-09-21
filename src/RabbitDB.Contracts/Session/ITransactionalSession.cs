// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionalSession.cs" company="">
//   
// </copyright>
// <summary>
//   The TransactionalSession interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

namespace RabbitDB.Contracts.Session
{
    /// <summary>
    /// The TransactionalSession interface.
    /// </summary>
    internal interface ITransactionalSession
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
        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);

        #endregion
    }
}