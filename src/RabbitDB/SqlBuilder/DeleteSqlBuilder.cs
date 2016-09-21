// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The delete sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using RabbitDB.Mapping;

#endregion

namespace RabbitDB.SqlBuilder
{
    /// <summary>
    ///     The delete sql builder.
    /// </summary>
    class DeleteSqlBuilder : SqlBuilder
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteSqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        public DeleteSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo)
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
            return $"DELETE FROM {SqlDialect.SqlCharacters.EscapeName(TableInfo.SchemedTableName)} {AppendPrimaryKeys()}";
        }

        #endregion
    }
}