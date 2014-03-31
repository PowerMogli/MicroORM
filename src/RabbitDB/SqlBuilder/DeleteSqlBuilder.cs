using RabbitDB.Mapping;

namespace RabbitDB.SqlBuilder
{
    class DeleteSqlBuilder : SqlBuilder
    {
        public DeleteSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo) { }

        internal override string CreateStatement()
        {
            return string.Format("DELETE FROM {0} {1}", _sqlDialect.SqlCharacters.EscapeName(_tableInfo.SchemedTableName), base.AppendPrimaryKeys());
        }
    }
}