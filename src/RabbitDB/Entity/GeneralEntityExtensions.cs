// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralEntityExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The general entity extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity
{
    using System;
    using System.Collections.Generic;

    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.SqlBuilder;

    /// <summary>
    /// The general entity extensions.
    /// </summary>
    internal static class GeneralEntityExtensions
    {
        #region Methods

        /// <summary>
        /// The prepare for update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="valuesToUpdate">
        /// The values to update.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        internal static Tuple<string, QueryParameterCollection> PrepareForUpdate<TEntity>(
            this TEntity entity, 
            KeyValuePair<string, object>[] valuesToUpdate)
        {
            var updateStatement = SqlBuilder<TEntity>.CreateUpdateStatement(valuesToUpdate);
            var queryParameterCollection =
                QueryParameterCollection.Create<TEntity>(new object[] { valuesToUpdate });

            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            queryParameterCollection.AddRange(tableInfo.GetPrimaryKeyValues(entity));

            return new Tuple<string, QueryParameterCollection>(updateStatement, queryParameterCollection);
        }

        /// <summary>
        /// The prepare for update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        internal static Tuple<bool, string, QueryParameterCollection> PrepareForUpdate<TEntity>(this TEntity entity)
            where TEntity : Entity
        {
            // Any changes made to entity?!
            var valuesToUpdate = entity.ComputeValuesToUpdate();

            if (valuesToUpdate == null || valuesToUpdate.Length == 0)
            {
                return new Tuple<bool, string, QueryParameterCollection>(false, null, null);
            }

            var result = entity.PrepareForUpdate(valuesToUpdate);

            return new Tuple<bool, string, QueryParameterCollection>(true, result.Item1, result.Item2);
        }

        #endregion
    }
}