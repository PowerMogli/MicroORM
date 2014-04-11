// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbCommandExecutor.cs" company="">
//   
// </copyright>
// <summary>
//   The DbCommandExecutor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System.Data;

    using RabbitDB.Reader;

    /// <summary>
    /// The DbCommandExecutor interface.
    /// </summary>
    internal interface IDbCommandExecutor
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the null value resolver.
        /// </summary>
        INullValueResolver NullValueResolver { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute command.
        /// </summary>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        void ExecuteCommand(IDbCommand dbCommand);

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        IDataReader ExecuteReader(IDbCommand dbCommand);

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        EntityReader<T> ExecuteReader<T>(IDbCommand dbCommand);

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="dbCommand">
        /// The db command.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T ExecuteScalar<T>(IDbCommand dbCommand);

        #endregion
    }
}