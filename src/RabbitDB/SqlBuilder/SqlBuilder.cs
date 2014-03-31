using RabbitDB.Mapping;
using System.Linq;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    internal abstract class SqlBuilder
    {
        protected SqlDialect.SqlDialect _sqlDialect;
        protected TableInfo _tableInfo;

        internal SqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
        {
            _tableInfo = tableInfo;
            _sqlDialect = sqlDialect;
        }

        internal abstract string CreateStatement();

        protected string AppendPrimaryKeys()
        {
            var primaryKeys = _tableInfo.PrimaryKeyColumns.Select(column => column.ColumnAttribute.ColumnName);
            var count = primaryKeys.Count();

            var whereClause = new StringBuilder(" WHERE ");
            var i = 0;
            var seperator = " AND ";
            foreach (var primaryKey in primaryKeys)
            {
                if (i >= count - 1) seperator = string.Empty;
                whereClause.AppendFormat("{0}=@{1}{2}", _sqlDialect.SqlCharacters.EscapeName(primaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }
    }
}