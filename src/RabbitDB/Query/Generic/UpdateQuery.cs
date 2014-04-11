// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The update query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using RabbitDB.Entity;
    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;
    using RabbitDB.Utils;

    /// <summary>
    /// The update query.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class UpdateQuery<TEntity> : IQuery
    {
        #region Fields

        /// <summary>
        /// The _entity.
        /// </summary>
        private readonly TEntity _entity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuery{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal UpdateQuery(TEntity entity)
        {
            _entity = entity;
        }

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
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public IDbCommand Compile(SqlDialect sqlDialect)
        {
            var valuesToUpdate = new EntityArgumentsReader().GetEntityArguments(
                _entity, 
                TableInfo<TEntity>.GetTableInfo);

            if (valuesToUpdate == null || valuesToUpdate.Length <= 0)
            {
                throw new InvalidOperationException("Entity had no properties provided!");
            }

            var result =
                _entity.PrepareForUpdate(valuesToUpdate[0] as KeyValuePair<string, object>[]);

            var sqlQuery = new SqlQuery(result.Item1, result.Item2);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}