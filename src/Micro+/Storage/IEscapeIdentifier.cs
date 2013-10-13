using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Storage
{
    public interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }
}
