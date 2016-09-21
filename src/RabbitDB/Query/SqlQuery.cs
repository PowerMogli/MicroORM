// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The sql query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.Query
{
    /// <summary>
    ///     The sql query.
    /// </summary>
    internal class SqlQuery : IQuery,
                              IArgumentQuery
    {
        #region Fields

        /// <summary>
        ///     The _sql.
        /// </summary>
        protected string Sql;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlQuery" /> class.
        /// </summary>
        /// <param name="sql">
        ///     The sql.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        internal SqlQuery(string sql, QueryParameterCollection arguments = null)
        {
            Sql = sql;
            Arguments = arguments;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the arguments.
        /// </summary>
        public IQueryParameterCollection Arguments { get; protected set; }

        /// <summary>
        ///     Gets the sql statement.
        /// </summary>
        public string SqlStatement
        {
            get { return Sql; }

            internal set { Sql = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compile.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        public virtual IDbCommand Compile(ISqlDialect sqlDialect)
        {
            DbCommandCompiler commandCompiler = new DbCommandCompiler(this, sqlDialect);

            IDbCommand dbCommand = commandCompiler.Compile();

            dbCommand.CommandType = CommandType.Text;

            return dbCommand;
        }

        #endregion
    }
}