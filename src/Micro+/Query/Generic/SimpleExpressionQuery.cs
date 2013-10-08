using System;
using System.Data;
using System.Linq.Expressions;
using MicroORM.Expression;
using MicroORM.Storage;

namespace MicroORM.Query.Generic
{
    class SimpleExpressionQuery<T> : IQuery
    {
        private Expression<Func<T, bool>> _condition;
        private ExpressionSqlBuilder<T> _expressionBuilder;

        internal SimpleExpressionQuery(Expression<Func<T, bool>> condition)
        {
            _condition = condition;
        }

        public IDbCommand Compile(IDbProvider provider)
        {
            return null;
        }
    }
}
