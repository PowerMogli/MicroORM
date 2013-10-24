using System;
using System.Collections.Generic;
using RabbitDB.Mapping;
using RabbitDB.Storage;

namespace RabbitDB.Schema
{
    internal abstract class DbSchemaReader : IDisposable
    {
        protected IDbProvider DbProvider { get; set; }
        protected DbTableCollection Tables { get; private set; }

        internal DbSchemaReader()
        {
            this.Tables = new DbTableCollection();
        }

        internal DbTable ReadSchema<T>()
        {
            DbTable dbTable = null;
            using (this)
            {
                TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));
                if (tableInfo == null) return null;

                else if ((dbTable = this.Tables[tableInfo.Name]) != null)
                    return dbTable;

                dbTable = GetTable(tableInfo.Name);
                dbTable.DbColumns = GetColumns(dbTable);
                SetPrimaryKeys(dbTable);
                this.Tables.Add(dbTable);
            }
            return dbTable;
        }

        protected abstract void SetPrimaryKeys(DbTable dbTable);
        protected abstract List<DbColumn> GetColumns(DbTable dbTable);
        protected abstract DbTable GetTable(string tableName);
        protected abstract List<string> GetPrimaryKeys(string table);

        public void Dispose()
        {
            this.DbProvider.Dispose();
        }

        public void Flush()
        {
            this.Tables.Clear();
        }
    }
}
