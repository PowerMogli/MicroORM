using System;
using System.Data;
using System.Linq.Expressions;
using RabbitDB.Expressions;
using RabbitDB.Storage;

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

        public IDbCommand Compile(IDbProvider dbProvider)
        {
            SqlExpressionBuilder<T> sqlExpressionBuilder = new SqlExpressionBuilder<T>(dbProvider);
            sqlExpressionBuilder.CreateSelect(_expression);
            string query = sqlExpressionBuilder.ToString();
            QueryParameterCollection queryParameterCollection =
                sqlExpressionBuilder.Parameters != null
                ? QueryParameterCollection.Create<T>(sqlExpressionBuilder.Parameters.ToArray())
                : new QueryParameterCollection();

            SqlQuery sqlQuery = new SqlQuery(query, queryParameterCollection);
            return sqlQuery.Compile(dbProvider);
        }
    }
}