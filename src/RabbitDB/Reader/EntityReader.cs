// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReader.cs" company="">
//   
// </copyright>
// <summary>
//   The entity reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;
using System.Data;

using RabbitDB.Contracts.Materialization;
using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Storage;
using RabbitDB.Mapping;
using RabbitDB.Materialization;

#endregion

namespace RabbitDB.Reader
{
    /// <summary>
    ///     The entity reader.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public sealed class EntityReader<TEntity> : IEntityReader<TEntity>
    {
        #region Fields

        /// <summary>
        ///     The _data reader.
        /// </summary>
        private readonly IDataReader _dataReader;

        /// <summary>
        ///     The _data reader schema.
        /// </summary>
        private readonly IDataSchemaCreator _dataReaderSchema;

        /// <summary>
        ///     The _db provider.
        /// </summary>
        private readonly IDbProvider _dbProvider;

        /// <summary>
        ///     The _materializer.
        /// </summary>
        private readonly IEntityMaterializer _materializer;

        /// <summary>
        ///     The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        /// <summary>
        ///     The _disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityReader{TEntity}" /> class.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        /// <param name="materializer">
        ///     The materializer.
        /// </param>
        internal EntityReader(IDataReader dataReader, IDbProvider dbProvider, IEntityMaterializer materializer)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
            _dbProvider = dbProvider;
            _dataReader = dataReader;
            _materializer = materializer;
            _dataReaderSchema = CreateDataSchemaCreator();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Returns the current materialized entity object.
        /// </summary>
        public TEntity Current { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Load(TEntity entity)
        {
            if (_dataReader.Read() == false)
            {
                return false;
            }

            Current = _materializer.Materialize(entity, _dataReaderSchema, _dataReader);

            return true;
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="materializer">
        ///     The materializer.
        /// </param>
        public void Load(TEntity entity, Action<TEntity, IDataReader> materializer)
        {
            materializer(entity, _dataReader);
        }

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="materializer">
        ///     The materializer.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<TEntity> Load(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            return _materializer.Materialize(materializer, _dataReader);
        }

        /// <summary>
        ///     Moves the reader to the next position in data stream.
        /// </summary>
        /// <returns>True if there have been any elements in the reader.</returns>
        public bool Read()
        {
            return Read(1);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The create data schema creator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IDataSchemaCreator" />.
        /// </returns>
        internal IDataSchemaCreator CreateDataSchemaCreator()
        {
            return new DataSchemaCreator(_dataReader, _tableInfo);
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        internal void Dispose()
        {
            if (_disposed
                || _dataReader == null
                || _dataReader.IsClosed
                || _dataReader.NextResult())
            {
                return;
            }

            _dataReader.Close();
            _dataReader.Dispose();
            _dbProvider.Dispose();
            _disposed = true;
        }

        /// <summary>
        ///     The read internal.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool ReadInternal()
        {
            Current = _materializer.Materialize<TEntity>(_dataReaderSchema, _dataReader);

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
        }

        /// <summary>
        ///     The get list of primitiv values.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool GetListOfPrimitivValues()
        {
            Current = (TEntity)_dataReader.GetValue(0);

            return true;
        }

        /// <summary>
        ///     The read.
        /// </summary>
        /// <param name="step">
        ///     The step.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private bool Read(int step)
        {
            if (step < 0)
            {
                throw new ArgumentException("Step is lower then 1. This is not allowed!", nameof(step));
            }

            for (int i = 0; i < step; i++)
            {
                if (_dataReader.Read())
                {
                    continue;
                }

                Dispose();

                return false;
            }

            return _tableInfo != null
                ? ReadInternal()
                : GetListOfPrimitivValues();
        }

        #endregion
    }
}