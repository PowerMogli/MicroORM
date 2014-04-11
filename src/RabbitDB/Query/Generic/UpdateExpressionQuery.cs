// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExpressionQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The update expression query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query.Generic
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    using RabbitDB.Expressions;
    using RabbitDB.Mapping;
    using RabbitDB.Reflection;
    using RabbitDB.SqlBuilder;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The update expression query.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class UpdateExpressionQuery<TEntity> : IQuery
    {
        #region Fields

        /// <summary>
        /// The _arguments.
        /// </summary>
        private readonly object[] _arguments;

        /// <summary>
        /// The _expression.
        /// </summary>
        private readonly Expression<Func<TEntity, bool>> _expression;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExpressionQuery{TEntity}"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        internal UpdateExpressionQuery(Expression<Func<TEntity, bool>> condition, params object[] arguments)
        {
            _expression = condition;
            _arguments = arguments;
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
            var updateTableBuilder = new UpdateTableBuilder<TEntity>(
                sqlDialect, 
                new UpdateSqlBuilder(sqlDialect, TableInfo<TEntity>.GetTableInfo));

            foreach (var parameter in ParameterTypeDescriptor.ToKeyValuePairs(_arguments))
            {
                updateTableBuilder.Set(parameter.Key, parameter.Value);
            }

            updateTableBuilder.Where(_expression);

            var sqlQuery = new SqlQuery(
                updateTableBuilder.GetSql(), 
                QueryParameterCollection.Create<TEntity>(updateTableBuilder.GetParameters()));

            return sqlQuery.Compile(sqlDialect);
        }

        #endregion
    }
}