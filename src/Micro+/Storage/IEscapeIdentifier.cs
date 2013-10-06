using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Base.Storage
{
    internal interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }
}
