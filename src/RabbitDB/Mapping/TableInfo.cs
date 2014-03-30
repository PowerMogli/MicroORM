using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RabbitDB.Attributes;
using RabbitDB.Base;
using RabbitDB.Schema;
using RabbitDB.Storage;

namespace RabbitDB.Mapping
{
    internal class TableInfo
    {
        internal TableInfo(Type type, TableAttribute tableAttribute)
        {
            this.EntityType = type;
            this.TableAttribute = tableAttribute;
            this.Columns = new PropertyInfoCollection();
        }

        private string SelectStatement { get; set; }
        private string InsertStatement { get; set; }
        private string DeleteStatement { get; set; }
        private string WithNolock
        {
            get
            {
                return this.TableAttribute.ReadWithNolock
                    ? " WITH (NOLOCK) "
                    : string.Empty;
            }
        }

        private string SchemedTableName
        {
            get { return string.Format("{0}.{1}", this.DbTable.Schema, this.Name); }
        }

        private TableAttribute TableAttribute { get; set; }

        private int _numberOfPrimaryKeys = -1;
        internal int NumberOfPrimaryKeys
        {
            get
            {
                if (_numberOfPrimaryKeys != -1) return _numberOfPrimaryKeys;

                return _numberOfPrimaryKeys = this.Columns.Where(columns => columns.ColumnAttribute.IsPrimaryKey).Count();
            }
        }

        internal string Name { get { return TableAttribute.EntityName; } }
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
                CleanUpUnusedColumns();
                ReconfigureByTableColumns();
            }
        }

        private IEnumerable<IPropertyInfo> _primaryKeyColumns;
        private IEnumerable<IPropertyInfo> PrimaryKeyColumns
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.TableAttribute.AlternativePKs) == false)
                {
                    string[] attrPrimaryKeys = this.TableAttribute.AlternativePKs.Split(',');
                    _primaryKeyColumns = this.Columns.Where(column => attrPrimaryKeys.Any(attrPrimaryKey => attrPrimaryKey == column.Name));
                    return _primaryKeyColumns;
                }

                return _primaryKeyColumns ?? (_primaryKeyColumns = this.Columns.Where(column => column.ColumnAttribute.IsPrimaryKey));
            }
        }

        internal PropertyInfoCollection Columns { get; private set; }

        private void ReconfigureByTableColumns()
        {
            foreach (DbColumn dbColumn in this.DbTable.DbColumns)
            {
                IPropertyInfo propertyInfo = this.Columns.FirstOrDefault(column => column.ColumnAttribute.ColumnName == dbColumn.Name);
                if (propertyInfo == null) continue;

                propertyInfo.DbType = dbColumn.DbType;
                propertyInfo.IsNullable = dbColumn.IsNullable;
                propertyInfo.ColumnAttribute.Size = dbColumn.Size;
                propertyInfo.ColumnAttribute.AutoNumber = dbColumn.IsAutoIncrement;
                propertyInfo.ColumnAttribute.IsPrimaryKey = dbColumn.IsPrimaryKey;
            }
        }

        private void CleanUpUnusedColumns()
        {
            var columnNames = new List<string>();
            for (var index = 0; index < this.Columns.Count; index++)
            {
                if (this.DbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name == this.Columns[index].ColumnAttribute.ColumnName) != null)
                    continue;

                columnNames.Add(this.Columns[index].Name);
            }
            columnNames.ForEach(name => this.Columns.Remove(name));
        }

        internal string CreateSelectStatement(IDbProvider dbProvider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false)
                return this.SelectStatement;

            var selectStatement = new StringBuilder();
            selectStatement.Append(GetBaseSelect(dbProvider));

            selectStatement.Append(AppendPrimaryKeys(dbProvider));
            return this.SelectStatement = selectStatement.ToString();
        }

        internal string GetBaseSelect(IDbProvider dbProvider)
        {
            var selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Columns.SelectValidColumnNames(this.DbTable, dbProvider)));
            selectStatement.AppendFormat(" FROM {0}{1}", dbProvider.EscapeName(this.SchemedTableName), this.WithNolock);

            return selectStatement.ToString();
        }

        internal string CreateDeleteStatement(IDbProvider dbProvider)
        {
            if (string.IsNullOrWhiteSpace(this.DeleteStatement) == false)
                return this.DeleteStatement;

            return this.DeleteStatement =
                string.Format("DELETE FROM {0} {1}", dbProvider.EscapeName(this.SchemedTableName), AppendPrimaryKeys(dbProvider));
        }

        private string AppendPrimaryKeys(IDbProvider dbProvider)
        {
            var primaryKeys = this.PrimaryKeyColumns.Select(column => column.ColumnAttribute.ColumnName);
            var count = primaryKeys.Count();

            var whereClause = new StringBuilder(" WHERE ");
            var i = 0;
            var seperator = " AND ";
            foreach (var primaryKey in primaryKeys)
            {
                if (i >= count - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", dbProvider.EscapeName(primaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }

        internal string CreateInsertStatement(IDbProvider dbProvider)
        {
            if (string.IsNullOrEmpty(this.InsertStatement) == false)
                return this.InsertStatement;

            var insertStatement = new StringBuilder();
            insertStatement.AppendFormat("INSERT INTO {0} ", dbProvider.EscapeName(this.SchemedTableName));

            insertStatement.AppendFormat("({0})", string.Join(", ", this.Columns.SelectValidNonAutoNumberColumnNames(dbProvider)));
            insertStatement.AppendFormat(" VALUES({0})", string.Join(", ", this.Columns.SelectValidNonAutoNumberPrefixedColumnNames()));

            return this.InsertStatement = string.Concat(insertStatement.ToString(), dbProvider.ResolveScopeIdentity(this));
        }

        internal string CreateUpdateStatement(IDbProvider dbProvider, KeyValuePair<string, object>[] arguments)
        {
            string updateStatement = GetBaseUpdate(dbProvider);
            updateStatement += string.Join(", ",
                arguments
                .SkipWhile(kvp => this.DbTable.SkipWhile(ResolveColumnName(kvp.Key)))
                .Select(kvp2 => string.Format("{0} = @{1}", dbProvider.EscapeName(kvp2.Key), kvp2.Key)));
            updateStatement += AppendPrimaryKeys(dbProvider);

            return updateStatement;
        }

        internal string GetBaseUpdate(IDbProvider dbProvider)
        {
            return string.Format("UPDATE {0} SET ", dbProvider.EscapeName(this.SchemedTableName));
        }

        internal DbType ConvertToDbType(string name)
        {
            IPropertyInfo propMetaInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            return propMetaInfo.DbType ?? TypeConverter.ToDbType(propMetaInfo.PropertyType);
        }

        internal bool IsColumn(string name)
        {
            return this.Columns.Any(column => column.Name == name);
        }

        internal string ResolveColumnName(string name)
        {
            IPropertyInfo propertyInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("The column with the name - '{0}' - doesn´t exist.", name), "name");

            return propertyInfo.ColumnAttribute.ColumnName;
        }

        internal int GetColumnSize(string name)
        {
            IPropertyInfo propertyInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            return propertyInfo.Size > 0 ? propertyInfo.Size : -1;
        }

        internal object[] GetPrimaryKeyValues<TEntity>(TEntity entity)
        {
            int index = 0;
            object[] primaryKeys = new object[this.PrimaryKeyColumns.Count()];

            foreach (IPropertyInfo propertyInfo in this.PrimaryKeyColumns)
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

        internal void SetAutoNumber<TEntity>(TEntity entity, object insertId)
        {
            if (insertId == null) return;

            IEnumerable<IPropertyInfo> propertyInfos = this.Columns.Where(column => column.ColumnAttribute.AutoNumber);
            foreach (IPropertyInfo propertyInfo in propertyInfos)
            {
                propertyInfo.SetValue(entity, insertId);
            }
        }

        internal Tuple<bool, string> GetIdentityType()
        {
            IPropertyInfo propertyInfo = this.Columns.Where(column => column.ColumnAttribute.AutoNumber).FirstOrDefault();
            if (propertyInfo == null)
                return new Tuple<bool, string>(false, string.Empty);

            string castTo = string.Empty;
            if (propertyInfo.PropertyType == typeof(int))
                castTo = "INT";
            if (propertyInfo.PropertyType == typeof(decimal))
                castTo = "NUMERIC";
            if (propertyInfo.PropertyType == typeof(short))
                castTo = "SMALLINT";
            if (propertyInfo.PropertyType == typeof(byte))
                castTo = "TINYINT";
            if (propertyInfo.PropertyType == typeof(long))
                castTo = "BIGINT";

            return new Tuple<bool, string>(true, castTo);
        }
    }
}