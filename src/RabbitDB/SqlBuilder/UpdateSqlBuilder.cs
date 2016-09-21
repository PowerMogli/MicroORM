// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The update sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.SqlBuilder
{
    /// <summary>
    ///     The update sql builder.
    /// </summary>
    internal class UpdateSqlBuilder : SqlBuilder
    {
        #region Fields

        /// <summary>
        ///     The _arguments.
        /// </summary>
        private readonly KeyValuePair<string, object>[] _arguments;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        internal UpdateSqlBuilder(SqlDialect.SqlDialect sqlDialect, TableInfo tableInfo, KeyValuePair<string, object>[] arguments)
            : base(sqlDialect, tableInfo)
        {
            _arguments = arguments;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        internal UpdateSqlBuilder(ISqlDialect sqlDialect, TableInfo tableInfo)
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
            string updateStatement = GetBaseUpdate();

            updateStatement += string.Join(
                ", ",
                _arguments.SkipWhile(
                    kvp => TableInfo.DbTable.SkipWhile(TableInfo.ResolveColumnName(kvp.Key)))
                          .Select(kvp2 => $"{SqlDialect.SqlCharacters.EscapeName(kvp2.Key)} = @{kvp2.Key}"));

            updateStatement += AppendPrimaryKeys();

            return updateStatement;
        }

        /// <summary>
        ///     The get base update.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string GetBaseUpdate()
        {
            return $"UPDATE {SqlDialect.SqlCharacters.EscapeName(TableInfo.SchemedTableName)} SET ";
        }

        #endregion
    }
}