// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MySqlDbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The my sql db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Collections.Generic;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The my sql db schema reader.
    /// </summary>
    internal class MySqlDbSchemaReader : DbSchemaReader
    {
        #region Constants

        /// <summary>
        /// The sq l_ table.
        /// </summary>
        private const string SqlTable = @"SELECT *
			FROM information_schema.tables
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDbSchemaReader"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal MySqlDbSchemaReader(SqlDialect sqlDialect)
            : base(sqlDialect)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get columns.
        /// </summary>
        /// <param name="dbTable">
        /// The db table.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override List<DbColumn> GetColumns(DbTable dbTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get primary keys.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        protected override List<string> GetPrimaryKeys(string table)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get table.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DbTable"/>.
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