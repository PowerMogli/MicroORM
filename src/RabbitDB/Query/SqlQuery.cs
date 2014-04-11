// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The sql query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System.Data;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The sql query.
    /// </summary>
    internal class SqlQuery : IQuery, IArgumentQuery
    {
        #region Fields

        /// <summary>
        /// The _sql.
        /// </summary>
        protected string Sql;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQuery"/> class.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        internal SqlQuery(string sql, QueryParameterCollection arguments = null)
        {
            this.Sql = sql;
            this.Arguments = arguments;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public QueryParameterCollection Arguments { get; protected set; }

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        public string SqlStatement
        {
            get
            {
                return this.Sql;
            }

            internal set
            {
                this.Sql = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        /// <returns>
        /// The <see cref="IDbCommand"/>.
        /// </returns>
        public virtual IDbCommand Compile(SqlDialect sqlDialect)
        {
            var commandCompiler = new DbCommandCompiler(this, sqlDialect);
            var dbCommand = commandCompiler.Compile();
            dbCommand.CommandType = CommandType.Text;

            return dbCommand;
        }

        #endregion
    }
}