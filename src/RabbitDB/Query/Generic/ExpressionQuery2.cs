// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionQuery2.cs" company="">
//   
// </copyright>
// <summary>
//   The expression query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.Generic
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    using RabbitDB.Expressions;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The expression query.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class ExpressionQuery<T> : IQuery, IArgumentQuery
    {
        #region Fields

        /// <summary>
        /// The _expression.
        /// </summary>
        protected Expression<Func<T, bool>> Expression;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionQuery{T}"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        internal ExpressionQuery(Expression<Func<T, bool>> expression)
        {
            this.Expression = expression;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public QueryParameterCollection Arguments { get; set; }

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
            var sqlExpressionBuilder = new SqlExpressionBuilder<T>(sqlDialect);
            sqlExpressionBuilder.CreateSelect(this.Expression);
            var query = sqlExpressionBuilder.ToString();
            var queryParameterCollection =
                sqlExpressionBuilder.Parameters != null
                    ? QueryParameterCollection.Create<T>(sqlExpressionBuilder.Parameters.ToArray())
                    : new QueryParameterCollection();

            var sqlQuery = new SqlQuery(query, queryParameterCollection);

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}