using System.Collections.ObjectModel;
using System.Linq;

namespace MicroORM.Schema
{
    internal class DbTableCollection : Collection<DbTable>
    {
        public DbTableCollection() { }

        public DbTable GetTable(string tableName)
        {
            return this.Single(dbTable => string.Compare(dbTable.Name, tableName, true) == 0);
        }

        public DbTable this[string tableName]
        {
            get { return GetTable(tableName); }
        }
    }
}
