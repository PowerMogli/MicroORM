// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The entity query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System.Data;

    using RabbitDB.Entity;
    using RabbitDB.Mapping;
    using RabbitDB.Query.Generic;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The entity query.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class EntityQuery<T> : IQuery
        where T : Entity
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityQuery{T}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal EntityQuery(T entity)
        {
            this.Entity = entity;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        internal T Entity { get; set; }

        #endregion

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
        public IDbCommand Compile(SqlDialect sqlDialect)
        {
            var tableInfo = TableInfo<T>.GetTableInfo;
            var primaryKeyValues = tableInfo.GetPrimaryKeyValues(this.Entity);

            var sqlQuery = new SqlQuery<T>(primaryKeyValues, null);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}