using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MicroORM.Base;
using MicroORM.Schema;
using MicroORM.Storage;

namespace MicroORM.Mapping
{
    internal sealed class TableInfo
    {
        private bool _isAnonymousType;

        internal TableInfo(Type type, string tableName)
        {
            this.EntityType = type;
            this.Name = tableName;
            this.Columns = new PropertyInfoCollection();

            _isAnonymousType = Utils.Utils.CheckIfAnonymousType(type);
        }

        private string SelectStatement { get; set; }
        private string InsertStatement { get; set; }
        private string DeleteStatement { get; set; }

        private int _numberOfPrimaryKeys = -1;
        internal int NumberOfPrimaryKeys
        {
            get
            {
                if (_numberOfPrimaryKeys != -1) return _numberOfPrimaryKeys;

                return _numberOfPrimaryKeys = this.Columns.Where(columns => columns.ColumnAttribute.IsPrimaryKey).Count();
            }
        }

        internal string Name { get; private set; }

        internal Type EntityType { get; private set; }

        private DbTable _dbTable;
        internal DbTable DbTable
        {
            get { return _dbTable; }
            set
            {
                if (value == null || this.DbTable != null)
                    return;

                _dbTable = value;
                CleanUpColumns();
                ReconfigureWith();
            }
        }

        internal bool IsAnonymousType
        {
            get { return _isAnonymousType; }
        }

        internal PropertyInfoCollection Columns { get; private set; }

        private void ReconfigureWith()
        {
            foreach (DbColumn dbColumn in this.DbTable.DbColumns)
            {
                IPropertyInfo propertyInfo = this.Columns.FirstOrDefault(column => column.Name == dbColumn.Name);
                if (propertyInfo == null) continue;

                propertyInfo.DbType = dbColumn.DbType;
                propertyInfo.IsNullable = dbColumn.IsNullable;
                propertyInfo.ColumnAttribute.Size = dbColumn.Size;
                propertyInfo.ColumnAttribute.AutoNumber = dbColumn.IsAutoIncrement;
                propertyInfo.ColumnAttribute.IsPrimaryKey = dbColumn.IsPrimaryKey;
            }
        }

        private void CleanUpColumns()
        {
            List<string> indexToRemove = new List<string>();
            for (int index = 0; index < this.Columns.Count; index++)
            {
                if (this.DbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name == this.Columns[index].Name) != null)
                    continue;

                indexToRemove.Add(this.Columns[index].Name);
            }
            indexToRemove.ForEach(name => this.Columns.Remove(name));
        }

        internal string CreateSelectStatement(IDbProvider dbProvider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false)
                return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Columns.Select(member => dbProvider.EscapeName(member.Name))));
            selectStatement.AppendFormat(" FROM {0}", dbProvider.EscapeName(this.Name));

            selectStatement.Append(AppendPrimaryKeys(dbProvider));
            return this.SelectStatement = selectStatement.ToString();
        }

        internal string CreateDeleteStatement(IDbProvider provider)
        {
            if (string.IsNullOrWhiteSpace(this.DeleteStatement) == false)
                return this.DeleteStatement;

            return this.DeleteStatement = string.Format("DELETE FROM {0} {1}", provider.EscapeName(this.Name), AppendPrimaryKeys(provider));
        }

        private string AppendPrimaryKeys(IDbProvider dbProvider)
        {
            string[] primaryKeys = this.Columns.Where(member => member.ColumnAttribute.IsPrimaryKey).Select(column => column.Name).ToArray();
            StringBuilder whereClause = new StringBuilder(" WHERE ");
            int i = 0;
            string seperator = " AND ";
            foreach (string primaryKey in primaryKeys)
            {
                if (i >= primaryKeys.Length - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", dbProvider.EscapeName(primaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }

        internal string CreateInsertStatement(IDbProvider dbProvider)
        {
            if (string.IsNullOrEmpty(this.InsertStatement) == false)
                return this.InsertStatement;

            StringBuilder insertStatement = new StringBuilder();
            insertStatement.AppendFormat("INSERT INTO {0} ", dbProvider.EscapeName(this.Name));

            insertStatement.AppendFormat("({0})", string.Join(", ", this.Columns.Where(column => !column.ColumnAttribute.AutoNumber).Select(column => dbProvider.EscapeName(column.Name))));
            insertStatement.AppendFormat(" VALUES({0})", string.Join(", ", this.Columns.Where(column => !column.ColumnAttribute.AutoNumber).Select(column => "@" + column.Name)));

            return this.InsertStatement = string.Concat(insertStatement.ToString(), dbProvider.ScopeIdentity);
        }

        internal string CreateUpdateStatement(IDbProvider dbProvider, KeyValuePair<string, object>[] arguments)
        {
            string updateStatement = string.Format("UPDATE {0} SET ", dbProvider.EscapeName(this.Name));
            updateStatement += string.Join(", ",
                arguments.SkipWhile(kvp => this.DbTable.DbColumns.Find(column => column.Name == kvp.Key && (column.IsPrimaryKey || column.IsAutoIncrement)) != null)
                .Select(kvp2 => string.Format("{0} = @{0}", kvp2.Key)));
            updateStatement += AppendPrimaryKeys(dbProvider);

            return updateStatement;
        }

        internal DbType ConvertToDbType(string name)
        {
            IPropertyInfo propMetaInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            return propMetaInfo.DbType ?? TypeConverter.ToDbType(propMetaInfo.PropertyType);
        }

        internal int GetColumnSize(string name)
        {
            IPropertyInfo propertyInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            return propertyInfo.Size > 0 ? propertyInfo.Size : -1;
        }

        internal object[] GetPrimaryKeyValues<TEntity>(TEntity entity)
        {
            int index = 0;
            var columnPrimaryKeys = this.Columns.Where(column => column.ColumnAttribute.IsPrimaryKey);
            object[] primaryKeys = new object[columnPrimaryKeys.Count()];

            foreach (IPropertyInfo propertyInfo in columnPrimaryKeys)
            {
                object primaryKey = null;
                if ((primaryKey = propertyInfo.GetValue(entity)) == null)
                    throw new PrimaryKeyException(string.Format("The requested pirmaryKey '{0}' was not found! Please set those Property to a valid value!", propertyInfo.Name));
                primaryKeys[index++] = primaryKey;
            }
            if (index <= 0)
                throw new PrimaryKeyException("It was no valid primaryKey available!");

            return primaryKeys;
        }

        /// <summary>
        /// Returns the mapping for a given object.
        /// </summary>
        /// <param name="obj">The object the mapping is returned for.</param>
        public static TableInfo GetTableInfo(object obj)
        {
            return TableInfoContainer.GetTableInfo(obj);
        }

        /// <summary>
        /// Returns the mapping for the given persistent type.
        /// </summary>
        /// <param name="type">Type of object the mapping is returned for.</param>
        public static TableInfo GetTableInfo(Type type)
        {
            return TableInfoContainer.GetTableInfo(type);
        }
    }

    internal static class TableInfo<T>
    {
        private static TableInfo InternalTableInfo { get; set; }

        internal static TableInfo GetTableInfo
        {
            get
            {
                if (InternalTableInfo != null)
                    return InternalTableInfo;

                InternalTableInfo = TableInfo.GetTableInfo(typeof(T));
                if (InternalTableInfo == null) return null;

                InternalTableInfo.DbTable = DbSchemaAllocator<T>.DbTable;
                return InternalTableInfo;
            }
        }
    }
}
