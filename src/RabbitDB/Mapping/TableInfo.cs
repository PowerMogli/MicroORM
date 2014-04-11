// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The table info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RabbitDB.Attributes;
    using RabbitDB.Schema;

    /// <summary>
    /// The table info.
    /// </summary>
    internal class TableInfo
    {
        #region Fields

        /// <summary>
        /// The _db table.
        /// </summary>
        private DbTable _dbTable;

        /// <summary>
        /// The _number of primary keys.
        /// </summary>
        private int _numberOfPrimaryKeys = -1;

        /// <summary>
        /// The _primary key columns.
        /// </summary>
        private IEnumerable<IPropertyInfo> _primaryKeyColumns;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="tableAttribute">
        /// The table attribute.
        /// </param>
        internal TableInfo(Type type, TableAttribute tableAttribute)
        {
            this.EntityType = type;
            this.TableAttribute = tableAttribute;
            this.Columns = new PropertyInfoCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the columns.
        /// </summary>
        internal PropertyInfoCollection Columns { get; private set; }

        /// <summary>
        /// Gets or sets the db table.
        /// </summary>
        internal DbTable DbTable
        {
            get
            {
                return _dbTable;
            }

            set
            {
                if (value == null || this.DbTable != null)
                {
                    return;
                }

                _dbTable = value;
                CleanUpUnusedColumns();
                ReconfigureByTableColumns();
            }
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        internal Type EntityType { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        internal string Name
        {
            get
            {
                return this.TableAttribute.EntityName;
            }
        }

        /// <summary>
        /// Gets the number of primary keys.
        /// </summary>
        internal int NumberOfPrimaryKeys
        {
            get
            {
                if (_numberOfPrimaryKeys != -1)
                {
                    return _numberOfPrimaryKeys;
                }

                return _numberOfPrimaryKeys = this.Columns.Count(columns => columns.ColumnAttribute.IsPrimaryKey);
            }
        }

        /// <summary>
        /// Gets the primary key columns.
        /// </summary>
        internal IEnumerable<IPropertyInfo> PrimaryKeyColumns
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.TableAttribute.AlternativePKs))
                {
                    return _primaryKeyColumns
                           ?? (_primaryKeyColumns =
                               this.Columns.Where(column => column.ColumnAttribute.IsPrimaryKey));
                }

                var attrPrimaryKeys = this.TableAttribute.AlternativePKs.Split(',');
                _primaryKeyColumns =
                    this.Columns.Where(
                        column => attrPrimaryKeys.Any(attrPrimaryKey => attrPrimaryKey == column.Name));
                return _primaryKeyColumns;
            }
        }

        /// <summary>
        /// Gets the schemed table name.
        /// </summary>
        internal string SchemedTableName
        {
            get
            {
                return string.Format("{0}.{1}", this.DbTable.Schema, this.Name);
            }
        }

        /// <summary>
        /// Gets the with nolock.
        /// </summary>
        internal string WithNolock
        {
            get
            {
                return this.TableAttribute.ReadWithNolock ? " WITH (NOLOCK) " : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the table attribute.
        /// </summary>
        private TableAttribute TableAttribute { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The convert to db type.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="DbType"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// </exception>
        internal DbType ConvertToDbType(string name)
        {
            var propMetaInfo = this.Columns.FirstOrDefault(column => column.Name == name);
            if (propMetaInfo != null)
            {
                return propMetaInfo.DbType ?? TypeConverter.ToDbType(propMetaInfo.PropertyType);
            }

            throw new NullReferenceException(string.Format("Die Property '{0}' konnte nicht gefunden werden!", name));
        }

        /// <summary>
        /// The get column size.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal int GetColumnSize(string name)
        {
            var propertyInfo = this.Columns.FirstOrDefault(column => column.Name == name);

            return propertyInfo != null && propertyInfo.Size > 0 ? propertyInfo.Size : -1;
        }

        /// <summary>
        /// The get identity type.
        /// </summary>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        internal Tuple<bool, string> GetIdentityType()
        {
            var propertyInfo = this.Columns.FirstOrDefault(column => column.ColumnAttribute.AutoNumber);
            if (propertyInfo == null)
            {
                return new Tuple<bool, string>(false, string.Empty);
            }

            var castTo = string.Empty;
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

        /// <summary>
        /// The get primary key values.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        /// <exception cref="PrimaryKeyException">
        /// </exception>
        internal object[] GetPrimaryKeyValues<TEntity>(TEntity entity)
        {
            var index = 0;
            var primaryKeys = new object[this.PrimaryKeyColumns.Count()];

            foreach (var propertyInfo in this.PrimaryKeyColumns)
            {
                object primaryKey;
                if ((primaryKey = propertyInfo.GetValue(entity)) == null)
                {
                    throw new PrimaryKeyException(
                        string.Format(
                            "The requested pirmaryKey '{0}' was not found! Please set those Property to a valid value!", 
                            propertyInfo.Name));
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
        /// The is column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsColumn(string name)
        {
            return this.Columns.Any(column => column.Name == name);
        }

        /// <summary>
        /// The resolve column name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal string ResolveColumnName(string name)
        {
            var propertyInfo = this.Columns.FirstOrDefault(column => column.Name == name);

            if (propertyInfo == null)
            {
                throw new ArgumentException(
                    string.Format("The column with the name - '{0}' - doesn´t exist.", name), 
                    "name");
            }

            return propertyInfo.ColumnAttribute.ColumnName;
        }

        /// <summary>
        /// The set auto number.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="insertId">
        /// The insert id.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        internal void SetAutoNumber<TEntity>(TEntity entity, object insertId)
        {
            if (insertId == null)
            {
                return;
            }

            var propertyInfos = this.Columns.Where(column => column.ColumnAttribute.AutoNumber);
            foreach (var propertyInfo in propertyInfos)
            {
                propertyInfo.SetValue(entity, insertId);
            }
        }

        /// <summary>
        /// The clean up unused columns.
        /// </summary>
        private void CleanUpUnusedColumns()
        {
            var columnNames = new List<string>();
            for (var index = 0; index < this.Columns.Count; index++)
            {
                if (
                    this.DbTable.DbColumns.SingleOrDefault(
                        dbColumn => dbColumn.Name == this.Columns[index].ColumnAttribute.ColumnName) != null)
                {
                    continue;
                }

                columnNames.Add(this.Columns[index].Name);
            }

            columnNames.ForEach(name => this.Columns.Remove(name));
        }

        /// <summary>
        /// The reconfigure by table columns.
        /// </summary>
        private void ReconfigureByTableColumns()
        {
            foreach (var dbColumn in this.DbTable.DbColumns)
            {
                var propertyInfo =
                    this.Columns.FirstOrDefault(column => column.ColumnAttribute.ColumnName == dbColumn.Name);
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