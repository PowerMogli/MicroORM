﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The DbProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System;
    using System.Data;

    using RabbitDB.Query;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The DbProvider interface.
    /// </summary>
    internal interface IDbProvider : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets the provider name.
        /// </summary>
        string ProviderName { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create command.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// The create connection.
        /// </summary>
        void CreateConnection();

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
        IDbCommand PrepareCommand(IQuery query, SqlDialect sqlDialect);

        #endregion
    }
}