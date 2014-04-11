// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The stored procedure query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.StoredProcedure
{
    using System.Data;

    using RabbitDB.SqlDialect;

    /// <summary>
    /// The stored procedure query.
    /// </summary>
    internal class StoredProcedureQuery : IQuery, IArgumentQuery
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureQuery"/> class.
        /// </summary>
        /// <param name="procedureWorkObject">
        /// The procedure work object.
        /// </param>
        internal StoredProcedureQuery(StoredProcedure procedureWorkObject)
        {
            this.SqlStatement = procedureWorkObject.StoredProcedureName;
            this.Arguments = QueryParameterCollection.Create(new object[] { procedureWorkObject.Parameters });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureQuery"/> class.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        internal StoredProcedureQuery(string storedProcedureName, QueryParameterCollection arguments = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Arguments = arguments;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public QueryParameterCollection Arguments { get; private set; }

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        public string SqlStatement { get; private set; }

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
        public IDbCommand Compile(SqlDialect sqlDialect)
        {
            var commandCompiler = new DbCommandCompiler(this, sqlDialect);
            var dbCommand = commandCompiler.Compile();
            dbCommand.CommandType = CommandType.StoredProcedure;

            return dbCommand;
        }

        #endregion
    }
}