// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostgreSqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The postgre sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    using System;
    using System.Linq;

    using RabbitDB.Expressions;
    using RabbitDB.Mapping;
    using RabbitDB.Storage;

    /// <summary>
    /// The postgre sql dialect.
    /// </summary>
    internal class PostgreSqlDialect : SqlDialect
    {
        // for Unit tests
        #region Fields

        /// <summary>
        /// The _builder helper.
        /// </summary>
        private IDbProviderExpressionBuildHelper _builderHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDialect"/> class.
        /// </summary>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        /// The db command executor.
        /// </param>
        internal PostgreSqlDialect(IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : base(SqlCharacters.PostgreSqlCharacters, dbProvider, dbCommandExecutor)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlDialect"/> class.
        /// </summary>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        internal PostgreSqlDialect(IDbProvider dbProvider)
            : base(SqlCharacters.PostgreSqlCharacters, dbProvider)
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
                return _builderHelper
                       ?? (_builderHelper = new PostgresExpressionBuilderHelper(this.SqlCharacters));
            }
        }

        /// <summary>
        /// Gets the scope identity.
        /// </summary>
        internal override string ScopeIdentity
        {
            get
            {
                return " returning {0}";
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
            var propertyInfo = tableInfo.Columns.FirstOrDefault(column => column.ColumnAttribute.AutoNumber);
            if (propertyInfo != null)
            {
                return string.Format(
                    this.ScopeIdentity, 
                    base.SqlCharacters.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
            }

            throw new InvalidOperationException("No column with autonumber functionality found.");
        }

        #endregion
    }
}