// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;

using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;

#endregion

namespace RabbitDB.SqlBuilder
{
    /// <summary>
    ///     The sql builder.
    /// </summary>
    internal abstract class SqlBuilder
    {
        #region Fields

        /// <summary>
        ///     The _sql dialect.
        /// </summary>
        protected ISqlDialect SqlDialect;

        /// <summary>
        ///     The _table info.
        /// </summary>
        protected TableInfo TableInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlBuilder" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        internal SqlBuilder(ISqlDialect sqlDialect, TableInfo tableInfo)
        {
            TableInfo = tableInfo;
            SqlDialect = sqlDialect;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The create statement.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal abstract string CreateStatement();

        #endregion

        #region Protected Methods

        /// <summary>
        ///     The append primary keys.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        protected string AppendPrimaryKeys()
        {
            IEnumerable<string> primaryKeys = TableInfo.PrimaryKeyColumns.Select(column => column.ColumnAttribute.ColumnName);
            int count = primaryKeys.Count();

            StringBuilder whereClause = new StringBuilder(" WHERE ");
            int i = 0;
            string seperator = " AND ";

            foreach (string primaryKey in primaryKeys)
            {
                if (i >= count - 1)
                {
                    seperator = string.Empty;
                }

                whereClause.Append($"{SqlDialect.SqlCharacters.EscapeName(primaryKey)}=@{i++}{seperator}");
            }

            return whereClause.ToString();
        }

        #endregion
    }
}