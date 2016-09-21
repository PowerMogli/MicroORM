// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The entity query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Entity;
using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;
using RabbitDB.Query.Generic;

#endregion

namespace RabbitDB.Query
{
    /// <summary>
    ///     The entity query.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class EntityQuery<T> : IQuery
        where T : IEntity
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityQuery{T}" /> class.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        internal EntityQuery(T entity)
        {
            Entity = entity;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the entity.
        /// </summary>
        private T Entity { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compile.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        public IDbCommand Compile(ISqlDialect sqlDialect)
        {
            TableInfo tableInfo = TableInfo<T>.GetTableInfo;

            object[] primaryKeyValues = tableInfo.GetPrimaryKeyValues(Entity);

            SqlQuery<T> sqlQuery = new SqlQuery<T>(primaryKeyValues, null);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}