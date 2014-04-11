// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The select sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using System.Text;

    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The select sql builder.
    /// </summary>
    internal class SelectSqlBuilder : SqlBuilder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        public SelectSqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo)
            : base(sqlDialect, tableInfo)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create statement.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal override string CreateStatement()
        {
            var selectStatement = new StringBuilder();
            selectStatement.Append(GetBaseSelect());

            selectStatement.Append(AppendPrimaryKeys());
            return selectStatement.ToString();
        }

        /// <summary>
        /// The get base select.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetBaseSelect()
        {
            var selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");

            selectStatement.AppendFormat(
                "{0}", 
                string.Join(
                    ", ", 
                    this.TableInfo.Columns.SelectValidColumnNames(this.TableInfo.DbTable, this.SqlDialect.SqlCharacters)));

            selectStatement.AppendFormat(
                " FROM {0}{1}", 
                this.SqlDialect.SqlCharacters.EscapeName(this.TableInfo.SchemedTableName), 
                this.TableInfo.WithNolock);

            return selectStatement.ToString();
        }

        #endregion
    }
}