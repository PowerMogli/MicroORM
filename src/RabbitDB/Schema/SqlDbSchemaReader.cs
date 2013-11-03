using System;
using System.Collections.Generic;
using System.Data;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Storage;
using RabbitDB.Utils;

namespace RabbitDB.Schema
{
    internal class SqlDbSchemaReader : DbSchemaReader
    {
        private const string SQL_TABLE = @"SELECT *
        FROM INFORMATION_SCHEMA.TABLES
        WHERE (TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW')
        AND TABLE_NAME = @tableName";

        private const string SQL_PRIMARYKEY = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

        private const string SQL_COLUMN = @"SELECT
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

        internal SqlDbSchemaReader(string connectionString)
            : base()
        {
            base.DbProvider = new SqlDbProvider(connectionString);
        }

        protected override List<DbColumn> GetColumns(DbTable dbTable)
        {
            List<DbColumn> columns = new List<DbColumn>();
            using (IDataReader dataReader = base.DbProvider.ExecuteReader(
                        new SqlQuery(SQL_COLUMN, QueryParameterCollection.Create(new object[] { new { tableName = dbTable.Name, schemaName = dbTable.Schema } }))))
            {
                while (dataReader.Read())
                {
                    try
                    {
                        DbColumn dbColumn =new DbColumn();
                        dbColumn.Name = SqlTools.GetDbValue<string>(dataReader["ColumnName"]);
                        dbColumn.PropertyName = DbSchemaCleaner.CleanUp(dbColumn.Name);
                        try { dbColumn.DbType = TypeConverter.ToDbType(SqlTools.GetDbValue<string>(dataReader["DataType"])); }
                        catch { }
                        dbColumn.Size = SqlTools.GetDbValue<int>(dataReader["MaxLength"]);
                        dbColumn.DefaultValue = SqlTools.GetDbValue<string>(dataReader["DefaultSetting"]);
                        try { dbColumn.Precision = SqlTools.GetDbValue<int>(dataReader["DatePrecision"]); }
                        catch { dbColumn.Precision = SqlTools.GetDbValue<short>(dataReader["DatePrecision"]); }
                        dbColumn.IsNullable = SqlTools.GetDbValue<string>(dataReader["IsNullable"]) == "YES";
                        dbColumn.IsAutoIncrement = Convert.ToBoolean(SqlTools.GetDbValue<int>(dataReader["IsIdentity"]));
                        dbColumn.IsComputed = Convert.ToBoolean(SqlTools.GetDbValue<int>(dataReader["IsComputed"]));
                        columns.Add(dbColumn);
                    }
                    catch (Exception) { }
                }
            }

            return columns;
        }

        protected override DbTable GetTable(string tableName)
        {
            using (IDataReader dataReader = base.DbProvider.ExecuteReader(new SqlQuery(SQL_TABLE, QueryParameterCollection.Create(new object[] { new { tableName = tableName } }))))
            {
                if (dataReader.Read())
                {
                    DbTable dbTable = new DbTable();
                    dbTable.Name = SqlTools.GetDbValue<string>(dataReader["TABLE_NAME"]);
                    dbTable.Schema = SqlTools.GetDbValue<string>(dataReader["TABLE_SCHEMA"]);
                    dbTable.IsView = string.Compare(SqlTools.GetDbValue<string>(dataReader["TABLE_TYPE"]), "View", true) == 0;
                    dbTable.CleanName = DbSchemaCleaner.CleanUp(dbTable.Name);

                    return dbTable;
                }
            }

            return new DbTable(tableName);
        }

        protected override List<string> GetPrimaryKeys(string table)
        {
            List<string> primaryKeys = new List<string>();

            using (IDataReader dataReader = base.DbProvider.ExecuteReader(new SqlQuery(SQL_PRIMARYKEY, QueryParameterCollection.Create(new object[] { new { tableName = table } }))))
            {
                while (dataReader.Read())
                {
                    primaryKeys.Add(SqlTools.GetDbValue<string>(dataReader["ColumnName"]));
                }
                return primaryKeys;
            }
        }
    }
}
