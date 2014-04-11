// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionQuery.cs" company="">
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
    /// <typeparam name="V">
    /// </typeparam>
    internal class ExpressionQuery<T, V> : IQuery
    {
        #region Fields

        /// <summary>
        /// The _condition.
        /// </summary>
        private Expression<Func<T, bool>> _condition;

        /// <summary>
        /// The _expression builder.
        /// </summary>
        private SqlExpressionBuilder<T> _expressionBuilder;

        /// <summary>
        /// The _selector.
        /// </summary>
        private Expression<Func<T, bool>> _selector;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionQuery{T,V}"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="expressionBuilder">
        /// The expression Builder.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        internal ExpressionQuery(
            Expression<Func<T, bool>> condition, 
            SqlExpressionBuilder<T> expressionBuilder, 
            Expression<Func<T, bool>> selector)
        {
            _condition = condition;
            _expressionBuilder = expressionBuilder;
            _selector = selector;
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
        public IDbCommand Compile(SqlDialect sqlDialect)
        {
            return null;
        }

        #endregion
    }
}