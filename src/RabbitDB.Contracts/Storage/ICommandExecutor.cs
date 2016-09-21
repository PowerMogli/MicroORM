// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandExecutor.cs" company="">
//   
// </copyright>
// <summary>
//   The CommandExecutor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Reader;

namespace RabbitDB.Contracts.Storage
{
    /// <summary>
    /// The CommandExecutor interface.
    /// </summary>
    internal interface ICommandExecutor
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute command.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        void ExecuteCommand(IQuery query);

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        IDataReader ExecuteReader(IQuery query);

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityReader"/>.
        /// </returns>
        IEntityReader<T> ExecuteReader<T>(IQuery query);

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T ExecuteScalar<T>(IQuery query);

        #endregion
    }
}