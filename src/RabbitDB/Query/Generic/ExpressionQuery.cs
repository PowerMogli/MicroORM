// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionQuery.cs" company="">
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
    /// <typeparam name="V">
    /// </typeparam>
    internal class ExpressionQuery<T, V> : IQuery
    {
        #region Fields

        /// <summary>
        ///     The _condition.
        /// </summary>
        private Expression<Func<T, bool>> _condition;

        /// <summary>
        ///     The _expression builder.
        /// </summary>
        private SqlExpressionBuilder<T> _expressionBuilder;

        /// <summary>
        ///     The _selector.
        /// </summary>
        private Expression<Func<T, bool>> _selector;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionQuery{T,V}" /> class.
        /// </summary>
        /// <param name="condition">
        ///     The condition.
        /// </param>
        /// <param name="expressionBuilder">
        ///     The expression Builder.
        /// </param>
        /// <param name="selector">
        ///     The selector.
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
            return null;
        }

        #endregion
    }
}