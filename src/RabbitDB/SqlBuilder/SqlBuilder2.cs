// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlBuilder2.cs" company="">
//   
// </copyright>
// <summary>
//   The sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using System.Collections.Generic;

    using RabbitDB.Mapping;
    using RabbitDB.Session;

    /// <summary>
    /// The sql builder.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal static class SqlBuilder<TEntity>
    {
        #region Properties

        /// <summary>
        /// Gets the delete statement.
        /// </summary>
        internal static string DeleteStatement
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(InternalDeleteStatement))
                {
                    return InternalDeleteStatement;
                }

                var tableInfo = TableInfo<TEntity>.GetTableInfo;
                var deleteBuilder = new DeleteSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                InternalDeleteStatement = deleteBuilder.CreateStatement();

                return InternalDeleteStatement;
            }
        }

        /// <summary>
        /// Gets the insert statement.
        /// </summary>
        internal static string InsertStatement
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(InternalInsertStatement))
                {
                    return InternalInsertStatement;
                }

                var tableInfo = TableInfo<TEntity>.GetTableInfo;
                var insertBuilder = new InsertSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                InternalInsertStatement = insertBuilder.CreateStatement();

                return InternalInsertStatement;
            }
        }

        /// <summary>
        /// Gets the select statement.
        /// </summary>
        internal static string SelectStatement
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(InternalSelectStatement))
                {
                    return InternalSelectStatement;
                }

                var tableInfo = TableInfo<TEntity>.GetTableInfo;
                var selectBuilder = new SelectSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                InternalSelectStatement = selectBuilder.CreateStatement();

                return InternalSelectStatement;
            }
        }

        /// <summary>
        /// Gets or sets the internal delete statement.
        /// </summary>
        private static string InternalDeleteStatement { get; set; }

        /// <summary>
        /// Gets or sets the internal insert statement.
        /// </summary>
        private static string InternalInsertStatement { get; set; }

        /// <summary>
        /// Gets or sets the internal select statement.
        /// </summary>
        private static string InternalSelectStatement { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The get update statement.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string CreateUpdateStatement(KeyValuePair<string, object>[] arguments)
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var updateBuilder = new UpdateSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo, arguments);
            return updateBuilder.CreateStatement();
        }

        #endregion
    }
}