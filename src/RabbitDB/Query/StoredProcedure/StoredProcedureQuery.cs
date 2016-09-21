// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoredProcedureQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The stored procedure query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Query.StoredProcedure;
using RabbitDB.Contracts.SqlDialect;

namespace RabbitDB.Query.StoredProcedure
{
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
        internal StoredProcedureQuery(IStoredProcedure procedureWorkObject)
        {
            SqlStatement = procedureWorkObject.StoredProcedureName;
            Arguments = QueryParameterCollection.Create(new object[] { procedureWorkObject.Parameters });
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
            SqlStatement = storedProcedureName;
            Arguments = arguments;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IQueryParameterCollection Arguments { get; }

        /// <summary>
        /// Gets the sql statement.
        /// </summary>
        public string SqlStatement { get; }

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
        public IDbCommand Compile(ISqlDialect sqlDialect)
        {
            DbCommandCompiler commandCompiler = new DbCommandCompiler(this, sqlDialect);

            IDbCommand dbCommand = commandCompiler.Compile();

            dbCommand.CommandType = CommandType.StoredProcedure;

            return dbCommand;
        }

        #endregion
    }
}