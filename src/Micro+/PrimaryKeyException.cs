using System;

namespace MicroORM.Base
{
    class PrimaryKeyException : Exception
    {
        internal PrimaryKeyException() : base() { }

        internal PrimaryKeyException(string message) : base(message) { }

        internal PrimaryKeyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
