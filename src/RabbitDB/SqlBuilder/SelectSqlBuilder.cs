// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The select sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Text;

using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.SqlBuilder
{
    /// <summary>
    ///     The select sql builder.
    /// </summary>
    internal class SelectSqlBuilder : SqlBuilder
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectSqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        public SelectSqlBuilder(ISqlDialect sqlDialect, TableInfo tableInfo)
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
            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append(GetBaseSelect());

            selectStatement.Append(AppendPrimaryKeys());

            return selectStatement.ToString();
        }

        /// <summary>
        ///     The get base select.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string GetBaseSelect()
        {
            StringBuilder selectStatement = new StringBuilder();

            selectStatement.Append("SELECT ");

            selectStatement.Append($"{string.Join(", ", TableInfo.Columns.SelectValidColumnNames(TableInfo.DbTable, SqlDialect.SqlCharacters))}");

            selectStatement.Append($" FROM {SqlDialect.SqlCharacters.EscapeName(TableInfo.SchemedTableName)}{TableInfo.WithNolock}");

            return selectStatement.ToString();
        }

        #endregion
    }
}