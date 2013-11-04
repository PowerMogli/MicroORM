using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private void SetPrimaryKeys(DbTable dbTable)
        {
            List<string> primaryKeys = GetPrimaryKeys(dbTable.Name);

            foreach (string primaryKey in primaryKeys)
            {
                DbColumn primaryKeyColumn = dbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
                if (primaryKeyColumn == null)
                    throw new MissingPrimaryKeyException("Not all primaryKeys were provided.");

                primaryKeyColumn.IsPrimaryKey = true;
            }
        }

        protected abstract List<DbColumn> GetColumns(DbTable dbTable);
        protected abstract DbTable GetTable(string tableName);
        protected abstract List<string> GetPrimaryKeys(string table);

        public void Dispose()
        {
            this.DbProvider.Dispose();
        }
    }
}