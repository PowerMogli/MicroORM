using System;

namespace MicroORM.Base
{
    class InvalidPrimaryKeyException : Exception
    {
        internal InvalidPrimaryKeyException() : base() { }

        internal InvalidPrimaryKeyException(string message) : base(message) { }

        internal InvalidPrimaryKeyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
