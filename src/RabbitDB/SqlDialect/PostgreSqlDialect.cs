// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The postgre sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq;

using RabbitDB.Contracts.Expressions;
using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Storage;
using RabbitDB.Expressions;

#endregion

namespace RabbitDB.SqlDialect
{
    /// <summary>
    ///     The postgre sql dialect.
    /// </summary>
    internal class PostgreSqlDialect : SqlDialect
    {
        #region Fields

        /// <summary>
        ///     The _builder helper.
        /// </summary>
        private IDbProviderExpressionBuildHelper _builderHelper;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="PostgreSqlDialect" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        ///     The db command executor.
        /// </param>
        internal PostgreSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(RabbitDB.SqlDialect.SqlCharacters.PostgreSqlCharacters, dbProvider, dbCommandExecutor)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PostgreSqlDialect" /> class.
        /// </summary>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        internal PostgreSqlDialect(IDbProvider dbProvider)
            : base(RabbitDB.SqlDialect.SqlCharacters.PostgreSqlCharacters, dbProvider)
        {
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the builder helper.
        /// </summary>
        public override IDbProviderExpressionBuildHelper BuilderHelper => _builderHelper ?? (_builderHelper = new PostgresExpressionBuilderHelper(SqlCharacters));

        /// <summary>
        ///     Gets the scope identity.
        /// </summary>
        internal override string ScopeIdentity => " returning {0}";

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
            IPropertyInfo propertyInfo = tableInfo.Columns.FirstOrDefault(column => column.ColumnAttribute.AutoNumber);

            if (propertyInfo != null)
            {
                return string.Format(ScopeIdentity, SqlCharacters.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
            }

            throw new InvalidOperationException("No column with autonumber functionality found.");
        }

        #endregion

        // for Unit tests
    }
}