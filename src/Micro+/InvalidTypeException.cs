using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM
{
    class InvalidTypeException : Exception
    {
        internal InvalidTypeException()
            : base() { }

        internal InvalidTypeException(string message)
            : base(message) { }

        internal InvalidTypeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
