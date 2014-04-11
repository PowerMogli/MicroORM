// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The Query interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System.Data;

    using RabbitDB.SqlDialect;

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
        IDbCommand Compile(SqlDialect sqlDialect);

        #endregion
    }
}