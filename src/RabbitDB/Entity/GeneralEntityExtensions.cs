// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralEntityExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The general entity extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;

using RabbitDB.Contracts.Entity;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.SqlBuilder;

#endregion

namespace RabbitDB.Entity
{
    /// <summary>
    ///     The general entity extensions.
    /// </summary>
    internal static class GeneralEntityExtensions
    {
        #region Internal Methods

        /// <summary>
        ///     The prepare for update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="valuesToUpdate">
        ///     The values to update.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Tuple" />.
        /// </returns>
        internal static Tuple<string, QueryParameterCollection> PrepareForUpdate<TEntity>(
            this TEntity entity,
            KeyValuePair<string, object>[] valuesToUpdate)
        {
            string updateStatement = SqlBuilder<TEntity>.CreateUpdateStatement(valuesToUpdate);
            QueryParameterCollection queryParameterCollection = QueryParameterCollection.Create<TEntity>(new object[] { valuesToUpdate });

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            queryParameterCollection.AddRange(tableInfo.GetPrimaryKeyValues(entity));

            return new Tuple<string, QueryParameterCollection>(updateStatement, queryParameterCollection);
        }

        /// <summary>
        ///     The prepare for update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Tuple" />.
        /// </returns>
        internal static Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(this TEntity entity)
            where TEntity : IEntity
        {
            // Any changes made to entity?!
            KeyValuePair<string, object>[] valuesToUpdate = entity.ComputeValuesToUpdate();

            if (valuesToUpdate == null || valuesToUpdate.Length == 0)
            {
                return new Tuple<bool, string, QueryParameterCollection>(false, null, null);
            }

            Tuple<string, QueryParameterCollection> result = entity.PrepareForUpdate(valuesToUpdate);

            return new Tuple<bool, string, QueryParameterCollection>(true, result.Item1, result.Item2);
        }

        #endregion
    }
}