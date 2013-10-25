using System;
using System.Data;
using RabbitDB.Mapping;
using RabbitDB.Materialization;
using RabbitDB.Storage;

namespace RabbitDB.Base
{
    public class ObjectReader<T> : IDisposable
    {
        private IDataReader _dataReader;
        private EntityMaterializer _materlizer;
        private DataReaderSchema _dataReaderSchema;
        private IDbProvider _dbProvider;
        private TableInfo _tableInfo;

        internal ObjectReader(IDataReader dataReader, IDbProvider dbProvider)
        {
            _tableInfo = TableInfo<T>.GetTableInfo;
            _dbProvider = dbProvider;
            _dataReader = dataReader;
            _materlizer = new EntityMaterializer(dbProvider);
            _dataReaderSchema = new DataReaderSchema(dataReader, _tableInfo);
        }

        public T Current { get; private set; }

        public bool Read()
        {
            return Read(1);
        }

        public bool Read(int step)
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

            if (_tableInfo != null)
            {
                return ReadInternal();
            }

            return GetListOfPrimitivValues();
        }

        private bool GetListOfPrimitivValues()
        {
            this.Current = (T)_dataReader.GetValue(0);
            return true;
        }

        private bool ReadInternal()
        {
            this.Current = _materlizer.Materialize<T>(_dataReaderSchema, _dataReader);
            return true;
        }

        internal bool Load(T entity)
        {
            if (_dataReader.Read() == false) return false;

            this.Current = _materlizer.Materialize<T>(entity, _dataReaderSchema, _dataReader);
            return true;
        }

        internal void Dispose()
        {
            if (_dataReader == null || _dataReader.IsClosed) return;

            _dataReader.Close();
            _dataReader.Dispose();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
