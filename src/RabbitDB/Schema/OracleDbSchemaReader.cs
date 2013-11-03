using System;
using System.Collections.Generic;

namespace RabbitDB.Schema
{
    internal class OracleDbSchemaReader : DbSchemaReader
    {
        private const string SQL_TABLE = @"select TABLE_NAME, 'Table' TABLE_TYPE, USER TABLE_SCHEMA
from USER_TABLES
union all
select VIEW_NAME, 'View', USER
from USER_VIEWS";

        private const string SQL_COLUMN = @"select table_name TableName,
column_name ColumnName,
data_type DataType,
 data_scale DataScale,
 nullable IsNullable
 from USER_TAB_COLS utc
 where table_name = :tableName
 order by column_id";

        private const string SQL_PRIMARYKEY = @"select column_name from USER_CONSTRAINTS uc
  inner join USER_CONS_COLUMNS ucc on uc.constraint_name = ucc.constraint_name
where uc.constraint_type = 'P'
and uc.table_name = upper(:tableName)
and ucc.position = 1";
        
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
