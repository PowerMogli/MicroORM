using System.Data;
using MicroORM.Base;
using MicroORM.Mapping;
using System.Collections.Generic;

namespace MicroORM.Materialization
{
    class Materializer
    {
        internal T Materialize<T>(KeyValuePair<string, object> entityValues)
        {
            TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));


            return default(T);
        }
    }
}
