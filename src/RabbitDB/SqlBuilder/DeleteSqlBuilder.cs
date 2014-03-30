using RabbitDB.Mapping;
using RabbitDB.Storage;

namespace RabbitDB.SqlBuilder
{
    class DeleteSqlBuilder : SqlBuilder
    {
        public DeleteSqlBuilder(IDbProvider dbProvider, TableInfo tableInfo)
            : base(dbProvider, tableInfo) { }

        internal override string CreateStatement()
        {
            return string.Format("DELETE FROM {0} {1}", _dbProvider.EscapeName(_tableInfo.SchemedTableName), base.AppendPrimaryKeys());
        }
    }
}