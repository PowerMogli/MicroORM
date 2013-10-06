using System;

namespace MicroORM.Base
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
