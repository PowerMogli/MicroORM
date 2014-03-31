using RabbitDB.Expressions;
using System;
using System.Data;
using System.Linq.Expressions;

namespace RabbitDB.Query.Generic
{
    internal class ExpressionQuery<T> : IQuery, IArgumentQuery
    {
        protected Expression<Func<T, bool>> _expression;

        public QueryParameterCollection Arguments { get; set; }
        public string SqlStatement { get; private set; }

        internal ExpressionQuery(Expression<Func<T, bool>> expression)
        {
            _expression = expression;
        }

        public IDbCommand Compile(SqlDialect.SqlDialect sqlDialect)
        {
            SqlExpressionBuilder<T> sqlExpressionBuilder = new SqlExpressionBuilder<T>(sqlDialect);
            sqlExpressionBuilder.CreateSelect(_expression);
            string query = sqlExpressionBuilder.ToString();
            QueryParameterCollection queryParameterCollection =
                sqlExpressionBuilder.Parameters != null
                ? QueryParameterCollection.Create<T>(sqlExpressionBuilder.Parameters.ToArray())
                : new QueryParameterCollection();

            SqlQuery sqlQuery = new SqlQuery(query, queryParameterCollection);
            return sqlQuery.Compile(sqlDialect);
        }
    }
}