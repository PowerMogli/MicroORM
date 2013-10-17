using System;

namespace MicroORM.Base.Storage
{
    public class NotSupportedProviderException : Exception
    {
        internal NotSupportedProviderException() : base() { }

        internal NotSupportedProviderException(string message) : base(message) { }

        internal NotSupportedProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
