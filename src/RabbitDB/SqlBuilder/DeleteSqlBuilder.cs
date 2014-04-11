// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The delete sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The delete sql builder.
    /// </summary>
    class DeleteSqlBuilder : SqlBuilder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        public DeleteSqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo)
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
            return string.Format(
                "DELETE FROM {0} {1}", 
                this.SqlDialect.SqlCharacters.EscapeName(this.TableInfo.SchemedTableName), 
                AppendPrimaryKeys());
        }

        #endregion
    }
}