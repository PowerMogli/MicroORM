using System;
using System.Data;
using System.Linq.Expressions;
using RabbitDB.Expression;
using RabbitDB.Storage;

namespace RabbitDB.Query
{
    internal class ExpressionQuery<T, V> : IQuery
    {
        private Expression<Func<T, bool>> _selector;
        private Expression<Func<T, bool>> _condition;
        private ExpressionSqlBuilder<T> _expressionBuilder;

        internal ExpressionQuery(Expression<Func<T, bool>> condition)
        {
            _condition = condition;
        }

        public IDbCommand Compile(IDbProvider provider)
        {
            return null;
        }
    }
}
