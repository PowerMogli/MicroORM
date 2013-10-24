using System.Collections.Generic;
using RabbitDB.Storage;

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
        {
            this.DbProvider = new SqlDbProvider(connectionString);
        }

        protected override void SetPrimaryKeys(DbTable dbTable)
        {
            throw new System.NotImplementedException();
        }

        protected override List<DbColumn> GetColumns(DbTable dbTable)
        {
            throw new System.NotImplementedException();
        }

        protected override DbTable GetTable(string tableName)
        {
            throw new System.NotImplementedException();
        }

        protected override List<string> GetPrimaryKeys(string table)
        {
            throw new System.NotImplementedException();
        }
    }
}
