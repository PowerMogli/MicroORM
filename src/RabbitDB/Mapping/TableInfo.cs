using RabbitDB.Attributes;
using RabbitDB.Base;
using RabbitDB.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        internal string WithNolock
        {
            get
            {
                return this.TableAttribute.ReadWithNolock
                    ? " WITH (NOLOCK) "
                    : string.Empty;
            }
        }

        internal string SchemedTableName
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
        internal IEnumerable<IPropertyInfo> PrimaryKeyColumns
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