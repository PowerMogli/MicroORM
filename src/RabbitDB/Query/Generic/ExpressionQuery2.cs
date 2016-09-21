// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionQuery2.cs" company="">
//   
// </copyright>
// <summary>
//   The expression query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;
using System.Linq.Expressions;

using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Expressions;

#endregion

namespace RabbitDB.Query.Generic
{
    /// <summary>
    ///     The expression query.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class ExpressionQuery<T> : IQuery,
                                        IArgumentQuery
    {
        #region Fields

        /// <summary>
        ///     The _expression.
        /// </summary>
        protected Expression<Func<T, bool>> Expression;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionQuery{T}" /> class.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        internal ExpressionQuery(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the arguments.
        /// </summary>
        public IQueryParameterCollection Arguments { get; set; }

        /// <summary>
        ///     Gets the sql statement.
        /// </summary>
        public string SqlStatement { get; private set; }

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
        public IDbCommand Compile(ISqlDialect sqlDialect)
        {
            SqlExpressionBuilder<T> sqlExpressionBuilder = new SqlExpressionBuilder<T>(sqlDialect);

            sqlExpressionBuilder.CreateSelect(Expression);

            string query = sqlExpressionBuilder.ToString();

            QueryParameterCollection queryParameterCollection = sqlExpressionBuilder.Parameters != null
                ? QueryParameterCollection.Create<T>(sqlExpressionBuilder.Parameters.ToArray())
                : new QueryParameterCollection();

            SqlQuery sqlQuery = new SqlQuery(query, queryParameterCollection);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}