// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OracleDbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The oracle db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;

using RabbitDB.Contracts.Schema;

#endregion

namespace RabbitDB.Schema
{
    /// <summary>
    ///     The oracle db schema reader.
    /// </summary>
    internal class OracleDbSchemaReader : DbSchemaReader
    {
        #region Fields

        /// <summary>
        ///     The sq l_ column.
        /// </summary>
        private const string SqlColumn = @"select table_name TableName,
column_name ColumnName,
data_type DataType,
 data_scale DataScale,
 nullable IsNullable
 from USER_TAB_COLS utc
 where table_name = :tableName
 order by column_id";

        /// <summary>
        ///     The sq l_ primarykey.
        /// </summary>
        private const string SqlPrimarykey = @"select column_name from USER_CONSTRAINTS uc
  inner join USER_CONS_COLUMNS ucc on uc.constraint_name = ucc.constraint_name
where uc.constraint_type = 'P'
and uc.table_name = upper(:tableName)
and ucc.position = 1";

        /// <summary>
        ///     The sq l_ table.
        /// </summary>
        private const string SqlTable = @"select TABLE_NAME, 'Table' TABLE_TYPE, USER TABLE_SCHEMA
from USER_TABLES
union all
select VIEW_NAME, 'View', USER
from USER_VIEWS";

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="OracleDbSchemaReader" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        internal OracleDbSchemaReader(SqlDialect.SqlDialect sqlDialect)
            : base(sqlDialect)
        {
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override List<IDbColumn> GetColumns(DbTable dbTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get primary keys.
        /// </summary>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override List<string> GetPrimaryKeys(string table)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     The get table.
        /// </summary>
        /// <param name="tableName">
        ///     The table name.
        /// </param>
        /// <returns>
        ///     The <see cref="DbTable" />.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override DbTable GetTable(string tableName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}