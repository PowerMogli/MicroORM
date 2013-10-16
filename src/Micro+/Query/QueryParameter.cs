using System.Data;
using System;

namespace MicroORM.Query
{
    class QueryParameter
    {
        internal DbType DbType { get; private set; }
        internal string Name { get; private set; }
        internal int? Size { get; private set; }
        internal object Value { get; private set; }

        internal QueryParameter(string name, DbType dbType, object value, int size = -1)
        {
            this.Name = name;
            this.DbType = dbType;
            this.Value = value;
            this.Size = size > 0 ? new Nullable<int>(size) : null;
        }
    }
}
