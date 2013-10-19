using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MicroORM.Base;
using MicroORM.Schema;
using MicroORM.Storage;
using System.Collections.Generic;

namespace MicroORM.Mapping
{
    internal sealed class TableInfo
    {
        private static TypeAttributes _nonPublic = TypeAttributes.NotPublic;
        private bool _isAnonymousType;

        internal TableInfo(Type type, string tableName)
        {
            this.EntityType = type;
            this.Name = tableName;
            this.Columns = new PropertyInfoCollection();

            _isAnonymousType = CheckIfAnonymousType(type);
        }

        private string SelectStatement { get; set; }

        private string InsertStatement { get; set; }

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
                ReconfigureWith();
                CleanUpColumns();
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

        internal string CreateSelectStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false) return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("select ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Columns.Select(member => provider.EscapeName(member.Name))));
            selectStatement.AppendFormat(" from {0}", provider.EscapeName(this.Name));
            string[] primaryKeys = this.Columns.Where(member => member.ColumnAttribute.IsPrimaryKey).Select(column => column.Name).ToArray();

            selectStatement.Append(AppendPrimaryKeys(provider, primaryKeys));
            return this.SelectStatement = selectStatement.ToString();
        }

        private string AppendPrimaryKeys(IDbProvider provider, string[] primaryKeys)
        {
            StringBuilder whereClause = new StringBuilder(" where ");
            int i = 0;
            string seperator = " and ";
            foreach (string primaryKey in primaryKeys)
            {
                if (i >= primaryKeys.Length - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", provider.EscapeName(primaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }

        internal string CreateInsertStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.InsertStatement) == false) return this.InsertStatement;

            StringBuilder insertStatement = new StringBuilder();
            insertStatement.AppendFormat("insert into {0} ", provider.EscapeName(this.Name));

            var tuple = CreateInsertIntoValues(provider);
            insertStatement.AppendFormat("({0})", tuple.Item1);
            insertStatement.AppendFormat(" values({0})", string.Join(", ", this.Columns.Where(column => !column.ColumnAttribute.IsPrimaryKey).Select(column => "@" + column.Name)));

            return this.InsertStatement = string.Concat(insertStatement.ToString(), provider.ScopeIdentity);
        }

        private Tuple<string> CreateInsertIntoValues(IDbProvider provider)
        {
            string insertValues = string.Join(", ", this.Columns.Select(column =>
            {
                if (this.Columns.Contains(column.Name) && column.ColumnAttribute.AutoNumber)
                    return string.Empty;

                return provider.EscapeName(column.Name);
            }));

            if (insertValues.StartsWith(","))
                insertValues = insertValues.Substring(2, insertValues.Length - 2);

            return new Tuple<string>(insertValues);
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

        /// <summary>
        /// Gets whether the given type is an anonymous type.
        /// </summary>
        /// <param name="type">The type that is inspected for being anonymous.</param>
        private bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && (type.Attributes & _nonPublic) == _nonPublic;
        }

        internal object[] GetPrimaryKeys<TEntity>(TEntity entity)
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
        internal static TableInfo GetTableInfo
        {
            get
            {
                TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));
                if (tableInfo == null) return null;

                tableInfo.DbTable = DbSchemaAllocator<T>.DbTable;
                return tableInfo;
            }
        }
    }
}
