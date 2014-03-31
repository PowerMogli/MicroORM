using RabbitDB.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Storage;
using System.Linq;

namespace RabbitDB.SqlDialect
{
    internal class PostgreSqlDialect : SqlDialect
    {
        // for Unit tests
        internal PostgreSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(SqlCharacters.PostgreSqlCharacters, dbProvider, dbCommandExecutor) { }

        internal PostgreSqlDialect(IDbProvider dbProvider)
            : base(SqlCharacters.PostgreSqlCharacters, dbProvider) { }

        internal override string ResolveScopeIdentity(TableInfo tableInfo)
        {
            IPropertyInfo propertyInfo = tableInfo.Columns.Where(column => column.ColumnAttribute.AutoNumber).FirstOrDefault();
            return string.Format(this.ScopeIdentity, base.SqlCharacters.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
        }

        internal override string ScopeIdentity
        {
            get { return " returning {0}"; }
        }

        private IDbProviderExpressionBuildHelper _builderHelper;
        internal override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get { return _builderHelper ?? (_builderHelper = new PostgresExpressionBuilderHelper(this.SqlCharacters)); }
        }
    }
}