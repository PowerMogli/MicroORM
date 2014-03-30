using RabbitDB.Mapping;
using RabbitDB.Storage;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    internal class SelectSqlBuilder : SqlBuilder
    {
        public SelectSqlBuilder(IDbProvider dbProvider, TableInfo tableInfo)
            : base(dbProvider, tableInfo) { }

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

            selectStatement.AppendFormat("{0}", string.Join(", ", _tableInfo.Columns.SelectValidColumnNames(_tableInfo.DbTable, _dbProvider)));
            selectStatement.AppendFormat(" FROM {0}{1}", _dbProvider.EscapeName(_tableInfo.SchemedTableName), _tableInfo.WithNolock);

            return selectStatement.ToString();
        }
    }
}