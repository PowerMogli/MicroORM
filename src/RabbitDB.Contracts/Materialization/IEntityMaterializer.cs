// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityMaterializer.cs" company="">
//   
// </copyright>
// <summary>
//   The EntityMaterializer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;

namespace RabbitDB.Contracts.Materialization
{
    /// <summary>
    /// The EntityMaterializer interface.
    /// </summary>
    internal interface IEntityMaterializer
    {
        #region Public Methods and Operators

        /// <summary>
        /// The materialize.
        /// </summary>
        /// <param name="dataReaderSchema">
        /// The data reader schema.
        /// </param>
        /// <param name="dataRecord">
        /// The data record.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Materialize<TEntity>(IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord);

        /// <summary>
        /// The materialize.
        /// </summary>
        /// <param name="materializer">
        /// The materializer.
        /// </param>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEnumerable<TEntity> Materialize<TEntity>(
            Func<IDataReader, IEnumerable<TEntity>> materializer, 
            IDataReader dataReader);

        /// <summary>
        /// The materialize.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="dataReaderSchema">
        /// The data reader schema.
        /// </param>
        /// <param name="dataRecord">
        /// The data record.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Materialize<TEntity>(TEntity entity, IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord);

        #endregion
    }
}