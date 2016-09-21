#region using directives

using RabbitDB.Contracts.Expressions;
using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Storage;

#endregion

namespace RabbitDB.Contracts.SqlDialect
{
    internal interface ISqlDialect
    {
        #region  Properties

        IDbProviderExpressionBuildHelper BuilderHelper { get; }

        IDbProvider DbProvider { get; }

        ISqlCharacters SqlCharacters { get; set; }

        #endregion

        #region Public Methods

        string ResolveScopeIdentity(ITableInfo tableInfo);

        #endregion
    }
}