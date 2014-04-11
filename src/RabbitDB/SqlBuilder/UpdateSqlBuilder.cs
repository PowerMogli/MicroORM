// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSqlBuilder.cs" company="">
//   
// </copyright>
// <summary>
//   The update sql builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlBuilder
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.Mapping;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The update sql builder.
    /// </summary>
    internal class UpdateSqlBuilder : SqlBuilder
    {
        #region Fields

        /// <summary>
        /// The _arguments.
        /// </summary>
        private readonly KeyValuePair<string, object>[] _arguments;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        internal UpdateSqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo, KeyValuePair<string, object>[] arguments)
            : base(sqlDialect, tableInfo)
        {
            _arguments = arguments;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        internal UpdateSqlBuilder(SqlDialect sqlDialect, TableInfo tableInfo)
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
            var updateStatement = GetBaseUpdate();
            updateStatement += string.Join(
                ", ", 
                _arguments.SkipWhile(
                    kvp => this.TableInfo.DbTable.SkipWhile(this.TableInfo.ResolveColumnName(kvp.Key)))
                    .Select(
                        kvp2 =>
                        string.Format("{0} = @{1}", this.SqlDialect.SqlCharacters.EscapeName(kvp2.Key), kvp2.Key)));
            updateStatement += AppendPrimaryKeys();

            return updateStatement;
        }

        /// <summary>
        /// The get base update.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetBaseUpdate()
        {
            return string.Format(
                "UPDATE {0} SET ", 
                this.SqlDialect.SqlCharacters.EscapeName(this.TableInfo.SchemedTableName));
        }

        #endregion
    }
}