// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The update query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;
using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Entity;
using RabbitDB.Mapping;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.Query.Generic
{
    /// <summary>
    ///     The update query.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class UpdateQuery<TEntity> : IQuery
    {
        #region Fields

        /// <summary>
        ///     The _entity.
        /// </summary>
        private readonly TEntity _entity;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateQuery{TEntity}" /> class.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        internal UpdateQuery(TEntity entity)
        {
            _entity = entity;
        }

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
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public IDbCommand Compile(ISqlDialect sqlDialect)
        {
            object[] valuesToUpdate = new EntityArgumentsReader().GetEntityArguments(_entity, TableInfo<TEntity>.GetTableInfo);

            if (valuesToUpdate == null || valuesToUpdate.Length <= 0)
            {
                throw new InvalidOperationException("Entity had no properties provided!");
            }

            Tuple<string, QueryParameterCollection> result = _entity.PrepareForUpdate(valuesToUpdate[0] as KeyValuePair<string, object>[]);

            SqlQuery sqlQuery = new SqlQuery(result.Item1, result.Item2);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}