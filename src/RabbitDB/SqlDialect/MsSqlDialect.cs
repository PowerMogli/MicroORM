// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsSqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The ms sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    using RabbitDB.Expressions;
    using RabbitDB.Mapping;
    using RabbitDB.Storage;

    /// <summary>
    /// The ms sql dialect.
    /// </summary>
    internal class MsSqlDialect : SqlDialect
    {
        #region Fields

        /// <summary>
        /// The _builder helper.
        /// </summary>
        private IDbProviderExpressionBuildHelper _builderHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        /// The db command executor.
        /// </param>
        internal MsSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(SqlCharacters.MsSqlCharacters, dbProvider, dbCommandExecutor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        internal MsSqlDialect(IDbProvider dbProvider)
            : base(SqlCharacters.MsSqlCharacters, dbProvider)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the builder helper.
        /// </summary>
        internal override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get
            {
                return _builderHelper ?? (_builderHelper = new ExpressionBuildHelper(this.SqlCharacters));
            }
        }

        /// <summary>
        /// Gets the scope identity.
        /// </summary>
        internal override string ScopeIdentity
        {
            get
            {
                return "; SELECT CAST(SCOPE_IDENTITY() AS {0}) AS ID";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The resolve scope identity.
        /// </summary>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal override string ResolveScopeIdentity(TableInfo tableInfo)
        {
            var result = tableInfo.GetIdentityType();
            return result.Item1 ? string.Format(this.ScopeIdentity, result.Item2) : string.Empty;
        }

        #endregion
    }
}