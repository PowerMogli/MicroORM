using RabbitDB.Mapping;
using RabbitDB.Storage;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.SqlBuilder
{
    internal class UpdateSqlBuilder : SqlBuilder
    {
        private KeyValuePair<string, object>[] _arguments;
        private IDbProvider dbProvider;
        private TableInfo tableInfo;
        private KeyValuePair<string, object>[] arguments;

        internal UpdateSqlBuilder(IDbProvider dbProvider, TableInfo tableInfo, KeyValuePair<string, object>[] arguments)
            : base(dbProvider, tableInfo)
        {
            _arguments = arguments;
        }

        internal UpdateSqlBuilder(IDbProvider dbProvider, TableInfo tableInfo)
            : base(dbProvider, tableInfo) { }

        internal override string CreateStatement()
        {
            string updateStatement = GetBaseUpdate();
            updateStatement += string.Join(", ",
                _arguments
                .SkipWhile(kvp => _tableInfo.DbTable.SkipWhile(_tableInfo.ResolveColumnName(kvp.Key)))
                .Select(kvp2 => string.Format("{0} = @{1}", _dbProvider.EscapeName(kvp2.Key), kvp2.Key)));
            updateStatement += base.AppendPrimaryKeys();

            return updateStatement;
        }

        internal string GetBaseUpdate()
        {
            return string.Format("UPDATE {0} SET ", _dbProvider.EscapeName(_tableInfo.SchemedTableName));
        }
    }
}