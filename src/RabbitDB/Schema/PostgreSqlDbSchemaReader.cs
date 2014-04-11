// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlDbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The postgre sql db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.SqlDialect;
    using RabbitDB.Utils;

    /// <summary>
    /// The postgre sql db schema reader.
    /// </summary>
    internal class PostgreSqlDbSchemaReader : DbSchemaReader
    {
        #region Constants

        /// <summary>
        /// The sq l_ column.
        /// </summary>
        private const string SqlColumn = @"SELECT column_name, is_nullable, udt_name, column_default
			FROM information_schema.columns
			WHERE table_name=@tableName;";

        /// <summary>
        /// The sq l_ primarykey.
        /// </summary>
        private const string SqlPrimarykey = @"SELECT kcu.column_name
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc
			ON kcu.constraint_name=tc.constraint_name
			WHERE lower(tc.constraint_type)='primary key'
			AND kcu.table_name=@tablename";

        /// <summary>
        /// The sq l_ table.
        /// </summary>
        private const string SqlTable = @"SELECT table_name, table_schema, table_type
			FROM information_schema.tables
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
				AND table_schema NOT IN ('pg_catalog', 'information_schema');";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDbSchemaReader"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal PostgreSqlDbSchemaReader(SqlDialect sqlDialect)
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
        protected override List<DbColumn> GetColumns(DbTable dbTable)
        {
            var columns = new List<DbColumn>();
            using (var dataReader = base.SqlDialect.ExecuteReader(
                new SqlQuery(
                    SqlColumn,
                    QueryParameterCollection.Create(
                        new object[] { new { tableName = dbTable.Name, schemaName = dbTable.Schema } }))))
            {
                while (dataReader.Read())
                {
                    try
                    {
                        var dbColumn = new DbColumn();
                        dbColumn.Name = SqlTools.GetDbValue<string>(dataReader["column_name"]);
                        dbColumn.PropertyName = DbSchemaCleaner.CleanUp(dbColumn.Name);
                        try
                        {
                            dbColumn.DbType = TypeConverter.ToDbType(
                                SqlTools.GetDbValue<string>(dataReader["udt_name"]));
                        }
                        catch
                        {
                        }

                        dbColumn.Size = SqlTools.GetDbValue<int>(dataReader["udt_name"]);
                        try
                        {
                            dbColumn.Precision = SqlTools.GetDbValue<int>(dataReader["udt_name"]);
                        }
                        catch
                        {
                            dbColumn.Precision = SqlTools.GetDbValue<short>(dataReader["udt_name"]);
                        }

                        dbColumn.IsNullable = SqlTools.GetDbValue<string>(dataReader["is_nullable"]) == "YES";
                        dbColumn.IsAutoIncrement =
                            SqlTools.GetDbValue<string>(dataReader["column_default"]).StartsWith("nextval(");
                        columns.Add(dbColumn);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return columns;
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
        protected override List<string> GetPrimaryKeys(string table)
        {
            var primaryKeys = new List<string>();

            using (
                var dataReader =
                    base.SqlDialect.ExecuteReader(
                        new SqlQuery(
                            SqlPrimarykey,
                            QueryParameterCollection.Create(new object[] { new { tableName = table } }))))
            {
                while (dataReader.Read())
                {
                    primaryKeys.Add(SqlTools.GetDbValue<string>(dataReader["column_name"]));
                }

                return primaryKeys;
            }
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
        protected override DbTable GetTable(string tableName)
        {
            using (
                var dataReader =
                    base.SqlDialect.ExecuteReader(
                        new SqlQuery(
                            SqlTable,
                            QueryParameterCollection.Create(new object[] { new { tableName = tableName } }))))
            {
                if (!dataReader.Read())
                {
                    return new DbTable(tableName);
                }

                var dbTable = new DbTable();
                dbTable.Name = SqlTools.GetDbValue<string>(dataReader["table_name"]);
                dbTable.Schema = SqlTools.GetDbValue<string>(dataReader["table_schema"]);
                dbTable.IsView = string.Compare(
                    SqlTools.GetDbValue<string>(dataReader["table_type"]),
                    "View",
                    StringComparison.OrdinalIgnoreCase) == 0;
                dbTable.CleanName = DbSchemaCleaner.CleanUp(dbTable.Name);

                return dbTable;
            }
        }

        #endregion
    }
}