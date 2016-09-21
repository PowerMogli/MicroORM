// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using RabbitDB.Contracts.Schema;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.Schema
{
    /// <summary>
    ///     The db schema reader.
    /// </summary>
    internal abstract class DbSchemaReader : IDisposable
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSchemaReader" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        internal DbSchemaReader(SqlDialect.SqlDialect sqlDialect)
        {
            SqlDialect = sqlDialect;
            Tables = new DbTableCollection();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the sql dialect.
        /// </summary>
        protected SqlDialect.SqlDialect SqlDialect { get; }

        /// <summary>
        ///     Gets the tables.
        /// </summary>
        protected DbTableCollection Tables { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            SqlDialect.Dispose();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The read schema.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="DbTable" />.
        /// </returns>
        internal DbTable ReadSchema<T>()
        {
            DbTable dbTable;
            using (this)
            {
                TableInfo tableInfo = TableInfo<T>.GetTableInfo;
                if (tableInfo == null)
                {
                    return null;
                }

                if ((dbTable = Tables[tableInfo.Name]) != null)
                {
                    return dbTable;
                }

                dbTable = GetTable(tableInfo.Name);
                dbTable.DbColumns = GetColumns(dbTable);
                SetPrimaryKeys(dbTable);
                Tables.Add(dbTable);
            }

            return dbTable;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     The get columns.
        /// </summary>
        /// <param name="dbTable">
        ///     The db table.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        protected abstract List<IDbColumn> GetColumns(DbTable dbTable);

        /// <summary>
        ///     The get primary keys.
        /// </summary>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        protected abstract List<string> GetPrimaryKeys(string table);

        /// <summary>
        ///     The get table.
        /// </summary>
        /// <param name="tableName">
        ///     The table name.
        /// </param>
        /// <returns>
        ///     The <see cref="DbTable" />.
        /// </returns>
        protected abstract DbTable GetTable(string tableName);

        #endregion

        #region Private Methods

        /// <summary>
        ///     The set primary keys.
        /// </summary>
        /// <param name="dbTable">
        ///     The db table.
        /// </param>
        /// <exception cref="MissingPrimaryKeyException">
        /// </exception>
        private void SetPrimaryKeys(DbTable dbTable)
        {
            List<string> primaryKeys = GetPrimaryKeys(dbTable.Name);

            foreach (string primaryKey in primaryKeys)
            {
                IDbColumn primaryKeyColumn = dbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name.ToLower()
                                                                                                  .Trim() == primaryKey.ToLower()
                                                                                                                       .Trim());
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