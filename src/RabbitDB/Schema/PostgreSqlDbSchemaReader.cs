using System;
using System.Collections.Generic;
using System.Data;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Storage;
using RabbitDB.Utils;

namespace RabbitDB.Schema
{
    internal class PostgreSqlDbSchemaReader : DbSchemaReader
    {
        private const string SQL_TABLE = @"SELECT table_name, table_schema, table_type
			FROM information_schema.tables
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')
				AND table_schema NOT IN ('pg_catalog', 'information_schema');";

        private const string SQL_COLUMN = @"SELECT column_name, is_nullable, udt_name, column_default
			FROM information_schema.columns
			WHERE table_name=@tableName;";

        private string SQL_PRIMARYKEY = @"SELECT kcu.column_name
			FROM information_schema.key_column_usage kcu
			JOIN information_schema.table_constraints tc
			ON kcu.constraint_name=tc.constraint_name
			WHERE lower(tc.constraint_type)='primary key'
			AND kcu.table_name=@tablename";

        internal PostgreSqlDbSchemaReader(string connectionString)
            : base()
        {
            this.DbProvider = new PostgresDbProvider(connectionString);
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
                        dbColumn.Name = SqlTools.GetDbValue<string>(dataReader["column_name"]);
                        dbColumn.PropertyName = DbSchemaCleaner.CleanUp(dbColumn.Name);
                        try { dbColumn.DbType = TypeConverter.ToDbType(SqlTools.GetDbValue<string>(dataReader["udt_name"])); }
                        catch { }
                        dbColumn.Size = SqlTools.GetDbValue<int>(dataReader["udt_name"]);
                        try { dbColumn.Precision = SqlTools.GetDbValue<int>(dataReader["udt_name"]); }
                        catch { dbColumn.Precision = SqlTools.GetDbValue<short>(dataReader["udt_name"]); }
                        dbColumn.IsNullable = SqlTools.GetDbValue<string>(dataReader["is_nullable"]) == "YES";
                        dbColumn.IsAutoIncrement = SqlTools.GetDbValue<string>(dataReader["column_default"]).StartsWith("nextval(");
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
                    dbTable.Name = SqlTools.GetDbValue<string>(dataReader["table_name"]);
                    dbTable.Schema = SqlTools.GetDbValue<string>(dataReader["table_schema"]);
                    dbTable.IsView = string.Compare(SqlTools.GetDbValue<string>(dataReader["table_type"]), "View", true) == 0;
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
                    primaryKeys.Add(SqlTools.GetDbValue<string>(dataReader["column_name"]));
                }
                return primaryKeys;
            }
        }
    }
}