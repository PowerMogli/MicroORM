using RabbitDB.Mapping;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    internal class SelectSqlBuilder : SqlBuilder
    {
        public SelectSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo) { }

        internal override string CreateStatement()
        {
            var selectStatement = new StringBuilder();
            selectStatement.Append(GetBaseSelect());

            selectStatement.Append(base.AppendPrimaryKeys());
            return selectStatement.ToString();
        }

        internal string GetBaseSelect()
        {
            var selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");

            selectStatement.AppendFormat("{0}", string.Join(", ", _tableInfo.Columns.SelectValidColumnNames(_tableInfo.DbTable, _sqlDialect.SqlCharacters)));
            selectStatement.AppendFormat(" FROM {0}{1}", _sqlDialect.SqlCharacters.EscapeName(_tableInfo.SchemedTableName), _tableInfo.WithNolock);

            return selectStatement.ToString();
        }
    }
}