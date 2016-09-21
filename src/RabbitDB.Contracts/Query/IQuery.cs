// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The Query interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

using RabbitDB.Contracts.SqlDialect;

namespace RabbitDB.Contracts.Query
{
    /// <summary>
    /// The Query interface.
    /// </summary>
    internal interface IQuery
    {
        #region Public Methods and Operators

        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        IDbCommand Compile(ISqlDialect sqlDialect);

        #endregion
    }
}