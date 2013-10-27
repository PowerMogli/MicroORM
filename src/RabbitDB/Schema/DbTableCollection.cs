using System.Collections.ObjectModel;
using System.Linq;

namespace RabbitDB.Schema
{
    internal class DbTableCollection : Collection<DbTable>
    {
        public DbTableCollection() { }

        internal DbTable GetTable(string tableName)
        {
            return this.SingleOrDefault(dbTable => string.Compare(dbTable.Name, tableName, true) == 0);
        }

        internal DbTable this[string tableName]
        {
            get { return GetTable(tableName); }
        }
    }
}
