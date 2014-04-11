// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The db schema reader.
    /// </summary>
    internal abstract class DbSchemaReader : IDisposable
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSchemaReader"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal DbSchemaReader(SqlDialect sqlDialect)
        {
            this.SqlDialect = sqlDialect;
            this.Tables = new DbTableCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the sql dialect.
        /// </summary>
        protected SqlDialect SqlDialect { get; private set; }

        /// <summary>
        /// Gets the tables.
        /// </summary>
        protected DbTableCollection Tables { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.SqlDialect.Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The read schema.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="DbTable"/>.
        /// </returns>
        internal DbTable ReadSchema<T>()
        {
            DbTable dbTable;
            using (this)
            {
                var tableInfo = TableInfo<T>.GetTableInfo;
                if (tableInfo == null)
                {
                    return null;
                }

                if ((dbTable = this.Tables[tableInfo.Name]) != null)
                {
                    return dbTable;
                }

                dbTable = this.GetTable(tableInfo.Name);
                dbTable.DbColumns = this.GetColumns(dbTable);
                SetPrimaryKeys(dbTable);
                this.Tables.Add(dbTable);
            }

            return dbTable;
        }

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <param name="dbTable">
        /// The db table.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        protected abstract List<DbColumn> GetColumns(DbTable dbTable);

        /// <summary>
        /// The get primary keys.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        protected abstract List<string> GetPrimaryKeys(string table);

        /// <summary>
        /// The get table.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DbTable"/>.
        /// </returns>
        protected abstract DbTable GetTable(string tableName);

        /// <summary>
        /// The set primary keys.
        /// </summary>
        /// <param name="dbTable">
        /// The db table.
        /// </param>
        /// <exception cref="MissingPrimaryKeyException">
        /// </exception>
        private void SetPrimaryKeys(DbTable dbTable)
        {
            var primaryKeys = this.GetPrimaryKeys(dbTable.Name);

            foreach (var primaryKey in primaryKeys)
            {
                var primaryKeyColumn =
                    dbTable.DbColumns.SingleOrDefault(
                        dbColumn => dbColumn.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
                if (primaryKeyColumn == null)
                {
                    throw new MissingPrimaryKeyException("Not all primaryKeys were provided.");
                }

                primaryKeyColumn.IsPrimaryKey = true;
            }
        }

        #endregion
    }
}