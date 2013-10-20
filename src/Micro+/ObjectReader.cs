using System;
using System.Data;
using MicroORM.Mapping;
using MicroORM.Materialization;
using MicroORM.Storage;

namespace MicroORM.Base
{
    internal class ObjectReader<T> : IDisposable
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

        internal T Current { get; private set; }

        internal bool Read()
        {
            return Read(1);
        }

        internal bool Read(int step)
        {
            if (step < 0)
                throw new ArgumentException("Step is lower then 1. This is not allowed!", "step");

            for (int i = 0; i < step; i++)
            {
                if (_dataReader.Read() == false) { return false; }
            }

            if (_tableInfo != null) { return ReadInternal(); }

            return GetListOfValues();
        }

        private bool GetListOfValues()
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

        public void Dispose()
        {
            if (_dataReader == null || _dataReader.IsClosed) return;

            _dataReader.Close();
            _dataReader.Dispose();
        }
    }
}
