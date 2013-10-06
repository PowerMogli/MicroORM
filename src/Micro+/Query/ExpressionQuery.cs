using System;
using System.Linq.Expressions;
using MicroORM.Base.Expression;
using MicroORM.Base.Storage;
using System.Data;

namespace MicroORM.Base.Query
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
