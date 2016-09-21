// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="EntityMaterializer.cs">
//   
// </copyright>
// <summary>
//   The entity materializer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;
using System.Data;

using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Materialization;
using RabbitDB.Contracts.Storage;
using RabbitDB.Mapping;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Materialization
{
    /// <summary>
    ///     The entity materializer.
    /// </summary>
    class EntityMaterializer : IEntityMaterializer
    {
        #region Fields

        /// <summary>
        ///     The _null value resolver.
        /// </summary>
        private readonly INullValueResolver _nullValueResolver;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityMaterializer" /> class.
        /// </summary>
        /// <param name="nullValueResolver">
        ///     The null value resolver.
        /// </param>
        internal EntityMaterializer(INullValueResolver nullValueResolver)
        {
            _nullValueResolver = nullValueResolver;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The materialize.
        /// </summary>
        /// <param name="materializer">
        ///     The materializer.
        /// </param>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<TEntity> Materialize<TEntity>(
            Func<IDataReader, IEnumerable<TEntity>> materializer,
            IDataReader dataReader)
        {
            return materializer(dataReader);
        }

        /// <summary>
        ///     The materialize.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="dataReaderSchema">
        ///     The data reader schema.
        /// </param>
        /// <param name="dataRecord">
        ///     The data record.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEntity" />.
        /// </returns>
        public TEntity Materialize<TEntity>(TEntity entity, IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord)
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            for (int index = 0; index < tableInfo.Columns.Count; index++)
            {
                IPropertyInfo propertyInfo = tableInfo.Columns[index];

                int columnIndex = dataReaderSchema.ColumnIndex(index);

                if (columnIndex < 0)
                {
                    continue;
                }

                MaterializeEntity(entity, propertyInfo, dataRecord[columnIndex]);
            }

            return entity;
        }

        /// <summary>
        ///     The materialize.
        /// </summary>
        /// <param name="dataReaderSchema">
        ///     The data reader schema.
        /// </param>
        /// <param name="dataRecord">
        ///     The data record.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TEntity" />.
        /// </returns>
        public TEntity Materialize<TEntity>(IDataSchemaCreator dataReaderSchema, IDataRecord dataRecord)
        {
            TEntity entity = Activator.CreateInstance<TEntity>();

            return Materialize(entity, dataReaderSchema, dataRecord);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The materialize entity.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="propertyInfo">
        ///     The property info.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        private void MaterializeEntity(object entity, IPropertyInfo propertyInfo, object value)
        {
            if (!propertyInfo.CanWrite)
            {
                return;
            }

            if (Convert.IsDBNull(value))
            {
                value = propertyInfo.IsNullable
                    ? null
                    : _nullValueResolver.ResolveNullValue(value, propertyInfo.PropertyType);
            }

            propertyInfo.SetValue(entity, value);
        }

        #endregion
    }
}