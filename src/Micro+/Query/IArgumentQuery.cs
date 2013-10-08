using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Query
{
    interface IArgumentQuery
    {
        object[] Arguments { get; }
        string SqlStatement { get; }
    }
}
