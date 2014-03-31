using RabbitDB.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.SqlBuilder
{
    internal class UpdateSqlBuilder : SqlBuilder
    {
        private KeyValuePair<string, object>[] _arguments;

        internal UpdateSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo, KeyValuePair<string, object>[] arguments)
            : base(sqlDialect, tableInfo)
        {
            _arguments = arguments;
        }

        internal UpdateSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo) { }

        internal override string CreateStatement()
        {
            string updateStatement = GetBaseUpdate();
            updateStatement += string.Join(", ",
                _arguments
                .SkipWhile(kvp => _tableInfo.DbTable.SkipWhile(_tableInfo.ResolveColumnName(kvp.Key)))
                .Select(kvp2 => string.Format("{0} = @{1}", _sqlDialect.SqlCharacters.EscapeName(kvp2.Key), kvp2.Key)));
            updateStatement += base.AppendPrimaryKeys();

            return updateStatement;
        }

        internal string GetBaseUpdate()
        {
            return string.Format("UPDATE {0} SET ", _sqlDialect.SqlCharacters.EscapeName(_tableInfo.SchemedTableName));
        }
    }
}