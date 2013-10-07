using System;

namespace MicroORM.Storage
{
    public class ObjectReader<T> : IDisposable
    {
        public T Current { get; private set; }

        public bool Read()
        {
            return false;
        }

        public void Dispose()
        {
        }
    }
}
