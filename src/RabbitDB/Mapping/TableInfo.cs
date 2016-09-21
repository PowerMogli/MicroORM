// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The table info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using RabbitDB.Contracts.Attributes;
using RabbitDB.Contracts.Mapping;
using RabbitDB.Schema;

#endregion

namespace RabbitDB.Mapping
{
    /// <summary>
    ///     The table info.
    /// </summary>
    internal class TableInfo : ITableInfo
    {
        #region Fields

        /// <summary>
        ///     The _db table.
        /// </summary>
        private DbTable _dbTable;

        /// <summary>
        ///     The _number of primary keys.
        /// </summary>
        private int _numberOfPrimaryKeys = -1;

        /// <summary>
        ///     The _primary key columns.
        /// </summary>
        private IEnumerable<IPropertyInfo> _primaryKeyColumns;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableInfo" /> class.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="tableAttribute">
        ///     The table attribute.
        /// </param>
        internal TableInfo(Type type, TableAttribute tableAttribute)
        {
            EntityType = type;

            TableAttribute = tableAttribute;

            Columns = new PropertyInfoCollection();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the columns.
        /// </summary>
        public IPropertyInfoCollection Columns { get; }

        /// <summary>
        ///     Gets or sets the db table.
        /// </summary>
        internal DbTable DbTable
        {
            get { return _dbTable; }

            set
            {
                if (value == null || DbTable != null)
                {
                    return;
                }

                _dbTable = value;

                CleanUpUnusedColumns();

                ReconfigureByTableColumns();
            }
        }

        /// <summary>
        ///     Gets the entity type.
        /// </summary>
        internal Type EntityType { get; private set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        internal string Name => TableAttribute.EntityName;

        /// <summary>
        ///     Gets the number of primary keys.
        /// </summary>
        internal int NumberOfPrimaryKeys
        {
            get
            {
                if (_numberOfPrimaryKeys != -1)
                {
                    return _numberOfPrimaryKeys;
                }

                return _numberOfPrimaryKeys = Columns.Count(columns => columns.ColumnAttribute.IsPrimaryKey);
            }
        }

        /// <summary>
        ///     Gets the primary key columns.
        /// </summary>
        internal IEnumerable<IPropertyInfo> PrimaryKeyColumns
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TableAttribute.AlternativePKs))
                {
                    return _primaryKeyColumns ?? (_primaryKeyColumns = Columns.Where(column => column.ColumnAttribute.IsPrimaryKey));
                }

                string[] attrPrimaryKeys = TableAttribute.AlternativePKs.Split(',');

                _primaryKeyColumns = Columns.Where(column => attrPrimaryKeys.Any(attrPrimaryKey => attrPrimaryKey == column.Name));

                return _primaryKeyColumns;
            }
        }

        /// <summary>
        ///     Gets the schemed table name.
        /// </summary>
        internal string SchemedTableName => $"{DbTable.Schema}.{Name}";

        /// <summary>
        ///     Gets the with nolock.
        /// </summary>
        internal string WithNolock => TableAttribute.ReadWithNolock
            ? " WITH (NOLOCK) "
            : string.Empty;

        /// <summary>
        ///     Gets or sets the table attribute.
        /// </summary>
        private TableAttribute TableAttribute { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The get identity type.
        /// </summary>
        /// <returns>
        ///     The <see cref="Tuple" />.
        /// </returns>
        public Tuple<bool, string> GetIdentityType()

        {
            IPropertyInfo propertyInfo = Columns.FirstOrDefault(column => column.ColumnAttribute.AutoNumber);
            if (propertyInfo == null)
            {
                return new Tuple<bool, string>(false, string.Empty);
            }

            string castTo = string.Empty;
            if (propertyInfo.PropertyType == typeof(int))
            {
                castTo = "INT";
            }

            if (propertyInfo.PropertyType == typeof(decimal))
            {
                castTo = "NUMERIC";
            }

            if (propertyInfo.PropertyType == typeof(short))
            {
                castTo = "SMALLINT";
            }

            if (propertyInfo.PropertyType == typeof(byte))
            {
                castTo = "TINYINT";
            }

            if (propertyInfo.PropertyType == typeof(long))
            {
                castTo = "BIGINT";
            }

            return new Tuple<bool, string>(true, castTo);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The convert to db type.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="DbType" />.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// </exception>
        internal DbType ConvertToDbType(string name)
        {
            IPropertyInfo propMetaInfo = Columns.FirstOrDefault(column => column.Name == name);
            if (propMetaInfo != null)
            {
                return propMetaInfo.DbType ?? TypeConverter.ToDbType(propMetaInfo.PropertyType);
            }

            throw new NullReferenceException($"Die Property '{name}' konnte nicht gefunden werden!");
        }

        /// <summary>
        ///     The get column size.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        internal int GetColumnSize(string name)
        {
            IPropertyInfo propertyInfo = Columns.FirstOrDefault(column => column.Name == name);

            return propertyInfo != null && propertyInfo.Size > 0
                ? propertyInfo.Size
                : -1;
        }

        /// <summary>
        ///     The get primary key values.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="object[]" />.
        /// </returns>
        /// <exception cref="PrimaryKeyException">
        /// </exception>
        internal object[] GetPrimaryKeyValues<TEntity>(TEntity entity)
        {
            int index = 0;
            object[] primaryKeys = new object[PrimaryKeyColumns.Count()];

            foreach (IPropertyInfo propertyInfo in PrimaryKeyColumns)
            {
                object primaryKey;
                if ((primaryKey = propertyInfo.GetValue(entity)) == null)
                {
                    throw new PrimaryKeyException($"The requested pirmaryKey '{propertyInfo.Name}' was not found! Please set those Property to a valid value!");
                }

                primaryKeys[index++] = primaryKey;
            }

            if (index <= 0)
            {
                throw new PrimaryKeyException("It was no valid primaryKey available!");
            }

            return primaryKeys;
        }

        /// <summary>
        ///     The is column.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool IsColumn(string name)
        {
            return Columns.Any(column => column.Name == name);
        }

        /// <summary>
        ///     The resolve column name.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal string ResolveColumnName(string name)
        {
            IPropertyInfo propertyInfo = Columns.FirstOrDefault(column => column.Name == name);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"The column with the name - '{name}' - doesn´t exist.", nameof(name));
            }

            return propertyInfo.ColumnAttribute.ColumnName;
        }

        /// <summary>
        ///     The set auto number.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="insertId">
        ///     The insert id.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        internal void SetAutoNumber<TEntity>(TEntity entity, object insertId)
        {
            if (insertId == null)
            {
                return;
            }

            IEnumerable<IPropertyInfo> propertyInfos = Columns.Where(column => column.ColumnAttribute.AutoNumber);
            foreach (IPropertyInfo propertyInfo in propertyInfos)
            {
                propertyInfo.SetValue(entity, insertId);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The clean up unused columns.
        /// </summary>
        private void CleanUpUnusedColumns()
        {
            List<string> columnNames = new List<string>();

            foreach (IPropertyInfo propertyInfo in Columns)
            {
                if (DbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name == propertyInfo.ColumnAttribute.ColumnName) != null)
                {
                    continue;
                }

                columnNames.Add(propertyInfo.Name);
            }

            columnNames.ForEach(name => Columns.Remove(name));
        }

        /// <summary>
        ///     The reconfigure by table columns.
        /// </summary>
        private void ReconfigureByTableColumns()
        {
            foreach (DbColumn dbColumn in DbTable.DbColumns)
            {
                IPropertyInfo propertyInfo = Columns.FirstOrDefault(column => column.ColumnAttribute.ColumnName == dbColumn.Name);

                if (propertyInfo == null)
                {
                    continue;
                }

                propertyInfo.DbType = dbColumn.DbType;
                propertyInfo.IsNullable = dbColumn.IsNullable;
                propertyInfo.ColumnAttribute.Size = dbColumn.Size;
                propertyInfo.ColumnAttribute.AutoNumber = dbColumn.IsAutoIncrement;
                propertyInfo.ColumnAttribute.IsPrimaryKey = dbColumn.IsPrimaryKey;
            }
        }

        #endregion
    }
}