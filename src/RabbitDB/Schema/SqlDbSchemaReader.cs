// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDbSchemaReader.cs" company="">
//   
// </copyright>
// <summary>
//   The sql db schema reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Collections.Generic;

    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.SqlDialect;
    using RabbitDB.Utils;

    /// <summary>
    /// The sql db schema reader.
    /// </summary>
    internal class SqlDbSchemaReader : DbSchemaReader
    {
        #region Constants

        /// <summary>
        /// The sq l_ column.
        /// </summary>
        private const string SqlColumn = @"SELECT
            COLUMN_NAME AS ColumnName,
            ORDINAL_POSITION AS OrdinalPosition,
            COLUMN_DEFAULT AS DefaultSetting,
            IS_NULLABLE AS IsNullable, 
            DATA_TYPE AS DataType,
            CHARACTER_MAXIMUM_LENGTH AS MaxLength,
            DATETIME_PRECISION AS DatePrecision,
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsIdentity') AS IsIdentity,
            COLUMNPROPERTY(object_id('[' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']'), COLUMN_NAME, 'IsComputed') as IsComputed
        FROM  INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME=@tableName AND TABLE_SCHEMA=@schemaName
        ORDER BY OrdinalPosition ASC";

        /// <summary>
        /// The sq l_ primarykey.
        /// </summary>
        private const string SqlPrimarykey = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

        /// <summary>
        /// The sq l_ table.
        /// </summary>
        private const string SqlTable = @"SELECT *
        FROM INFORMATION_SCHEMA.TABLES
        WHERE (TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW')
        AND TABLE_NAME = @tableName";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDbSchemaReader"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal SqlDbSchemaReader(SqlDialect sqlDialect)
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
        /// The <see cref="List"/>.
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
                        var dbColumn = new DbColumn
                                           {
                                               Name = SqlTools.GetDbValue<string>(dataReader["ColumnName"])
                                           };
                        dbColumn.PropertyName = DbSchemaCleaner.CleanUp(dbColumn.Name);
                        try
                        {
                            dbColumn.DbType = TypeConverter.ToDbType(
                                SqlTools.GetDbValue<string>(dataReader["DataType"]));
                        }
                        catch
                        {
                        }

                        dbColumn.Size = SqlTools.GetDbValue<int>(dataReader["MaxLength"]);
                        dbColumn.DefaultValue = SqlTools.GetDbValue<string>(dataReader["DefaultSetting"]);
                        try
                        {
                            dbColumn.Precision = SqlTools.GetDbValue<int>(dataReader["DatePrecision"]);
                        }
                        catch
                        {
                            dbColumn.Precision = SqlTools.GetDbValue<short>(dataReader["DatePrecision"]);
                        }

                        dbColumn.IsNullable = SqlTools.GetDbValue<string>(dataReader["IsNullable"]) == "YES";
                        dbColumn.IsAutoIncrement = Convert.ToBoolean(SqlTools.GetDbValue<int>(dataReader["IsIdentity"]));
                        dbColumn.IsComputed = Convert.ToBoolean(SqlTools.GetDbValue<int>(dataReader["IsComputed"]));
                        columns.Add(dbColumn);
                    }
                    catch
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
        /// The <see cref="List"/>.
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
                    primaryKeys.Add(SqlTools.GetDbValue<string>(dataReader["ColumnName"]));
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
                dbTable.Name = SqlTools.GetDbValue<string>(dataReader["TABLE_NAME"]);
                dbTable.Schema = SqlTools.GetDbValue<string>(dataReader["TABLE_SCHEMA"]);
                dbTable.IsView = string.Compare(
                    SqlTools.GetDbValue<string>(dataReader["TABLE_TYPE"]), 
                    "View", 
                    StringComparison.OrdinalIgnoreCase)
                                 == 0;
                dbTable.CleanName = DbSchemaCleaner.CleanUp(dbTable.Name);

                return dbTable;
            }
        }

        #endregion
    }
}