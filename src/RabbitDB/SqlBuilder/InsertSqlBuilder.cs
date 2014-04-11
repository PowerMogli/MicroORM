// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InsertSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The insert sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using System.Text;

    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The insert sql builder.
    /// </summary>
    class InsertSqlBuilder : SqlBuilder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertSqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        public InsertSqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo)
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
            var insertStatement = new StringBuilder();
            insertStatement.AppendFormat(
                "INSERT INTO {0} ", 
                this.SqlDialect.SqlCharacters.EscapeName(this.TableInfo.SchemedTableName));

            insertStatement.AppendFormat(
                "({0})", 
                string.Join(
                    ", ", 
                    this.TableInfo.Columns.SelectValidNonAutoNumberColumnNames(this.SqlDialect.SqlCharacters)));

            insertStatement.AppendFormat(
                " VALUES({0})", 
                string.Join(", ", this.TableInfo.Columns.SelectValidNonAutoNumberPrefixedColumnNames()));

            return string.Concat(insertStatement.ToString(), this.SqlDialect.ResolveScopeIdentity(this.TableInfo));
        }

        #endregion
    }
}