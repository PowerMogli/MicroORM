using RabbitDB.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Schema
{
    internal class DbTable
    {
        internal DbTable() { }

        internal DbTable(string tableName)
        {
            this.Name = tableName;
        }

        public List<DbColumn> DbColumns { get; set; }
        //public List<DbTableIndex> Indices { get; set; }
        public List<DbForeignKey> ForeignKeys { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public bool IsView { get; set; }
        public string CleanName { get; set; }
        public string SequenceName { get; set; }
        public bool Ignore { get; set; }
        public string SQL { get; set; }

        private List<DbColumn> _primaryKeys;
        public List<DbColumn> PrimaryKeys
        {
            get { return _primaryKeys ?? (_primaryKeys = this.DbColumns.Where(column => column.IsPrimaryKey).ToList()); }
        }

        public DbColumn GetColumn(string columnName)
        {
            return DbColumns.Single(column => string.Compare(column.Name, columnName, true) == 0);
        }

        public DbColumn this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }

        public bool HasPrimaryKeys
        {
            get { return PrimaryKeys != null && this.PrimaryKeys.Count > 0; }
        }

        //public DbTableIndex GetIndex(string indexName)
        //{
        //    return Indices.Single(tableIndex => string.Compare(tableIndex.Name, indexName, true) == 0);
        //}

        internal bool SkipWhile(string resolvedColumnName)
        {
            return DbColumns.Find(dbColumn => dbColumn.IsToSkip(resolvedColumnName)) != null;
        }
    }
}
