using System;
using System.Collections.Generic;

namespace RabbitDB.Schema
{
    internal class MySqlDbSchemaReader : DbSchemaReader
    {
        private const string SQL_TABLE = @"SELECT *
			FROM information_schema.tables
			WHERE (table_type='BASE TABLE' OR table_type='VIEW')";

        internal MySqlDbSchemaReader(SqlDialect.SqlDialect sqlDialect)
            : base(sqlDialect) { }

        protected override List<DbColumn> GetColumns(DbTable dbTable)
        {
            throw new NotImplementedException();
        }

        protected override DbTable GetTable(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override List<string> GetPrimaryKeys(string table)
        {
            throw new NotImplementedException();
        }
    }
}