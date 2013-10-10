using System;
using System.Data;
using MicroORM.Materialization;
using MicroORM.Storage;
using System.Collections.Generic;

namespace MicroORM.Base
{
    internal class ObjectReader<T> : IDisposable
    {
        private IDataReader _dataReader;
        private EntityMaterializer _materlizer;
        private DataReaderSchema _dataReaderSchema;
        private IDbProvider _dbProvider;

        internal ObjectReader(IDataReader dataReader, IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
            _dataReader = dataReader;
            _materlizer = new EntityMaterializer(dbProvider);
            _dataReaderSchema = new DataReaderSchema(dataReader, typeof(T));
        }

        internal T Current { get; private set; }

        internal bool Read()
        {
            return Read(1);
        }

        internal bool Read(int step)
        {
            if (step < 0)
                throw new ArgumentException("Step is lower then 1. Not allowed.", "step");

            for (int i = 0; i < step; i++)
            {
                if (_dataReader.Read() == false) return false;
            }

            return ReadInternal();
        }

        private bool ReadInternal()
        {
            this.Current = _materlizer.Materialize<T>(_dataReaderSchema, _dataReader);
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
