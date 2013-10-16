using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroORM.Query
{
    interface IArgumentQuery
    {
        QueryParameterCollection Arguments { get; }
        string SqlStatement { get; }
    }
}
