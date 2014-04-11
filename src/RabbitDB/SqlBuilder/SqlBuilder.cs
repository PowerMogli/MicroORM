// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using System.Linq;
    using System.Text;

    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The sql builder.
    /// </summary>
    internal abstract class SqlBuilder
    {
        #region Fields

        /// <summary>
        /// The _sql dialect.
        /// </summary>
        protected SqlDialect SqlDialect;

        /// <summary>
        /// The _table info.
        /// </summary>
        protected TableInfo TableInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        internal SqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo)
        {
            this.TableInfo = tableInfo;
            this.SqlDialect = sqlDialect;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create statement.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal abstract string CreateStatement();

        /// <summary>
        /// The append primary keys.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string AppendPrimaryKeys()
        {
            var primaryKeys = this.TableInfo.PrimaryKeyColumns.Select(column => column.ColumnAttribute.ColumnName);
            var count = primaryKeys.Count();

            var whereClause = new StringBuilder(" WHERE ");
            var i = 0;
            var seperator = " AND ";
            foreach (var primaryKey in primaryKeys)
            {
                if (i >= count - 1)
                {
                    seperator = string.Empty;
                }

                whereClause.AppendFormat(
                    "{0}=@{1}{2}", 
                    this.SqlDialect.SqlCharacters.EscapeName(primaryKey), 
                    i++, 
                    seperator);
            }

            return whereClause.ToString();
        }

        #endregion
    }
}