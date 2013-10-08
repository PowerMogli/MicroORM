using System;
using System.Data;
using MicroORM.Materialization;

namespace MicroORM.Base
{
    internal class ObjectReader<T> : IDisposable
    {
        private IDataReader _dataReader;
        private Materializer _materlizer;
        private DataSchema _dataSchema;

        internal ObjectReader(IDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        public T Current { get; private set; }

        public bool Read()
        {
            return false;
        }

        public void Dispose()
        {
            if (_dataReader == null) return;

            _dataReader.Close();
            _dataReader.Dispose();
        }
    }
}
