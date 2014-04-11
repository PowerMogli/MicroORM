// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityReader.cs" company="">
//   
// </copyright>
// <summary>
//   The entity reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using RabbitDB.Mapping;
    using RabbitDB.Materialization;
    using RabbitDB.Storage;

    /// <summary>
    /// The entity reader.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public sealed class EntityReader<TEntity> : IDisposable
    {
        #region Fields

        /// <summary>
        /// The _data reader.
        /// </summary>
        private readonly IDataReader _dataReader;

        /// <summary>
        /// The _data reader schema.
        /// </summary>
        private readonly IDataSchemaCreator _dataReaderSchema;

        /// <summary>
        /// The _db provider.
        /// </summary>
        private readonly IDbProvider _dbProvider;

        /// <summary>
        /// The _materializer.
        /// </summary>
        private readonly IEntityMaterializer _materializer;

        /// <summary>
        /// The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        /// <summary>
        /// The _disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityReader{TEntity}"/> class.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        /// <param name="materializer">
        /// The materializer.
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

        #region Public Properties

        /// <summary>
        /// Returns the current materialized entity object.
        /// </summary>
        public TEntity Current { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Moves the reader to the next position in data stream.
        /// </summary>
        /// <returns>True if there have been any elements in the reader.</returns>
        public bool Read()
        {
            return Read(1);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create data schema creator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDataSchemaCreator"/>.
        /// </returns>
        internal IDataSchemaCreator CreateDataSchemaCreator()
        {
            return new DataSchemaCreator(_dataReader, _tableInfo);
        }

        /// <summary>
        /// The dispose.
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
        /// The load.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool Load(TEntity entity)
        {
            if (_dataReader.Read() == false)
            {
                return false;
            }

            this.Current = _materializer.Materialize(entity, _dataReaderSchema, _dataReader);

            return true;
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="materializer">
        /// The materializer.
        /// </param>
        internal void Load(TEntity entity, Action<TEntity, IDataReader> materializer)
        {
            materializer(entity, _dataReader);
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="materializer">
        /// The materializer.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal IEnumerable<TEntity> Load(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            return _materializer.Materialize(materializer, _dataReader);
        }

        /// <summary>
        /// The read internal.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool ReadInternal()
        {
            this.Current = _materializer.Materialize<TEntity>(_dataReaderSchema, _dataReader);

            return true;
        }

        /// <summary>
        /// The get list of primitiv values.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool GetListOfPrimitivValues()
        {
            this.Current = (TEntity)_dataReader.GetValue(0);

            return true;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="step">
        /// The step.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private bool Read(int step)
        {
            if (step < 0)
            {
                throw new ArgumentException("Step is lower then 1. This is not allowed!", "step");
            }

            for (var i = 0; i < step; i++)
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