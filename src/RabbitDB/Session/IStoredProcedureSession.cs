// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStoredProcedureSession.cs" company="">
//   
// </copyright>
// <summary>
//   The StoredProcedureSession interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using System;

    using RabbitDB.Query.StoredProcedure;

    /// <summary>
    /// The StoredProcedureSession interface.
    /// </summary>
    internal interface IStoredProcedureSession : ITransactionalSession, IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        void ExecuteStoredProcedure(StoredProcedure procedureObject);

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments);

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="procedureObject">
        /// The procedure object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T ExecuteStoredProcedure<T>(StoredProcedure procedureObject);

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T ExecuteStoredProcedure<T>(string storedProcedureName, params object[] arguments);

        #endregion
    }
}