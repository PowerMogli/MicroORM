using RabbitDB.Mapping;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    class InsertSqlBuilder : SqlBuilder
    {
        public InsertSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo) { }

        internal override string CreateStatement()
        {
            var insertStatement = new StringBuilder();
            insertStatement.AppendFormat("INSERT INTO {0} ", _sqlDialect.SqlCharacters.EscapeName(_tableInfo.SchemedTableName));

            insertStatement.AppendFormat("({0})", string.Join(", ", _tableInfo.Columns.SelectValidNonAutoNumberColumnNames(_sqlDialect.SqlCharacters)));
            insertStatement.AppendFormat(" VALUES({0})", string.Join(", ", _tableInfo.Columns.SelectValidNonAutoNumberPrefixedColumnNames()));

            return string.Concat(insertStatement.ToString(), _sqlDialect.ResolveScopeIdentity(_tableInfo));
        }
    }
}