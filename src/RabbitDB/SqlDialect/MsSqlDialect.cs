using RabbitDB.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Storage;
using System;

namespace RabbitDB.SqlDialect
{
    internal class MsSqlDialect : SqlDialect
    {
        internal MsSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(SqlCharacters.MsSqlCharacters, dbProvider, dbCommandExecutor) { }

        internal MsSqlDialect(IDbProvider dbProvider)
            : base(SqlCharacters.MsSqlCharacters, dbProvider) { }

        internal override string ScopeIdentity
        {
            get { return "; SELECT CAST(SCOPE_IDENTITY() AS {0}) AS ID"; }
        }

        internal override string ResolveScopeIdentity(TableInfo tableInfo)
        {
            Tuple<bool, string> result = tableInfo.GetIdentityType();
            return result.Item1 ? string.Format(this.ScopeIdentity, result.Item2) : string.Empty;
        }

        private IDbProviderExpressionBuildHelper _builderHelper;
        internal override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get { return _builderHelper ?? (_builderHelper = new ExpressionBuildHelper(this.SqlCharacters)); }
        }
    }
}