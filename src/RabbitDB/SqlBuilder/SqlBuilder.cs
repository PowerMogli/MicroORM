using RabbitDB.Mapping;
using RabbitDB.Storage;
using System.Linq;
using System.Text;

namespace RabbitDB.SqlBuilder
{
    internal abstract class SqlBuilder
    {
        protected IDbProvider _dbProvider;
        protected TableInfo _tableInfo;

        internal SqlBuilder(IDbProvider dbProvider, TableInfo tableInfo)
        {
            _tableInfo = tableInfo;
            _dbProvider = dbProvider;
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
                whereClause.AppendFormat("{0}=@{1}{2}", _dbProvider.EscapeName(primaryKey), i++, seperator);
            }
            return whereClause.ToString();
        }
    }
}