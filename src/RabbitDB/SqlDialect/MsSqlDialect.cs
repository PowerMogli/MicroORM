// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The ms sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Contracts.Expressions;
using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Storage;
using RabbitDB.Expressions;

#endregion

namespace RabbitDB.SqlDialect
{
    /// <summary>
    ///     The ms sql dialect.
    /// </summary>
    internal class MsSqlDialect : SqlDialect
    {
        #region Fields

        /// <summary>
        ///     The _builder helper.
        /// </summary>
        private IDbProviderExpressionBuildHelper _builderHelper;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="MsSqlDialect" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        ///     The db command executor.
        /// </param>
        internal MsSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(RabbitDB.SqlDialect.SqlCharacters.MsSqlCharacters, dbProvider, dbCommandExecutor)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MsSqlDialect" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        internal MsSqlDialect(IDbProvider dbProvider)
            : base(RabbitDB.SqlDialect.SqlCharacters.MsSqlCharacters, dbProvider)
        {
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the builder helper.
        /// </summary>
        public override IDbProviderExpressionBuildHelper BuilderHelper => _builderHelper ?? (_builderHelper = new ExpressionBuildHelper(SqlCharacters));

        /// <summary>
        ///     Gets the scope identity.
        /// </summary>
        internal override string ScopeIdentity => "; SELECT CAST(SCOPE_IDENTITY() AS {0}) AS ID";

        #endregion

        #region Public Methods

        /// <summary>
        ///     The resolve scope identity.
        /// </summary>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ResolveScopeIdentity(ITableInfo tableInfo)
        {
            Tuple<bool, string> result = tableInfo.GetIdentityType();

            return result.Item1
                ? string.Format(ScopeIdentity, result.Item2)
                : string.Empty;
        }

        #endregion
    }
}