using System;

namespace MicroORM.Base
{
    public class PrimaryKeyException : Exception
    {
        internal PrimaryKeyException() : base() { }

        internal PrimaryKeyException(string message) : base(message) { }

        internal PrimaryKeyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
