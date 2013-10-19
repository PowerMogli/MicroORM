using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MicroORM.Attributes;
using MicroORM.Base;
using MicroORM.Storage;
using MicroORM.Schema;

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
            this.PrimaryKeys = new PropertyInfoCollection();

            _isAnonymousType = CheckIfAnonymousType(type);
        }

        private string SelectStatement { get; set; }

        private string InsertStatement { get; set; }

        internal int NumberOfPrimaryKeys
        {
            get
            {
                return this.PrimaryKeys.Count;
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

                ReconfigureWith(value);
                _dbTable = value;
            }
        }

        internal bool IsAnonymousType
        {
            get { return _isAnonymousType; }
        }

        internal PropertyInfoCollection Columns { get; private set; }

        internal PropertyInfoCollection PrimaryKeys { get; set; }

        private void ReconfigureWith(DbTable dbTable)
        {
            for (int index = 0; index < dbTable.DbColumns.Count; index++)
            {
                this.Columns[index].DbType = dbTable.DbColumns[index].PropertyType;
                this.Columns[index].IsNullable = dbTable.DbColumns[index].IsNullable;
                ((ColumnAttribute)this.Columns[index].ColumnAttribute).Size = dbTable.DbColumns[index].Size;
                ((ColumnAttribute)this.Columns[index].ColumnAttribute).AutoNumber = dbTable.DbColumns[index].IsAutoIncrement;
            }
        }

        internal string CreateSelectStatement(IDbProvider provider)
        {
            if (string.IsNullOrEmpty(this.SelectStatement) == false) return this.SelectStatement;

            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("select ");

            selectStatement.AppendFormat("{0}", string.Join(", ", this.Columns.Select(member => provider.EscapeName(member.Name))));
            selectStatement.AppendFormat(" from {0}", provider.EscapeName(this.Name));
            string[] primaryKeys = this.PrimaryKeys.Select(member => member.Name).ToArray();

            selectStatement.Append(AppendPrimaryKeys(provider, primaryKeys));
            return this.SelectStatement = selectStatement.ToString();
        }

        private string AppendPrimaryKeys(IDbProvider provider, string[] primaryKeys)
        {
            StringBuilder whereClause = new StringBuilder(" where ");
            int i = 0;
            string seperator = " and ";
            foreach (string pirmaryKey in primaryKeys)
            {
                if (i >= primaryKeys.Length - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", provider.EscapeName(pirmaryKey), i++, seperator);
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
            insertStatement.AppendFormat(" values(@{0})", string.Join(",@", Enumerable.Range(0, this.Columns.Count - tuple.Item2)));

            return this.InsertStatement = string.Concat(insertStatement.ToString(), provider.ScopeIdentity);
        }

        private Tuple<string, int> CreateInsertIntoValues(IDbProvider provider)
        {
            int excludedValues = 0;
            string insertValues = string.Join(", ", this.Columns.Select(member =>
            {
                if (this.PrimaryKeys.Contains(member.Name)
                    && ((ColumnAttribute)(member.ColumnAttribute)).AutoNumber)
                {
                    excludedValues++;
                    return string.Empty;
                }
                return provider.EscapeName(member.Name);
            }));

            if (insertValues.StartsWith(","))
                insertValues = insertValues.Substring(2, insertValues.Length - 2);

            return new Tuple<string, int>(insertValues, excludedValues);
        }

        internal DbType ConvertToDbType(string name)
        {
            IPropertyInfo propMetaInfo = this.Columns.FirstOrDefault(mem => mem.Name == name);
            return propMetaInfo.DbType ?? TypeConverter.ToDbType(propMetaInfo.PropertyType);
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
            object[] primaryKeys = null;
            string[] tablePrimaryKeys = this.PrimaryKeys.Select(member => member.Name).ToArray();
            if (tablePrimaryKeys.Length <= 0) throw new PrimaryKeyException("It was no valid primaryKey available!");

            primaryKeys = new object[tablePrimaryKeys.Length];
            int index = 0;
            foreach (string tablePrimaryKey in tablePrimaryKeys)
            {
                object primaryKey = null;
                IPropertyInfo memberInfo = this.Columns.FirstOrDefault(member => member.Name == tablePrimaryKey);

                if (memberInfo == null || (primaryKey = memberInfo.GetValue(entity)) == null)
                    throw new PrimaryKeyException(string.Format("The requested pirmaryKey '{0}' was not found! Please set those Property to a valid value!", tablePrimaryKey));

                primaryKeys[index++] = primaryKey;
            }
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
