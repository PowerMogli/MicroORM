using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using RabbitDB.Expressions;
using RabbitDB.Reflection;
using RabbitDB.Storage;

namespace RabbitDB.Query.Generic
{
    internal class UpdateExpressionQuery<TEntity> : IQuery
    {
        private readonly Expression<Func<TEntity, bool>> _expression;
        private readonly object[] _arguments;

        internal UpdateExpressionQuery(Expression<Func<TEntity, bool>> condition, params object[] arguments)
        {
            _expression = condition;
            _arguments = arguments;
        }

        public IDbCommand Compile(IDbProvider dbProvider)
        {
            UpdateTableBuilder<TEntity> updateTableBuilder = new UpdateTableBuilder<TEntity>(dbProvider);
            foreach (KeyValuePair<string, object> parameter in ParameterTypeDescriptor.ToKeyValuePairs(_arguments))
            {
                updateTableBuilder.Set(parameter.Key, parameter.Value);
            }
            updateTableBuilder.Where(_expression);

            SqlQuery sqlQuery = new SqlQuery(updateTableBuilder.GetSql(), QueryParameterCollection.Create<TEntity>(updateTableBuilder.GetParameters()));
            return sqlQuery.Compile(dbProvider);
        }
    }
}