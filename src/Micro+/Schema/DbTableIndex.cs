using System.Collections.Generic;

namespace MicroORM.Schema
{
    internal class DbTableIndex
    {
        public string Name;
        public List<DbIndexColumn> IndexColumns;
        public bool IsUnique;
        public string SQL;
    }
}
