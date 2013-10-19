using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Storage;
using MicroORM.Utils;

namespace MicroORM.Schema
{
    internal class SqlDbSchemaReader : DbSchemaReader
    {
        private const string __sql_table__ = @"SELECT *
        FROM INFORMATION_SCHEMA.TABLES
        WHERE (TABLE_TYPE='BASE TABLE' OR TABLE_TYPE='VIEW')
        AND TABLE_NAME = @tableName";

        private const string __sql_pk__ = @"SELECT c.name AS ColumnName
                FROM sys.indexes AS i
                INNER JOIN sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.objects AS o ON i.object_id = o.object_id
                LEFT OUTER JOIN sys.columns AS c ON ic.object_id = c.object_id AND c.column_id = ic.column_id
                WHERE (i.is_primary_key = 1) AND (o.name = @tableName)";

        private const string __sql_column__ = @"SELECT
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

        public override void ReadSchema<T>()
        {
            DbTable dbTable;
            using (this)
            {
                TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));
                if (tableInfo == null) return;

                if ((dbTable = base.Tables[tableInfo.Name]) != null)
                {
                    tableInfo.DbTable = dbTable;
                    return;
                }
                dbTable = GetTable(tableInfo.Name);
                dbTable.DbColumns = GetColumns(dbTable);
                SetPrimaryKeys(dbTable);
                base.Tables.Add(dbTable);

                tableInfo.DbTable = dbTable;
            }
        }

        private void SetPrimaryKeys(DbTable dbTable)
        {
            List<string> primaryKeys = GetPrimaryKeys(dbTable.Name);

            foreach (string primaryKey in primaryKeys)
            {
                DbColumn primaryKeyColumn = dbTable.DbColumns.SingleOrDefault(dbColumn => dbColumn.Name.ToLower().Trim() == primaryKey.ToLower().Trim());
                if (primaryKeyColumn == null)
                    throw new MissingPrimaryKeyException("Not all primaryKeys were provided.");

                primaryKeyColumn.IsPrimaryKey = true;
            }
        }

        private List<DbColumn> GetColumns(DbTable dbTable)
        {
            List<DbColumn> columns = new List<DbColumn>();
            IDataReader dataReader = null;

            using (base.DbProvider)
            {
                try
                {
                    dataReader = base.DbProvider.ExecuteReader(
                    new SqlQuery(__sql_column__, QueryParameterCollection.Create(new object[] { new { tableName = dbTable.Name, schemaName = dbTable.Schema } })));
                    while (dataReader.Read())
                    {
                        try
                        {
                            DbColumn dbColumn =new DbColumn();
                            dbColumn.Name = dataReader["ColumnName"].ToString();
                            dbColumn.PropertyName = DbSchemaCleaner.CleanUp(dbColumn.Name);
                            try { dbColumn.PropertyType = TypeConverter.ToDbType(SqlTools.GetDbValue<string>(dataReader["DataType"])); }
                            catch { }
                            dbColumn.Size = SqlTools.GetDbValue<int>(dataReader["MaxLength"]);
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
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }

            return columns;
        }

        private DbTable GetTable(string tableName)
        {
            IDataReader dataReader = null;

            using (base.DbProvider)
            {
                try
                {
                    dataReader = base.DbProvider.ExecuteReader(new SqlQuery(__sql_table__, QueryParameterCollection.Create(new object[] { new { tableName = tableName } })));
                    if (dataReader.Read())
                    {
                        DbTable dbTable = new DbTable();
                        dbTable.Name = dataReader["TABLE_NAME"].ToString();
                        dbTable.Schema = dataReader["TABLE_SCHEMA"].ToString();
                        dbTable.IsView = string.Compare(dataReader["TABLE_TYPE"].ToString(), "View", true) == 0;
                        dbTable.CleanName = DbSchemaCleaner.CleanUp(dbTable.Name);

                        return dbTable;
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
            return new DbTable(tableName);
        }

        private List<string> GetPrimaryKeys(string table)
        {
            IDataReader dataReader = null;
            List<string> primaryKeys = new List<string>();

            using (base.DbProvider)
            {
                try
                {
                    dataReader = base.DbProvider.ExecuteReader(new SqlQuery(__sql_pk__, QueryParameterCollection.Create(new object[] { new { tableName = table } })));

                    while (dataReader.Read())
                    {
                        primaryKeys.Add(dataReader["ColumnName"].ToString());
                    }
                    return primaryKeys;
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
        }
    }
}
