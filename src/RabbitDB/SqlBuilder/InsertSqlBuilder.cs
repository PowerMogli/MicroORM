// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsertSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The insert sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Text;

using RabbitDB.Mapping;

#endregion

namespace RabbitDB.SqlBuilder
{
    /// <summary>
    ///     The insert sql builder.
    /// </summary>
    internal class InsertSqlBuilder : SqlBuilder
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="InsertSqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        public InsertSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo)
        {
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The create statement.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal override string CreateStatement()
        {
            StringBuilder insertStatement = new StringBuilder();

            insertStatement.Append($"INSERT INTO {SqlDialect.SqlCharacters.EscapeName(TableInfo.SchemedTableName)} ");

            insertStatement.Append($"({string.Join(", ", TableInfo.Columns.SelectValidNonAutoNumberColumnNames(SqlDialect.SqlCharacters))})");

            insertStatement.Append($" VALUES({string.Join(", ", TableInfo.Columns.SelectValidNonAutoNumberPrefixedColumnNames())})");

            return string.Concat(insertStatement.ToString(), SqlDialect.ResolveScopeIdentity(TableInfo));
        }

        #endregion
    }
}