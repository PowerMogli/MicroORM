using RabbitDB.Mapping;
using RabbitDB.Storage;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    class InsertSqlBuilder : SqlBuilder
    {
        public InsertSqlBuilder(IDbProvider dbProvider, TableInfo tableInfo)
            : base(dbProvider, tableInfo) { }

        internal override string CreateStatement()
        {
            var insertStatement = new StringBuilder();
            insertStatement.AppendFormat("INSERT INTO {0} ", _dbProvider.EscapeName(_tableInfo.SchemedTableName));

            insertStatement.AppendFormat("({0})", string.Join(", ", _tableInfo.Columns.SelectValidNonAutoNumberColumnNames(_dbProvider)));
            insertStatement.AppendFormat(" VALUES({0})", string.Join(", ", _tableInfo.Columns.SelectValidNonAutoNumberPrefixedColumnNames()));

            return string.Concat(insertStatement.ToString(), _dbProvider.ResolveScopeIdentity(_tableInfo));
        }
    }
}