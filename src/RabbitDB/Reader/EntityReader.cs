using RabbitDB.Mapping;
using RabbitDB.Materialization;
using RabbitDB.Storage;
using System;
using System.Collections.Generic;
using System.Data;

namespace RabbitDB.Reader
{
    public class EntityReader<TEntity> : IDisposable
    {
        private IDataReader _dataReader;
        private IEntityMaterializer _materializer;
        private IDataSchemaCreator _dataReaderSchema;
        private IDbProvider _dbProvider;
        private TableInfo _tableInfo;
        private bool _disposed;

        internal EntityReader(IDataReader dataReader, IDbProvider dbProvider, IEntityMaterializer materializer)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
            _dbProvider = dbProvider;
            _dataReader = dataReader;
            _materializer = materializer;
            _dataReaderSchema = CreateDataSchemaCreator();
        }

        /// <summary>
        /// Returns the current materialized entity object.
        /// </summary>
        public TEntity Current { get; private set; }

        internal virtual IDataSchemaCreator CreateDataSchemaCreator()
        {
            return new DataSchemaCreator(_dataReader, _tableInfo);
        }

        /// <summary>
        /// Moves the reader to the next position in data stream.
        /// </summary>
        /// <returns>True if there have been any elements in the reader.</returns>
        public bool Read()
        {
            return Read(1);
        }

        private bool Read(int step)
        {
            if (step < 0)
                throw new ArgumentException("Step is lower then 1. This is not allowed!", "step");

            for (int i = 0; i < step; i++)
            {
                if (_dataReader.Read() == false)
                {
                    Dispose();
                    return false;
                }
            }

            return _tableInfo != null ?
                ReadInternal() :
                GetListOfPrimitivValues();
        }

        private bool GetListOfPrimitivValues()
        {
            this.Current = (TEntity)_dataReader.GetValue(0);
            return true;
        }

        internal bool ReadInternal()
        {
            this.Current = _materializer.Materialize<TEntity>(_dataReaderSchema, _dataReader);
            return true;
        }

        internal bool Load(TEntity entity)
        {
            if (_dataReader.Read() == false) return false;

            this.Current = _materializer.Materialize<TEntity>(entity, _dataReaderSchema, _dataReader);
            return true;
        }

        internal void Load(TEntity entity, Action<TEntity, IDataReader> materializer)
        {
            materializer(entity, _dataReader);
        }

        internal IEnumerable<TEntity> Load(Func<IDataReader, IEnumerable<TEntity>> materializer)
        {
            return _materializer.Materialize<TEntity>(materializer, _dataReader);
        }

        internal void Dispose()
        {
            if (_disposed
                || _dataReader == null
                || _dataReader.IsClosed
                || _dataReader.NextResult()) return;

            _dataReader.Close();
            _dataReader.Dispose();
            _dbProvider.Dispose();
            _disposed = true;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}